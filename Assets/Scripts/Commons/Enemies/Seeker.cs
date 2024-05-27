using HKR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This is called when the IA is in searching or in spotted state.
/// This is only applicable to not static entities ( like drones for example ).
/// </summary>
public class Seeker : MonoBehaviour
{
    [SerializeField]
    SecurityStateController securityStateController;

    [SerializeField]
    Spotter spotter;

    [SerializeField]
    NavMeshAgent agent;

    [SerializeField]
    float alarmOffSpeed;

    [SerializeField]
    float alarmOnSpeed;

    [SerializeField]
    float spottedStoppingDistance;

    [SerializeField]
    float searchingStoppingDistance;

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
        AlarmSystemController.OnStateChanged += HandleOnAlarmStateChanged;
    }

    private void OnDisable()
    {
        securityStateController.OnStateChanged -= HandleOnStateChanged;
        securityStateController.OnSpawned -= HandleOnStateControllerSpawned;
        AlarmSystemController.OnStateChanged -= HandleOnAlarmStateChanged;
    }

    private void HandleOnAlarmStateChanged(AlarmSystemController asc, AlarmSystemState oldState, AlarmSystemState newState)
    {
        if (newState == AlarmSystemState.Activated)
            agent.speed = alarmOnSpeed;
        else
            agent.speed = alarmOffSpeed;
    }

    private void HandleOnStateControllerSpawned()
    {
    }

    private void HandleOnStateChanged(SecurityState oldState, SecurityState newState)
    {
        switch (newState)
        {
            case SecurityState.Searching:
                // We set the destination only once ( if we see any player again the state will change back to spotted )
                agent.stoppingDistance = searchingStoppingDistance;
                agent.destination = spotter.LastSpottedPosition;
                activated = true;
                break;
            case SecurityState.Spotted:
                // We set the destination only once ( if we see any player again the state will change back to spotted )
                agent.stoppingDistance = spottedStoppingDistance;
                activated = true;
                break;
            default:
                activated = false;
                break;
        }
    }

    void UpdateState()
    {
        switch(securityStateController.State)
        {
            case SecurityState.Searching:
                // We already set the destination in the state changed handler ( we don't have a target to follow anymore )
                break;
            case SecurityState.Spotted:
                // We must follow the target
                agent.destination = spotter.CurrentTarget.transform.position;
                break;
        }
    }
}
