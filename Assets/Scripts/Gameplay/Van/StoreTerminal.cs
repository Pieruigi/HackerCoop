using Fusion;
using HKR.Scriptables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class StoreTerminal : NetworkBehaviour
    {
        [SerializeField]
        List<DeviceAsset> devices;

        [SerializeField]
        Collider interactionCollider;

        [SerializeField]
        float interactionRange = 1.5f;

        [SerializeField]
        Transform playerTarget;

        [SerializeField]
        Transform spawnPoint;

        [SerializeField]
        GameObject _testDevice;

        [UnitySerializeField]
        [Networked]
        public bool Activated { get; private set; }

        bool spawned = false;
                

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if(Input.GetKeyDown(KeyCode.O)) 
            {
                //PurchaseRpc(0);
                SendActivationRequestRpc();
            }

#endif
            

            if (!spawned)
                return;

            if (!Activated)
            {
                // Check the local player distance
                float distance = Vector3.ProjectOnPlane(PlayerController.Local.transform.position - interactionCollider.transform.position, Vector3.up).magnitude;
                if(distance < interactionRange)
                {
                    Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                    RaycastHit hit;
                    int mask = LayerMask.GetMask(new string[] { Layers.Pickable });
                    if (Physics.Raycast(ray, out hit, interactionRange, mask))
                    {
                        if (hit.collider == interactionCollider)
                        {
                            if (Input.GetKeyDown(KeyCode.F))
                            {
                                Debug.Log("Interacting with the store device...");
                                
                                SendActivationRequestRpc();
                            }
                            
                        }
                    }

                    

                    
                }
                
            }


        }

        public override void Spawned()
        {
            base.Spawned();

            spawned = true;
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
        private void SendActivationRequestRpc(RpcInfo info = default)
        {
            bool ret = false;
            if (!Activated)
            {
                // Activation failed
                Activated = true;
                ret = true;
            }
            SendActivationResponseRpc(info.Source, ret);
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        private void SendActivationResponseRpc([RpcTarget] PlayerRef player, bool value)
        {
            Debug.Log($"Activation response:{value}");
            if (value)
            {
                // Move the player and start interaction
                PlayerController.Local.SetExternalState(playerTarget.position, -90, 20, .25f);
                
            }
        }

        [Rpc(sources:RpcSources.All, targets:RpcTargets.StateAuthority)]
        public void PurchaseRpc(int assetId)
        {
            //SessionManager.Instance.NetworkRunner.
            Runner.Spawn(_testDevice, spawnPoint.position, spawnPoint.rotation, null,
                (r, o) =>
                {
                    Picker picker = o.GetComponent<Picker>();
                    picker.Owned = false;
                });
        }
    }

}
