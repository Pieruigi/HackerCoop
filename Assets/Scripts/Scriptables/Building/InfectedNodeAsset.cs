using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR.Scriptables
{
    public class InfectedNodeAsset : ScriptableObject
    {
        public const string ResourceFolder = "InfectedNodes";

        [SerializeField]
        GameObject prefab;

        public GameObject Prefab
        {
            get { return prefab; }
        }
    }

}
