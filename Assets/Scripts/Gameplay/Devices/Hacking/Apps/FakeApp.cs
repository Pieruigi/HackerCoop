using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class FakeApp : MonoBehaviour
    {
        HackingController controller;

        bool initialized = false;

        private void Awake()
        {
            controller = GetComponentInParent<HackingController>();

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            if (!initialized)
            {
                initialized = true;
                return;
            }
                
            controller.SetAppData(0, 1, 0);
            controller.HitSucceded();
        }
    }

}
