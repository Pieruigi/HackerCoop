using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class AlarmDeviceController : MonoBehaviour
    {
        float range;
        float speed;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(float range, float speed)
        {
            this.range = range;
            this.speed = speed;
        }
    }

}
