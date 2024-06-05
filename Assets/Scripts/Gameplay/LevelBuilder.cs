
using DG.Tweening.Plugins.Options;
using ExitGames.Client.Photon;
using Fusion;
using HKR.Scriptables;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static Fusion.Sockets.NetBitBuffer;

namespace HKR.Building
{
    public enum FloorSize { Small, Medium, Large }

    public class LevelBuilder : MonoBehaviour
    {
        public static LevelBuilder Instance {  get; private set; }

        public const int MinFloorCount = 3;
        public const int MaxFloorCount = 5;

        public const int MinInfectedCount = 9;
        public const int MaxInfectedCount = 12;

        //#if BUILDING_TEST
        [SerializeField]
        GameObject helperBlockPrefab;
        //#endif

        [SerializeField]
        GameObject alarmSystemPrefab;

        
        
        int floorCount;
        //FloorSize floorSize = FloorSize.Small;
        int connectorCount = 3;
        int startingFloor = 0;
        
        //List<Vector2> blocksPerSize = new List<Vector2>(new Vector2[] { new Vector2(10, 12), new Vector2(14, 16), new Vector2(18, 20) });
        //List<int> minBlocksPerSize = new List<int>(new int[] { 20, 28, 36 });
        //List<int> maxBlocksPerSize = new List<int>(new int[] { 24, 32, 40 });

        int minBlocks = 80;
        int maxBlocks = 90;

        int blockCount;

        List<Floor> floors = new List<Floor>();
        List<FloorConnector> connectors = new List<FloorConnector>();

        [SerializeField]
        List<ShapeBlock> shapeBlocks = new List<ShapeBlock>();

        [SerializableType]
        List<Transform> infectionPoints = new List<Transform>();

        [SerializeField]
        GameObject flyingAgentNavMeshPrefab;

        [SerializeField]
        GameObject groundedAgentNavMeshPrefab;

        List<BuildingBlock> buildingBlocks = new List<BuildingBlock>();

        List<ShapeDrone> drones = new List<ShapeDrone>();

        List<ShapeDestroyer> destroyers = new List<ShapeDestroyer>();


        //Dictionary<Vector3, Camera> securityCameras = new List<Vector3>();

        int infectedBlockCount;

