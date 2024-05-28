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

        //[SerializeField]
        //float alarmTolleranceThreshold = 4;

        //List<Data> dataList = new List<Data>();

        PlayerController currentTarget = null;
        public PlayerController CurrentTarget
        {
            get { return currentTarget; }
        }

        //System.DateTime currentTargetTime;

        List<PlayerController> inTriggerList = new List<PlayerController>();

        protected abstract bool IsPlayerSpotted(PlayerController target);

        
        Vector3 lastSpottedPosition;
        public Vector3 LastSpottedPosition
        {
            get { return lastSpottedPosition; }
        }


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
                //Debug.Log($"BUG - current target:{currentTarget} no longer spotted");
                if (currentTarget)
                {
                    // Target is no longer in sight, we go back to the normal state
                    currentTarget = null;
                    securityStateController.State = SecurityState.Searching;
                }
            }
            else
            {
                // We store the last target position 
                lastSpottedPosition = currentTarget.transform.position;
            }
        }

        void UpdateSearchingState()
        {
            if(CheckAnyTarget())
            {
                lastSpottedPosition = currentTarget.transform.position;
                securityStateController.State = SecurityState.Spotted;
            }

        }
    }

}
