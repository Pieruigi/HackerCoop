using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class PlayerSpottedReporter : MonoBehaviour
    {
        [SerializeField]
        Spotter spotter;

        [SerializeField]
        SecurityStateController securityStateController;

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
            spotter.OnPlayerSpotted += HandleOnPlayerSpotted;
            spotter.OnPlayerLost += HandleOnPlayerLost;
        }

        

        private void OnDisable()
        {
            spotter.OnPlayerSpotted -= HandleOnPlayerSpotted;
            spotter.OnPlayerLost -= HandleOnPlayerLost;
        }

        private void HandleOnPlayerSpotted(PlayerController arg0)
        {
            AlarmSystemController.GetAlarmSystemController(securityStateController.FloorLevel).PlayerSpotted(arg0);
        }

        private void HandleOnPlayerLost(PlayerController arg0)
        {
            AlarmSystemController.GetAlarmSystemController(securityStateController.FloorLevel).PlayerLost(arg0);
        }

    
    }

}
