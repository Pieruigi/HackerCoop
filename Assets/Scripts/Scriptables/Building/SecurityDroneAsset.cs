using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR.Scriptables
{
    public class SecurityDroneAsset : ScriptableObject
    {
        public const string ResourceFolder = "Drones";

        [SerializeField]
        GameObject prefab;
        public GameObject Prefab
        {
            get { return prefab; }
        }

        [SerializeField]
        [Range(1, 100)]
        int weight;
        public int Weight
        {
            get { return weight; }
        }
    }

}
