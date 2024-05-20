using HKR.Building;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class SightSpotter : MonoBehaviour
    {
        //struct Data
        //{
        //    public PlayerController player;
        //    public System.DateTime time; // When the player entered the camera sight 
        //    public bool seen;
        //}

        [SerializeField]
        Transform eyes;

        // X is horizontal, y is vertical
        [SerializeField]
        Vector2 sightAngles = new Vector2(60, 45);

        [SerializeField]
        float sightRange = 9f;


        [SerializeField]
        SecurityStateController securityStateController;

        [SerializeField]
        float alarmTolleranceThreshold = 4;

        //List<Data> dataList = new List<Data>();

        PlayerController currentTarget = null;
        public PlayerController CurrentTarget
        {
            get { return currentTarget; }
        }

        System.DateTime currentTargetTime;

        List<PlayerController> inTriggerList = new List<PlayerController>();

        
        private void Awake()
        {
            // Adjust the trigger size
            Vector3 scale = transform.localScale;
            scale.x = scale.z = sightRange * 2f;
            transform.localScale = scale;
            
        }

        // Update is called once per frame
        void Update()
        {
            if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
                return;

            if (!PlayerManager.Instance.PlayerInGameAll())
                return;

            // Check the player list
            UpdateState();
        }

               

        private void OnTriggerEnter(Collider other)
        {
           
            if (!other.CompareTag(Tags.Player))
                return;

            if (inTriggerList.Exists(p => p.gameObject == other.gameObject))
                return;

            // Add player to the check list
            inTriggerList.Add(other.GetComponent<PlayerController>());
        }

        private void OnTriggerExit(Collider other)
        {
         
            if (!other.CompareTag(Tags.Player))
                return;
            // Remove player from the check list
            inTriggerList.RemoveAll(p=>p.gameObject == other.gameObject);
         
        }

        

        void UpdateState()
        {
            switch (securityStateController.State)
            {
                case SecurityState.Normal:
                    UpdateNormalState();
                    break;
                case SecurityState.Spotted:
                    UpdateSpottedState();
                    break;
                case SecurityState.Alarmed:
                    UpdateAlarmedState();
                    break;
            }
        }

        bool IsPlayerInSight(PlayerController player)
        {
            // Get the sight target which is the camera root
            Transform sightTarget = player.GetSightTarget();

            // Get the direction 
            Vector3 direction = sightTarget.position - eyes.position;

            // Check the horizontal angle
            Vector3 hDir = Vector3.ProjectOnPlane(direction, Vector3.up);
            Vector3 hFwd = Vector3.ProjectOnPlane(eyes.forward, Vector3.up);
            float angle = Vector3.Angle(hDir, hFwd);
            if (angle < sightAngles.x)
            {
                // Check the vertical angle
                Vector3 vDir = Vector3.ProjectOnPlane(direction, eyes.right);
                angle = Vector3.Angle(vDir, eyes.forward);

                if (angle < sightAngles.y)
                {
                    // Do raycast to check if there is any obstacle between the camera and the potential target
                    RaycastHit hitInfo;
                    Ray ray = new Ray(eyes.position, direction.normalized);
                    if (Physics.Raycast(ray, out hitInfo, sightRange, ~LayerMask.GetMask(), QueryTriggerInteraction.Ignore))
                    {
                   
                        // Check if the hit object is the player we are checking 
                        if (hitInfo.collider.gameObject == player.gameObject)
                            return true;
                    }


                }
            }

            return false;
        }

        void UpdateAlarmedState()
        {
            if (currentTarget)
            {
                // Camera already has a target, check if it's still in sight
                if (!IsPlayerInSight(currentTarget))
                    currentTarget = null;
            }
            else
            {
                // No target, check any
                for (int i = 0; i < inTriggerList.Count && !currentTarget; i++)
                {
                    if (IsPlayerInSight(inTriggerList[i]))
                    {
                        currentTarget = inTriggerList[i];
                    }

                }
            }

            // If there is a target we reset the alarm system timer
            if (currentTarget)
                AlarmSystemController.GetAlarmSystemController(securityStateController.FloorLevel).ResetAlarmTimer();
            
        }

        void UpdateNormalState()
        {
            // Being in normal state means the camera has no target at all, so we check for a new target if any
            for(int i=0; i<inTriggerList.Count && !currentTarget; i++)
            {
                if (IsPlayerInSight(inTriggerList[i]))
                {
                    currentTarget = inTriggerList[i];
                    currentTargetTime = System.DateTime.Now;
                }
                    
            }

            if(currentTarget)
                securityStateController.State = SecurityState.Spotted;
        }

        void UpdateSpottedState()
        {
            if (!IsPlayerInSight(currentTarget))
            {
                // Target is no longer in sight, we go back to the normal state
                currentTarget = null;
                securityStateController.State = SecurityState.Normal;
            }
            else
            {
                // Target is in sight, we check for how long
                if((System.DateTime.Now-currentTargetTime).TotalSeconds > alarmTolleranceThreshold) 
                {
                    // Lock the floor down
                    AlarmSystemController.GetAlarmSystemController(securityStateController.FloorLevel).SwitchAlarmOnRpc();
                }
            }
        }
    }

}
