using Fusion;
using HKR.Building;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace HKR
{
    public class Patroller : NetworkBehaviour
    {
        [SerializeField]
        float speed = 3f;

        [SerializeField]
        float standByTime = 10;

        [SerializeField]
        float lookAroundAngularSpeed = 30;

        SecurityStateController stateController;

        bool activated = false;

        NavMeshAgent agent;

        float stoppingDistance;
        float standByElapsed = 0;
        bool lookAround = false;

        private void Awake()
        {
            stateController = GetComponent<SecurityStateController>();
            agent = GetComponent<NavMeshAgent>();
            stoppingDistance = agent.stoppingDistance;
            agent.speed = speed;
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

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
                return;
            if (!PlayerManager.Instance.PlayerInGameAll())
                return;
            if (!activated)
                return;
            switch(stateController.State) 
            {
                case SecurityState.Normal:
                    // Look around ???
                    if (lookAround)
                    {
                        agent.enabled = false;
                        transform.rotation *= Quaternion.Euler(0f, lookAroundAngularSpeed * Time.fixedDeltaTime, 0f);
                        agent.enabled = true;
                    }
                    
                    break;
            }
        }

        private void OnEnable()
        {
            stateController.OnStateChanged += HandleOnStateChanged;
            stateController.OnSpawned += HandleOnStateControllerSpawned;
        }

        private void OnDisable()
        {
            stateController.OnStateChanged -= HandleOnStateChanged;
            stateController.OnSpawned -= HandleOnStateControllerSpawned;
        }

        private void HandleOnStateControllerSpawned()
        {
            
            if (stateController.State == SecurityState.Normal)
                activated = true;
            
        }

        private void HandleOnStateChanged(SecurityState oldState, SecurityState newState)
        {
            switch(newState)
            {
                case SecurityState.Normal:
                    activated = true;
                    //agent.ResetPath();
                    agent.stoppingDistance = stoppingDistance;
                    agent.speed = speed;
                    agent.enabled = true;
                    standByElapsed = standByTime;
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
            if (!agent.isOnNavMesh)
                return;

            // If the agent already has a path then keep moving,
            // otherwise it hangs around for a while and then starts moving.
            if (!agent.hasPath)
            {
                
                standByElapsed += Time.deltaTime;
                if(standByElapsed >= standByTime)
                {
                    // Get the current floor level
                    int floorLevel = Utility.GetFloorLevelByVerticalCoordinate(transform.position.y);

                    // Get all the blocks in the current floor
                    IList<BuildingBlock> availables = BuildingBlock.Blocks;
                    BuildingBlock currentBlock;
                    // Remove the current block
                    if (BuildingBlock.TryGetBlockByPoint(transform.position, out currentBlock))
                        availables.Remove(currentBlock);

                    BuildingBlock nextBlock = availables[UnityEngine.Random.Range(0, availables.Count)];
                    // Get a random destination point 
                    Vector3 blockPos = nextBlock.transform.position;
                    Vector3 destination = new Vector3(UnityEngine.Random.Range(blockPos.x, blockPos.x + BuildingBlock.Size), blockPos.y, UnityEngine.Random.Range(blockPos.z, blockPos.z + BuildingBlock.Size));
                    // Set destination
                    agent.destination = destination;
                    // Set the rotation sign for the next time
                    lookAroundAngularSpeed *= UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
                }
                else
                {
                    if(lookAroundAngularSpeed != 0)
                    {
                        lookAround = true;
                       
                    }
                    
                }
                
            }
            else
            {
                standByElapsed = 0;
                lookAround = false;
            }
            
        }

       
    }

}
