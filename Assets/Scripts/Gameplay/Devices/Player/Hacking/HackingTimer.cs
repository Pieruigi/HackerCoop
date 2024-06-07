using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HKR
{
    public class HackingTimer : MonoBehaviour
    {
       
        HackingController hackingController;
        float timer = 0;
        bool loop = false;
        float delay = 0;

        public float Timer
        {
            get { return timer; }
        }

        public bool Running
        {
            get { return loop; }
        }

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
            if(loop)
            {
                if(delay > 0)
                {
                    delay -= Time.deltaTime;
                }
                else
                {
                    timer -= Time.deltaTime;
                    if (timer < 0)
                    {
                        hackingController.OnHackingDetected();
                        loop = false;
                    }
                }

                
            }
        }

        public void StartTimer(float duration)
        {
            timer = duration;
            delay = Constants.HackingAppPlayDelay;
            loop = true;
        }

        public void StopTimer()
        {
            timer = 0;
            loop = false;
        }

        public void PauseTimer()
        {
            loop = false;
        }
    }

}
