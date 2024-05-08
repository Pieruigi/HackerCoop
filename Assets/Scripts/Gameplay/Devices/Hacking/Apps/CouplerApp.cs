using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class CouplerApp : MonoBehaviour
    {
        [SerializeField]
        GameObject circles;


        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.T)) 
            {
                GetComponentInParent<HackingController>().OnHackingSucceded();
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                GetComponentInParent<HackingController>().OnHackingFailed();
            }
        }


    }

}
