using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HKR
{
    public class LobbyManager : MonoBehaviour
    {
       


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                SessionManager.Instance.Shutdown();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                bool ready = PlayerManager.Instance.LocalPlayer.Ready;
                PlayerManager.Instance.LocalPlayer.Ready = !ready;
            }
#endif
        }


     


    }

}
