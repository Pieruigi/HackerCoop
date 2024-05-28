using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR.Scriptables
{
    public class DestroyerAsset : ScriptableObject
    {
        public const string ResourceFolder = "Destroyers";

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
