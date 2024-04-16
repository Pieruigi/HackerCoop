using HKR.Building;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR.Scriptables
{
    public enum BorderConfiguration { Free, North, NorthSouth, NorthEast, NorthEastSouth }

    public class BuildingBlockAsset : ScriptableObject
    {
        public const string ResourceFolder = "BuildingBlocks";

        //[SerializeField]
        //BuildingBlockType type;
        //public BuildingBlockType Type { get { return type; } }


        //[SerializeField]
        //BorderConfiguration borderConfiguration;

        [SerializeField]
        [Range(Constants.RarityMinLevel, Constants.RarityMaxLevel)]
        int rarity;

        [SerializeField]
        GameObject prefab;
        public GameObject Prefab { get { return prefab; } }
    }

}
