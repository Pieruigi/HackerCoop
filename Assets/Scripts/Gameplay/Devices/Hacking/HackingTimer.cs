using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class HackingTimer : MonoBehaviour
    {
        
        HackingController hackingController;
        float timer = 0;

        private void Awake()
        {
            hackingController = GetComponentInParent<HackingController>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(timer > 0)
            {
                timer-=Time.deltaTime;
                if(timer < 0)
                {
                    hackingController.OnHackingDetected();
                }
            }
        }

        public void StartTimer(float duration)
        {
            timer = duration;
        }

        public void ResetTimer()
        {
            timer = 0;
        }
    }

}
