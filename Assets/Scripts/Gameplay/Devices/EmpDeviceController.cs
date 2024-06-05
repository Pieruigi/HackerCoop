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

        //int[] chargeLevels = new int[] {1,2};

        float[] rangeLevels = new float[] { 10, 15, 20 };

        float[] durationLevels = new float[] { 10, 15, 20 };

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(int rangeLevel, int durationLevel, int chargeLeft)
        {
            range = rangeLevels[rangeLevel];
            duration = durationLevels[durationLevel];
            if(chargeLeft < 0)
                this.chargeLeft = chargeMax;
            else
                this.chargeLeft = chargeLeft;
        }
    }

}
