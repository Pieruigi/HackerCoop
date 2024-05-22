using HKR.Building;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HKR
{
    public class CommonRenderingController : MonoBehaviour
    {
        [SerializeField]
        int floorLevel;

        bool hidden = false;

        List<Renderer> renderers;

        List<Light> lights;

        // Start is called before the first frame update
        void Start()
        {
            // Get the floor level
            floorLevel = Utility.GetFloorLevelByVerticalCoordinate(transform.position.y);// Mathf.FloorToInt(transform.position.y / BuildingBlock.Height);

            // Fill the renderer list
            renderers = GetComponentsInChildren<Renderer>().ToList();

            // Fill the light list
            lights = GetComponentsInChildren<Light>().ToList();
        }

        // Update is called once per frame
        void Update()
        {
            //if(PlayerController.Local.GetCurrentFloorLevel() == floorLevel)
            if(Utility.GetFloorLevelByVerticalCoordinate(PlayerController.Local.transform.position.y) == floorLevel)
            {
                if (hidden)
                {
                    hidden = false;
                    Show();
                }
            }
            else
            {
                if(!hidden)
                {
                    hidden= true;
                    Hide();
                }
            }
        }

        protected virtual void Show()
        {
            foreach (Renderer r in renderers)
                r.enabled = true;

            foreach(Light light in lights)
                light.enabled = true;
            
        }

        protected virtual void Hide()
        {
            foreach(Renderer r in renderers)
                r.enabled = false;

            foreach (Light light in lights)
                light.enabled = false;
        }
    }

}
