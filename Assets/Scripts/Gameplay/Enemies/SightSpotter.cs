using HKR.Building;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class SightSpotter : Spotter
    {
      
        [SerializeField]
        Transform eyes;

        // X is horizontal, y is vertical
        [SerializeField]
        Vector2 sightAngles = new Vector2(60, 45);

     
      
        protected override bool IsPlayerSpotted(PlayerController player)
        {
            // Get the sight target which is the camera root
            Transform sightTarget = player.GetSightTarget();

            // Get the direction 
            Vector3 direction = sightTarget.position - eyes.position;

            // Check the horizontal angle
            Vector3 hDir = Vector3.ProjectOnPlane(direction, Vector3.up);
            Vector3 hFwd = Vector3.ProjectOnPlane(eyes.forward, Vector3.up);
            float angle = Vector3.Angle(hDir, hFwd);
            
            if (angle < sightAngles.x || SecurityStateController.State == SecurityState.Spotted)
            {
                
                // Check the vertical angle
                Vector3 vDir = Vector3.ProjectOnPlane(direction, eyes.right);
                angle = Vector3.Angle(vDir, eyes.forward);

                if (angle < sightAngles.y || SecurityStateController.State == SecurityState.Spotted)
                {
                    
                    // Do raycast to check if there is any obstacle between the camera and the potential target
                    RaycastHit hitInfo;
                    Ray ray = new Ray(eyes.position, direction.normalized);
                    if (Physics.Raycast(ray, out hitInfo, Range, ~LayerMask.GetMask(), QueryTriggerInteraction.Ignore))
                    {
                   
                        // Check if the hit object is the player we are checking 
                        if (hitInfo.collider.gameObject == player.gameObject)
                            return true;
                    }


                }
            }

            return false;
        }

        

    }

}
