using HKR.Building;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HKR
{
    public class FloorRenderingController : MonoBehaviour
    {
        int tollerance = 1; 
        int blockCount = 0;

        Dictionary<int, List<BuildingBlock>> levels = new Dictionary<int, List<BuildingBlock>>();
        
        // Update is called once per frame
        void Update()
        {
            // Wait for the local palyer
            if (!PlayerController.Local)
                return;

            // Wait for blocks to spawn
            if (levels.Count == 0)
                return;

            // Check the floor the player stands in
            UpdateFloors();
        }

        private void OnEnable()
        {
            BuildingBlock.OnSpawned += HandleOnBlockSpawned;
        }

        private void OnDisable()
        {
            BuildingBlock.OnSpawned -= HandleOnBlockSpawned;
        }

        private void HandleOnBlockSpawned(BuildingBlock arg0)
        {
            blockCount++;
            if (blockCount == LevelManager.Instance.BlockCount)
            {
                // Get all blocks
                List<BuildingBlock> blocks = new List<BuildingBlock>(FindObjectsOfType<BuildingBlock>());

                // Fill the level dictionary
                foreach (BuildingBlock block in blocks)
                {
                    if(!levels.ContainsKey(block.FloorLevel))
                        levels.Add(block.FloorLevel, new List<BuildingBlock>());
                    
                    // Add the block 
                    levels[block.FloorLevel].Add(block);

                }
                
            }

        }

        void UpdateFloors()
        {
            int level = Utility.GetFloorLevelByVerticalCoordinate(PlayerController.Local.transform.position.y);

            // Enable block in the current level and disable blocks in the other levels depending on the tollerance
            foreach(var key in levels.Keys)
            {
                

                foreach (BuildingBlock block in levels[key])
                {
                    if (Mathf.Abs(key - level) <= tollerance)
                        block.GetComponent<BlockRenderingController>()?.Show();
                    else
                        block.GetComponent<BlockRenderingController>()?.Hide();
                }
                    
            }
            
            

        }
    }

}
