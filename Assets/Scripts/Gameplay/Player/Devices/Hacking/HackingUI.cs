using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HKR.UI
{
    public class HackingUI : MonoBehaviour
    {
        [SerializeField]
        HackingController hackingController;

        [SerializeField]
        List<Sprite> aimingSprites;

        [SerializeField]
        SpriteRenderer aimingImage;

        [SerializeField]
        SpriteRenderer lockedImage;

        

        bool aiming = false;
        float aimingAnimationTime = 3;

        int currentAimingFrame = 0;
        System.DateTime lastAimingFrameTime;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(aiming)
            {
                if ((System.DateTime.Now - lastAimingFrameTime).TotalSeconds > aimingAnimationTime / aimingSprites.Count)
                {
                    lastAimingFrameTime = System.DateTime.Now;
                    currentAimingFrame++;
                    if (currentAimingFrame >= aimingSprites.Count)
                        currentAimingFrame = 0;
                    aimingImage.sprite = aimingSprites[currentAimingFrame];
                }
            }
        }

        private void OnEnable()
        {
            ResetAll();
            hackingController.OnAiming += HandleOnAiming;
        }

        private void OnDisable()
        {
            ResetAll();
            hackingController.OnAiming -= HandleOnAiming;
        }

        private void HandleOnAiming(bool value, InfectedNodeState nodeState)
        {
            if (aiming == value)
                return;
            currentAimingFrame = 0;
            aimingImage.sprite = aimingSprites[currentAimingFrame];
            aimingImage.enabled = value;
            aiming = value;
            lastAimingFrameTime = System.DateTime.Now;

            // Check the node state on aiming
            if (aiming)
            {
                switch (nodeState)
                {
                    case InfectedNodeState.Locked:
                        lockedImage.enabled = true;
                        break;
                }
            }
            else
            {
                lockedImage.enabled = false;
            }
        }

        void ResetAll()
        {
            aiming = false;
            aimingImage.enabled = false;
            currentAimingFrame = 0;
            lockedImage.enabled = false;
        }
    }

}
