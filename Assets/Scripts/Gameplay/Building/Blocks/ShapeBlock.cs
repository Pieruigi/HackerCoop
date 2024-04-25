using Fusion;
using HKR.Scriptables;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HKR.Building
{
    /// <summary>
    /// This class is only used by the master client to define the shape of the hole building, and it's just a reference to build floors
    /// </summary>
    public class ShapeBlock : MonoBehaviour
    {
        [SerializeField]
        List<MeshRenderer> renderers = new List<MeshRenderer>();

        public Floor floor = null;

        public Vector2 Coordinates;
        //public Floor Floor { get; private set; }

        int height = 3;
        

        public bool IsNorthBorder = false;

        public bool IsSouthBorder = false;
        public bool IsEastBorder = false;
        public bool IsWestBorder = false;

        //public Vector3 _position = Vector3.zero;

        public bool IsEnteringBlock = false;
        public bool IsConnectorBlock = false;

        public int ConnectorIndex = -1;
        
        public bool IsBorder
        {
            get { return IsNorthBorder || IsSouthBorder || IsEastBorder || IsWestBorder; }
        }

        public BuildingBlockAsset blockAsset;

        /// <summary>
        /// 0: staircase
        /// 1: elevator
        /// </summary>
        public int ConnectorType = 0; 

        public bool IsColumnBlock = false;

        //public bool IsInfectedBlock { get { return infectionIndices.Count > 0; } }
        //public List<int> infectionPoints = new List<int>();

        public Vector3 Center { get { return new Vector3(Coordinates.x * BuildingBlock.Size / 2f, floor.Level * height, Coordinates.y * BuildingBlock.Size / 2f); } }
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void SetColor(Color color)
        {
            foreach (var renderer in renderers)
            {
                renderer.material = new Material(renderer.material);
                renderer.material.color = color;
            }
        }

        public void SetCoordinates(Vector2 coordinates)
        {
            //Floor = floor;
            Coordinates = coordinates;
            
        }

        public void Colorize()
        {
            if (IsEnteringBlock)
                SetColor(Color.blue);
            else if (IsConnectorBlock)
                SetColor(Color.green);
        }

        public void Move()
        {
            Vector3 position = Vector3.zero;
            position.y = floor.Level * height;
            position.x = BuildingBlock.Size * Coordinates.x;
            position.z = BuildingBlock.Size * Coordinates.y;
            transform.position = position;
            
        }

        public bool IsCommonBlock()
        {
            return !IsEnteringBlock && !IsConnectorBlock && !IsColumnBlock;
        }

        
    }

}
