using Fusion;
using HKR.Building;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class ConnectorBlock : BuildingBlock
    {

        [UnitySerializeField]
        [Networked]
        [Capacity(LevelBuilder.MaxFloorCount)]
        public NetworkLinkedList<int> ConnectedFloorLevels { get; } = MakeInitializer<int>(new int[] { });

    }

}
