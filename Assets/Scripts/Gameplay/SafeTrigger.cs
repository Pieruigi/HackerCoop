using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class SafeTrigger : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

       
        

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(Tags.Player))
                return;

            other.GetComponent<PlayerController>().InSafeZone = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(Tags.Player))
                return;

            other.GetComponent<PlayerController>().InSafeZone = false;
        }


    }

}
