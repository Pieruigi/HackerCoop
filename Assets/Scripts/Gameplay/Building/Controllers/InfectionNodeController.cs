using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public enum InfectedNodeState : byte { Infected, Clear, Locked, Hacking }

    public class InfectionNodeController : NetworkBehaviour
    {
        [UnitySerializeField]
        [Networked]
        public InfectedNodeState State { get; private set; }

        [UnitySerializeField]
        [Networked]
        public int FloorLevel { get; set; }

        ChangeDetector changeDetector;

        AlarmSystemController alarmSystemController;


        private void Update()
        {
            DetectChanges();
        }

        private void OnEnable()
        {
            AlarmSystemController.OnSpawned += HandleOnAlarmSystemSpawned;
            AlarmSystemController.OnStateChanged += HandleOnAlarmSystemStateChanged;
        }

        private void OnDisable()
        {
            AlarmSystemController.OnSpawned -= HandleOnAlarmSystemSpawned;
            AlarmSystemController.OnStateChanged -= HandleOnAlarmSystemStateChanged;
        }

        
        public override void Spawned()
        {
            base.Spawned();

            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
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
                        var stateReader = GetPropertyReader<InfectedNodeState>(propertyName);
                        var (statePrev, stateCurr) = stateReader.Read(previousBuffer, currentBuffer);
                        //EnterNewState(statePrev, stateCurr);
                        break;
                }
            }
        }

        private void HandleOnAlarmSystemSpawned(AlarmSystemController arg0)
        {
            if(FloorLevel == arg0.FloorLevel)
                alarmSystemController = arg0;
        }

        private void HandleOnAlarmSystemStateChanged(AlarmSystemController arg0, AlarmSystemState oldState, AlarmSystemState newState)
        {
            if (FloorLevel != arg0.FloorLevel)
                return;

            switch (newState)
            {
                case AlarmSystemState.Activated:
                    if(State != InfectedNodeState.Clear)
                        State = InfectedNodeState.Locked;
                    break;
                case AlarmSystemState.Deactivated:
                    if(State != InfectedNodeState.Clear)
                        State = InfectedNodeState.Infected;
                    break;
            }
        }


        public void OnHackingFailed()
        {
            if (State != InfectedNodeState.Hacking) return;
            // Activate alarm 
            alarmSystemController.SwitchAlarmOn();
        }

        public void OnHackingSucceeded()
        {
            if (State != InfectedNodeState.Hacking) return;
            State = InfectedNodeState.Clear;
        }
    }

}
