
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

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

        List<ShapeBlock> shapeBlocks = new List<ShapeBlock>();


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
            

            for(int i=0; i<blockCount-left; i++)
            {
                GameObject block = CreateShapeBlock();
                ShapeBlock bb = block.GetComponent<ShapeBlock>();
                block.transform.parent = root.transform;
                Vector2 coords = new Vector2(i%width, i/width);
                bb.Init(null, coords);
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
            ChooseEnteringBlock();

            // Choose connectors
            ChooseConnectorBlocks();

            foreach (var b in shapeBlocks)
            {

                b.Move();
#if BUILDING_TEST
                b.Colorize();
#endif
            }

        }

        void ChooseConnectorBlocks()
        {
            // Choose configuration 
            // 3 elevators, 3 stairscases, 1 elevator and 2 staircases, 2 elevators and 1 staircase
            int conf = Random.Range(0, 4);
            int elevatorCount = 0;
            int staircaseCount = 0;
            switch(conf)
            {
                case 0:
                    elevatorCount = 3;
                    break;
                case 1:
                    staircaseCount = 3;
                    break;
                case 2:
                    elevatorCount = 2;
                    staircaseCount = 1;
                    break;
                case 3:
                    elevatorCount = 1;
                    staircaseCount = 2;
                    break;
            }

        }

        void ChooseEnteringBlock()
        {
            // Get all the blocks that are south borders
            List<ShapeBlock> blocks = shapeBlocks.Where(b => b.IsSouthBorder).ToList();
            // Choose a random block
            ShapeBlock block = blocks[Random.Range(0, blocks.Count)];
            block.IsEnteringBlock = true;
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
                leftBlock.Init(null, coords);
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
                        block.Init(block.Floor, new Vector2(block.Coordinates.x, max + 1));
                        break;
                    case 2: // South
                        col = shapeBlocks.Where(b => b.Coordinates.x == block.Coordinates.x).ToList();
                        float min = col.Min(b => b.Coordinates.y);
                        Debug.Log($"CreateHoles() - South, min:{min}");
                        block.Init(block.Floor, new Vector2(block.Coordinates.x, min - 1));
                        break;
                    case 1: // East
                        List<ShapeBlock> row = shapeBlocks.Where(b => b.Coordinates.y == block.Coordinates.y).ToList();
                        max = row.Max(b => b.Coordinates.x);
                        Debug.Log($"CreateHoles() - East, max:{max}");
                        block.Init(block.Floor, new Vector2(max + 1, block.Coordinates.y));
                        break;
                    case 3: // West
                        row = shapeBlocks.Where(b => b.Coordinates.y == block.Coordinates.y).ToList();
                        min = row.Min(b => b.Coordinates.x);
                        Debug.Log($"CreateHoles() - West, min:{min}");
                        block.Init(block.Floor, new Vector2(min - 1, block.Coordinates.y));
                        break;

                }
            }

        }

        void TwistShape(bool symmetrical, int width, int length)
        {
            // Twist values
            List<int> moves = new List<int>();
            for (int k = 0; k < 7; k++)
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
                    current.Init(current.Floor, current.Coordinates + new Vector2(0, r + diff));
                    // Move all blocks in the same column of the current one
                    List<ShapeBlock> col = shapeBlocks.Where(b => b.Coordinates.x == current.Coordinates.x).ToList();
                    foreach (var b in col)
                    {
                        if (b != current)
                            b.Init(b.Floor, b.Coordinates + new Vector2(0, r + diff));

                    }
                }

                // Move the symmetrical column ?
                if (symmetrical)
                {
                    int symId = firstRow.Count - 1 - i;


                    current = firstRow[symId];
                    current.Init(current.Floor, current.Coordinates + new Vector2(0, r + diff));
                    // Move all blocks in the same column of the current one
                    List<ShapeBlock> col = shapeBlocks.Where(b => b.Coordinates.x == current.Coordinates.x).ToList();
                    foreach (var b in col)
                    {
                        if (b != current)
                            b.Init(b.Floor, b.Coordinates + new Vector2(0, r + diff));

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
                    current.Init(current.Floor, current.Coordinates + new Vector2(0, r + diff));
                    // Move all blocks in the same column of the current one
                    List<ShapeBlock> col = shapeBlocks.Where(b => b.Coordinates.x == current.Coordinates.x).ToList();
                    foreach (var b in col)
                    {
                        if (b != current)
                            b.Init(b.Floor, b.Coordinates + new Vector2(0, r + diff));

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
#if BUILDING_TEST
            GameObject block = Instantiate(helperBlockPrefab);
#else
                GameObject block = new GameObject();
#endif

            ShapeBlock bb = block.GetComponent<ShapeBlock>();
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
