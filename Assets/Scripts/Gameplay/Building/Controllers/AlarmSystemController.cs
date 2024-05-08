using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    public enum AlarmSystemState : byte { Deactivated, Activated }

    public class AlarmSystemController : NetworkBehaviour
    {
        public static UnityAction<AlarmSystemController, AlarmSystemState, AlarmSystemState> OnStateChanged;
        public static UnityAction<AlarmSystemController> OnSpawned;

        [UnitySerializeField]
        [Networked]
        public int FloorLevel {  get; set; }

        [UnitySerializeField]
        [Networked]
        public AlarmSystemState State { get; private set; } = AlarmSystemState.Deactivated;

        ChangeDetector changeDetector;

        private void Update()
        {
            DetectChanges();
        }

        public override void Spawned()
        {
            base.Spawned();

            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            OnSpawned?.Invoke(this);
            
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
                        var stateReader = GetPropertyReader<AlarmSystemState>(propertyName);
                        var (statePrev, stateCurr) = stateReader.Read(previousBuffer, currentBuffer);
                        EnterNewState(statePrev, stateCurr);
                        break;
                }
            }
        }

        void EnterNewState(AlarmSystemState oldState, AlarmSystemState newState)
        {
            OnStateChanged?.Invoke(this, oldState, newState);
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
        public void SwitchAlarmOnRpc()
        {
            State = AlarmSystemState.Activated;
        }

        public void SwitchAlarmOff()
        {
            State = AlarmSystemState.Deactivated;
        }

        public static AlarmSystemController GetAlarmSystemController(int floorLevel)
        {
            return FindObjectsOfType<AlarmSystemController>().Where(o=>o.FloorLevel == floorLevel).FirstOrDefault();
        }
    }

}
