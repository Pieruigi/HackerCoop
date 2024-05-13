using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    public enum InfectedNodeState : byte { Infected, Clear, Locked, Hacking }

    

    public class InfectionNodeController : NetworkBehaviour
    {
        public UnityAction</*Old*/InfectedNodeState, /*New*/InfectedNodeState> OnStateChanged;

        [UnitySerializeField]
        [Networked]
        public InfectedNodeState State { get; private set; }

        [UnitySerializeField]
        [Networked]
        public int FloorLevel { get; set; }

        [UnitySerializeField]
        [Networked]
        public byte InfectionType { get; set; }

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
                        EnterNewState(statePrev, stateCurr);
                        break;
                }
            }
        }

        void EnterNewState(InfectedNodeState prevState, InfectedNodeState currState)
        {
            OnStateChanged?.Invoke(prevState, currState);
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


    

        [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
        public void SendStartHackingRequestRpc(RpcInfo rpcInfo)
        {
            
            if (State != InfectedNodeState.Infected)
            {
                SendStartHackingResponseRpc(rpcInfo.Source, false);
            }
            else
            {
                State = InfectedNodeState.Hacking;
                SendStartHackingResponseRpc(rpcInfo.Source, true);
            }

            

        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        public void SendStartHackingResponseRpc([RpcTarget] PlayerRef target, NetworkBool allowed)
        {
            if(allowed)
                PlayerController.Local.GetComponentInChildren<HackingController>().StartHacking(this);
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
        public void ResetHackingStateRpc()
        {

            if (State != InfectedNodeState.Hacking)
                return;
            
            State = InfectedNodeState.Infected;
        }

        [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
        public void SetClearStateRpc()
        {
            if (State != InfectedNodeState.Hacking)
                return;

            State = InfectedNodeState.Clear;
        }

    }

}
