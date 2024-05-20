using Fusion;
using HKR.Building;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class SecurityCameraController : NetworkBehaviour
    {
        [SerializeField]
        SecurityStateController stateController;

        [SerializeField]
        SightSpotter sightSpotter;

        [SerializeField]
        Transform yawPivot;

        [SerializeField]
        float yawSpeed;

        [SerializeField]
        float aimingYawSpeed;

        [SerializeField]
        float pitchSpeed = 50;

        [SerializeField]
        Transform pitchPivot;

        bool spawned = false;

        float pitchDefault;
        
        private void Awake()
        {
            yawSpeed *= UnityEngine.Random.Range(0, 2) == 0 ? 1f : -1f;
            pitchDefault = pitchPivot.localEulerAngles.x;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
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
            if (!PlayerManager.Instance.PlayerInGameAll())
                return;

            switch(stateController.State)
            {
                case SecurityState.Normal:
                    UpdateNormalState();
                    break;
                case SecurityState.Spotted:
                    UpdateSpottedState();
                    break;
                case SecurityState.Alarmed:
                    UpdateAlarmeddState();
                    break;
                case SecurityState.Freezed:
                    UpdateFreezedState();
                    break;

            }
        }

        private void UpdateFreezedState()
        {
            
        }

        private void UpdateAlarmeddState()
        {
            if (sightSpotter.CurrentTarget)
            {
                YawTarget();
                PitchTarget();
            }
            else
            {
                YawNoTarget();
                PitchNoTarget();
            }
                
        }

        private void UpdateSpottedState()
        {
            if(SessionManager.Instance.NetworkRunner.IsSinglePlayer || SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
            {
                if (!sightSpotter.CurrentTarget) // You should have a target at this point
                    return;

                // Yaw
                YawTarget();
              
                // Pitch
                PitchTarget();
            }
            
        }

        private void UpdateNormalState()
        {
            if (SessionManager.Instance.NetworkRunner.IsSinglePlayer || SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
            {
                YawNoTarget();

                PitchNoTarget();
                
                
            }
         
        }

    

      

        void YawTarget()
        {
            Vector3 direction = sightSpotter.CurrentTarget.transform.position - yawPivot.position;
            float angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(direction, Vector3.up), yawPivot.forward, Vector3.up);
            if (angle != 0)
            {
                float yawRot = Mathf.Min(Mathf.Abs(aimingYawSpeed * Time.fixedDeltaTime), Mathf.Abs(angle));
                yawRot *= -Mathf.Sign(angle);
                yawPivot.rotation *= Quaternion.AngleAxis(yawRot, Vector3.up);
            }
        }

        void YawNoTarget()
        {
            Quaternion yawMat = Quaternion.AngleAxis(yawSpeed * Time.fixedDeltaTime, Vector3.up);
            yawPivot.rotation *= yawMat;
        }

        void PitchNoTarget()
        {
            float pitch = pitchPivot.localEulerAngles.x;
            float pitchRot = 0;
            if (pitch != pitchDefault)
            {
                pitchRot = Mathf.Min(Mathf.Abs(pitchSpeed * Time.fixedDeltaTime), Mathf.Abs(pitch - pitchDefault));
                pitchRot *= Mathf.Sign(pitchDefault - pitch);
                pitchPivot.rotation *= Quaternion.AngleAxis(pitchRot, Vector3.right);
            }
        }

        void PitchTarget()
        {
            Vector3 direction = sightSpotter.CurrentTarget.transform.position - pitchPivot.position;
            float angle = Vector3.SignedAngle(Vector3.ProjectOnPlane(direction, pitchPivot.right), pitchPivot.forward, pitchPivot.right);
            if (angle != 0)
            {
                float pitchRot = Mathf.Min(Mathf.Abs(pitchSpeed * Time.fixedDeltaTime), Mathf.Abs(angle));
                pitchRot *= -Mathf.Sign(angle);
                pitchPivot.rotation *= Quaternion.AngleAxis(pitchRot, Vector3.right);
            }
        }
    }

}
