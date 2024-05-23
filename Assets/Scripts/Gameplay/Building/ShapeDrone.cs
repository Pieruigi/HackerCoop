using HKR.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR.Building
{
    public class ShapeDrone : MonoBehaviour
    {
        SecurityDroneAsset asset;
        public SecurityDroneAsset Asset { get { return asset; } }

        Floor floor;
        public Floor Floor { get { return floor; } }
        
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(SecurityDroneAsset asset, Floor floor)
        {
            this.asset = asset;
            this.floor = floor; 
            
        }
    }

}
