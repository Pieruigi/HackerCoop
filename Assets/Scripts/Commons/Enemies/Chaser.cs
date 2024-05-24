using HKR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chaser : MonoBehaviour
{
    [SerializeField]
    SecurityStateController securityStateController;

    [SerializeField]
    Spotter spotter;

    [SerializeField]
    NavMeshAgent agent;

    [SerializeField]
    float spottedSpeed;

    [SerializeField]
    float alarmedSpeed;

    [SerializeField]
    float spottedStoppingDistance;

    [SerializeField]
    float alarmedStoppingDistance;

  

    bool activated = false;

    // Update is called once per frame
    void Update()
    {
        if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
            return;

        if (!PlayerManager.Instance.PlayerInGameAll())
            return;

        if(!activated) return;

        // Check the player list
        UpdateState();
    }

    private void OnEnable()
    {
        securityStateController.OnStateChanged += HandleOnStateChanged;
        securityStateController.OnSpawned += HandleOnStateControllerSpawned;
    }

    private void OnDisable()
    {
        securityStateController.OnStateChanged -= HandleOnStateChanged;
        securityStateController.OnSpawned -= HandleOnStateControllerSpawned;
    }

    private void HandleOnStateControllerSpawned()
    {

        //if (securityStateController.State == SecurityState.Spotted || securityStateController.State == SecurityState.Alarmed)
        //    activated = true;
    }

    private void HandleOnStateChanged(SecurityState oldState, SecurityState newState)
    {
        switch (newState)
        {
            case SecurityState.Spotted:
            //case SecurityState.Alarmed:
            //    agent.ResetPath();
            //    if(newState == SecurityState.Spotted)
            //    {
            //        agent.stoppingDistance = spottedStoppingDistance;
            //        agent.speed = spottedSpeed;
            //    }
            //    else
            //    {
            //        agent.stoppingDistance = alarmedStoppingDistance;
            //        agent.speed = alarmedSpeed;
            //    }
                    
            //    activated = true;
            //    break;
            default:
                activated = false;
                break;
        }
    }

    void UpdateState()
    {
        switch(securityStateController.State)
        {
            case SecurityState.Spotted:
                
                break;
            //case SecurityState.Alarmed:

            //    break;
        }
    }
}
