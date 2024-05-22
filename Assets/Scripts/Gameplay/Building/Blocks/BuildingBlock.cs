using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace HKR.Building
{
    public enum BuildingBlockType { Common, Entrance, Connector, Column, Infected }

#if BUILDING_TEST
    public class BuildingBlock : MonoBehaviour
#else
    public class BuildingBlock : NetworkBehaviour
#endif
    {
        public static UnityAction<BuildingBlock> OnSpawned;

        public const float Size = 9;
        public const float Height = 3;

        static List<BuildingBlock> blocks = new List<BuildingBlock>();
        public static IList<BuildingBlock> Blocks { get { return blocks.AsReadOnlyList(); } }

        [UnitySerializeField]
        [Networked]
        public float GeometryRootAngle { get; set; }

        [UnitySerializeField]
        [Networked]
        public int FloorLevel { get; set; }

        //[UnitySerializeField]
        //[Networked]
        //public List<int> InfectedNodeIds { get; } = default;

        [SerializeField]
        GameObject geometryRoot;

        private void Awake()
        {
            blocks.Add(this);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDestroy()
        {
            blocks.Remove(this);
        }

#if BUILDING_TEST
        public  void Spawned() 
        {

            geometryRoot.transform.rotation = Quaternion.Euler(0, GeometryRootAngle, 0);                  

            
        }
#else
        public override void Spawned() 
        {
            //if(SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient || SessionManager.Instance.NetworkRunner.IsSinglePlayer)
            geometryRoot.transform.rotation = Quaternion.Euler(0, GeometryRootAngle, 0);

            OnSpawned?.Invoke(this);

        }
#endif

        public static bool TryGetBlockByPoint(Vector3 point, out BuildingBlock block)
        {
            // Get the corresponding floor level
            int floorLevel = Utility.GetFloorLevelByVerticalCoordinate(point.y);
            // Get all the blocks in the given floor
            List<BuildingBlock> tmp = blocks.Where(b => b.FloorLevel == floorLevel).ToList();
            foreach(BuildingBlock b in tmp)
            {
                if (b.transform.position.x < point.x && b.transform.position.x + Size > point.x &&
                   b.transform.position.z < point.z && b.transform.position.z + Size > point.z)
                {
                    block = b;
                    return true;
                }
            }

            block = null;
            return false;
        }


    }

}
