using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HKR
{
    public class HackingTimerUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text textField;

        HackingTimer timer;

        HackingController controller;

        string format = "{0:0.00}";

        private void Awake()
        {
            timer = GetComponentInParent<HackingTimer>();
            controller = GetComponentInParent<HackingController>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(timer.Running)
                textField.text = string.Format(format, timer.Timer);
        }

        private void OnEnable()
        {
            controller.OnAiming += HandleOnControllerAiming;
            
            textField.text = string.Format(format, 0f);   

        }

        private void OnDisable()
        {
            controller.OnAiming -= HandleOnControllerAiming;
        }

        private void HandleOnControllerAiming(bool arg0, InfectedNodeState arg1)
        {
            textField.text = string.Format(format, 0f);
        }
    }

}
