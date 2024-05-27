using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class SecurityDroneController : MonoBehaviour
    {
        [SerializeField]
        SecurityStateController stateController;

        [SerializeField]
        SightSpotter spotter;

        
        private void Awake()
        {
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
                return;

            if (!PlayerManager.Instance.PlayerInGameAll())
                return;

            UpdateState();
        }

        void UpdateState()
        {
            switch (stateController.State)
            {
                case SecurityState.Normal:
                    break;
                case SecurityState.Spotted:
                    break;
                case SecurityState.Searching:
                    break;
                case SecurityState.Freezed:
                    break;
            }
        }

        void UpdateSpotted()
        {
            //// Get the target
            //var target = spotter.CurrentTarget;
            //if(!target) return; 


        }
    }

}
