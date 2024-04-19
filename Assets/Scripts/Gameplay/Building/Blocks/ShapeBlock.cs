using Fusion;
using System.Collections;
using System.Collections.Generic;
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

        int type;
        public Vector2 Coordinates { get; private set; }
        //public Floor Floor { get; private set; }

        int height = 3;
        

        public bool IsNorthBorder { get; set; } = false;

        public bool IsSouthBorder { get; set; } = false;
        public bool IsEastBorder { get; set; } = false;
        public bool IsWestBorder { get; set; } = false;

        Vector3 position = Vector3.zero;

        public bool IsEnteringBlock { get; set; } = false;
        public bool IsConnectorBlock { get; set; } = false;

        public int ConnectorIndex { get; set; } = -1;
        
        public bool IsBorder
        {
            get { return IsNorthBorder || IsSouthBorder || IsEastBorder || IsWestBorder; }
        }

        /// <summary>
        /// 0: staircase
        /// 1: elevator
        /// </summary>
        public int ConnectorType { get; set; } = 0; 

        public bool IsColumnBlock { get; set; } = false;

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
            //if (Floor)
            //{
            //    position.y = Floor.Level * height;
            //}
            position.y = 0;
            position.x = BuildingBlock.Size * Coordinates.x;
            position.z = BuildingBlock.Size * Coordinates.y;
            
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
            transform.position = position;
            
        }
    }

}
