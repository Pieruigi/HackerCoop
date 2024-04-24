using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public enum InfectedNodeState : byte { Infected, Clear }

    public class InfectionNodeController : NetworkBehaviour
    {
        [UnitySerializeField]
        [Networked]
        public InfectedNodeState State { get; private set; }

        ChangeDetector changeDetector;

        private void Update()
        {
            DetectChanges();
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
                        var stateReader = GetPropertyReader<PlayerState>(propertyName);
                        var (statePrev, stateCurr) = stateReader.Read(previousBuffer, currentBuffer);
                        //EnterNewState(statePrev, stateCurr);
                        break;
                }
            }
        }
    }

}
