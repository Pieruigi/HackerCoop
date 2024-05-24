using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace HKR
{
    public abstract class Spotter : MonoBehaviour
    {
        [SerializeField]
        float range = 9f;
        public float Range
        {
            get { return range; }
        }

        [SerializeField]
        SecurityStateController securityStateController;

        [SerializeField]
        float alarmTolleranceThreshold = 4;

        [SerializeField]
        float searchingDuration = 5;

        //List<Data> dataList = new List<Data>();

        PlayerController currentTarget = null;
        public PlayerController CurrentTarget
        {
            get { return currentTarget; }
        }

        System.DateTime currentTargetTime;

        List<PlayerController> inTriggerList = new List<PlayerController>();

        protected abstract bool IsPlayerSpotted(PlayerController target);

        DateTime startSearchingTime;
        Vector3 lastSpottedPosition;


        protected virtual void Awake()
        {
            // Adjust the trigger size
            Vector3 scale = transform.localScale;
            scale.x = scale.z = range * 2f;
            transform.localScale = scale;

        }

        

        // Update is called once per frame
        protected virtual void Update()
        {
            if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
                return;

            if (!PlayerManager.Instance.PlayerInGameAll())
                return;

            // Check the player list
            UpdateState();
        }


        private void OnEnable()
        {
            securityStateController.OnStateChanged += HandleOnStateChanged;
        }

        private void OnDisable()
        {
            securityStateController.OnStateChanged -= HandleOnStateChanged;
        }

        private void HandleOnStateChanged(SecurityState oldState, SecurityState newState)
        {
            switch(newState)
            {
                case SecurityState.Spotted:
                    currentTargetTime = DateTime.Now;
                    break;
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {

            if (!other.CompareTag(Tags.Player))
                return;

            if (inTriggerList.Exists(p => p.gameObject == other.gameObject))
                return;

            // Add player to the check list
            inTriggerList.Add(other.GetComponent<PlayerController>());
        }

        protected virtual void OnTriggerExit(Collider other)
        {

            if (!other.CompareTag(Tags.Player))
                return;
            // Remove player from the check list
            inTriggerList.RemoveAll(p => p.gameObject == other.gameObject);

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
                //case SecurityState.Alarmed:
                //    UpdateAlarmedState();
                //    break;
                case SecurityState.Searching:
                    UpdateSearchingState();
                    break;
            }
        }

        bool CheckAnyTarget()
        {
            // No target, check any
            for (int i = 0; i < inTriggerList.Count && !currentTarget; i++)
            {
                if (IsPlayerSpotted(inTriggerList[i]))
                {
                    currentTarget = inTriggerList[i];
                    return true;
                }

            }

            return false;
        }

        //void UpdateAlarmedState()
        //{
        //    if (currentTarget)
        //    {
        //        // Camera already has a target, check if it's still in sight
        //        if (!IsPlayerSpotted(currentTarget))
        //            currentTarget = null;
        //        else
        //            lastSpottedPosition = currentTarget.transform.position;
        //    }
        //    else
        //    {
        //        // No target, check any
        //        if (CheckAnyTarget())
        //            lastSpottedPosition = currentTarget.transform.position;
        //    }

        //    // If there is a target we reset the alarm system timer
        //    if (currentTarget)
        //        AlarmSystemController.GetAlarmSystemController(securityStateController.FloorLevel).ResetAlarmTimer();

        //}

        void UpdateNormalState()
        {
            // Being in normal state means the camera has no target at all, so we check for a new target if any
            if (CheckAnyTarget())
            {
                lastSpottedPosition = currentTarget.transform.position;
                securityStateController.State = SecurityState.Spotted;
            }

                
        }

        void UpdateSpottedState()
        {
            if (!IsPlayerSpotted(currentTarget))
            {
                if (currentTarget)
                {
                    // Target is no longer in sight, we go back to the normal state
                    
                    currentTarget = null;
                    startSearchingTime = System.DateTime.Now;
                    securityStateController.State = SecurityState.Searching;
                    
                }
                
            }
            else
            {
                lastSpottedPosition = currentTarget.transform.position;
                var asc = AlarmSystemController.GetAlarmSystemController(securityStateController.FloorLevel);

                if (asc.State == AlarmSystemState.Activated)
                {
                    // If the alarm is activated we simply reset the timer every time any player is spotted
                    asc.ResetAlarmTimer();
                    // We can do something else here, like shoot the target and/or report the others
                }
                else
                {
                    // If any player is spotted we check if it's time to switch the alarm on
                    if ((System.DateTime.Now - currentTargetTime).TotalSeconds > alarmTolleranceThreshold)
                    {
                        // Lock the floor down
                        asc.SwitchAlarmOnRpc();
                    }
                }    

                
                
            }
        }

        void UpdateSearchingState()
        {
            if(CheckAnyTarget())
            {
                lastSpottedPosition = currentTarget.transform.position;
                securityStateController.State = SecurityState.Spotted;
            }
            else
            {
                if((System.DateTime.Now-startSearchingTime).TotalSeconds > searchingDuration) 
                {
                    //if(AlarmSystemController.GetAlarmSystemController(securityStateController.FloorLevel).State == AlarmSystemState.Activated)
                    //    securityStateController.State = SecurityState.Alarmed;
                    //else
                    securityStateController.State = SecurityState.Normal;
                }
            }
        }
    }

}
