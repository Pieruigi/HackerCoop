using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class HackingHitHintUI : MonoBehaviour
    {
        [SerializeField]
        Color succeededColor;

        [SerializeField]
        Color failedColor;

        [SerializeField]
        SpriteRenderer spriteRenderer;

        [SerializeField]
        HackingController controller;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR

            if(Input.GetKey(KeyCode.U))
            {
                HandleOnHitFailed();
            }
#endif
        }

        private void OnEnable()
        {
            controller.OnHitSuccedeed += HandleOnHitSucceeded;
            controller.OnHitFailed += HandleOnHitFailed;
        }

        private void OnDisable()
        {
            controller.OnHitSuccedeed -= HandleOnHitSucceeded;
            controller.OnHitFailed -= HandleOnHitFailed;
        }

        private void HandleOnHitSucceeded()
        {
            Shake(succeededColor);

            
        }

        private void HandleOnHitFailed()
        {
            Shake(failedColor);
        }

        void Shake(Color color)
        {
            Color c = color;
            c.a = 0;
            spriteRenderer.color = c;

            Vector3 alphaV = Vector3.zero;
            DOTween.Shake(() => alphaV, x => alphaV = x, .5f, 0.5f, 10).onUpdate += () =>
            {
                c.a = alphaV.x;
                spriteRenderer.color = c;
            };
        }
    }

}
