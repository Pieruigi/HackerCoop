using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class SecurityLightSetter : MonoBehaviour
    {
        [SerializeField]
        Renderer lightRenderer;

        [SerializeField]
        Light _light;


        [SerializeField]
        Material normalMaterial, spottedMaterial, alarmedMaterial, freezedMaterial;

        //[SerializeField]
        //Color normalColor, suspectColor, spottedColor, freezedColor;

        
        SecurityStateController controller;

        private void Awake()
        {
            controller = GetComponentInParent<SecurityStateController>();
            
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            controller.OnSpawned += HandleOnControllerSpawned;
            controller.OnStateChanged += HandleOnControllerStateChanged;
        }

        private void OnDisable()
        {
            controller.OnSpawned -= HandleOnControllerSpawned;
            controller.OnStateChanged -= HandleOnControllerStateChanged;
        }

        private void HandleOnControllerStateChanged(SecurityState arg0, SecurityState arg1)
        {
            HandleOnControllerSpawned();
        }

        private void HandleOnControllerSpawned()
        {
            SetMaterial(controller.State);
            SetLight(controller.State);
        }

        void SetMaterial(SecurityState state)
        {
            switch (state)
            {
                case SecurityState.Normal:
                    lightRenderer.material = normalMaterial;
                    break;
                case SecurityState.Spotted:
                    lightRenderer.material = spottedMaterial;
                    break;
                case SecurityState.Alarmed:
                    lightRenderer.material = alarmedMaterial;
                    break;
                case SecurityState.Freezed:
                    lightRenderer.material = freezedMaterial;
                    break;
            }

            
        }

        void SetLight(SecurityState state)
        {
            Color c;
            switch (state)
            {
                case SecurityState.Spotted:
                    c = spottedMaterial.GetColor("_Color");
                    break;
                case SecurityState.Alarmed:
                    c = alarmedMaterial.GetColor("_Color");
                    break;
                case SecurityState.Freezed:
                    c = freezedMaterial.GetColor("_Color");
                    break;
                default:
                    c = normalMaterial.GetColor("_Color");
                    break;
            }
            _light.color = c;
        }
    }

}
