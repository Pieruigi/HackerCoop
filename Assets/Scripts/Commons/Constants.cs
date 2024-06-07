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
        public const string Pickable = "Pickable";
    }

    public class Tags
    {
        public const string Player = "Player";
        public const string InfectionNode = "InfectionNode";
    }

    public class PlayerDeviceConstants
    {
        // Hacking device
        //public static readonly float[] HackingDeviceRangeLevels = { 2, 3, 4 };
        //public static readonly float[] HackingDeviceSpeedLevels = { 1, 1.5f, 2 };
        //public static readonly int[] HackingDeviceMemoryLevels = { 3, 4, 5 };
        

        // Radar device
        //public static readonly float[] RadarDeviceRangeLevels = { 7, 9, 11 };

        // Flashlight
        //public static readonly int[] FlashlightChargeLevels = { 100, 150, 200 };

        // Emp device
        //public static readonly int[] EmpDeviceChargeLevels = { 1 };
        //public static readonly float[] EmpDeviceRangeLevels = { 10, 15, 20 };
        //public static readonly float[] EmpDeviceDurationLevels = { 10, 15, 20 };

        // Alarm device
        //public static readonly float[] AlarmDeviceRangeLevels = { 3, 4 };
        //public static readonly float[] AlarmDeviceSpeedLevels = { 1, 1.5f, 2f };
    }


    
}
