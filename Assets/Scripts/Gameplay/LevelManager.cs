using Fusion;
using HKR.Building;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HKR
{


    public class LevelManager : NetworkBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [UnitySerializeField]
        [Networked]
        public int Seed { get; set; }

        string theme = "HighTech";
        public string Theme { get { return theme; } }

        ChangeDetector changeDetector;

       

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
          
            foreach (var propertyName in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (propertyName)
                {
                    //case nameof(Name):
                    //    var nameReader = GetPropertyReader<NetworkString<_16>>(propertyName);
                    //    var (namePrev, nameCurr) = nameReader.Read(previousBuffer, currentBuffer);
                    //    Debug.Log($"Player - Name changed from {namePrev} to {nameCurr}");
                    //    break;
                    case nameof(Seed):
                        var reader = GetPropertyReader<int>(propertyName);
                        var (prev, curr) = reader.Read(previousBuffer, currentBuffer);
                        Debug.Log($"Player - Ready changed from {prev} to {curr}");
                        Random.InitState(Seed);
                        break;
                }
            }
        }

        public override void Spawned()
        {
            base.Spawned();

            Debug.Log("LevelManager has spawned");

            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            CreateRandomSeed();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleOnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleOnSceneLoaded;
        }

        public static void DespawnCurrentInstance()
        {
            if (!Instance)
                return;
            Instance.Runner.Despawn(Instance.Object);
            Instance = null;
        }

        private void HandleOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.buildIndex)
            {
                case Constants.MainSceneIndex:
                    // Destroy object
                    Destroy(gameObject);
                    break;
                case Constants.GameSceneIndex:
                    if(Runner.IsSharedModeMasterClient || Runner.IsSinglePlayer)
                        Building.LevelBuilder.Instance.Build();
                    break;
            }
        }

        void CreateRandomSeed()
        {
            if (!Runner.IsSharedModeMasterClient)
                return;

            int seed = (int)System.DateTime.Now.Ticks;
            Debug.Log($"Random seed:{seed}");
            //Random.InitState(seed);
            Seed = seed;
        
        }

        public void UseCustomSeed(int seed)
        {
            if (!Runner.IsSharedModeMasterClient)
                return;
            //Random.InitState(seed);
            Seed = seed;
        }

        
    }

}
