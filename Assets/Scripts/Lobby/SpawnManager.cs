using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HKR
{
    /// <summary>
    /// This class is responsible for the scene networked objects.
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField]
        LevelManager levelManagerPrefab;

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.V))
            {
                LevelManager.TryDespawnCurrentInstance();

                SessionManager.Instance.NetworkRunner.Spawn(levelManagerPrefab);
            }
#endif
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleOnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleOnSceneLoaded;
        }

        private void HandleOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.buildIndex)
            {
                case Constants.LobbySceneIndex:
                    if (SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient)
                    {
                        // Every time you enter the lobby the old level manager, if any, is destroyed and a new one is created
                        LevelManager.TryDespawnCurrentInstance();
                        SessionManager.Instance.NetworkRunner.Spawn(levelManagerPrefab);
                    }
                    
                    break;

            }
        }
    }

}
