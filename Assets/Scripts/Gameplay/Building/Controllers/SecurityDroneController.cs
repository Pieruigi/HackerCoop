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

        
        bool lookAtTarget = false;
        
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
                
                    
                case SecurityState.Spotted:
                    // Look at the target
                    //lookAtTarget = true;
                    //Vector3 dir = Vector3.ProjectOnPlane(spotter.CurrentTarget.transform.position-transform.position, Vector3.up);
                    //Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
                    //Quaternion.RotateTowards(transform.rotation, lookRot, aimingSpeed);
                    break;
                case SecurityState.Searching:
                case SecurityState.Normal:
                case SecurityState.Freezed:
                    lookAtTarget = false;
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
