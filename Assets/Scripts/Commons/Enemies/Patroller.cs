using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HKR
{
    public class Patroller : MonoBehaviour
    {
        SecurityStateController stateController;

        bool activated = false;

        Transform target;

        NavMeshAgent agent;

        private void Awake()
        {
            stateController = GetComponent<SecurityStateController>();
            agent = GetComponent<NavMeshAgent>();
        }

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
            if (!activated)
                return;
            UpdateState();
        }

        private void OnEnable()
        {
            stateController.OnStateChanged += HandleOnStateChanged;
        }

        private void OnDisable()
        {
            stateController.OnStateChanged -= HandleOnStateChanged;
        }

        private void HandleOnStateChanged(SecurityState oldState, SecurityState newState)
        {
            switch(newState)
            {
                case SecurityState.Normal:
                    activated = true;
                    break;
                default:
                    activated = false;
                    break;
            }
        }

        void UpdateState()
        {
            switch(stateController.State)
            {
                case SecurityState.Normal:
                    UpdateNormalState();
                    break;
            }
        }

        void UpdateNormalState()
        {
            //if(agent.destination == null)
        }
    }

}
