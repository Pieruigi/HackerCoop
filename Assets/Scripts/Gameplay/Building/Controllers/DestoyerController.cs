using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HKR
{
    public class DestoyerController : MonoBehaviour
    {
        [SerializeField]
        SecurityStateController securityStateController;

        [SerializeField]
        Spotter spotter;

        [SerializeField]
        Patroller patroller;

        float checkSpottedTime = 1f;

        float checkSpottedElapsed = 0;

        PlayerController target;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
                return;
            if (!PlayerManager.Instance.PlayerInGameAll())
                return;

            CheckSpottedPlayers();
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
            switch (newState)
            {
                case SecurityState.Freezed:
                    if(target != null)
                    {
                        AlarmSystemController.GetAlarmSystemController(securityStateController.FloorLevel).RemoveTaker(gameObject);
                        target = null;
                    }
                    
                    break;
                
            }
            
        }

        void CheckSpottedPlayers()
        {
            // Time to check???
            checkSpottedElapsed += Time.deltaTime;
            if (checkSpottedElapsed < checkSpottedTime)
                return;
            checkSpottedElapsed = 0;

            // Spotted state
            if (securityStateController.State == SecurityState.Spotted || securityStateController.State == SecurityState.Freezed)
                return;

            var asc = AlarmSystemController.GetAlarmSystemController(securityStateController.FloorLevel);
            if (asc.State == AlarmSystemState.Deactivated)
            {
                target = null;
                return;
            }

            // Get or update target
            if(asc.TryGetOrUpdateTarget(gameObject, out target))
            {
                // We can use the patroller if we have a target
                patroller.SetDestination(target.transform.position);
            }
            

        }
    }

}
