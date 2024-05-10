using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace HKR
{
    public class HackingSynchUI : MonoBehaviour
    {

        [SerializeField]
        List<SpriteRenderer> connectionImages;

        bool connecting = false;
        float connectionElapsed;
        float colorTime = .1f;
        float timeStep;
        int step = 0;
        bool connected = false;
        float connectedTime = .5f;
        
        HackingController controller;

        Color colorOn, colorOff;


        private void Awake()
        {
            controller = GetComponentInParent<HackingController>();
            // Initialize colors
            colorOn = connectionImages[0].color;
            colorOff = colorOn;
            colorOff.a = 0f;
        }

        // Start is called before the first frame update
        void Start()
        {
            Hide();
        }

        // Update is called once per frame
        void Update()
        {
            if(connecting)
            {
                connectionElapsed += Time.deltaTime;
                if(connectionElapsed >= timeStep)
                {
                    connectionElapsed -= timeStep;
                    step++;
                    TweenColor(connectionImages[step], colorOn);
                    if(step == connectionImages.Count-1)
                    {
                        connecting = false;
                        connected = true;
                        connectionElapsed = 0;
                        
                    }
                 }
            }
            else
            {
                if(connected)
                {
                    connectionElapsed += Time.deltaTime;
                    if(connectionElapsed >= connectedTime)
                    {
                        connected = false;
                        TweenColorAll(colorOff);
                    }
                }
            }
        }

        private void OnEnable()
        {
            controller.OnStartConnecting += HandleOnStartConnecting;
            controller.OnStopConnecting += HandleOnStopConnecting;
        }

        

        private void OnDisable()
        {
            controller.OnStartConnecting -= HandleOnStartConnecting;
            controller.OnStopConnecting -= HandleOnStopConnecting;
        }

        private void HandleOnStopConnecting()
        {
            ResetAll();
        }

        private void HandleOnStartConnecting(float connectionTime)
        {
            connecting = true;
            connectionElapsed = 0;
            timeStep = connectionTime / (connectionImages.Count - 1);
            step = 0;
            // Show the first bar
            TweenColor(connectionImages[0], colorOn);
        }

        void ResetAll()
        {
            connecting = false;
            connected = false;
            connectionElapsed = 0;
            step = 0;
            TweenColorAll(colorOff);
        }

        void TweenColorAll(Color newColor)
        {
            foreach (SpriteRenderer image in connectionImages)
            {
                TweenColor(image, newColor);
            }
        }

        void TweenColor(SpriteRenderer target, Color newColor)
        {
            target.DOColor(newColor, colorTime);
        }

        public void UpdateConnection(float time, float totalTime)
        {

        }

        public void Show()
        {
            ResetAll();
        }

        public void Hide()
        {
            ResetAll();
        }


    }

}
