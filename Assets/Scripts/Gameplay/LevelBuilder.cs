using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR.Building
{
    public enum FloorSize { Small, Medium, Large }

    public class LevelBuilder : MonoBehaviour
    {
        public static LevelBuilder Instance {  get; private set; }

        int minFloorCount = 3;
        int maxFloorCount = 8;
        
        int floorCount;
        FloorSize floorSize = FloorSize.Small;
        int connectorCount = 3;
        int startingFloor = 0;

        List<Floor> floors = new List<Floor>();
        List<FloorConnector> connectors = new List<FloorConnector>();



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
        }
#endif

        public void Build()
        {
            Debug.Log($"Start building level with seed {Random.state}");

            // Create main schema
            CreateBuildingSchema();

            // Create logic structure
            BuildLogicStructure();

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
