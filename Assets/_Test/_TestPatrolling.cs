using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class _TestPatrolling : MonoBehaviour
{
    [SerializeField]
    List<Transform> targets;

    NavMeshAgent agent;

    [SerializeField]
    Transform currentTarget;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(agent.destination == null)
        if(!agent.hasPath)
        {
            currentTarget = targets[Random.Range(0, targets.Count)];
            agent.destination = currentTarget.transform.position;
        }
        else
        {
            if(currentTarget != null)
            {
                // Start hanging around
            }
        }
    }
}
