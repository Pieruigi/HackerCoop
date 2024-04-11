using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HKR
{
    public class MatchManager : MonoBehaviour
    {
        public static MatchManager Instance { get; private set; }

       
        float elapsed = 310;
        bool completed = false;

        private void Awake()
        {
            if(!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (!SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient || completed)
                return;

            elapsed -= Time.deltaTime;
            if(elapsed < 0 )
            {
                completed = true;
                SessionManager.Instance.NetworkRunner.LoadScene(SceneRef.FromIndex(Constants.LobbySceneIndex), UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }

       
    }

}
