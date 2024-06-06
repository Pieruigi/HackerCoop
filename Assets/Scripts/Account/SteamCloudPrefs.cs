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
        public bool radarDeviceOwned = true;
        public int radarDeviceRangeLevel = 0;

        // Spotlight
        public bool flashlightOwned = true;
        public int flashlightChargeLevel = 0;
        public float flashlightCharge = -1; 

        // Emp device
        public bool empDeviceOwned = true;
        public int empDeviceRangeLevel = 0;
        public int empDeviceDurationLevel = 0;
        public int empDeviceChargeLeft = -1;
        public int empDeviceChargeLevel = 0;

        // Alarm device
        public bool alarmDeviceOwned = true;
        public int alarmDeviceRangeLevel = 0;
        public int alarmDeviceSpeedLevel = 0;

        #region reset methods
        public void ResetDefault()
        {
            ResetHackingDevice();
            ResetRadarDevice();
            ResetFlashlight();
            ResetEmpDevice();
        }

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
            flashlightChargeLevel = 0;
            flashlightCharge = (float)DeviceDatabase.Instance.FlashlightAsset.Data.Details[0].Levels[0].Value;
        }

        public void ResetEmpDevice()
        {
            empDeviceRangeLevel = 0;
            empDeviceDurationLevel = 0;
            empDeviceChargeLeft = (int)DeviceDatabase.Instance.EmpDeviceAsset.Data.Details[2].Levels[0].Value;
            empDeviceChargeLevel = 0;
        }

        public void ResetAlarmDevice()
        {
            alarmDeviceRangeLevel = 0;
            alarmDeviceSpeedLevel = 0;
        }
        #endregion
    }

}
