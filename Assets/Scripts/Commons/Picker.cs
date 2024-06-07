using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HKR
{
    public class Picker : NetworkBehaviour
    {

        [UnitySerializeField]
        [Networked]
        public NetworkBool Owned { get; set; } // Can you pick it or is owned by a player?

        private async void Start()
        {
            //Task t = Task => { };
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (!Owned)
                {
                    if (HasStateAuthority)
                    {
                        
                        PickUp();
                    }
                    else
                    {
                        //Object.RequestStateAuthority();
                        PickUpRequestRpc();
                    }
                }
                
            }
        }

        public override void Spawned()
        {
            base.Spawned();

            //if (HasStateAuthority)
            //    Object.ReleaseStateAuthority();

        }

        [Rpc(sources:RpcSources.All, targets:RpcTargets.StateAuthority)]
        void PickUpRequestRpc(RpcInfo info = default)
        {
            Debug.Log($"TEST - info.Source:{info.Source}");

            bool isOk = false;
            if (!Owned)
            {
                // Release authority
                Object.ReleaseStateAuthority();

                isOk = true;
            }
            
            // Answer back 
            PickUpResponseRpc(info.Source, isOk);

        }

        [Rpc(sources:RpcSources.All, targets:RpcTargets.All)]
        async void PickUpResponseRpc([RpcTarget] PlayerRef player, NetworkBool isOk)
        {
            Debug.Log($"TEST - PickUp response:{isOk}");
            if (isOk)
            {
                Object.RequestStateAuthority();
                // Await for authority
                await Task.Run(async () => { while (!HasStateAuthority) await Task.Yield(); });
                // Pick up
                PickUp();
            }
        }

       

        void PickUp()
        {
            Owned = true;
        }
    }

}
