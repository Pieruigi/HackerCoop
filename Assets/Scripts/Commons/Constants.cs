using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class Constants
    {
        public const int MainSceneIndex = 0;
        public const int LobbySceneIndex = 1;
        public const int GameSceneIndex = 2;

        public const int RarityMinLevel = 0;
        public const int RarityMaxLevel = 9;

        public const int InfectionTypeCount = 1;
        public const float HackingAppPlayDelay = 1f;
    }

    public class Layers
    {
        public const string RadarTarget = "RadarTarget";

    }

    public class Tags
    {
        public const string Player = "Player";
        public const string InfectionNode = "InfectionNode";
    }

    public class PlayerDeviceConstants
    {
        public const int HackingDeviceRangeLevelMax = 2;
        public const int HackingDeviceSpeedLevelMax = 2;
        public const int HackingDeviceMemoryLevelMax = 2;

        public const int RadarDeviceRangeLevelMax = 2;

        public const int FlashlightChargeLevelMax = 2;
        
        public const int EmpDeviceRangeLevelMax = 2;
        public const int EmpDeviceDurationLevelMax = 2;
    }

}
