using Fusion;
using HKR.Building;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HKR
{
    /// <summary>
    /// This class is responsible for the scene networked objects.
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance { get; private set; }

        [SerializeField]
        LevelManager levelManagerPrefab;

        [SerializeField]
        List<PlayerController> playerControllerPrefabs;

        //[SerializeField]
        //GameObject _nestedTest;


        private void Awake()
        {
            if(!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
            
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.V))
            {
                LevelManager.DespawnCurrentInstance();

                SessionManager.Instance.NetworkRunner.Spawn(levelManagerPrefab);
            }
#endif
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleOnSceneLoaded;
            BuildingBlock.OnSpawned += HandleOnBuildingBlockSpawned;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleOnSceneLoaded;
            BuildingBlock.OnSpawned -= HandleOnBuildingBlockSpawned;
        }

        private void HandleOnBuildingBlockSpawned(BuildingBlock block)
        {
            PlayerControllerSpawnPointGroup spawnGroup = block.GetComponentInChildren<PlayerControllerSpawnPointGroup>();
            if (spawnGroup)
            {
                SpawnLocalPlayerController();
            }
        }

        private void HandleOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.buildIndex)
            {
                case Constants.LobbySceneIndex:
                    // Every time you enter the lobby the old level manager, if any, is destroyed and a new one is created
                    SpawnLevelManager();
                    
                    // PlayerController
                    SpawnLocalPlayerController();

                    //SessionManager.Instance.NetworkRunner.Spawn(_nestedTest);
                    break;

                case Constants.GameSceneIndex:
                    // PlayerController
                    //SpawnLocalPlayerController();
                    break;

            }
        }

        /// <summary>
        /// In shared mode each player spawns their own controller on which they have authority
        /// </summary>
        async void SpawnLocalPlayerController()
        {
            PlayerController.DespawnLocalPlayerController();
            PlayerRef localRef = SessionManager.Instance.NetworkRunner.LocalPlayer;
            Transform spawnPoint = FindObjectOfType<PlayerControllerSpawnPointGroup>().GetPlayerControllerSpawnPoint(localRef);
            Debug.Log($"SpawnPoint {spawnPoint.gameObject.name}, position:{spawnPoint.position}");
            // We could store the character id in the player prefs
            int characterId = 0;
            if (Player.Local)
                characterId = Player.Local.CharacterId;
            // Spawn player
            
            SessionManager.Instance.NetworkRunner.Spawn(playerControllerPrefabs[characterId], spawnPoint.position, spawnPoint.rotation, localRef);
            await Task.Delay(1000);
            PlayerController.Local.SetAlive();
                
        }

        /// <summary>
        /// The master client spawns the level manager
        /// </summary>
        void SpawnLevelManager()
        {
            
            if (!SessionManager.Instance.NetworkRunner.IsSharedModeMasterClient && !SessionManager.Instance.NetworkRunner.IsSinglePlayer)
                return;
            LevelManager.DespawnCurrentInstance();
            SessionManager.Instance.NetworkRunner.Spawn(levelManagerPrefab);
        }
    }

}
