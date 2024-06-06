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

        //[SerializeField]
        
    }

}
