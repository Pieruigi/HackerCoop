using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    public enum SecurityState { Normal, Suspect, Spotted, Freezed }

    public class SecurityStateController : NetworkBehaviour
    {
        public UnityAction</*old*/SecurityState, /*new*/SecurityState> OnStateChanged;
        public UnityAction OnSpawned;

        [UnitySerializeField]
        [Networked]
        public SecurityState State { get; set; } = SecurityState.Normal;

        

        ChangeDetector changeDetector;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void Update()
        {
            DetectChanges();
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
