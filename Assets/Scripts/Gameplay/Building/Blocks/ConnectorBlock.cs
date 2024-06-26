using Fusion;
using HKR.Building;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    public class ConnectorBlock : BuildingBlock
    {

        [UnitySerializeField]
        [Networked]
        [Capacity(LevelBuilder.MaxFloorCount)]
        public NetworkLinkedList<int> ConnectedFloorLevels { get; } = default;// MakeInitializer<int>(new int[] { });

        

        public override void Spawned()
        {
            base.Spawned();

        }

        
    }

}
