using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HKR
{
    public class FlashlightController : MonoBehaviour
    {
        [SerializeField]
        Light _light;

        PlayerDevice playerDevice;

        Transform defaultParent;
        float chargeMax = 0;
        float currentCharge = 0;

       
        private void Awake()
        {
            playerDevice = GetComponent<PlayerDevice>();
            defaultParent = transform.parent;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!playerDevice.IsLocalPlayer())
                return;

            if (playerDevice.GetButtonDown())
            {
                // Light on/off
                if(_light.enabled) 
                    _light.enabled = false;
                else
                    _light.enabled = true;
            }


        }

        private void OnEnable()
        {
            _light.enabled = false;
            _light.transform.parent = Camera.main.transform;
            _light.transform.localPosition = Vector3.zero;
            _light.transform.localRotation = Quaternion.identity;
        }

        private void OnDisable()
        {
            _light.enabled = false;
            _light.transform.parent = defaultParent;
            _light.transform.localPosition = Vector3.zero;
            _light.transform.localRotation = Quaternion.identity;
        }

        public void Init(float chargeMax, float charge)
        {
            this.chargeMax = chargeMax;
            currentCharge = Mathf.Min(charge, chargeMax);
        }
    }

}
