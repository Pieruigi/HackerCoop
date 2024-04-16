using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static Fusion.NetworkEvents;

namespace HKR
{
    public class SessionManager : SingletonPersistent<SessionManager>, INetworkRunnerCallbacks
    {
        public static UnityAction OnStartSessionFailed;
        public static UnityAction<NetworkRunner, PlayerRef> OnPlayerJoinedEvent;
        public static UnityAction<NetworkRunner, PlayerRef> OnPlayerLeftEvent;
        public static UnityAction<NetworkRunner, ShutdownReason> OnShutdownEvent;
        public static UnityAction<NetworkRunner, List<SessionInfo>> OnSessionListUpdatedEvent;
        public static UnityAction OnJoinedToSessionLobbyEvent;
        public static UnityAction OnJoinToSessionLobbyFailedEvent;

        public const int MaxPlayers = 2;


        NetworkSceneManagerDefault sceneManager;
        bool loading = false;
        bool shutdown = false;
        bool privateSession = false;

        NetworkRunner networkRunner;
        public NetworkRunner NetworkRunner
        {
            get { if (!networkRunner) networkRunner = GetComponent<NetworkRunner>(); return networkRunner; }
        }

        protected override void Awake()
        {
            base.Awake();
            sceneManager = GetComponent<NetworkSceneManagerDefault>();
        }

        void Update()
        {
            if (NetworkRunner == null || !NetworkRunner.IsSceneAuthority || NetworkRunner.SessionInfo == null || !Player.Local) return;

            if (SceneManager.GetActiveScene().buildIndex == Constants.GameSceneIndex)
                return;

            if (!loading) // Game not started yet
            {
                foreach (var player in PlayerManager.Instance.Players)
                {
                    if (!player.Ready)
                        return;
                }

                // Start game
                NetworkRunner.SessionInfo.IsOpen = false;
                loading = true;
                Debug.Log("Loading game scene");
                NetworkRunner.LoadScene(SceneRef.FromIndex(Constants.GameSceneIndex), LoadSceneMode.Single).AddOnCompleted((op) => { loading = false; });
                
            }
        }
     
        async void StartSession(StartGameArgs args)
        {
            //if (!started) return;
            loading = false;

            NetworkRunner runner = GetComponent<NetworkRunner>();
            if (!runner)
                runner = gameObject.AddComponent<NetworkRunner>();


            var result = await runner.StartGame(args);

            if (result.Ok)
            {
                Debug.Log($"Session started:{runner.SessionInfo.Name}");
            }
            else
            {
                Debug.Log("Start session failed");
                OnStartSessionFailed?.Invoke();
            }
        }

        public void CreateOnlineSession(bool isPrivate)
        {
            privateSession = isPrivate;

            //if (!started) return;
            StartGameArgs args = new StartGameArgs()
            {

                GameMode = GameMode.Shared,
                SessionName = $"{AccountManager.Instance.UserName}_{System.Guid.NewGuid()}",
                //MatchmakingMode = Fusion.Photon.Realtime.MatchmakingMode.,

                PlayerCount = MaxPlayers,
                SceneManager = sceneManager,
                DisableNATPunchthrough = true,
                IsVisible = !isPrivate
                
            };

            StartSession(args);
        }

        public void CreateOfflineSession()
        {
            StartGameArgs args = new StartGameArgs()
            {

                GameMode = GameMode.Single,
                SessionName = $"{AccountManager.Instance.UserName}_{System.Guid.NewGuid()}",
                //MatchmakingMode = Fusion.Photon.Realtime.MatchmakingMode.,

                PlayerCount = 1,
                SceneManager = sceneManager,
                //DisableNATPunchthrough = true,
                //IsVisible = !isPrivate

            };

            StartSession(args);
        }

        public async void JoinSessionLobby()
        {
            //if (!started) return;
            NetworkRunner runner = GetComponent<NetworkRunner>();
            if (!runner || shutdown)
            {
                if (shutdown)
                {
                    shutdown = false;
                    DestroyImmediate(runner);
                }

                runner = gameObject.AddComponent<NetworkRunner>();
            }

            var result = await runner.JoinSessionLobby(SessionLobby.Shared);

            if (result.Ok)
            {
                Debug.Log($"Joined to session lobby");
                OnJoinedToSessionLobbyEvent?.Invoke();
            }
            else
            {
                Debug.Log("Join to session lobby failed");
                OnJoinToSessionLobbyFailedEvent?.Invoke();
            }
        }

        public void JoinSession(SessionInfo sessionInfo)
        {
            StartGameArgs args = new StartGameArgs()
            {

                GameMode = GameMode.Shared,
                SessionName = sessionInfo.Name


            };

            StartSession(args);
        }

        public void Shutdown()
        {
            //if (!started) return;
            try
            {
                var runner = GetComponent<NetworkRunner>();
                runner.Shutdown(false);
            }
            catch (Exception ex) { }
            finally
            {
                if (SceneManager.GetActiveScene().buildIndex != Constants.MainSceneIndex)
                {
                    SceneManager.LoadScene(Constants.MainSceneIndex, LoadSceneMode.Single);
                }
            }

            
        }

        #region fusion callbacks
        public void OnConnectedToServer(NetworkRunner runner)
        {
            //throw new NotImplementedException();
            Debug.Log("Connected to server");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log($"Host migration, local is the new master client :{runner.IsSharedModeMasterClient}");
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {

        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"Player {player.PlayerId} joined the session {runner.SessionInfo.Name}");

            // If is the local player then enter the lobby scene
            if (player == runner.LocalPlayer && ( runner.IsSharedModeMasterClient || runner.IsSinglePlayer))
                NetworkRunner.LoadScene(SceneRef.FromIndex(Constants.LobbySceneIndex), LoadSceneMode.Single);

            // If the local player is the master client and the session is full then set invisible
            if(runner.IsSharedModeMasterClient && runner.SessionInfo.PlayerCount == MaxPlayers && !privateSession)
                runner.SessionInfo.IsVisible = false;

            OnPlayerJoinedEvent?.Invoke(runner, player);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"Player {player.PlayerId} left the session {runner.SessionInfo.Name}");

            // If the local player is the master client and you are in lobby the set the session as visible
            if (runner.IsSharedModeMasterClient && SceneManager.GetActiveScene().buildIndex == Constants.LobbySceneIndex && !privateSession)
                runner.SessionInfo.IsVisible = true;

            OnPlayerLeftEvent?.Invoke(runner, player);
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {

        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {

        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            //switch (SceneManager.GetActiveScene().buildIndex)
            //{
            //    case Constants.GameSceneIndex:
            //        loading = false;
            //        break;
            //}
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            //Debug.Log($"SceneLoadStart:{}")
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log($"Session list updated, session count: {sessionList.Count}");
            OnSessionListUpdatedEvent?.Invoke(runner, sessionList);
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            shutdown = true;

            Destroy(runner);
            OnShutdownEvent?.Invoke(runner, shutdownReason);

        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {

        }
        #endregion
    }

}
