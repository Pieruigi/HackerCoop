using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class BlockRenderingController : MonoBehaviour
    {
        List<Renderer> renderers = new List<Renderer>();

        bool isHidden = false;

        // Start is called before the first frame update
        void Start()
        {
            renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Show()
        {
            //if(!isHidden) return;

            isHidden = false;

            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }
            
        }

        

        public void Hide()
        {
            //if (isHidden)   return;

            isHidden = true;
           
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
            
        }
    }

}
