using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    public enum SecurityState { Normal, Spotted, Alarmed, Freezed }

    public class SecurityStateController : NetworkBehaviour
    {
        public UnityAction</*old*/SecurityState, /*new*/SecurityState> OnStateChanged;
        public UnityAction OnSpawned;

        [UnitySerializeField]
        [Networked]
        public SecurityState State { get; set; } = SecurityState.Normal;

        [UnitySerializeField]
        [Networked]
        public int FloorLevel { get; set; }

        [SerializeField]
        float freezingRecoveryTime = 10f;

        ChangeDetector changeDetector;

        DateTime freezingTime;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void Update()
        {
            // Single player and master client only
            if (SessionManager.Instance.NetworkRunner.IsSinglePlayer || SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
            {
                if(State == SecurityState.Freezed)
                {
                    // We check the recovery from freezing
                    if ((System.DateTime.Now - freezingTime).TotalSeconds > freezingRecoveryTime)
                    {
                        if (AlarmSystemController.GetAlarmSystemController(FloorLevel).State == AlarmSystemState.Activated)
                            State = SecurityState.Alarmed;
                        else
                            State = SecurityState.Normal;
                    }
                }
                
            }

            DetectChanges();
        }

        private void OnEnable()
        {
            AlarmSystemController.OnStateChanged += HandleOnAlarmSystemStateChanged;
        }

        private void OnDisable()
        {
            AlarmSystemController.OnStateChanged -= HandleOnAlarmSystemStateChanged;
        }

        private void HandleOnAlarmSystemStateChanged(AlarmSystemController systemController, AlarmSystemState oldState, AlarmSystemState newState)
        {
            switch(newState)
            {
                case AlarmSystemState.Activated:
                    if (State == SecurityState.Alarmed || State == SecurityState.Freezed)
                        return;
                    State = SecurityState.Alarmed;
                    break;
                case AlarmSystemState.Deactivated:
                    if (State == SecurityState.Freezed)
                        return;
                    State = SecurityState.Normal;
                    break;

            }
            
        }

        public override void Spawned()
        {
            base.Spawned();

            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            OnSpawned?.Invoke();
        }



        void DetectChanges()
        {
            if (changeDetector == null)
                return;
            foreach (var propertyName in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (propertyName)
                {

                    case nameof(State):
                        var stateReader = GetPropertyReader<SecurityState>(propertyName);
                        var (statePrev, stateCurr) = stateReader.Read(previousBuffer, currentBuffer);
                        EnterNewState(statePrev, stateCurr);
                        break;
                }
            }
        }

        void EnterNewState(SecurityState oldState, SecurityState newState)
        {
            

            OnStateChanged?.Invoke(oldState, newState);
        }
    }

}
