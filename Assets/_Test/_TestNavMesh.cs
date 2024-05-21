using HKR.Building;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class _TestNavMesh : MonoBehaviour
{
    [SerializeField]
    GameObject floor;

    [SerializeField]
    GameObject navMesh;

    // Start is called before the first frame update
    void Start()
    {
        CreateNavMesh(floor.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateNavMesh(Transform target)
    {
        GameObject nm = Instantiate(navMesh, target.position, target.rotation);
        nm.GetComponent<NavMeshSurface>().BuildNavMesh();
    }
}
