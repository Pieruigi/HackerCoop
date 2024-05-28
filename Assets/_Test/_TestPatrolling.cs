using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        DoUpdate();  
    }

    async void DoUpdate()
    {
        //if(agent.destination == null)
        if (!agent.hasPath)
        {
            await Task.Delay(3000);
            currentTarget = targets[Random.Range(0, targets.Count)];
            agent.destination = currentTarget.transform.position;
        }
        else
        {
            if (currentTarget != null)
            {
                // Start hanging around
            }
        }
    }
}
