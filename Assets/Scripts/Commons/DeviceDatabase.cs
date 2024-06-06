using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    #region common data
    public class DeviceDatabase: SingletonPersistent<DeviceDatabase>
    {
        [SerializeField]
        DeviceData hackingDeviceData;
        public DeviceData HackingDeviceData {  get { return hackingDeviceData; } }
        [SerializeField]
        DeviceData radarDeviceData;
        public DeviceData RadarDeviceData { get { return radarDeviceData; } }
        [SerializeField]
        DeviceData flashlightData;
        public DeviceData FlashlightData { get { return flashlightData; } }
        [SerializeField]
        DeviceData empDeviceData;
        public DeviceData EmpDeviceData { get { return empDeviceData; } }
        [SerializeField]
        DeviceData alarmDeviceData;
        public DeviceData AlarmDeviceData { get { return alarmDeviceData; } }
    }

    [System.Serializable]
    public class DeviceLevel
    {
       
        [SerializeField]
        double value;
        public double Value { get { return value; } }
        [SerializeField]
        int purchaseCost;
        public int PurchaseCost { get { return purchaseCost; } }
        [SerializeField]
        int exp;
        public int Exp { get { return exp; } }
        [SerializeField]
        int rechargeCost;
        public int RechargeCost { get { return rechargeCost; } }

    }

    [System.Serializable]
    public class DeviceDetail
    {
        [SerializeField]
        string name = string.Empty;
        public string Name { get { return name; } }

        [SerializeField]
        List<DeviceLevel> levels = new List<DeviceLevel>();
        public IList<DeviceLevel> Levels { get { return levels.AsReadOnly(); } }
    }

    [System.Serializable]
    public class DeviceData
    {
        [SerializeField]
        List<DeviceDetail> details = new List<DeviceDetail>();
        public IList<DeviceDetail> Details { get { return details.AsReadOnly(); } }
    
    }
    #endregion


    #region player device data
    //[System.Serializable]
    //public class HackingDeviceData : DeviceData
    //{
    //    [SerializeField]
    //    public DeviceLevelData[] ranges = { new DeviceLevelData() { Value = 2f, PurchaseCost = 100, Exp = 100 },
    //                              new DeviceLevelData() { Value = 3f, PurchaseCost = 100, Exp = 200 },
    //                              new DeviceLevelData() { Value = 4f, PurchaseCost = 100, Exp = 300 }};
    //    [SerializeField]
    //    public DeviceLevelData[] speeds = { new DeviceLevelData() { Value = 1f, PurchaseCost = 100, Exp = 100 },
    //                              new DeviceLevelData() { Value = 1.5f, PurchaseCost = 100, Exp = 200 },
    //                              new DeviceLevelData() { Value = 2f, PurchaseCost = 100, Exp = 300 }};
    //    [SerializeField]
    //    public DeviceLevelData[] memoryBlocks = { new DeviceLevelData() { Value = 2, PurchaseCost = 100, Exp = 100 },
    //                                    new DeviceLevelData() { Value = 3, PurchaseCost = 100, Exp = 200 },
    //                                    new DeviceLevelData() { Value = 4, PurchaseCost = 100, Exp = 300 }};


    //}

    //[System.Serializable]
    //public class RadarDeviceData: DeviceData
    //{
    //    [SerializeField]
    //    public DeviceLevelData[] ranges = { new DeviceLevelData() { Value = 2f, PurchaseCost = 100, Exp = 100 },
    //                              new DeviceLevelData() { Value = 3f, PurchaseCost = 200, Exp = 200 },
    //                              new DeviceLevelData() { Value = 4f, PurchaseCost = 300, Exp = 300 }};
    //}

    //[System.Serializable]
    //public class FlashlightData: DeviceData
    //{
    //    [SerializeField]
    //    public DeviceLevelData[] charge = { new DeviceLevelData() { Value = 3f, PurchaseCost = 100, Exp = 100 },
    //                              new DeviceLevelData() { Value = 4f, PurchaseCost = 200, Exp = 200 },
    //                              new DeviceLevelData() { Value = 5f, PurchaseCost = 300, Exp = 300 }};
    //}

    //[System.Serializable]
    //public class EmpDeviceData : DeviceData
    //{
    //    [SerializeField]
    //    public DeviceLevelData[] ranges = { new DeviceLevelData() { Value = 2f, PurchaseCost = 100, Exp = 100 },
    //                              new DeviceLevelData() { Value = 3f, PurchaseCost = 100, Exp = 200 },
    //                              new DeviceLevelData() { Value = 4f, PurchaseCost = 100, Exp = 300 }};
    //    [SerializeField]
    //    public DeviceLevelData[] durations = { new DeviceLevelData() { Value = 1f, PurchaseCost = 100, Exp = 100 },
    //                              new DeviceLevelData() { Value = 1.5f, PurchaseCost = 100, Exp = 200 },
    //                              new DeviceLevelData() { Value = 2f, PurchaseCost = 100, Exp = 300 }};
    //    [SerializeField]
    //    public DeviceLevelData[] charges = { new DeviceLevelData() { Value = 1, PurchaseCost = 100, Exp = 100 } };


    //}

    #endregion
}
