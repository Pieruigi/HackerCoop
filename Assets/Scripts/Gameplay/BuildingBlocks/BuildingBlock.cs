using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR.Building
{
    public class BuildingBlock : MonoBehaviour
    {
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

        public void Spawn()
        {
#if BUILDING_TEST
            transform.position = position;
            
#else
#endif
        }
    }

}
