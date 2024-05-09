using Fusion;
using HKR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    public class HackingController : MonoBehaviour
    {
        public UnityAction<bool, InfectedNodeState> OnAiming;
        public UnityAction<float> OnStartConnecting;
        public UnityAction OnStopConnecting;
        
        [SerializeField]
        float hackingRadius;

        [SerializeField]
        float connectingTime = 3;

        [SerializeField]
        float detectingTime = 6;

        [SerializeField]
        List<GameObject> apps;

        [SerializeField]
        HackingTimer hackingTimer;


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
                OnAiming?.Invoke(true, node.State);
            else
            {
                OnAiming?.Invoke(false, default);
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
                {
                    currentHackingNode.ResetHackingStateRpc();
                    StopHacking();
                }
                else
                    KeepHacking();
            }
            else
            {
                if (node && node.State == InfectedNodeState.Infected)
                {
                    if (device.GetButton())
                    {
                        if (!connecting)
                        {
                            connecting = true;
                            connectionTimeElapsed = 0;
                            OnStartConnecting?.Invoke(connectingTime);
                        }

                    }
                    else
                    {
                        if (connecting)
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
                    //StartHacking(node);
                    node.SendStartHackingRequestRpc(new RpcInfo() { Source = SessionManager.Instance.NetworkRunner.LocalPlayer });
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
           // Deactivate all apps 
           foreach(var app in apps)
                app.SetActive(false);
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

        void ResetNodeAndApp()
        {
            // Disable the current app
            apps[currentHackingNode.InfectionType].SetActive(false);
            currentHackingNode = null;
        }

        void StopHacking()
        {
            // Disable the current app
            ResetNodeAndApp();
            // Reset timer
            hackingTimer.StopTimer();
        }

        void KeepHacking()
        {
            Debug.Log("Keep hacking...");
        }

        public void StartHacking(InfectionNodeController node)
        {
            Debug.Log("Start kacking...");
            // Set the node state
            currentHackingNode = node;
            // Launch the suitable app
            apps[currentHackingNode.InfectionType].SetActive(true);
            // Start timer
            hackingTimer.StartTimer(detectingTime);
        }

        

        public void OnHackingSucceded()
        {
            hackingTimer.StopTimer();
            currentHackingNode.SetClearStateRpc();
            StopHacking();
        }

        public void OnHackingFailed()
        {
            AlarmSystemController asc = AlarmSystemController.GetAlarmSystemController(currentHackingNode.FloorLevel);
            // Switch the alarm on
            asc.SwitchAlarmOnRpc();
            StopHacking();
        }

        public void OnHackingDetected()
        {
            // If you get detected the hacking failed
            OnHackingFailed();
        }
    }

}

