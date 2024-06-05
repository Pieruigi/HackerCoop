using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HKR
{
    public class SecurityLightSetter : MonoBehaviour
    {
        [SerializeField]
        Renderer lightRenderer;

        [SerializeField]
        int materialId;

        [SerializeField]
        Light _light;


        [SerializeField]
        Material notAlarmedMaterial, spottedMaterial, alarmedMaterial, freezedMaterial;

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
            AlarmSystemController.OnStateChanged += HandleOnAlarmStateChanged;
        }

        

        private void OnDisable()
        {
            controller.OnSpawned -= HandleOnControllerSpawned;
            controller.OnStateChanged -= HandleOnControllerStateChanged;
            AlarmSystemController.OnStateChanged -= HandleOnAlarmStateChanged;
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

        private void HandleOnAlarmStateChanged(AlarmSystemController asc, AlarmSystemState oldState, AlarmSystemState newState)
        {
            HandleOnControllerSpawned();
        }

        void SetMaterial(SecurityState state)
        {
            if (!lightRenderer)
                return;

            Material[] materials = lightRenderer.materials;

            switch (state)
            {
                case SecurityState.Normal:

                    if (AlarmSystemController.GetAlarmSystemController(controller.FloorLevel).State == AlarmSystemState.Activated)
                        materials[materialId] = alarmedMaterial;
                    else
                        materials[materialId] = notAlarmedMaterial;
                    break;
                case SecurityState.Spotted:
                    if(AlarmSystemController.GetAlarmSystemController(controller.FloorLevel).State == AlarmSystemState.Activated)
                        materials[materialId] = alarmedMaterial;
                    else
                        materials[materialId] = spottedMaterial;
                    break;
                case SecurityState.Searching:
                    if (AlarmSystemController.GetAlarmSystemController(controller.FloorLevel).State == AlarmSystemState.Activated)
                        materials[materialId] = alarmedMaterial;
                    else
                        materials[materialId] = spottedMaterial;
                    break;
                case SecurityState.Freezed:
                    materials[materialId] = freezedMaterial;
                    break;
            }

            lightRenderer.materials = materials;
            
        }

        void SetLight(SecurityState state)
        {
            if (!_light)
                return;

            Color c;
            switch (state)
            {
                case SecurityState.Normal:
                    if (AlarmSystemController.GetAlarmSystemController(controller.FloorLevel).State == AlarmSystemState.Activated)
                        c = alarmedMaterial.color;
                    else
                        c = notAlarmedMaterial.color;
                    break;

                case SecurityState.Spotted:
                    if (AlarmSystemController.GetAlarmSystemController(controller.FloorLevel).State == AlarmSystemState.Activated)
                        c = alarmedMaterial.GetColor("_Color");
                    else
                        c = spottedMaterial.GetColor("_Color");
                    break;
                case SecurityState.Searching:
                    if (AlarmSystemController.GetAlarmSystemController(controller.FloorLevel).State == AlarmSystemState.Activated)
                        c = alarmedMaterial.GetColor("_Color");
                    else
                        c = spottedMaterial.GetColor("_Color");
                    break;
                case SecurityState.Freezed:
                    c = freezedMaterial.GetColor("_Color");
                    break;
                default:
                    c = notAlarmedMaterial.GetColor("_Color");
                    break;
            }
            _light.color = c;
        }
    }

}
