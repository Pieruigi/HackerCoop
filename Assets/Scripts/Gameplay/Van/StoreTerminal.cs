using Fusion;
using HKR.Scriptables;
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
        Transform spawnPoint;

        [SerializeField]
        GameObject _testDevice;

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
                PurchaseRpc(0);
            }
           
#endif

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
