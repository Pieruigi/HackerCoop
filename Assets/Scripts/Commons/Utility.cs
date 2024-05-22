using Fusion;
using HKR.Building;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class Utility
    {
        public static int GetFloorLevelByVerticalCoordinate(float verticalCoordinate)
        {
            int level = Mathf.FloorToInt(verticalCoordinate / BuildingBlock.Height);
            return level;
        }
    }

}
