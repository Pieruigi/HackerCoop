using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    public abstract class Spotter : MonoBehaviour
    {
        public UnityAction<PlayerController> OnPlayerSpotted;
        public UnityAction<PlayerController> OnPlayerLost;

        [SerializeField]
        float range = 9f;
        public float Range
        {
            get { return range; }
        }

        [SerializeField]
        SecurityStateController securityStateController;
        public SecurityStateController SecurityStateController { get { return securityStateController; } }

        PlayerController currentTarget = null;
        public PlayerController CurrentTarget
        {
            get { return currentTarget; }
        }

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
            PlayerController.OnDead += HandleOnPlayerDead;
            PlayerController.OnEnterSafeZone += HandleOnEnterSafeZone;

        }

        private void OnDisable()
        {
            securityStateController.OnStateChanged -= HandleOnStateChanged;
            PlayerController.OnDead -= HandleOnPlayerDead;
            PlayerController.OnEnterSafeZone -= HandleOnEnterSafeZone;

        }

        private void HandleOnEnterSafeZone(PlayerController arg0)
        {
            HandleOnPlayerDead(arg0); // It's the same behaviour
        }

        private void HandleOnPlayerDead(PlayerController arg0)
        {
            // Eventually remove the dead player from the trigger list
            inTriggerList.Remove(arg0);

            // If the player who just died is the target then clear the target field
            if(currentTarget == arg0)
            {
                currentTarget = null;
                if(securityStateController.State != SecurityState.Freezed)
                    securityStateController.State = SecurityState.Normal;
            }
        }

        private void HandleOnStateChanged(SecurityState oldState, SecurityState newState)
        {
            switch(newState)
            {
                case SecurityState.Freezed:
                    if (currentTarget)
                    {
                        var oldTarget = currentTarget;
                        currentTarget = null;
                        OnPlayerLost?.Invoke(oldTarget);
                    }
                    break;

            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
                return;

            if (!other.CompareTag(Tags.Player))
                return;

            if (inTriggerList.Exists(p => p.gameObject == other.gameObject))
                return;

            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc.State == PlayerState.Dead)
                return;

            // Add player to the check list
            inTriggerList.Add(other.GetComponent<PlayerController>());
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
                return;

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
                case SecurityState.Freezed:
                    UpdateFreezedState();
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
                    bool hadTarget = currentTarget != null;
                    
                    currentTarget = inTriggerList[i];
                    if(!hadTarget)
                        OnPlayerSpotted?.Invoke(currentTarget);
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
                Debug.Log($"TEST - Player seen by {transform.root.gameObject.name} which is on floor {securityStateController.FloorLevel}");
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
                    PlayerController oldTarget = currentTarget;
                    currentTarget = null;
                    securityStateController.State = SecurityState.Searching;
                    OnPlayerLost?.Invoke(oldTarget);
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

        void UpdateFreezedState()
        {
            
        }
    }

}
