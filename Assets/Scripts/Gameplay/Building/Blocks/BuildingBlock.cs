using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace HKR.Building
{
    public enum BuildingBlockType { Common, Entrance, Connector }

#if BUILDING_TEST
    public class BuildingBlock : MonoBehaviour
#else
    public class BuildingBlock : NetworkBehaviour
#endif
    {
        public static UnityAction<BuildingBlock> OnSpawned;

        public const float Size = 18;
        public const float Height = 6;

        [UnitySerializeField]
        [Networked]
        public float GeometryRootAngle { get; set; }

        [UnitySerializeField]
        [Networked]
        public int FloorLevel { get; set; }

        [SerializeField]
        GameObject geometryRoot;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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




    }

}
