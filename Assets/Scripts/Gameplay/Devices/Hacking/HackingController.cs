using HKR;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    public class HackingController : MonoBehaviour
    {
        public UnityAction<bool> OnAiming;
        public UnityAction<float> OnStartConnecting;
        public UnityAction OnStopConnecting;

        [SerializeField]
        float hackingRadius;

        [SerializeField]
        float connectingTime = 3;

        
        InfectionNodeController currentHackingNode; // The node we are actually hacking

        List<InfectionNodeController> infectionNodes;

        PlayerDevice device;

        bool connecting = false;
        float connectionTimeElapsed = 0;


        private void Awake()
        {
            device = GetComponent<PlayerDevice>();
        }


        private void Update()
        {
            if (!device.IsLocalPlayer() || PlayerController.Local.State != PlayerState.Normal)
                return;

            
            

            InfectionNodeController node;
            if(TryCheckForInfectionNodeAiming(out node))
                OnAiming?.Invoke(true);
            else
            {
                OnAiming?.Invoke(false);
                // You must keep aiming at the node to connect the hacking device
                if (connecting)
                {
                    connecting = false;
                    OnStopConnecting?.Invoke();
                }
            }
                

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
                    // Check for input
                    //if (device.GetButtonDown())
                    //    StartHacking(node);
                    if (device.GetButton())
                    {
                        if(!connecting)
                        {
                            connecting = true;
                            connectionTimeElapsed = 0;
                            OnStartConnecting?.Invoke(connectingTime);
                        }

                    }
                    else
                    {
                        if(connecting)
                        {
                            connecting = false;
                            OnStopConnecting?.Invoke();
                        }
                    }    
                }
              
            }

            if (connecting)
            {
                connectionTimeElapsed += Time.deltaTime;
                if(connectionTimeElapsed > connectingTime)
                {
                    connecting = false;
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

