using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR.Building
{
    public class ShapeBlock : MonoBehaviour
    {
        [SerializeField]
        List<MeshRenderer> renderers = new List<MeshRenderer>();

        int type;
        public Vector2 Coordinates { get; private set; }
        public Floor Floor { get; private set; }

        int height = 3;
        int size = 3;

        public bool IsNorthBorder { get; set; } = false;

        public bool IsSouthBorder { get; set; } = false;
        public bool IsEastBorder { get; set; } = false;
        public bool IsWestBorder { get; set; } = false;

        Vector3 position = Vector3.zero;

        public bool IsEnteringBlock { get; set; } = false;
        public bool IsConnectorBlock { get; set; } = false;
        
        public bool IsBorder
        {
            get { return IsNorthBorder || IsSouthBorder || IsEastBorder || IsWestBorder; }
        }

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

        public void Init(Floor floor, Vector2 coordinates)
        {
            Floor = floor;
            Coordinates = coordinates;
            if (Floor)
            {
                position.y = Floor.Level * height;
            }
            position.x = size * Coordinates.x;
            position.z = size * Coordinates.y;
            
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
