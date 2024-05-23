using HKR.Building;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class _TestNavMesh : MonoBehaviour
{
    [SerializeField]
    GameObject floor;

    [SerializeField]
    GameObject navMesh;

    [SerializeField]
    Transform sampleCenter;

    // Start is called before the first frame update
    void Start()
    {
        //CreateNavMesh(floor.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            NavMeshHit hit;
            
            
            if (NavMesh.SamplePosition(sampleCenter.position, out hit, 5, 1<<3))
            {
                Debug.Log($"Sampling: {hit.position}");
            }
            else
            {
                Debug.Log($"Not sampling");
            }
        }
    }

    //void CreateNavMesh(Transform target)
    //{
    //    GameObject nm = Instantiate(navMesh, target.position, target.rotation);
    //    nm.GetComponent<NavMeshSurface>().BuildNavMesh();
    //}
}