        int width, length;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.R)) 
            {
                ClearAll();
                int seed = (int)System.DateTime.Now.Ticks;
                Random.InitState(seed);
                Build();
            }
        }

        void ClearAll()
        {
            floors.Clear();
            connectors.Clear();
            int count = transform.childCount;
            for(int i = 0; i < count; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            shapeBlocks.Clear();

            foreach(var b in buildingBlocks)
                Destroy(b.gameObject);

            buildingBlocks.Clear();

        }
#endif

        public void Build()
        {
            Debug.Log($"Start building level with seed {Random.state}");

            // Create main schema
            CreateBuildingSchema();

            // Create logic structure
            BuildLogicStructure();

            // Create shape
            ShapeLevel();

            // Build geometry
            BuildGeometry();

        }

        void CreateNavMeshes()
        {
            // We attach the nav mesh to the initial block ( and all the block at the same coordinates in any other floor )
            // Get the entering block
            ShapeBlock enteringBlock = shapeBlocks.First(b => b.IsEnteringBlock);
            foreach(var f in floors)
            {
                // Get the block corresponding to the entering block on the current floor ( or the entering block itself )
                ShapeBlock b = enteringBlock;
                if(f.Level != 0)
                {
                    b = shapeBlocks.Find(b => b.Coordinates == enteringBlock.Coordinates && b.floor == f);
                }

                // Spawn object on network ( we need the navmesh to be baked on every client to support host migration )
                SessionManager.Instance.NetworkRunner.Spawn(groundedAgentNavMeshPrefab, b.GetPhysicalPosition(), Quaternion.identity);
                SessionManager.Instance.NetworkRunner.Spawn(flyingAgentNavMeshPrefab, b.GetPhysicalPosition(), Quaternion.identity);
            }
        }

        void BuildGeometry()
        {
            // Spawn floors
            SpawnFloors();

            // Create nav meshes
            CreateNavMeshes();

            // Spawn infected nodes
            SpawnInfectedNodes();

            // Alarm system controllers
            SpawnAlarmSystemControllers();

            // Spawn security cameras
            SpawnSecurityCameras();

            // Spawn drones
            SpawnSecurityDrones();

            // Spawn destroyers
            SpawnDestroyers();
        }

        void SpawnDestroyers()
        {
            foreach (var destroyer in destroyers)
            {
                SessionManager.Instance.NetworkRunner.Spawn(destroyer.Asset.Prefab, destroyer.transform.position, Quaternion.identity, null,
                        (r, o) =>
                        {
                            o.GetComponent<SecurityStateController>().FloorLevel = destroyer.Floor.Level;

                        });
            }
        }

        void SpawnSecurityDrones()
        {
            foreach(var drone in drones)
            {
                SessionManager.Instance.NetworkRunner.Spawn(drone.Asset.Prefab, drone.transform.position, Quaternion.identity, null,
                        (r, o) =>
                        {
                            o.GetComponent<SecurityStateController>().FloorLevel = drone.Floor.Level;
                          
                        });
            }

#if UNITY_EDITOR
            // We need the asset prefab
            //SessionManager.Instance.NetworkRunner.Spawn(block.SecurityCameraAsset.Prefab, position, rotation, null,
            //        (r, o) =>
            //        {
            //            o.GetComponent<SecurityStateController>().FloorLevel = block.floor.Level;
            //            //InfectionNodeController controller = o.GetComponent<InfectionNodeController>();
            //            //controller.FloorLevel = level;
            //            //controller.InfectionType = (byte)Random.Range(0, Constants.InfectionTypeCount);
            //        });

#endif

            //foreach (var block in shapeBlocks)
            //{
            //    if (block.SecurityCameraAsset)
            //    {
            //        // Position and rotation
            //        Vector3 position = block.GetPhysicalPosition() + new Vector3(BuildingBlock.Size / 2f, 0f, BuildingBlock.Size / 2f);
            //        Quaternion rotation = Quaternion.Euler(0, Random.Range(-180f, 180f), 0);

            //        SessionManager.Instance.NetworkRunner.Spawn(block.SecurityCameraAsset.Prefab, position, rotation, null,
            //        (r, o) =>
            //        {
            //            o.GetComponent<SecurityStateController>().FloorLevel = block.floor.Level;
            //            //InfectionNodeController controller = o.GetComponent<InfectionNodeController>();
            //            //controller.FloorLevel = level;
            //            //controller.InfectionType = (byte)Random.Range(0, Constants.InfectionTypeCount);
            //        });
            //    }
            //}
        }

        void SpawnSecurityCameras()
        {
            foreach(var block in shapeBlocks)
            {
                if (block.SecurityCameraAsset)
                {
                    // Position and rotation
                    Vector3 position = block.GetPhysicalPosition() + new Vector3(BuildingBlock.Size / 2f, 0f, BuildingBlock.Size / 2f);
                    Quaternion rotation = Quaternion.Euler(0, Random.Range(-180f, 180f), 0);

                    SessionManager.Instance.NetworkRunner.Spawn(block.SecurityCameraAsset.Prefab, position, rotation, null,
                    (r, o) =>
                    {
                        o.GetComponent<SecurityStateController>().FloorLevel = block.floor.Level;
                        //InfectionNodeController controller = o.GetComponent<InfectionNodeController>();
                        //controller.FloorLevel = level;
                        //controller.InfectionType = (byte)Random.Range(0, Constants.InfectionTypeCount);
                    });
                }
            }
        }

        void SpawnInfectedNodes()
        {
            // Load all the infected node assets
            List<InfectedNodeAsset> assets = new List<InfectedNodeAsset>(Resources.LoadAll<InfectedNodeAsset>(InfectedNodeAsset.ResourceFolder));
            Debug.Log($"SpawnInfectedNodes() - Loaded {assets.Count} asset(s) from resources.");
            // Spawn nodes
            foreach(var point in infectionPoints)
            {
                // Get floor level
                float objectY = point.position.y;
                int level = Utility.GetFloorLevelByVerticalCoordinate(objectY);  //Mathf.FloorToInt(objectY / BuildingBlock.Height);

                // Get a random asset
                InfectedNodeAsset currentAsset = assets[Random.Range(0, assets.Count)];
                // Spawn the prefab
                SessionManager.Instance.NetworkRunner.Spawn(currentAsset.Prefab, point.position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), null,
                    (r, o) =>
                    {
                        InfectionNodeController controller = o.GetComponent<InfectionNodeController>();
                        controller.FloorLevel = level;
                        controller.InfectionType = (byte)Random.Range(0, Constants.InfectionTypeCount);
                    });
            }
        }

        void SpawnAlarmSystemControllers()
        {
            for(int i=0; i<floors.Count; i++)
            {
                
                int level = floors[i].Level;

                SessionManager.Instance.NetworkRunner.Spawn(alarmSystemPrefab, Vector3.zero, Quaternion.identity, null,
                   (r, o) =>
                   {
                       o.GetComponent<AlarmSystemController>().FloorLevel = level;
                   });
            }
            
            
        }

        void SpawnFloors()
        {
            // Load resources
#if BUILDING_TEST
            string themeFolder = "HighTech";
#else
            string themeFolder = LevelManager.Instance.Theme;
#endif
            List<BuildingBlockAsset> blockAssets = new List<BuildingBlockAsset>( Resources.LoadAll<BuildingBlockAsset>(System.IO.Path.Combine(BuildingBlockAsset.ResourceFolder, themeFolder)));

            Debug.Log($"Found {blockAssets.Count} blocks in resources");

            BuildingBlockAsset test = blockAssets[3];
            
            List<Transform> tList = test.Prefab.GetComponentsInChildren<Transform>().Where(t=>t.CompareTag(Tags.InfectionNode)).ToList();
           
            foreach (var b in shapeBlocks)
            {

                // Get the asset name prefix
                //string namePrefix = GetBlockNamePrefix(b);
                //Debug.Log($"Shape:{b.name}, Prefix:{namePrefix}");

                //List<BuildingBlockAsset> candidates = blockAssets.Where(bb => bb.name.ToLower().StartsWith(namePrefix.ToLower())).ToList();
                //BuildingBlockAsset chosenAsset = candidates[Random.Range(0, candidates.Count)];
                BuildingBlockAsset chosenAsset = b.blockAsset;

                 Vector3 position = GetBuildingBlockPosition(b.floor, b.Coordinates);
                float geometryAngle = GetGeometryRootAngle(b);
                int configurationId = 0;
                SpawnBuildingBlock(b, chosenAsset.Prefab, position, geometryAngle, configurationId);

                
            }

            //// Arrange all the blocks under some root objects
            //// Create the base root
            //GameObject floorRoot = new GameObject("FloorRoot");
            //// For each floor create a new game object as root child
            //for(int i = 0; i< floors.Count; i++)
            //{
            //    var fObj = new GameObject($"Floor-{i}");
            //    fObj.transform.parent = floorRoot.transform;
            //}
                
        }

       
        string GetBlockNamePrefix(ShapeBlock block)
        {
            string namePrefix = "";
            if (!block.IsEnteringBlock && !block.IsConnectorBlock)
            {
                if (block.IsColumnBlock)
                    namePrefix = "3_";
                else
                    namePrefix = "0_";
            }
            else
            {
                if (block.IsEnteringBlock)
                {
                    namePrefix = "1_";
                }
                else
                {
                    namePrefix = $"2_{block.ConnectorType.ToString()}_";
                }
            }

           
            // Check walls
            int wallCount = 0;
            if(block.IsNorthBorder)
                wallCount++;
            if (block.IsSouthBorder)   
                wallCount++;
            if (block.IsWestBorder)
                wallCount++;
            if (block.IsEastBorder)
                wallCount++;

            switch(wallCount)
            {
                case 0:
                    namePrefix += "F_";
                    break;
                case 1:
                    if (!block.IsEnteringBlock)
                    {
                        namePrefix += "N_";
                    }
                    else
                    {
                        if (block.IsEastBorder)
                            namePrefix += "E_";
                        else
                            namePrefix += "W_";
                    }
                    break;
                case 2:
                    if (!block.IsEnteringBlock)
                    {
                        if ((block.IsNorthBorder && block.IsSouthBorder) || (block.IsWestBorder && block.IsEastBorder))
                            namePrefix += "NS_";
                        else
                            namePrefix += "NE_";
                    }
                    else
                    {
                        namePrefix += "EW_";
                    }
                    break;
                case 3:
                    namePrefix += "NES_";
                    break;
            }
          
            

            return namePrefix;
        }

        float GetGeometryRootAngle(ShapeBlock block)
        {
            
            if(block.IsEnteringBlock)
                return 0;

            int wallCount = 0;
            if(block.IsNorthBorder)
                wallCount++;
            if (block.IsSouthBorder)
                wallCount++;
            if (block.IsWestBorder)
                wallCount++;
            if (block.IsEastBorder)
                wallCount++;

            if(wallCount == 0)
                return 0;
            if(wallCount == 1)
            {
                if (block.IsNorthBorder)
                    return 0;
                if (block.IsEastBorder)
                    return 90;
                if (block.IsSouthBorder)
                    return 180;
                if (block.IsWestBorder)
                    return 270;
            }
            if(wallCount == 2)
            {
                if((block.IsNorthBorder && block.IsSouthBorder) || (block.IsNorthBorder && block.IsEastBorder))
                    return 0;
                if ((block.IsEastBorder && block.IsWestBorder) || (block.IsEastBorder && block.IsSouthBorder))
                    return 90;
                if (block.IsSouthBorder && block.IsWestBorder)
                    return 180;
                if (block.IsWestBorder && block.IsNorthBorder)
                    return 270;
            }

            if(wallCount == 3)
            {
                if (block.IsNorthBorder && block.IsEastBorder && block.IsSouthBorder)
                    return 0;
                if (block.IsEastBorder && block.IsSouthBorder && block.IsWestBorder)
                    return 90;
                if (block.IsSouthBorder && block.IsWestBorder && block.IsNorthBorder)
                    return 180;
                if (block.IsWestBorder && block.IsNorthBorder && block.IsEastBorder)
                    return 270;
            }
            
            return 0;
        }

        Vector3 GetBuildingBlockPosition(Floor floor, Vector2 coordinates)
        {
            Vector3 pos = Vector3.zero;
            pos.y = floor.Level * BuildingBlock.Height;
            pos.x = coordinates.x * BuildingBlock.Size;
            pos.z = coordinates.y * BuildingBlock.Size;
            return pos;
        }

        void SpawnBuildingBlock(ShapeBlock shapeBlock, GameObject blockPrefab, Vector3 position, float geometryAngle, int configurationId)
        {
#if BUILDING_TEST
            GameObject block = Instantiate(blockPrefab);
            BuildingBlock buildingBlock= block.GetComponent<BuildingBlock>();
            buildingBlock.GeometryRootAngle = geometryAngle;
            buildingBlocks.Add(buildingBlock);
            block.transform.position = position;
            buildingBlock.Spawned();
#else
SessionManager.Instance.NetworkRunner.Spawn(blockPrefab, position, Quaternion.identity, null, (r, o) =>
            {
                BuildingBlock bblock = o.GetComponent<BuildingBlock>(); 
                bblock.GeometryRootAngle = geometryAngle;
                bblock.FloorLevel = shapeBlock.floor.Level;
                if(shapeBlock.IsConnectorBlock)
                {
                    var connector = connectors[shapeBlock.ConnectorIndex];
                    foreach(var f in connector.Floors)
                    {
                        (bblock as ConnectorBlock).ConnectedFloorLevels.Add(f.Level);
                    }
                }
                
            });
#endif


        }

        void ShapeLevel()
        {
            //blockCount = Random.Range(minBlocksPerSize[(int)floorSize], maxBlocksPerSize[(int)floorSize] + 1);
            blockCount = Random.Range(minBlocks, maxBlocks) / floorCount;
            Debug.Log($"CreateShape() - blockCount:{blockCount}");
            // Width must be greater than length
            float widthLengthRatio = Random.Range(.7f, 1.5f);//Random.Range(1.5f, 3f);
            // width * length = blockCount => r*length * length = blockCount => length = Sqrt(blockCount/r)
            length = (int)Mathf.Sqrt(blockCount / widthLengthRatio);
            width = blockCount / length;
            // Check for remaining blocks
            int left = blockCount - (width * length);
            Debug.Log($"CreateShape() - width:{width}, length:{length}, diff:{left}");



            //
            // Create shape blocks.
            //
            // Create the root
            GameObject root = new GameObject("Shape");
            root.transform.parent = transform;


            for (int i = 0; i < blockCount - left; i++)
            {
                GameObject block = CreateShapeBlock();
                ShapeBlock bb = block.GetComponent<ShapeBlock>();
                block.transform.parent = root.transform;
                Vector2 coords = new Vector2(i % width, i / width);
                bb.SetCoordinates(coords);
                bb.name = $"S_{bb.Coordinates}";

            }
            
            //
            // Twist the main shape
            //
            bool symmetrical = Random.Range(0, 2) == 0;
            //symmetrical = true;
            TwistShape(symmetrical, width, length);

            // Set border flags
            ComputeBorders();

            
            // Create some holes in the section shape
            CreateHoles(symmetrical, width, length);

            // Compute borders again to account for the new blocks
            ComputeBorders();

            //
            // Add remaining blocks
            //
            AddRemainingBlocks(left, root);

            // Set border flags
            ComputeBorders();

            // Choose entering 
            ChooseEnteringBlock(root);

            // Choose connectors
            ChooseConnectorBlocks(root);

            // Choose columns
            ChooseColumnBlocks(root);

            // Replicate the shape for each floor
            ShapeRemainingFloors(root);

            // Assign an asset to each block
            AssignAssetToShapeBlocks();

            // Fill shape blocks with spawn points
            FillShapeBlocks();

            // Choose infected nodes
            ChooseInfectedNodes();

            // Choose security camera positions
            //ChooseSecurityCameras();

            // Choose drones
            //ChooseSecurityDrones();
            
            // Destroyers
            //ChooseDestroyers();

            foreach (var b in shapeBlocks)
            {

                b.Move();
#if BUILDING_TEST
                b.Colorize();
#endif
            }

            // At this point we know exactly how many blocks we are going to spawn
            LevelManager.Instance.BlockCount = shapeBlocks.Count;
        }

        void ChooseDestroyers()
        {
            // Load all the available assets
            List<DestroyerAsset> assets = new List<DestroyerAsset>(Resources.LoadAll<DestroyerAsset>(DestroyerAsset.ResourceFolder));
            Debug.Log($"ChooseDestroyers() - Loaded {assets.Count} asset(s) from resources.");
            // Take into account weight
            List<DestroyerAsset> tmp = new List<DestroyerAsset>();
            foreach (var asset in assets)
            {
                Debug.Log($"Asset, name:{asset.name}, weight:{asset.Weight}");
                for (int i = 0; i < asset.Weight; i++)
                    tmp.Add(asset);
            }
            assets = tmp;
#if _UNITY_EDITOR
            // Just add a drone to the entering level ( for testing purpose )
            List<ShapeBlock> tmpB = shapeBlocks.Where(b => b.floor.Level == 0 && b.IsEnteringBlock).ToList();
            ShapeBlock chosenBlock = tmpB[0];
            var chosenAsset = assets[UnityEngine.Random.Range(0, assets.Count)];
            // Starting from the middle of the block we look for an available point on the navmesh
            Vector3 spawnPosition = chosenBlock.GetPhysicalPosition() + Vector3.right * BuildingBlock.Size / 2f + Vector3.forward * BuildingBlock.Size / 2f + Vector3.up * BuildingBlock.Height * .5f;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, BuildingBlock.Size * .5f, chosenAsset.Prefab.GetComponent<NavMeshAgent>().areaMask))
                spawnPosition = hit.position;

            Debug.Log($"Destroyer spawn position:{spawnPosition}");
            
            // Instantiate the helper 
            GameObject destroyer = new GameObject("Destroyer");
            destroyer.transform.position = spawnPosition;
            var comp = destroyer.AddComponent<ShapeDestroyer>();
            comp.Init(chosenAsset, floors.First(f=>f.Level == 0));
            destroyers.Add(comp);
