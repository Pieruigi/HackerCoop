using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{

    [System.Serializable]
    public class SteamCloudPrefs
    {
        // Hacking device
        public int hackingDeviceRangeLevel = 0;
        public int hackingDeviceMemoryLevel = 0;
        public int hackingDeviceSpeedLevel = 0;
        
        public bool hackingDeviceThroughObstacles = false;
       
        // Radar device
        public bool radarDeviceAvailable = true;
        public int radarDeviceRangeLevel = 0;

        // Spotlight
        public bool flashlightAvailable = true;


        public void ResetHackingDevice()
        {
            hackingDeviceMemoryLevel = 0;
            hackingDeviceRangeLevel = 0;
            hackingDeviceSpeedLevel = 0;
            hackingDeviceThroughObstacles = false;
        }

        public void ResetRadarDevice()
        {
            radarDeviceRangeLevel = 0;
        }

        public void ResetFlashlight()
        {
        }
    }

}
