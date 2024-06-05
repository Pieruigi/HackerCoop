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

        PlayerDevice hackingDevice;
        PlayerDevice radarDevice;
        PlayerDevice flashlight;


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
                SetHackingDeviceRangeLevel(1);
            }
#endif
        }

        

        #region hacking device private methods
        void AddHackingDevice()
        {
            if (hackingDevice)
                return;
            GameObject device = GameObject.Instantiate(hackingDeviceAsset.Prefab);
            hackingDevice = device.GetComponent<PlayerDevice>();
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

            GameObject device = GameObject.Instantiate(radarDeviceAsset.Prefab);
            radarDevice = device.GetComponent<PlayerDevice>();
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

            GameObject device = GameObject.Instantiate(flashlightAsset.Prefab);
            flashlight = device.GetComponent<PlayerDevice>();
            OnDeviceAdded?.Invoke(flashlight);
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
            // Store data on Steam cloud
            AccountManager.Instance.StoreCloudPrefs();
        }
        #endregion

        #region hacking device public methods
        public void SetHackingDeviceRangeLevel(int level)
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

        #endregion

        #region radar device public methods
        /// <summary>
        /// Called on purchase
        /// </summary>
        public void SetRadarDeviceAvailable()
        {
            AccountManager.Instance.CloudPrefs.radarDeviceAvailable = true;
            AccountManager.Instance.StoreCloudPrefs();
            AddRadarDevice();
        }

        #endregion

        #region flashlight public methods
        /// <summary>
        /// Called on purchase
        /// </summary>
        public void SetFlashlightAvailable()
        {
            AccountManager.Instance.CloudPrefs.flashlightAvailable = true;
            AccountManager.Instance.StoreCloudPrefs();
            AddFlashlight();
        }

        
        
        #endregion
    }

}