#else
            foreach (var floor in floors)
            {
                // We use blocks to get the spawn point only ( drone are free to fly through the entire floor )
                List<ShapeBlock> blocks = shapeBlocks.Where(b => b.floor == floor && !b.IsEnteringBlock && !b.IsConnectorBlock).ToList();
                float ratioMin = .08f;
                float ratioMax = .12f;
                int count = Mathf.RoundToInt(Random.Range(ratioMin, ratioMax) * blocks.Count);

                for (int i = 0; i < count; i++)
                {
                    // Get a random block
                    var chosen = blocks[UnityEngine.Random.Range(0, blocks.Count)];
                    // Remove the block
                    blocks.Remove(chosen);
                    // Choose a drone amongs the assets
                    var chosenAsset = assets[UnityEngine.Random.Range(0, assets.Count)];
                    // Get a random point around the chosen block
                    Vector3 spawnPosition = chosen.GetPhysicalPosition() + Vector3.right * BuildingBlock.Size / 2f + Vector3.forward * BuildingBlock.Size / 2f + Vector3.up * BuildingBlock.Height * .5f;
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(spawnPosition, out hit, BuildingBlock.Size * .5f, chosenAsset.Prefab.GetComponent<NavMeshAgent>().areaMask))// 1 << 0))
                        spawnPosition = hit.position;

                    
                    // Instantiate the helper 
                    GameObject destroyer = new GameObject("Destroyer");
                    destroyer.transform.position = spawnPosition;
                    var comp = destroyer.AddComponent<ShapeDestroyer>();
                    comp.Init(chosenAsset, floor);
                    destroyers.Add(comp);
                }

            }
