using HKR.Building;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR.Scriptables
{
    

    public class BuildingBlockAsset : ScriptableObject
    {
        public const string ResourceFolder = "BuildingBlocks";

        [SerializeField]
        BuildingBlockType type;
        public BuildingBlockType Type { get { return type; } }

        [SerializeField]
        bool isNorthBorder, isEastBorder, isSouthBorder, isWestBorder;
        public bool IsNorthBorder { get {  return isNorthBorder; } }
        public bool IsEastBorder { get { return isEastBorder; } }
        public bool IsSouthBorder { get { return isSouthBorder; } } 
        public bool IsWestBorder { get { return isWestBorder; } }

        //[SerializeField]
        //bool isEntrance;

        //[SerializeField]
        //bool isConnector;

        [SerializeField]
        [Range(Constants.RarityMinLevel, Constants.RarityMaxLevel)]
        int rarity;

        [SerializeField]
        GameObject prefab;
        public GameObject Prefab { get { return prefab; } }
    }

}
