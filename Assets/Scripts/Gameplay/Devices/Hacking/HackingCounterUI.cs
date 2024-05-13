using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HKR
{
    public class HackingCounterUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text hitField;

        [SerializeField]
        TMP_Text errorField;

        [SerializeField]
        HackingController controller;

        bool running = false;

        private void Awake()
        {
            ResetFields();   
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (running)
            {
                hitField.text = $"{controller.HitCount} / {controller.HitTarget}";
                errorField.text = $"{controller.MaxErrors} / {controller.ErrorCount}";
            }
        }

        private void OnEnable()
        {
            controller.OnHackingStarted += HandleOnHackingStarted;
            controller.OnHackingStopped += HandleOnHackingStopped;
        }

        private void OnDisable()
        {
            controller.OnHackingStarted -= HandleOnHackingStarted;
            controller.OnHackingStopped -= HandleOnHackingStopped;
        }

        private void HandleOnHackingStopped()
        {
            running = false;
            ResetFields();
        }

        private void HandleOnHackingStarted()
        {
            running = true;
        }

        private void ResetFields()
        {
            hitField.text = "--";
            errorField.text = "--";
        }
    }

}
