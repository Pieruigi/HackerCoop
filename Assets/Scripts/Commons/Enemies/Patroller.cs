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
    public class Patroller : MonoBehaviour
    {
        [SerializeField]
        float speed = 3f;

        SecurityStateController stateController;

        bool activated = false;

        NavMeshAgent agent;

        float stoppingDistance;

        private void Awake()
        {
            stateController = GetComponent<SecurityStateController>();
            agent = GetComponent<NavMeshAgent>();
            stoppingDistance = agent.stoppingDistance;
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
                // Get the current floor level
                int floorLevel = Utility.GetFloorLevelByVerticalCoordinate(transform.position.y);

                // Get all the blocks in the current floor
                IList<BuildingBlock> availables = BuildingBlock.Blocks;
                BuildingBlock currentBlock;
                // Remove the current block
                if(BuildingBlock.TryGetBlockByPoint(transform.position, out currentBlock))
                    availables.Remove(currentBlock);

                BuildingBlock nextBlock = availables[UnityEngine.Random.Range(0, availables.Count)];
                // Get a random destination point 
                Vector3 blockPos = nextBlock.transform.position;
                Vector3 destination = new Vector3(UnityEngine.Random.Range(blockPos.x, blockPos.x + BuildingBlock.Size), blockPos.y, UnityEngine.Random.Range(blockPos.z, blockPos.z + BuildingBlock.Size));
                // Set destination
                agent.destination = destination;
            }
            
        }

       
    }

}
