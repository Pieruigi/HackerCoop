using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HKR.UI
{
    public class MainPanel : MonoBehaviour
    {

        //private void OnEnable()
        //{
        //    // Disable buttons
        //    //SetInteractableAll(false);

        //    SessionManager.OnPlayerJoinedEvent += HandleOnPlayerJoined;
            
        //}

        //private void OnDisable()
        //{
        //    SessionManager.OnPlayerJoinedEvent -= HandleOnPlayerJoined;
          
        //}


        public void CreateOfflineSession()
        {
            SessionManager.Instance.CreateOfflineSession();
        }
    }

}
