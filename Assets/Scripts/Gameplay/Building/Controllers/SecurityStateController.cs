using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    public enum SecurityState { Normal, Spotted, /*Alarmed, */Freezed, Searching }

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

        [SerializeField]
        float searchingTime = 7f;

        [SerializeField]
        float alarmTolleranceThreshold = 4;

        ChangeDetector changeDetector;

        DateTime freezingTime;
        float searchingElapsed = 0;
        float spottedElapsed = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void Update()
        {
            DetectChanges();

            // Single player and master client only
            if (SessionManager.Instance.NetworkRunner.IsSinglePlayer || SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
            {
                switch (State)
                {
                    case SecurityState.Freezed:
                        // We check the recovery from freezing
                        if ((System.DateTime.Now - freezingTime).TotalSeconds > freezingRecoveryTime)
                        {
                            State = SecurityState.Normal;
                        }
                        break;
                    case SecurityState.Searching:
                        searchingElapsed += Time.deltaTime;
                        if(searchingElapsed >= searchingTime)
                        {
                            State = SecurityState.Normal;
                        }
                        break;
                    case SecurityState.Spotted:
                        spottedElapsed += Time.deltaTime;
                        AlarmSystemController asc = AlarmSystemController.GetAlarmSystemController(FloorLevel);
                        if(asc.State == AlarmSystemState.Activated)
                        {
                            asc.ResetAlarmTimer();
                        }
                        else
                        {
                            if (spottedElapsed >= alarmTolleranceThreshold)
                            {
                                asc.SwitchAlarmOnRpc();
                            }
                        }
                        
                        break;
                }
                
                
            }

            
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
                    //if (State == SecurityState.Alarmed || State == SecurityState.Freezed)
                    //    return;
                    //State = SecurityState.Alarmed;
                    break;
                case AlarmSystemState.Deactivated:
                    //if (State == SecurityState.Freezed)
                    //    return;
                    //State = SecurityState.Normal;
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
            switch (newState)
            {
                case SecurityState.Searching:
                    UnityEngine.Debug.Log("Enter searching...");
                    searchingElapsed = 0;
                    break;
                case SecurityState.Spotted:
                    spottedElapsed = 0;
                    break;
                case SecurityState.Freezed:
                    break;
                default: 
                    break;
            }

            OnStateChanged?.Invoke(oldState, newState);
        }
    }

}
