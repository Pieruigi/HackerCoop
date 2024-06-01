using DG.Tweening.Plugins.Options;
using Fusion;
using HKR;
using HKR.Building;
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

    float lookAroundElapsed = 0;

    [SerializeField]
    float lookAroundTime = 4;

    float searchDistance = 3;

    Quaternion lookAtRotation = Quaternion.identity;
    bool moveOnSearch = false;

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

                //Vector3 dir = Vector3.ProjectOnPlane(agent.destination - transform.position, Vector3.up);
                if(!agent.hasPath)// || agent.remainingDistance <= agent.stoppingDistance)
                {
                    //agent.enabled = false;
                    // Look around
                    lookAroundElapsed += Time.fixedDeltaTime;
                    
                    if (lookAroundElapsed > lookAroundTime)
                    {
                        // Move or look at ?
                        moveOnSearch = UnityEngine.Random.Range(0, 3) == 0 ? true : false;
                        //move = true;
                        if (!moveOnSearch)
                        {
                            // Choose another direction to look at
                            float angle = UnityEngine.Random.Range(60f, 140f);
                            angle *= UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
                            Vector3 fwdRot = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
                            Vector3 newTarget = transform.position + fwdRot * 2f;// - transform.forward * 2f; ;
                            Vector3 newDir = Vector3.ProjectOnPlane(newTarget - transform.position, Vector3.up);
                            lookAtRotation = Quaternion.LookRotation(newDir, Vector3.up);
                            lookAroundElapsed = 0;
                        }
                        else
                        {
                            // Get a new destination closed to the current position
                            NavMeshHit hit;
                            Debug.Log("Sampling position");
                            Vector3 dir = Vector3.forward * UnityEngine.Random.Range(-1f, 1f) + Vector3.right * UnityEngine.Random.Range(-1f, 1f);
                            dir = dir.normalized * UnityEngine.Random.Range(searchDistance * .5f, searchDistance);
                            if (NavMesh.SamplePosition(spotter.LastSpottedPosition + dir, out hit, searchDistance*.25f, agent.areaMask))
                            {
                                Debug.Log("Position sampled:" + hit.position + ", distance:" + Vector3.ProjectOnPlane(hit.position-spotter.LastSpottedPosition, Vector3.up).magnitude );
                                //agent.enabled = true;
                                agent.destination = hit.position;
                            }
                            
                        }
                        
                    }
                    if (!moveOnSearch)
                    {
                        agent.enabled = false;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAtRotation, agent.angularSpeed * Time.fixedDeltaTime);
                        agent.enabled = true;
                    }
                    
                }
                else
                {
                    //agent.enabled = true;
                    lookAroundElapsed = 0;
                    lookAtRotation = transform.rotation;
                }
                
                break;

            case SecurityState.Spotted:
                
                if(Vector3.Distance(transform.position, spotter.CurrentTarget.transform.position) > agent.stoppingDistance)
                {
                    //agent.enabled = true;
                    agent.stoppingDistance = spottedStoppingDistance;
                    agent.destination = spotter.CurrentTarget.transform.position;
                    Debug.Log($"Setting destination:{agent.destination}");
                }
                else
                {
                    Vector3 dir = Vector3.ProjectOnPlane(spotter.CurrentTarget.transform.position - transform.position, Vector3.up);
                    agent.stoppingDistance = spottedStoppingDistance;// * 1.5f;
                    
                    lookAtRotation = Quaternion.LookRotation(dir, Vector3.up);
                    agent.enabled = false;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAtRotation, agent.angularSpeed * Time.fixedDeltaTime);
                    agent.enabled = true;
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
