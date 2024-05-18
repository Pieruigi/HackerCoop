using HKR.Building;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class SecurityCameraController : MonoBehaviour
    {
        [SerializeField]
        SecurityStateController stateController;

        [SerializeField]
        Transform rotationPivot;

        [SerializeField]
        float rotationSpeed;

        [SerializeField]
        float aimingRotationSpeed;

        bool spawned = false;
        
        private void Awake()
        {
            rotationSpeed *= UnityEngine.Random.Range(0, 2) == 0 ? 1f : -1f;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (/*!spawned || */(!SessionManager.Instance.NetworkRunner.IsSinglePlayer && !SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)) return;

            UpdateState();
        }


        //private void OnEnable()
        //{
        //    stateController.OnSpawned += HandleOnStateControllerSpawned;
        //}

        //private void OnDisable()
        //{
        //    stateController.OnSpawned -= HandleOnStateControllerSpawned;
        //}

        //private void HandleOnStateControllerSpawned()
        //{
        //    spawned = true;
        //}

        void UpdateState()
        {
            switch(stateController.State)
            {
                case SecurityState.Normal:
                    UpdateNormalState();
                    break;
                case SecurityState.Suspect:
                    UpdateSuspectState();
                    break;
                case SecurityState.Spotted:
                    UpdateSpottedState();
                    break;
                case SecurityState.Freezed:
                    UpdateFreezedState();
                    break;

            }
        }

        private void UpdateFreezedState()
        {
            
        }

        private void UpdateSpottedState()
        {
            
        }

        private void UpdateSuspectState()
        {
            
        }

        private void UpdateNormalState()
        {
            // Rotate camera
            Quaternion rotMat = Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
            rotationPivot.rotation *= rotMat;
        }
    }

}