#endif

        }

        void ChooseSecurityDrones()
        {
            
            // Load all the available assets
            List<SecurityDroneAsset> assets = new List<SecurityDroneAsset>(Resources.LoadAll<SecurityDroneAsset>(SecurityDroneAsset.ResourceFolder));
            Debug.Log($"ChooseSecurityDrones() - Loaded {assets.Count} asset(s) from resources.");
            // Take into account weight
            List<SecurityDroneAsset> tmp = new List<SecurityDroneAsset>();
            foreach (var asset in assets)
            {
                Debug.Log($"Asset, name:{asset.name}, weight:{asset.Weight}");
                for (int i = 0; i < asset.Weight; i++)
                    tmp.Add(asset);
            }
            assets = tmp;
            Debug.Log($"SecurityDrones asset count:{assets.Count}");
#if _UNITY_EDITOR
            // Just add a drone to the entering level ( for testing purpose )
            List<ShapeBlock> tmpB = shapeBlocks.Where(b => b.floor.Level == 0 && b.IsEnteringBlock).ToList();
            ShapeBlock chosenBlock = tmpB[0];
            var chosenDrone = assets[UnityEngine.Random.Range(0, assets.Count)];
            // Starting from the middle of the block we look for an available point on the navmesh
            Vector3 spawnPosition = chosenBlock.GetPhysicalPosition() + Vector3.right * BuildingBlock.Size / 2f + Vector3.forward * BuildingBlock.Size / 2f + Vector3.up * BuildingBlock.Height * .75f;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, BuildingBlock.Size * .5f, chosenDrone.Prefab.GetComponent<NavMeshAgent>().areaMask))//1 << 3))
                spawnPosition = hit.position;

            
            // Instantiate the helper 
            GameObject drone = new GameObject("Drone");
            drone.transform.position = spawnPosition;
            var comp = drone.AddComponent<ShapeDrone>();
            comp.Init(chosenDrone, floors.First(f=>f.Level == 0));
            drones.Add(comp);
#else
            foreach(var floor in floors)
            {
                // We use blocks to get the spawn point only ( drone are free to fly through the entire floor )
                List<ShapeBlock> blocks = shapeBlocks.Where(b => b.floor == floor && !b.IsEnteringBlock && !b.IsConnectorBlock).ToList();
                float ratioMin = .15f;
                float ratioMax = .2f;
                int count = Mathf.RoundToInt(Random.Range(ratioMin, ratioMax) * blocks.Count);

                for(int i=0; i<count; i++)
                {
                    // Get a random block
                    var chosen = blocks[UnityEngine.Random.Range(0, blocks.Count)];
                    // Remove the block
                    blocks.Remove(chosen);
                    // Get a random point around the chosen block
                    Vector3 spawnPosition = chosen.GetPhysicalPosition() + Vector3.right * BuildingBlock.Size / 2f + Vector3.forward * BuildingBlock.Size / 2f + Vector3.up * BuildingBlock.Height * .75f;
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(spawnPosition, out hit, BuildingBlock.Size * .5f, 1 << 3))
                        spawnPosition = hit.position;

                    // Choose a drone amongs the assets
                    var chosenDrone = assets[UnityEngine.Random.Range(0, assets.Count)];
                    // Instantiate the helper 
                    GameObject drone = new GameObject("Drone");
                    drone.transform.position = spawnPosition;
                    var comp = drone.AddComponent<ShapeDrone>();
                    comp.Init(chosenDrone, floor);
                    drones.Add(comp);
                }
                
            }
