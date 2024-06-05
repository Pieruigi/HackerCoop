using HKR.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    
    [System.Serializable]
    public partial class PlayerDeviceManager : SingletonPersistent<PlayerDeviceManager>
    {

        public UnityAction<PlayerDevice> OnDeviceAdded;
        public UnityAction<PlayerDevice> OnDeviceRemoved;
        public UnityAction<PlayerDevice> OnDeviceUpdated;


        [SerializeField]
        DeviceAsset hackingDeviceAsset;

        [SerializeField]
        DeviceAsset radarDeviceAsset;

        [SerializeField]
        DeviceAsset flashlightAsset;

        [SerializeField]
        DeviceAsset empDeviceAsset;

        PlayerDevice hackingDevice;
        PlayerDevice radarDevice;
        PlayerDevice flashlight;
        PlayerDevice empDevice;


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

        #endregion

        #region hacking device private methods
        void AddHackingDevice()
        {
            if (hackingDevice)
                return;
            //GameObject device = GameObject.Instantiate(hackingDeviceAsset.Prefab);
            //hackingDevice = device.GetComponent<PlayerDevice>();
            hackingDevice = CreateDevice(hackingDeviceAsset);
            InitHackingDevice();
            OnDeviceAdded?.Invoke(hackingDevice);
        }

        void InitHackingDevice()
        {
            bool throughObstacles = AccountManager.Instance.CloudPrefs.hackingDeviceThroughObstacles;
            int rangeLevel = AccountManager.Instance.CloudPrefs.hackingDeviceRangeLevel;
            int speedLevel = AccountManager.Instance.CloudPrefs.hackingDeviceSpeedLevel;
            int memoryLevel = AccountManager.Instance.CloudPrefs.hackingDeviceMemoryLevel;
            hackingDevice.GetComponent<HackingController>().Init(throughObstacles, rangeLevel, speedLevel, memoryLevel);
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
            radarDevice = CreateDevice(radarDeviceAsset);//device.GetComponent<PlayerDevice>();
            InitRadarDevice();
            OnDeviceAdded?.Invoke(radarDevice);
        }

        void InitRadarDevice()
        {
            int rangeLevel = AccountManager.Instance.CloudPrefs.radarDeviceRangeLevel;
            radarDevice.GetComponent<RadarController>().Init(rangeLevel);
        }

        void ResetRadarDevicePrefs()
        {
            AccountManager.Instance.CloudPrefs.ResetRadarDevice();
        }

        void RemoveRadarDevice()
        {
            if (!radarDevice)
                return;

            Destroy(radarDevice.gameObject);
            OnDeviceRemoved?.Invoke(radarDevice);
        }
        #endregion

        #region flashlight private methods

        void AddFlashlight()
        {
            if (/*!AccountManager.Instance.CloudPrefs.flashlightAvailable || */flashlight)
                return;

            //GameObject device = GameObject.Instantiate(flashlightAsset.Prefab);
            flashlight = CreateDevice(flashlightAsset);//device.GetComponent<PlayerDevice>();
            
            InitFlashlight();
            OnDeviceAdded?.Invoke(flashlight);
        }

        void InitFlashlight()
        {
            int chargeLevel = AccountManager.Instance.CloudPrefs.flashlightChargeLevel;
            float charge = AccountManager.Instance.CloudPrefs.flashlightCharge;
            flashlight.GetComponent<FlashlightController>().Init(chargeLevel, charge);
        }

        void RemoveFlashlight()
        {
            if (!flashlight)
                return;

            Destroy(flashlight.gameObject);
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

            empDevice = CreateDevice(empDeviceAsset);
            InitEmpDevice();
            OnDeviceAdded?.Invoke(empDevice);
        }

        void InitEmpDevice()
        {
            int rangeLevel = AccountManager.Instance.CloudPrefs.empDeviceRangeLevel;
            int durationLevel = AccountManager.Instance.CloudPrefs.empDeviceDurationLevel;
            int chargeLeft = AccountManager.Instance.CloudPrefs.empDeviceChargeLeft;
            empDevice.GetComponent<EmpDeviceController>().Init(rangeLevel, durationLevel, chargeLeft);
        }

        void RemoveEmpDevice()
        {
            if (!empDevice)
                return;

            Destroy(empDevice.gameObject);
            OnDeviceRemoved?.Invoke(empDevice);
        }

        void ResetEmpDevicePrefs()
        {
            AccountManager.Instance.CloudPrefs.ResetEmpDevice();
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
            if(AccountManager.Instance.CloudPrefs.radarDeviceAvailable)
                AddRadarDevice();
           
            // Flashlight
            if(AccountManager.Instance.CloudPrefs.flashlightAvailable)
                AddFlashlight();

            // Emp device
            if(AccountManager.Instance.CloudPrefs.empDeviceAvailable)
                AddEmpDevice();
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
            AccountManager.Instance.CloudPrefs.radarDeviceAvailable = false;
            ResetRadarDevicePrefs();
            RemoveRadarDevice();
            // Flashlight
            AccountManager.Instance.CloudPrefs.flashlightAvailable = false;
            ResetFlashlightPrefs();
            RemoveFlashlight();
            // Emp device
            AccountManager.Instance.CloudPrefs.empDeviceAvailable = false;
            ResetEmpDevicePrefs();
            RemoveEmpDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }
        #endregion

        #region hacking device public methods
        public void UpgradeHackingDeviceRangeLevel(int level)
        {
            // Check level
            if (level > PlayerDeviceConstants.HackingDeviceRangeLevelMax)
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
            if (level > PlayerDeviceConstants.HackingDeviceSpeedLevelMax)
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
            if (level > PlayerDeviceConstants.HackingDeviceMemoryLevelMax)
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
            AccountManager.Instance.CloudPrefs.radarDeviceAvailable = true;
            AccountManager.Instance.StoreCloudPrefs();
            AddRadarDevice();
        }

        public void UpgradeRadarDeviceRangeLevel(int level)
        {
            // Check level
            if (level > PlayerDeviceConstants.RadarDeviceRangeLevelMax)
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
            AccountManager.Instance.CloudPrefs.flashlightAvailable = true;
            AccountManager.Instance.CloudPrefs.flashlightCharge = -1; // We just purchased the flashlight so we set the max charge
            AccountManager.Instance.StoreCloudPrefs();
            AddFlashlight();
        }

        public void UpgradeFlashlightChargeLevel(int level)
        {
            // Check level
            if (level > PlayerDeviceConstants.FlashlightChargeLevelMax)
                return;

            // Update prefs
            AccountManager.Instance.CloudPrefs.flashlightChargeLevel = level;
            AccountManager.Instance.CloudPrefs.flashlightCharge = -1;
            // Update device
            InitFlashlight();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }

        public void RechargeFlashlight()
        {
            // Update prefs
            AccountManager.Instance.CloudPrefs.flashlightCharge = 1.0f;
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
            AccountManager.Instance.CloudPrefs.empDeviceAvailable = true;
            AccountManager.Instance.CloudPrefs.empDeviceChargeLeft = -1; // We just purchased the device so we set max charge
            AccountManager.Instance.StoreCloudPrefs();
            AddEmpDevice();
        }

        public void UpgradeEmpDeviceRangeLevel(int level)
        {
            // Check level
            if (level > PlayerDeviceConstants.EmpDeviceRangeLevelMax)
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
            if (level > PlayerDeviceConstants.EmpDeviceDurationLevelMax)
                return;

            // Update prefs
            AccountManager.Instance.CloudPrefs.empDeviceDurationLevel = level;
            // Update device
            InitEmpDevice();
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }
        #endregion
    }

}
