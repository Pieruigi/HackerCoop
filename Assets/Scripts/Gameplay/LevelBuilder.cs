
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HKR.Building
{
    public enum FloorSize { Small, Medium, Large }

    public class LevelBuilder : MonoBehaviour
    {
        public static LevelBuilder Instance {  get; private set; }

#if BUILDING_TEST
        [SerializeField]
        GameObject helperBlockPrefab;
#endif

        int minFloorCount = 3;
        int maxFloorCount = 8;
        
        int floorCount;
        FloorSize floorSize = FloorSize.Small;
        int connectorCount = 3;
        int startingFloor = 0;
        int blockSize = 9;
        //List<Vector2> blocksPerSize = new List<Vector2>(new Vector2[] { new Vector2(10, 12), new Vector2(14, 16), new Vector2(18, 20) });
        List<int> minBlocksPerSize = new List<int>(new int[] { 20, 28, 36 });
        List<int> maxBlocksPerSize = new List<int>(new int[] { 24, 32, 40 });
        int blockCount;

        List<Floor> floors = new List<Floor>();
        List<FloorConnector> connectors = new List<FloorConnector>();

        List<BuildingBlock> shapeBlocks = new List<BuildingBlock>();


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
            CreateShape();

        }

        void CreateShape()
        {
            blockCount = Random.Range(minBlocksPerSize[(int)floorSize], maxBlocksPerSize[(int)floorSize] + 1);
            Debug.Log($"CreateShape() - blockCount:{blockCount}");
            // Width must be greater than length
            float widthLengthRatio = Random.Range(1.5f, 3f);
            // width * length = blockCount => r*length * length = blockCount => length = Sqrt(blockCount/r)
            int length = (int)Mathf.Sqrt(blockCount / widthLengthRatio);
            int width = blockCount / length;
            // Check for remaining blocks
            int left = blockCount - (width * length);
            Debug.Log($"CreateShape() - width:{width}, length:{length}, diff:{left}");



            //
            // Create shape blocks.
            //
            // Create the root
            GameObject root = new GameObject("Shape");
            root.transform.parent = transform;
            bool symmetric = Random.Range(0,2) == 0;

            for(int i=0; i<blockCount-left; i++)
            {
                GameObject block = CreateShapeBlock();
                BuildingBlock bb = block.GetComponent<BuildingBlock>();
                block.transform.parent = root.transform;
                Vector2 coords = Vector2.zero;
                if(i==0)
                {
                    // The first block is the left bottom border at (0,0)
                    coords = Vector3.zero;
                    //bb.IsWestBorder = true;
                    //bb.IsSouthBorder = true;
                }
                else
                {

                    if(symmetric && ((width % 2 == 0 && i%width >= width / 2) || (width % 2 == 1 && i%width > width/2)))
                    {
                        // We set the coordinates of the symmetric block
                        int symIndex = 0;
                        int offset = i % width;
                        int back = 2 * (offset - (width / 2)) + (width%2 == 0 ? 1 : 0) ;
                        symIndex = i - back;
                        //if (width%2 == 0)
                        //{
                        //    int offset = i % width;
                        //    int back = 2 * ( offset - (width / 2) )+ 1;
                        //    symIndex = i - back;
                        //}
                        //else
                        //{
                        //    int offset = i % width;
                        //    int back = 2 * (offset - (width / 2));
                        //    symIndex = i - back;
                        //}
                        BuildingBlock symBlock = shapeBlocks[symIndex];
                        coords = new Vector2(i%width, symBlock.Coordinates.y);
                        //bb.IsNorthBorder = symBlock.IsNorthBorder;
                        //bb.IsSouthBorder = symBlock.IsSouthBorder;
                        //bb.IsEastBorder = symBlock.IsWestBorder;
                        
                    }
                    else
                    {
                        coords = new Vector2(i%width, 0);
                        if (i / width > 0)
                        {
                            coords.y = shapeBlocks[i-width].Coordinates.y+1;
                        }
                        else
                        {
                            coords.y = 0;
                        }
                        
                    }
                }

                bb.Init(null, coords);
                bb.name = $"S_{bb.Coordinates}";

            }

            // Set border flags
            ComputeBorders();

            // Add the remaining blocks
            for(int i=0; i<left; i++)
            {
                GameObject block = CreateShapeBlock();
                block.transform.parent = root.transform;
                BuildingBlock leftBlock = block.GetComponent<BuildingBlock>();
                // Get all border blocks
                List<BuildingBlock> blocks = shapeBlocks.Where(b=>b.IsBorder).ToList();
                // Get a random block
                var b = blocks[Random.Range(0, blocks.Count)];
                // Choose a random one if many
                List<int> sides = new List<int>();
                if(b.IsNorthBorder)
                    sides.Add(0);
                if (b.IsEastBorder)
                    sides.Add(1);
                if (b.IsSouthBorder)
                    sides.Add(2);
                if (b.IsWestBorder)
                    sides.Add(3);
                int side = sides[Random.Range(0, sides.Count)];
                Vector2 coords = b.Coordinates;
                switch(side)
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
                leftBlock.Init(null, coords);
                leftBlock.name = $"S_{leftBlock.Coordinates}";
            }

            // Compute borders again to account for the new blocks
            ComputeBorders();

            foreach(var b in shapeBlocks)
            {
#if BUILDING_TEST
                b.Spawn();
#endif
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
#if BUILDING_TEST
            GameObject block = Instantiate(helperBlockPrefab);
#else
                GameObject block = new GameObject();
#endif

            BuildingBlock bb = block.AddComponent<BuildingBlock>();
            shapeBlocks.Add(bb);
            return block;
        }

        /// <summary>
        /// How many floors 
        /// </summary>
        void CreateBuildingSchema()
        {
            floorCount = Random.Range( minFloorCount, maxFloorCount+1 );
            Debug.Log($"SetBuildingSchema() - FloorCount:{floorCount}");
            int step  = (maxFloorCount + 1 - minFloorCount) / 3;
            Debug.Log($"SetBuildingSchema() - Step:{step}");
            bool found = false;
            int size = 0;
            int max = minFloorCount + step;
            while(!found)
            {
                if(floorCount < max)
                {
                    found = true;
                }
                else
                {
                    size++;
                    max += step;
                }
            }
            floorSize = (FloorSize)size;
            Debug.Log($"SetBuildingSchema() - FloorSize:{floorSize}");
        }

        void BuildLogicStructure()
        {
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
                List<FloorConnector> tmpConnectors = connectors.FindAll(e => e != nextConnector && e != lastConnector);
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
            startingFloor = Random.Range(0, floorCount);
            // Reset floor levels if you are not starting from the floor 0
            if(startingFloor > 0)
            {
                foreach(Floor floor in floors)
                    floor.Level -= startingFloor;
            }
        }
    }

}
