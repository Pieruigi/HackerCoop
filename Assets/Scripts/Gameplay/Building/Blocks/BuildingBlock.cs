using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR.Building
{
    public enum BuildingBlockType { Common, Entrance, Connector }

#if BUILDING_TEST
    public class BuildingBlock : MonoBehaviour
#else
    public class BuildingBlock : NetworkBehaviour
#endif
    {
        public const float Size = 9;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


    }

}
