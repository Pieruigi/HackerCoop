using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HKR
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(Instance);
            }
        }

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
                bool ready = Player.Local.Ready;
                Player.Local.Ready = !ready;
            }
#endif
        }

       

    }

}
