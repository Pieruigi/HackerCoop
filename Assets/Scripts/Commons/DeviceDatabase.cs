using HKR.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    #region common data
    public class DeviceDatabase: SingletonPersistent<DeviceDatabase>
    {
        [SerializeField]
        DeviceAsset hackingDeviceAsset;
        public DeviceAsset HackingDeviceAsset {  get { return hackingDeviceAsset; } }
        [SerializeField]
        DeviceAsset radarDeviceAsset;
        public DeviceAsset RadarDeviceAsset { get { return radarDeviceAsset; } }
        [SerializeField]
        DeviceAsset flashlightAsset;
        public DeviceAsset FlashlightAsset { get { return flashlightAsset; } }
        [SerializeField]
        DeviceAsset empDeviceAsset;
        public DeviceAsset EmpDeviceAsset { get { return empDeviceAsset; } }
        [SerializeField]
        DeviceAsset alarmDeviceAsset;
        public DeviceAsset AlarmDeviceAsset { get { return alarmDeviceAsset; } }
      
    }

    //[System.Serializable]
    //public class DeviceLevel
    //{
       
    //    [SerializeField]
    //    double value;
    //    public double Value { get { return value; } }
    //    [SerializeField]
    //    int purchaseCost;
    //    public int PurchaseCost { get { return purchaseCost; } }
    //    [SerializeField]
    //    int exp;
    //    public int Exp { get { return exp; } }
    //    [SerializeField]
    //    int rechargeCost;
    //    public int RechargeCost { get { return rechargeCost; } }

    //}

    //[System.Serializable]
    //public class DeviceDetail
    //{
    //    [SerializeField]
    //    string name = string.Empty;
    //    public string Name { get { return name; } }

    //    [SerializeField]
    //    List<DeviceLevel> levels = new List<DeviceLevel>();
    //    public IList<DeviceLevel> Levels { get { return levels.AsReadOnly(); } }
    //}

    //[System.Serializable]
    //public class DeviceData
    //{
    //    [SerializeField]
    //    List<DeviceDetail> details = new List<DeviceDetail>();
    //    public IList<DeviceDetail> Details { get { return details.AsReadOnly(); } }
    
    //}
    #endregion


    
}
