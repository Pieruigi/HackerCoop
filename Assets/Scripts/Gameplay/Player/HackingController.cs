using HKR;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HKR
{
    public class HackingController : MonoBehaviour
    {
        

        [SerializeField]
        float hackingRadius;

        InfectionNodeController currentHackingNode; // The node we are actually hacking

        List<InfectionNodeController> infectionNodes;

        PlayerDevice device;

        private void Awake()
        {
            device = GetComponent<PlayerDevice>();
        }


        private void Update()
        {
            if (!device.IsLocalPlayer() || PlayerController.Local.State != PlayerState.Normal)
                return;

            
            

            InfectionNodeController node;
            TryCheckForInfectionNodeAiming(out node);
            Debug.Log($"Node:{node}");
            if (currentHackingNode) // We are already hacking a node
            {
                // Check if we are still aiming the node
                if (node != currentHackingNode)
                    StopHacking();
                else
                    KeepHacking();
            }
            else
            {
                if (node)
                {
                    // We may call some event to report the UI here

                   
                    // Check for input
                    if (device.GetButtonDown())
                        StartHacking(node);
                }
            }
        }

        protected void OnEnable()
        {
          
            if (!device.IsLocalPlayer() || infectionNodes != null)
                return;
            // Load all the infection nodes
            infectionNodes = FindObjectsOfType<InfectionNodeController>().ToList();
        }

        protected  void OnDisable()
        {
           
        }

        bool TryCheckForInfectionNodeAiming(out InfectionNodeController node)
        {
            node = null;

            RaycastHit hitInfo;
            LayerMask mask = LayerMask.GetMask();
            mask = ~mask; // We may need to upgrade the hacking device and let it do its work through walls for example
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, hackingRadius, mask, QueryTriggerInteraction.Collide))
            {
                Debug.Log($"Hit:{hitInfo.collider.gameObject.name}");
                node = hitInfo.collider.GetComponentInParent<InfectionNodeController>();
            }
                

            return node;
        }

        void StopHacking()
        {
            currentHackingNode.ResetHackingState();
            currentHackingNode = null;
        }

        void KeepHacking()
        {
            Debug.Log("Keep hacking...");
        }

        void StartHacking(InfectionNodeController node)
        {
            Debug.Log("Start kacking...");
            node.SetHackingState();
            currentHackingNode = node;
        }
    }

}

