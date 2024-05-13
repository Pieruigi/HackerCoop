using Fusion;
using HKR;
using Microsoft.Win32.SafeHandles;
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
        public UnityAction OnHitSuccedeed;
        public UnityAction OnHitFailed;
        public UnityAction OnHackingStarted;
        public UnityAction OnHackingStopped;

        [SerializeField]
        float hackingRadius;

        [SerializeField]
        float connectingTime = 3;

        //[SerializeField]
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
        HandController handController;

        int appHitTarget = 0;
        int appMaxErrors = 0;
        int appHitCount = 0;
        int appErrorCount = 0;

        public int HitTarget
        {
            get { return appHitTarget; }
        }

        public int HitCount
        {
            get { return appHitCount; }
        }

        public int MaxErrors
        {
            get { return appMaxErrors; }
        }

        public int ErrorCount
        {
            get { return appErrorCount; }
        }

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
                    handController.MoveDown();
                    OnStopConnecting?.Invoke();
                }
            }
                

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
                            // Move hand up
                            handController.MoveUp();
                        }

                    }
                    else
                    {
                        if (connecting)
                        {
                            connecting = false;
                            OnStopConnecting?.Invoke();
                            // Move hand down
                            handController.MoveDown();
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
          
            if (!device.IsLocalPlayer())
                return;
            // Load all the infection nodes
            if(infectionNodes == null)
                infectionNodes = FindObjectsOfType<InfectionNodeController>().ToList();

            // Get the hand controller
            handController = GetComponentInParent<HandController>();

            // Deactivate all apps 
            DeactivateAppAll();
        }

        protected  void OnDisable()
        {
            // Deactivate all apps 
            DeactivateAppAll();

        }

        void DeactivateAppAll()
        {
            foreach (var app in apps)
                app.SetActive(false);
        }

        bool TryCheckForInfectionNodeAiming(out InfectionNodeController node)
        {
            node = null;

            RaycastHit hitInfo;
            LayerMask mask = LayerMask.GetMask();
            mask = ~mask; // We may need to upgrade the hacking device and let it do its work through walls for example
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, hackingRadius, mask, QueryTriggerInteraction.Collide))
                node = hitInfo.collider.GetComponentInParent<InfectionNodeController>();
                

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
            // Move hand down
            handController.MoveDown();

            OnHackingStopped?.Invoke();
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

            OnHackingStarted?.Invoke();

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

        public void SetAppData(float timer, int target, int maxErrors)
        {
            appHitTarget = target;
            appMaxErrors = maxErrors;
            detectingTime = timer;
            appHitCount = 0;
            appErrorCount = 0;

            
        }

        public void HitSucceded()
        {
            appHitCount++;
            if(appHitCount >= appHitTarget)
            {
                // Succeed and stop
                OnHackingSucceded();
            }
            OnHitSuccedeed?.Invoke();
        }

        public void HitFailed()
        {
            appErrorCount++;
            if(appErrorCount >= appMaxErrors)
            {
                // Fail and stop
                OnHackingFailed();
            }
            OnHitFailed?.Invoke();
        }

        

    }

}

