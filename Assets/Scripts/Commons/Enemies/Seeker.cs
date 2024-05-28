using DG.Tweening.Plugins.Options;
using Fusion;
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
public class Seeker : NetworkBehaviour
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

    [SerializeField]
    float lookAroundElapsed = 0;

    [SerializeField]
    float lookAroundTime = 4;

    Quaternion lookAtRotation = Quaternion.identity;

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

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
            return;

        if (!PlayerManager.Instance.PlayerInGameAll())
            return;

        if (!activated) return;

        FixedUpdateNetworkState();
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
                // You must activate the agent before setting it
                activated = true;
                // We set the destination only once ( if we see any player again the state will change back to spotted )
                agent.enabled = true;
                agent.stoppingDistance = searchingStoppingDistance;
                agent.destination = spotter.LastSpottedPosition;
                
                break;
            case SecurityState.Spotted:
                activated = true;
                // We set the destination only once ( if we see any player again the state will change back to spotted )
                agent.enabled = true;
                agent.stoppingDistance = spottedStoppingDistance;
                break;
            default:
                activated = false;
                break;
        }
    }

    

    void FixedUpdateNetworkState()
    {
        switch (securityStateController.State)
        {
            case SecurityState.Searching:

                Vector3 dir = Vector3.ProjectOnPlane(spotter.LastSpottedPosition - transform.position, Vector3.up);
                if(dir.magnitude <= agent.stoppingDistance)
                {
                    agent.enabled = false;
                    // Look around
                    lookAroundElapsed += Time.fixedDeltaTime;
                    if (lookAroundElapsed > lookAroundTime)
                    {
                        // Choose another direction to look at
                        Vector3 newTarget = transform.position - transform.forward * 2f; ;
                        Vector3 newDir = Vector3.ProjectOnPlane(newTarget - transform.position, Vector3.up);
                        lookAtRotation = Quaternion.LookRotation(newDir, Vector3.up);
                        
                        lookAroundElapsed = 0;
                    }
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAtRotation, agent.angularSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    agent.enabled = true;
                    lookAroundElapsed = 0;
                    lookAtRotation = transform.rotation;
                }
                
                break;

            case SecurityState.Spotted:
                dir = Vector3.ProjectOnPlane(spotter.CurrentTarget.transform.position - transform.position, Vector3.up);
                if (dir.magnitude > agent.stoppingDistance)
                {
                    agent.enabled = true;
                    agent.stoppingDistance = spottedStoppingDistance;
                    agent.destination = spotter.CurrentTarget.transform.position;
                }
                else
                {
                    agent.stoppingDistance = spottedStoppingDistance * 1.5f;
                    agent.enabled = false;
                    lookAtRotation = Quaternion.LookRotation(dir, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAtRotation, agent.angularSpeed * Time.fixedDeltaTime);

                }
             
                break;

            default:

                break;
        }
    }

    void UpdateState()
    {
       
    }
}
