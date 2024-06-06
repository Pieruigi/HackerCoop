using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR.Scriptables
{
    public class DeviceAsset : ScriptableObject
    {
        public const string ResourceFolder = "Devices";

        /// <summary>
        /// Code for the Steam cloud
        /// </summary>
        //[SerializeField]
        //string code;
        //public string Code
        //{
        //    get { return code; }
        //}

        /// <summary>
        /// The object prefab if needed
        /// </summary>
        [SerializeField]
        GameObject prefab; 
        public GameObject Prefab
        {
            get { return prefab; }
        }

        [SerializeField]
        DeviceData data;
        public DeviceData Data { get { return data; } }
        
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

}
