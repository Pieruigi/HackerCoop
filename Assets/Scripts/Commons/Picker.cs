using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HKR
{
    public class Picker : NetworkBehaviour
    {

        [UnitySerializeField]
        [Networked]
        public NetworkBool Owned { get; set; } // Can you pick it or is owned by a player?

       

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.I))
            //{
            //    if (!Owned)
            //    {
            //        if (HasStateAuthority)
            //        {
                        
            //            DoPickUp();
            //        }
            //        else
            //        {
            //            //Object.RequestStateAuthority();
            //            PickUpRequestRpc();
            //        }
            //    }
                
            //}
        }

        public override void Spawned()
        {
            base.Spawned();


        }

        /// <summary>
        /// Called by the a client with no authority on the object to the client with the authority
        /// </summary>
        /// <param name="info"></param>
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
                DoPickUp();
            }
        }

        [Rpc(sources:RpcSources.StateAuthority, targets:RpcTargets.All)]
        void AddToEquipmentRpc(RpcInfo info = default)
        {
            // Put the object at the end of the equipment device list ( we don't need to specify an index )
            // Get the equipment of the source player
            Equipment equipment = FindObjectsOfType<Equipment>().First(e=>e.Object.StateAuthority == info.Source);
            equipment.Add(GetComponent<PlayerDevice>());


        }

        /// <summary>
        /// Call by the player who's picking up the object
        /// </summary>
        void DoPickUp()
        {
            Owned = true;
            // We send an rpc to the other players in order to setup equipment too
            AddToEquipmentRpc();
        }

        public bool TryPickUp()
        {
            if(Owned)
                return false;

            if (HasStateAuthority)
                DoPickUp();
            else
                PickUpRequestRpc();
            
            return true;
        }
    }

}
