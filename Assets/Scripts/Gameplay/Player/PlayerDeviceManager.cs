using HKR.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    
    [System.Serializable]
    public class PlayerDeviceManager : SingletonPersistent<PlayerDeviceManager>
    {

        public UnityAction<PlayerDevice> OnDeviceAdded;
        public UnityAction<PlayerDevice> OnDeviceRemoved;
        public UnityAction<PlayerDevice> OnDeviceUpdated;

        PlayerDevice hackingDevice;
        PlayerDevice radarDevice;
        PlayerDevice flashlight;
        PlayerDevice empDevice;
        PlayerDevice alarmDevice;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR

            if(Input.GetKeyDown(KeyCode.K))
            {
                ResetAllToDefault();
            }

            if(Input.GetKeyDown(KeyCode.U))
            {
                UpgradeHackingDeviceRangeLevel(1);
            }
#endif
        }

        #region common private methods
        PlayerDevice CreateDevice(DeviceAsset asset)
        {
            GameObject device = GameObject.Instantiate(asset.Prefab);
            return device.GetComponent<PlayerDevice>();
        }

        void DestroyDevice(PlayerDevice device)
        {
            if (!device)
                return;
            Destroy(device.gameObject);
            
        }

        #endregion

        #region hacking device private methods
        void AddHackingDevice()
        {
            if (hackingDevice)
                return;
            //GameObject device = GameObject.Instantiate(hackingDeviceAsset.Prefab);
            //hackingDevice = device.GetComponent<PlayerDevice>();
            hackingDevice = CreateDevice(DeviceDatabase.Instance.HackingDeviceAsset);
            InitHackingDevice();
            OnDeviceAdded?.Invoke(hackingDevice);
        }

        void InitHackingDevice()
        {
            bool throughObstacles = AccountManager.Instance.CloudPrefs.hackingDeviceThroughObstacles;
            float range = (float)DeviceDatabase.Instance.HackingDeviceAsset.Data.Details[0].Levels[AccountManager.Instance.CloudPrefs.hackingDeviceRangeLevel].Value;
            float speed = (float)DeviceDatabase.Instance.HackingDeviceAsset.Data.Details[1].Levels[AccountManager.Instance.CloudPrefs.hackingDeviceSpeedLevel].Value;
            int memory = (int)DeviceDatabase.Instance.HackingDeviceAsset.Data.Details[2].Levels[AccountManager.Instance.CloudPrefs.hackingDeviceMemoryLevel].Value;
            hackingDevice.GetComponent<HackingController>().Init(throughObstacles, range, speed, memory);
        }

        void ResetHackingDevicePrefs()
        {
            AccountManager.Instance.CloudPrefs.ResetHackingDevice();
        }
        #endregion

        #region radar device private methods

        void AddRadarDevice()
        {
            if (/*!AccountManager.Instance.CloudPrefs.radarDeviceAvailable || */radarDevice)
                return;

            //GameObject device = GameObject.Instantiate(radarDeviceAsset.Prefab);
            radarDevice = CreateDevice(DeviceDatabase.Instance.RadarDeviceAsset);//device.GetComponent<PlayerDevice>();
            InitRadarDevice();
            OnDeviceAdded?.Invoke(radarDevice);
        }

        void InitRadarDevice()
        {
            float range = (float)DeviceDatabase.Instance.RadarDeviceAsset.Data.Details[0].Levels[AccountManager.Instance.CloudPrefs.radarDeviceRangeLevel].Value;
            radarDevice.GetComponent<RadarDeviceController>().Init(range);
        }

        void ResetRadarDevicePrefs()
        {
            AccountManager.Instance.CloudPrefs.ResetRadarDevice();
        }

        void RemoveRadarDevice()
        {
            DestroyDevice(radarDevice);
            OnDeviceRemoved?.Invoke(radarDevice);
        }
        #endregion

        #region flashlight private methods

        void AddFlashlight()
        {
            if (/*!AccountManager.Instance.CloudPrefs.flashlightAvailable || */flashlight)
                return;

            //GameObject device = GameObject.Instantiate(flashlightAsset.Prefab);
            flashlight = CreateDevice(DeviceDatabase.Instance.FlashlightAsset);//device.GetComponent<PlayerDevice>();
            
            InitFlashlight();
            OnDeviceAdded?.Invoke(flashlight);
        }

        void InitFlashlight()
        {
            float chargeMax = (float)DeviceDatabase.Instance.FlashlightAsset.Data.Details[0].Levels[AccountManager.Instance.CloudPrefs.flashlightChargeLevel].Value;
            float charge = AccountManager.Instance.CloudPrefs.flashlightCharge;
            flashlight.GetComponent<FlashlightController>().Init(chargeMax, charge);
        }

        void RemoveFlashlight()
        {
            DestroyDevice(flashlight);
            OnDeviceRemoved?.Invoke(flashlight);
        }

        void ResetFlashlightPrefs()
        {
            AccountManager.Instance.CloudPrefs.ResetFlashlight();
        }
        #endregion

        #region emp device private methods
        void AddEmpDevice()
        {
            if (empDevice)
                return;

            empDevice = CreateDevice(DeviceDatabase.Instance.EmpDeviceAsset);
            InitEmpDevice();
            OnDeviceAdded?.Invoke(empDevice);
        }

        void InitEmpDevice()
        {
            float range = (float)DeviceDatabase.Instance.EmpDeviceAsset.Data.Details[0].Levels[AccountManager.Instance.CloudPrefs.empDeviceRangeLevel].Value;
            float duration = (float)DeviceDatabase.Instance.EmpDeviceAsset.Data.Details[1].Levels[AccountManager.Instance.CloudPrefs.empDeviceDurationLevel].Value;
            int chargeMax = (int)DeviceDatabase.Instance.EmpDeviceAsset.Data.Details[2].Levels[AccountManager.Instance.CloudPrefs.empDeviceChargeLevel].Value;
            int chargeLeft = AccountManager.Instance.CloudPrefs.empDeviceChargeLeft;
            empDevice.GetComponent<EmpDeviceController>().Init(chargeMax, range, duration, chargeLeft);
        }

        void RemoveEmpDevice()
        {
            DestroyDevice(empDevice);
            OnDeviceRemoved?.Invoke(empDevice);
        }

        void ResetEmpDevicePrefs()
        {
            AccountManager.Instance.CloudPrefs.ResetEmpDevice();
        }
        #endregion

        #region alarm device private methods
        void AddAlarmDevice()
        {
            if (/*!AccountManager.Instance.CloudPrefs.radarDeviceAvailable || */alarmDevice)
                return;

            //GameObject device = GameObject.Instantiate(radarDeviceAsset.Prefab);
            alarmDevice = CreateDevice(DeviceDatabase.Instance.AlarmDeviceAsset);
            InitAlarmDevice();
            OnDeviceAdded?.Invoke(alarmDevice);
        }

        void InitAlarmDevice()
        {
            float range = (float)DeviceDatabase.Instance.AlarmDeviceAsset.Data.Details[0].Levels[AccountManager.Instance.CloudPrefs.alarmDeviceRangeLevel].Value;
            float speed = (float)DeviceDatabase.Instance.AlarmDeviceAsset.Data.Details[1].Levels[AccountManager.Instance.CloudPrefs.alarmDeviceSpeedLevel].Value;
            alarmDevice.GetComponent<AlarmDeviceController>().Init(range, speed);
        }

        void ResetAlarmDevicePrefs()
        {
            AccountManager.Instance.CloudPrefs.ResetAlarmDevice();
        }

        void RemoveAlarmDevice()
        {
            DestroyDevice(alarmDevice);
            OnDeviceRemoved?.Invoke(alarmDevice);
        }
        #endregion

        #region common public methods 
        /// <summary>
        /// Called by the player equipment manager after the player has been instantiated
        /// </summary>
        public void InitializeDevices()
        {
            // Hacking device is always available
            AddHackingDevice();

            // Radar device
            if(AccountManager.Instance.CloudPrefs.radarDeviceOwned)
                AddRadarDevice();
           
            // Flashlight
            if(AccountManager.Instance.CloudPrefs.flashlightOwned)
                AddFlashlight();

            // Emp device
            if(AccountManager.Instance.CloudPrefs.empDeviceOwned)
                AddEmpDevice();

            // Radar device
            if(AccountManager.Instance.CloudPrefs.alarmDeviceOwned)
                AddAlarmDevice();
        }

        /// <summary>
        /// Called on player death
        /// </summary>
        public void ResetAllToDefault()
        {
            // Hacking device
            ResetHackingDevicePrefs();
            InitHackingDevice();
            // Radar device
            AccountManager.Instance.CloudPrefs.radarDeviceOwned = false;
            ResetRadarDevicePrefs();
            RemoveRadarDevice();
            // Flashlight
            AccountManager.Instance.CloudPrefs.flashlightOwned = false;
            ResetFlashlightPrefs();
            RemoveFlashlight();
            // Emp device
            AccountManager.Instance.CloudPrefs.empDeviceOwned = false;
            ResetEmpDevicePrefs();
            RemoveEmpDevice();
            // Alarm device 
            AccountManager.Instance.CloudPrefs.alarmDeviceOwned = false;
            ResetAlarmDevicePrefs();
            RemoveAlarmDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }
        #endregion

        #region hacking device public methods
        public void UpgradeHackingDeviceRangeLevel(int level)
        {
            // Check level
            //if (level > PlayerDeviceConstants.HackingDeviceRangeLevels.Length - 1)
            //    return;
            if (level > DeviceDatabase.Instance.HackingDeviceAsset.Data.Details[0].Levels.Count - 1)
                return;

            // Update prefs
            AccountManager.Instance.CloudPrefs.hackingDeviceRangeLevel = level;
            // Update device
            InitHackingDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }

        public void UpgradeHackingDeviceSpeedLevel(int level)
        {
            // Check level
            if (level > DeviceDatabase.Instance.HackingDeviceAsset.Data.Details[1].Levels.Count - 1)
                return;

            // Update prefs
            AccountManager.Instance.CloudPrefs.hackingDeviceSpeedLevel = level;
            // Update device
            InitHackingDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }

        public void UpgradeHackingDeviceMemoryLevel(int level)
        {
            // Check level
            if (level > DeviceDatabase.Instance.HackingDeviceAsset.Data.Details[2].Levels.Count - 1)
                return;

            // Update prefs
            AccountManager.Instance.CloudPrefs.hackingDeviceMemoryLevel = level;
            // Update device
            InitHackingDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }

        public void UpgradeHackingDeviceThroughObstacles()
        {
            
            // Update prefs
            AccountManager.Instance.CloudPrefs.hackingDeviceThroughObstacles = true;
            // Update device
            InitHackingDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }

        #endregion

        #region radar device public methods
        /// <summary>
        /// Called on purchase
        /// </summary>
        public void PurchaseRadarDevice()
        {
            AccountManager.Instance.CloudPrefs.radarDeviceOwned = true;
            AccountManager.Instance.StoreCloudPrefs();
            AddRadarDevice();
        }

        public void UpgradeRadarDeviceRangeLevel(int level)
        {
            // Check level
            if (level > DeviceDatabase.Instance.RadarDeviceAsset.Data.Details[0].Levels.Count - 1)
                return;

            // Update prefs
            AccountManager.Instance.CloudPrefs.radarDeviceRangeLevel = level;
            // Update device
            InitRadarDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }

        #endregion

        #region flashlight public methods
        /// <summary>
        /// Called on purchase
        /// </summary>
        public void PurchaseFlashlight()
        {
            AccountManager.Instance.CloudPrefs.flashlightOwned = true;
            AccountManager.Instance.CloudPrefs.flashlightCharge = (float)DeviceDatabase.Instance.FlashlightAsset.Data.Details[0].Levels[0].Value; // Just purchased
            AccountManager.Instance.StoreCloudPrefs();
            AddFlashlight();
        }

        public void UpgradeFlashlightChargeLevel(int level)
        {
            // Check level
            if (level > DeviceDatabase.Instance.FlashlightAsset.Data.Details[0].Levels.Count - 1)
                return;

            // Update prefs
            AccountManager.Instance.CloudPrefs.flashlightChargeLevel = level;
            AccountManager.Instance.CloudPrefs.flashlightCharge = (float)DeviceDatabase.Instance.FlashlightAsset.Data.Details[0].Levels[level].Value;
            // Update device
            InitFlashlight();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }

        public void RechargeFlashlight()
        {
            // Update prefs
            int level = AccountManager.Instance.CloudPrefs.flashlightChargeLevel;
            AccountManager.Instance.CloudPrefs.flashlightCharge = (float)DeviceDatabase.Instance.FlashlightAsset.Data.Details[0].Levels[level].Value;
            // Update device
            InitFlashlight();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();

        }

        #endregion

        #region emp device public methods
        /// <summary>
        /// Called on purchase
        /// </summary>
        public void PurchaseEmpDevice()
        {
            AccountManager.Instance.CloudPrefs.empDeviceOwned = true;
            AccountManager.Instance.CloudPrefs.empDeviceChargeLeft = -1; // We just purchased the device so we set max charge
            AccountManager.Instance.StoreCloudPrefs();
            AddEmpDevice();
        }

        public void UpgradeEmpDeviceChargeLevel(int level)
        {
            // Check level
            if (level > DeviceDatabase.Instance.EmpDeviceAsset.Data.Details[2].Levels.Count - 1)
                return;

            // Update prefs
            AccountManager.Instance.CloudPrefs.empDeviceChargeLevel = level;
            AccountManager.Instance.CloudPrefs.empDeviceChargeLeft = (int)DeviceDatabase.Instance.EmpDeviceAsset.Data.Details[2].Levels[level].Value;
            // Update device
            InitEmpDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }

        public void UpgradeEmpDeviceRangeLevel(int level)
        {
            // Check level
            if (level > DeviceDatabase.Instance.EmpDeviceAsset.Data.Details[0].Levels.Count - 1)
                return;

            // Update prefs
            AccountManager.Instance.CloudPrefs.empDeviceRangeLevel = level;
            // Update device
            InitEmpDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }

        public void UpgradeEmpDeviceDurationLevel(int level)
        {
            // Check level
            if (level > DeviceDatabase.Instance.EmpDeviceAsset.Data.Details[1].Levels.Count - 1)
                return;

            // Update prefs
            AccountManager.Instance.CloudPrefs.empDeviceDurationLevel = level;
            // Update device
            InitEmpDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }

        public void RechargeEmpDevice()
        {
            // Update prefs
            int level = AccountManager.Instance.CloudPrefs.empDeviceChargeLevel;
            AccountManager.Instance.CloudPrefs.empDeviceChargeLeft = (int)DeviceDatabase.Instance.EmpDeviceAsset.Data.Details[2].Levels[level].Value;
            // Update device
            InitEmpDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }
        #endregion

        #region alarm device public methods
        /// <summary>
        /// Called on purchase
        /// </summary>
        public void PurchaseAlarmDevice()
        {
            AccountManager.Instance.CloudPrefs.alarmDeviceOwned = true;
            AccountManager.Instance.StoreCloudPrefs();
            AddAlarmDevice();
        }

        public void UpgradeAlarmDeviceRangeLevel(int level)
        {
            // Check level
            if (level > DeviceDatabase.Instance.AlarmDeviceAsset.Data.Details[0].Levels.Count - 1)
                return;

            // Update prefs
            AccountManager.Instance.CloudPrefs.alarmDeviceRangeLevel = level;
            // Update device
            InitAlarmDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }

        public void UpgradeAlarmDeviceSpeedLevel(int level)
        {
            // Check level
            if (level > DeviceDatabase.Instance.AlarmDeviceAsset.Data.Details[1].Levels.Count - 1)
                return;

            // Update prefs
            AccountManager.Instance.CloudPrefs.alarmDeviceSpeedLevel = level;
            // Update device
            InitAlarmDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }
        #endregion
    }

}
