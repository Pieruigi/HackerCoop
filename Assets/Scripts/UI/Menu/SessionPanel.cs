using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HKR.UI
{
    public class SessionPanel : MonoBehaviour
    {
        [SerializeField]
        Button buttonCreate;

        [SerializeField]
        Button buttonBack;

        [SerializeField]
        SessionList sessionList;

        bool active = false;
       
        private void OnEnable()
        {
            if (!active) return;

            // Disable buttons
            SetInteractableAll(false);

            SessionManager.OnPlayerJoinedEvent += HandleOnPlayerJoined;
            SessionManager.OnShutdownEvent += HandleOnShutdown;
            SessionManager.OnJoinedToSessionLobbyEvent += HandleOnJoinedToSessionLobby;
            SessionManager.OnJoinToSessionLobbyFailedEvent += HandleOnJoinToSessionLobbyFailed;
            SessionManager.OnStartSessionFailed += HandleOnStartSessionFailed;
            SessionManager.Instance?.JoinSessionLobby();

        }

        private void OnDisable()
        {
            if(!active)
            {
                active = true;
                return;
            }

            SessionManager.OnPlayerJoinedEvent -= HandleOnPlayerJoined;
            SessionManager.OnShutdownEvent -= HandleOnShutdown;
            SessionManager.OnJoinedToSessionLobbyEvent -= HandleOnJoinedToSessionLobby;
            SessionManager.OnJoinToSessionLobbyFailedEvent -= HandleOnJoinToSessionLobbyFailed;
            SessionManager.OnStartSessionFailed -= HandleOnStartSessionFailed;
        }

        private void HandleOnStartSessionFailed()
        {
            SetInteractableAll(true);
        }

        private void HandleOnJoinToSessionLobbyFailed()
        {
            GetComponentInParent<MainMenu>().ShowMainPanel();
        }

        private void HandleOnJoinedToSessionLobby()
        {
            // We joined the session lobby, enable buttons back
            SetInteractableAll(true);
        }

        private void HandleOnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
           
        }

        private void HandleOnShutdown(NetworkRunner arg0, ShutdownReason arg1)
        {
            GetComponentInParent<MainMenu>().ShowMainPanel();
        }

        void SetInteractableAll(bool value)
        {
            buttonCreate.interactable = value;
            buttonBack.interactable = value;
            sessionList.SetInteractable(value);
        }

        public void CreateGameSession()
        {
            SetInteractableAll(false);
            SessionManager.Instance.CreateOnlineSession(false);
        }

        public void Shutdown()
        {
            SetInteractableAll(false);
            SessionManager.Instance.Shutdown(); 
        }
    }

}
