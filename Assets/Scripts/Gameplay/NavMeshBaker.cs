using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace HKR.Building
{
    public class NavMeshBaker : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<NavMeshSurface>().BuildNavMesh();
        }

   
    }

}