#endif
        }

        void ChooseSecurityCameras()
        {
           
            // Load all the available assets
            List<SecurityCameraAsset> assets = new List<SecurityCameraAsset>(Resources.LoadAll<SecurityCameraAsset>(SecurityCameraAsset.ResourceFolder));
            Debug.Log($"SpawnSecurityCameras() - Loaded {assets.Count} asset(s) from resources.");

            List<SecurityCameraAsset> tmp = new List<SecurityCameraAsset>();
            foreach (var asset in assets)
            {
                for (int i = 0; i < asset.Weight; i++)
                    tmp.Add(asset);
            }
            assets = tmp;

#if _UNITY_EDITOR
            // Testing the entering block
            ShapeBlock block = shapeBlocks.Where(s=>s.IsEnteringBlock).First();
            block.SecurityCameraAsset = assets[Random.Range(0, assets.Count)];
            return;
#endif
            foreach(var floor in floors)
            {
                // Get all the blocks in the current floor ( except for entering and connection blocks )
                List<ShapeBlock> blocks = shapeBlocks.Where(b=>b.floor == floor && !b.IsEnteringBlock && !b.IsConnectorBlock).ToList();
                float ratioMin = .20f;
                float ratioMax = .30f;
                int count = Mathf.RoundToInt(Random.Range(ratioMin, ratioMax) * blocks.Count);
                for(int i=0; i<count; i++)
                {
                    // Choose a new block to set the security camera in
                    ShapeBlock b = blocks[Random.Range(0, blocks.Count)];
                    // Remove the block from the list
                    blocks.Remove(b);
                    // Add the camera
                    b.SecurityCameraAsset = assets[Random.Range(0, assets.Count)];
                }
            }
        }

        void FillShapeBlocks()
        {
            foreach(var sb in shapeBlocks)
            {
                // Get the root object
                Transform root = sb.transform.GetChild(0);
                // Get the asset
                BuildingBlockAsset asset = sb.blockAsset;
                // Get infection spawn points from the prefab
                List<Transform> points = asset.Prefab.GetComponentsInChildren<Transform>().Where(t=>t.CompareTag(Tags.InfectionNode)).ToList();
                
                foreach(var p in points)
                {
                    // Create a new empty object
                    GameObject pObj = new GameObject("InfectionPoint");
                    pObj.tag = Tags.InfectionNode;
                    // Assign the root as parent of the current node
                    pObj.transform.parent = root;
                    // Adjust position and rotation
                    pObj.transform.localPosition = p.localPosition;
                    pObj.transform.localRotation = p.localRotation;
                }

                // Rotate the root depending on the block orientation
                float angle = GetGeometryRootAngle(sb);
                root.rotation = Quaternion.Euler(0f, angle, 0f);

                
            }
        }

        void AssignAssetToShapeBlocks()
        {
            // Load assets
#if BUILDING_TEST
            string themeFolder = "HighTech";
#else
            string themeFolder = LevelManager.Instance.Theme;
#endif
            List<BuildingBlockAsset> blockAssets = new List<BuildingBlockAsset>(Resources.LoadAll<BuildingBlockAsset>(System.IO.Path.Combine(BuildingBlockAsset.ResourceFolder, themeFolder)));
          
            // Assign to each shape block
            foreach (var sb in shapeBlocks)
            {
                string namePrefix = GetBlockNamePrefix(sb);
                Debug.Log($"Shape:{sb.name}, Prefix:{namePrefix}");

                List<BuildingBlockAsset> candidates = blockAssets.Where(bb => bb.name.ToLower().StartsWith(namePrefix.ToLower())).ToList();
                BuildingBlockAsset chosenAsset = candidates[Random.Range(0, candidates.Count)];
                sb.blockAsset = chosenAsset;
            }
        }

        void ChooseInfectedNodes()
        {
            // How many infected nodes are in the whole building
            infectedBlockCount = Random.Range(MinInfectedCount, MaxInfectedCount + 1);
            // Distribute infected nodes among floors
            int[] floorInfections = new int[floors.Count];
            for (int i = 0; i < infectedBlockCount; i++)
            {
                int index = Random.Range(0, floors.Count);
                floorInfections[index]++;
            }

            // Loop through each floor and fill them with infected nodes
            for (int i = 0; i < floorInfections.Length; i++)
            {
                // Get all the available infection spawn points for the current floor.
                // We've already spawned all the infection spawn points in every block, so we just look for them using tag.
                List<ShapeBlock> blocks = shapeBlocks.Where(s=>s.floor == floors[i]).ToList();
                List<Transform> spawnList = new List<Transform>();
                foreach (var b in blocks)
                    spawnList.AddRange(b.GetComponentsInChildren<Transform>().Where(t=>t.CompareTag(Tags.InfectionNode)));
    

                for (int j = 0; j < floorInfections[i]; j++)
                {
                    // Get a random node
                    Transform point = spawnList[Random.Range(0, spawnList.Count)];
                    // We don't want to use the same node twice, so remove it from the list
                    spawnList.Remove(point);
                    // Add the new infected node to the list
                    infectionPoints.Add(point);
                }

            }

        }

        

        void ShapeRemainingFloors(GameObject root)
        {
            int count = shapeBlocks.Count;
            for(int i= 0; i<floors.Count; i++)
            {
                
                for (int j = 0; j < count; j++)
                {
                    ShapeBlock s = null;
                    if (i == 0) 
                    {
                        // Get the original shape block
                        s = shapeBlocks[j];
                    }
                    else
                    {
                        // Create a new shape block
                        s = Instantiate(shapeBlocks[j]);
                        s.transform.parent = root.transform;
                        shapeBlocks.Add(s);
                    }

                    s.floor = floors[i];
                    Debug.Log($"SFloor: {s.floor.name}, level:{s.floor.Level}");
                    
                }
            }

            foreach(var s in shapeBlocks)
            {
                if (s.IsEnteringBlock && s.floor.Level != 0)
                {
                    s.IsEnteringBlock = false;
                    s.IsSouthBorder = true;
                }
            }

        }

        void ChooseColumnBlocks(GameObject root)
        {
            // Get all blocks except for entering and connector blocks
            List<ShapeBlock> availables = shapeBlocks.Where(b=>!b.IsEnteringBlock && !b.IsConnectorBlock).ToList();
            int columnCount = Mathf.RoundToInt(availables.Count * .2f);
            while(availables.Count > 0 && columnCount > 0)
            {
                ShapeBlock chosen = availables[Random.Range(0, availables.Count)];
                chosen.IsColumnBlock = true;
                //availables.Remove(chosen);
                // Remove the current block and also all the blocks that are too close
                int minDist = 2;
                availables.RemoveAll(b => Vector2.Distance(chosen.Coordinates, b.Coordinates) < minDist);
                columnCount--;
            }
            
        }

        void ChooseConnectorBlocks(GameObject root)
        {
            bool useWidth = width > length;

            // Get the minimum and maximum X coordinates
            float xMin = useWidth ? shapeBlocks.Min(b => b.Coordinates.x) : shapeBlocks.Min(b => b.Coordinates.y);
            float xMax = useWidth ? shapeBlocks.Max(b => b.Coordinates.x) : shapeBlocks.Max(b => b.Coordinates.y);

            //
            // Choose connectors
            //

            float min = xMin;
            float max = xMax;
            float middle = (xMax + xMin) / 2;
            //float offset = (xMax - xMin) / connectorCount;
            for (int i=0; i<connectorCount; i++)
            {
                switch (i)
                {
                    case 0:
                        min = xMin;
                        max = min + 2;
                        break;
                    case 1:
                        if(connectorCount == 3)
                        {
                            min = middle - 1;
                            max = middle + 1;
                        }
                        else
                        {
                            min = xMax - 2;
                            max = xMax;
                        }
                        break;
                    case 2:
                        min = xMax - 2;
                        max = xMax;
                        break;
                }
               
                Debug.Log($"ChooseConnectorBlocks() - X coordinates, min:{min}, max:{max}");

                // Get candidates
                List<ShapeBlock> blocks = shapeBlocks.Where(b => (useWidth ? b.Coordinates.x : b.Coordinates.y) >= min && (useWidth ? b.Coordinates.x : b.Coordinates.y) <= max && !b.IsEnteringBlock).ToList();
                ShapeBlock chosen = blocks[Random.Range(0, blocks.Count)];
                
                int connType = Random.Range(0, 2); // 0: staircase, 1:elevator
                connType = 0; // We will add elavator later

                if (!chosen.IsBorder || Random.Range(0,2) == 0 || true )
                {
                    // We set the old block itself as connector
                    chosen.IsConnectorBlock = true;
                    chosen.ConnectorType = connType;
                    chosen.ConnectorIndex = i;

//#if BUILDING_TEST
//                    chosen.Colorize();
//#endif
                }
                else
                {
                    // We create a new block as connector
                    GameObject con = Instantiate(helperBlockPrefab, root.transform);
                    ShapeBlock conBlock = con.GetComponent<ShapeBlock>();
                    conBlock.IsConnectorBlock = true;
                    conBlock.ConnectorType = connType;
                    conBlock.ConnectorIndex = i;
                    shapeBlocks.Add(conBlock);
                    // Get all the block borders
                    List<int> dirs = new List<int>();
                    if(chosen.IsNorthBorder)
                        dirs.Add(0);
                    if (chosen.IsEastBorder)
                        dirs.Add(1);
                    if (chosen.IsSouthBorder)
                        dirs.Add(2);
                    if (chosen.IsWestBorder)
                        dirs.Add(3);
                    // Choose the direction we want to apply the connector to
                    int dir = dirs[Random.Range(0, dirs.Count)];
                    switch(dir)
                    {
                        case 0: // North
                            conBlock.IsNorthBorder = conBlock.IsEastBorder = conBlock.IsWestBorder = true;
                            conBlock.IsSouthBorder = false;
                            conBlock.SetCoordinates(chosen.Coordinates + new Vector2(0, 1));
                            chosen.IsNorthBorder = false;
                            break;
                        case 1: // East
                            conBlock.IsNorthBorder = conBlock.IsEastBorder = conBlock.IsSouthBorder = true;
                            conBlock.IsWestBorder = false;
                            conBlock.SetCoordinates(chosen.Coordinates + new Vector2(1, 0));
                            chosen.IsEastBorder = false;
                            break;
                        case 2: // South
                            conBlock.IsSouthBorder = conBlock.IsEastBorder = conBlock.IsWestBorder = true;
                            conBlock.IsNorthBorder = false;
                            conBlock.SetCoordinates(chosen.Coordinates + new Vector2(0, -1));
                            chosen.IsSouthBorder = false;
                            break;
                        case 3: // West
                            conBlock.IsNorthBorder = conBlock.IsWestBorder = conBlock.IsSouthBorder = true;
                            conBlock.IsEastBorder = false;
                            conBlock.SetCoordinates(chosen.Coordinates + new Vector2(-1, 0));
                            chosen.IsWestBorder = false;
                            break;
                    }
//                    conBlock.Move();
//#if BUILDING_TEST
//                    conBlock.Colorize();
//#endif


                }


                

            }
            
            
        }

        void ChooseEnteringBlock(GameObject root)
        {
            // Get all the blocks that are south borders
            List<ShapeBlock> blocks = shapeBlocks.Where(b => b.IsSouthBorder && !shapeBlocks.Exists(b2=>b2.Coordinates.x == b.Coordinates.x && b2.Coordinates.y < b.Coordinates.y)).ToList();
            // Choose a random block
            ShapeBlock block = blocks[Random.Range(0, blocks.Count)];
            block.IsSouthBorder = false;
            // Create a new block for the entering
            GameObject newBlock = CreateShapeBlock();
            newBlock.transform.parent = root.transform;
            ShapeBlock enterBlock = newBlock.GetComponent<ShapeBlock>();
            //shapeBlocks.Add(enterBlock);
            // The entering block is oriented towards north ever
            enterBlock.IsSouthBorder = false;
            enterBlock.IsNorthBorder = false;
            enterBlock.IsWestBorder = enterBlock.IsEastBorder = true;
            enterBlock.IsEnteringBlock = true;
            enterBlock.SetCoordinates(block.Coordinates + new Vector2(0, -1));
            // Check if there is any block attached to the entering block to the east or the west
            // East
            ShapeBlock other = shapeBlocks.Find(b => b.Coordinates == enterBlock.Coordinates + new Vector2(1, 0));
            if (other)
            {
                other.IsWestBorder = false;
                enterBlock.IsEastBorder = false;
            }

            other = shapeBlocks.Find(b => b.Coordinates == enterBlock.Coordinates + new Vector2(-1, 0));
            if (other)
            {
                other.IsEastBorder = false;
                enterBlock.IsWestBorder = false;
            }

        }

        void AddRemainingBlocks(int left, GameObject root)
        {
            for (int i = 0; i < left; i++)
            {

                GameObject block = CreateShapeBlock();
                block.transform.parent = root.transform;
                ShapeBlock leftBlock = block.GetComponent<ShapeBlock>();
                // Get all border blocks
                List<ShapeBlock> blocks = shapeBlocks.Where(b => b.IsBorder).ToList();
                // Get a random block
                var b = blocks[Random.Range(0, blocks.Count)];
                // Choose a random one if many
                List<int> sides = new List<int>();
                if (b.IsNorthBorder)
                    sides.Add(0);
                if (b.IsEastBorder)
                    sides.Add(1);
                if (b.IsSouthBorder)
                    sides.Add(2);
                if (b.IsWestBorder)
                    sides.Add(3);
                int side = sides[Random.Range(0, sides.Count)];
                Vector2 coords = b.Coordinates;
                switch (side)
                {
                    case 0: // North
                        coords.y++;
                        break;
                    case 1: // East
                        coords.x++;
                        break;
                    case 2: // South
                        coords.y--;
                        break;
                    case 3: // West
                        coords.x--;
                        break;
                }
                leftBlock.SetCoordinates(coords);
                leftBlock.name = $"S_{leftBlock.Coordinates}";
            }
        }

        void CreateHoles(bool symmetrical, int width, int length)
        {
            List<ShapeBlock> candidates = shapeBlocks.Where(b=>!b.IsBorder && 
                                                              (!symmetrical || (width % 2 == 0 && b.Coordinates.x < width/2) || (width % 2 == 1 && b.Coordinates.x <= width / 2)) &&
                                                              (shapeBlocks.Exists(c=>c.Coordinates.x == b.Coordinates.x - 1 && c.Coordinates.y == b.Coordinates.y +1)) && 
                                                              (shapeBlocks.Exists(c => c.Coordinates.x == b.Coordinates.x && c.Coordinates.y == b.Coordinates.y + 1)) &&
                                                              (shapeBlocks.Exists(c => c.Coordinates.x == b.Coordinates.x + 1 && c.Coordinates.y == b.Coordinates.y + 1)) &&
                                                              (shapeBlocks.Exists(c => c.Coordinates.x == b.Coordinates.x - 1 && c.Coordinates.y == b.Coordinates.y)) &&
                                                              (shapeBlocks.Exists(c => c.Coordinates.x == b.Coordinates.x + 1 && c.Coordinates.y == b.Coordinates.y)) &&
                                                              (shapeBlocks.Exists(c => c.Coordinates.x == b.Coordinates.x - 1 && c.Coordinates.y == b.Coordinates.y - 1)) &&
                                                              (shapeBlocks.Exists(c => c.Coordinates.x == b.Coordinates.x && c.Coordinates.y == b.Coordinates.y - 1)) &&
                                                              (shapeBlocks.Exists(c => c.Coordinates.x == b.Coordinates.x + 1 && c.Coordinates.y == b.Coordinates.y - 1))
                                                              ).ToList();
            // Blocks to be removed must be surrounded in all the directions
            

            Debug.Log($"CreateHoles() - Candidates.Count:{candidates.Count}");
            int maxHoleCount =  candidates.Count;
            if (symmetrical)
            {
                int middleCount = 0;
                if (width % 2 == 1)
                    middleCount = candidates.Count(b => b.Coordinates.x == width / 2);
                
                maxHoleCount -= middleCount;
                maxHoleCount /= 2;
                maxHoleCount += middleCount;
            }

            int holeCount = Random.Range(0, maxHoleCount+1);
            Debug.Log($"CreateHoles() - Holes.Count:{holeCount}");

            for (int i = 0; i < holeCount; i++)
            {
                var block = candidates[Random.Range(0, candidates.Count)];
                // Remove the block
                candidates.Remove(block);
                int dir = -1;
                bool doSymBlock = false;
                if(!symmetrical)
                {
                    dir = Random.Range(0, 4);
                }
                else
                {
                    if(block.Coordinates.x == width / 2) 
                    {
                        // Is the block in the middle
                        dir = Random.Range(0, 2);
                        if (dir == 1)
                            dir = 2; // South
                    }
                    else
                    {
                        doSymBlock = true;
                        dir = Random.Range(0,3);
                        if(dir == 1)
                            dir = 3; // West
                    }
                }
                Debug.Log($"CreateHoles() - block:{block.gameObject.name}");
                Debug.Log($"CreateHoles() - dir:{dir}");

                switch (dir)
                {
                    case 0: // North
                        List<ShapeBlock> col = shapeBlocks.Where(b=>b.Coordinates.x == block.Coordinates.x).ToList();
                        float max = col.Max(b=>b.Coordinates.y);
                        Debug.Log($"CreateHoles() - North, max:{max}");
                        block.SetCoordinates(new Vector2(block.Coordinates.x, max + 1));
                        break;
                    case 2: // South
                        col = shapeBlocks.Where(b => b.Coordinates.x == block.Coordinates.x).ToList();
                        float min = col.Min(b => b.Coordinates.y);
                        Debug.Log($"CreateHoles() - South, min:{min}");
                        block.SetCoordinates(new Vector2(block.Coordinates.x, min - 1));
                        break;
                    case 1: // East
                        List<ShapeBlock> row = shapeBlocks.Where(b => b.Coordinates.y == block.Coordinates.y).ToList();
                        max = row.Max(b => b.Coordinates.x);
                        Debug.Log($"CreateHoles() - East, max:{max}");
                        block.SetCoordinates(new Vector2(max + 1, block.Coordinates.y));
                        break;
                    case 3: // West
                        row = shapeBlocks.Where(b => b.Coordinates.y == block.Coordinates.y).ToList();
                        min = row.Min(b => b.Coordinates.x);
                        Debug.Log($"CreateHoles() - West, min:{min}");
                        block.SetCoordinates(new Vector2(min - 1, block.Coordinates.y));
                        break;

                }
            }

        }

        void TwistShape(bool symmetrical, int width, int length)
        {
            // Twist values
            List<int> moves = new List<int>();
            for (int k = 0; k < 3; k++)
                moves.Add(0);

            for (int j = 1; j < length; j++)
            {
                moves.Add(j);
                moves.Add(-j);
            }
            List<ShapeBlock> firstRow = shapeBlocks.Where(b => b.Coordinates.y == 0).ToList();
            Debug.Log($"FirstRow.Count:{firstRow.Count}");
            int count = symmetrical ? firstRow.Count / 2 : firstRow.Count;
            for (int i = 0; i < count; i++)
            {
                ShapeBlock current = firstRow[i];
                // Allign the current block with the previous one if any
                float diff = 0;
                if (i > 0)
                    diff = firstRow[i - 1].Coordinates.y - current.Coordinates.y;

                int r = moves[Random.Range(0, moves.Count)];
                if (r != 0 || diff != 0)
                {
                    // Move the current block
                    current.SetCoordinates(current.Coordinates + new Vector2(0, r + diff));
                    // Move all blocks in the same column of the current one
                    List<ShapeBlock> col = shapeBlocks.Where(b => b.Coordinates.x == current.Coordinates.x).ToList();
                    foreach (var b in col)
                    {
                        if (b != current)
                            b.SetCoordinates(b.Coordinates + new Vector2(0, r + diff));

                    }
                }

                // Move the symmetrical column ?
                if (symmetrical)
                {
                    int symId = firstRow.Count - 1 - i;


                    current = firstRow[symId];
                    current.SetCoordinates(current.Coordinates + new Vector2(0, r + diff));
                    // Move all blocks in the same column of the current one
                    List<ShapeBlock> col = shapeBlocks.Where(b => b.Coordinates.x == current.Coordinates.x).ToList();
                    foreach (var b in col)
                    {
                        if (b != current)
                            b.SetCoordinates(b.Coordinates + new Vector2(0, r + diff));

                    }
                }

            }
            if (symmetrical && width % 2 == 1)
            {
                // Move the block in the middle
                ShapeBlock current = firstRow[count];
                // Allign the current block with the previous one
                float diff = 0;
                diff = firstRow[count - 1].Coordinates.y - current.Coordinates.y;

                int r = moves[Random.Range(0, moves.Count)];
                if (r != 0 || diff != 0)
                {
                    // Move the current block
                    current.SetCoordinates(current.Coordinates + new Vector2(0, r + diff));
                    // Move all blocks in the same column of the current one
                    List<ShapeBlock> col = shapeBlocks.Where(b => b.Coordinates.x == current.Coordinates.x).ToList();
                    foreach (var b in col)
                    {
                        if (b != current)
                            b.SetCoordinates(b.Coordinates + new Vector2(0, r + diff));

                    }
                }
            }
        }

        void ComputeBorders()
        {
            foreach (var b in shapeBlocks)
            {
                b.IsEastBorder = false;
                b.IsSouthBorder = false;
                b.IsWestBorder = false;
                b.IsNorthBorder = false;

                if (!shapeBlocks.Exists(s => s.Coordinates.y == b.Coordinates.y && s.Coordinates.x == b.Coordinates.x - 1))
                    b.IsWestBorder = true;
                if (!shapeBlocks.Exists(s => s.Coordinates.y == b.Coordinates.y && s.Coordinates.x == b.Coordinates.x + 1))
                    b.IsEastBorder = true;
                if (!shapeBlocks.Exists(s => s.Coordinates.y == b.Coordinates.y + 1 && s.Coordinates.x == b.Coordinates.x))
                    b.IsNorthBorder = true;
                if (!shapeBlocks.Exists(s => s.Coordinates.y == b.Coordinates.y - 1 && s.Coordinates.x == b.Coordinates.x))
                    b.IsSouthBorder = true;
            }
        }

        GameObject CreateShapeBlock()
        {
//#if BUILDING_TEST
            GameObject block = Instantiate(helperBlockPrefab);
//#else
            //GameObject block = new GameObject();
//#endif

            ShapeBlock bb = block.GetComponent<ShapeBlock>();
            shapeBlocks.Add(bb);
            return block;
        }

        /// <summary>
        /// How many floors 
        /// </summary>
        void CreateBuildingSchema()
        {
            floorCount = Random.Range( MinFloorCount, MaxFloorCount+1 );
            Debug.Log($"SetBuildingSchema() - FloorCount:{floorCount}");
            int step  = (MaxFloorCount + 1 - MinFloorCount) / 3;
            Debug.Log($"SetBuildingSchema() - Step:{step}");
            //bool found = false;
            //int size = 0;
            //int max = MinFloorCount + step;
            //while(!found)
            //{
            //    if(floorCount < max)
            //    {
            //        found = true;
            //    }
            //    else
            //    {
            //        size++;
            //        max += step;
            //    }
            //}
            //floorSize = (FloorSize)size;
            
        }

        void BuildLogicStructure()
        {
            if(floorCount == 3)
                connectorCount = 2;
            else
                connectorCount = Random.Range(2, 4);

         
            // Create connectors
            for (int i = 0; i < connectorCount; i++)
            {
                GameObject g = new GameObject($"Connector-{i}");
                g.transform.parent = transform;
                FloorConnector c = g.AddComponent<FloorConnector>();
                connectors.Add(c);
            }

            // Create floors
            for (int i = 0; i < floorCount; i++)
            {
                GameObject g = new GameObject($"Floor-{i}");
                g.transform.parent = transform;
                Floor c = g.AddComponent<Floor>();
                c.Level = i;
                floors.Add(c);
            }

            // 
            // Connect floors
            //
            // Store all not connected floors
            List<Floor> notConnectedFloors = new List<Floor>(floors.ToArray());
            // Choose the floor to start with and remove it from the list
            Floor nextFloor = notConnectedFloors[Random.Range(0, notConnectedFloors.Count)];
            notConnectedFloors.Remove(nextFloor);
            // Choose the connector
            FloorConnector nextConnector = connectors[Random.Range(0, connectors.Count)];
            // Add the selected floor to the selected connector
            nextConnector.AddFloor(nextFloor);

            // Last elevator 
            FloorConnector lastConnector = null;
            // We store all the used connectors to a list to prevent any connector from being not used
            List<FloorConnector> usedConnectors = new List<FloorConnector>();
            usedConnectors.Add(nextConnector);

            // Loop through all remaining floors
            while (notConnectedFloors.Count > 0)
            {
                // Get a new floor to connect to the last selected
                nextFloor = notConnectedFloors[Random.Range(0, notConnectedFloors.Count)];
                notConnectedFloors.Remove(nextFloor);
                // Connect both floors
                nextConnector.AddFloor(nextFloor);
                // Get any elevator different from the last two used elevators
                List<FloorConnector> tmpConnectors = null;
                if(connectorCount > 2)
                    tmpConnectors = connectors.FindAll(e => e != nextConnector && e != lastConnector);
                else
                    tmpConnectors = connectors.FindAll(e => e != nextConnector);

                lastConnector = nextConnector;
                nextConnector = tmpConnectors[Random.Range(0, tmpConnectors.Count)];

                // An elevator that has already been used means the next floor is reacheable from a previously
                // connected floor; in this way we can have less connections
                if (!usedConnectors.Contains(nextConnector))
                {
                    usedConnectors.Add(nextConnector);
                    // Add the new floor to the new elevator
                    nextConnector.AddFloor(nextFloor);
                }
            }

            // Set the floor you want to start from
            startingFloor = Random.Range(1, floorCount);
            // Reset floor levels if you are not starting from the floor 0
            if(startingFloor > 0)
            {
                foreach(Floor floor in floors)
                    floor.Level -= startingFloor;
            }
        }
    }

}
