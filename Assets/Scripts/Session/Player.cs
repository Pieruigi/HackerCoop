using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Fusion.NetworkBehaviour;

namespace HKR
{
    public class Player : NetworkBehaviour
    {

        public static Player Local { get; private set; }

        [UnitySerializeField]
        [Networked]
        public NetworkString<_16> Name { get; set; }

      
        [UnitySerializeField]
        [Networked]
        public NetworkBool Ready { get; set; }

        [UnitySerializeField]
        [Networked]
        public NetworkBool InGame { get; set; }

        //[UnitySerializeField]
        //public NetworkBool LevelReady { get; set; } = false; // True when the level has been built

        public int CharacterId { get; private set; } = 0;
        

        ChangeDetector changeDetector;

        private void Awake()
        {
            DontDestroyOnLoad(this);
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
                    case nameof(Ready):
                        var readyReader = GetPropertyReader<NetworkBool>(propertyName);
                        var (readyPrev, readyCurr) = readyReader.Read(previousBuffer, currentBuffer);
                        Debug.Log($"Player - Ready changed from {readyPrev} to {readyCurr}");
                        break;
                }
            }
        }

        public override void Spawned()
        {
            base.Spawned();
            if (HasStateAuthority)
                Local = this;
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            PlayerManager.Instance.AddPlayer(this);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {

            base.Despawned(runner, hasState);

            if (HasStateAuthority)
                Local = null;

            PlayerManager.Instance.RemovePlayer(this);
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
                    if(Local == this)
                    {
                        Ready = false;
                    }
                    break;
            }
        }
    }

}
