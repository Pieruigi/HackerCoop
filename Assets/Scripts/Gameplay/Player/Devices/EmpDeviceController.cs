using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class EmpDeviceController : MonoBehaviour
    {
        int chargeMax = 1;

        float range;

        float duration;

        int chargeLeft = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(int chargeMax, float range, float duration, int chargeLeft)
        {
            this.chargeMax = chargeMax;
            this.range = range;
            this.duration = duration;
            this.chargeLeft = chargeLeft;
        }
    }

}
