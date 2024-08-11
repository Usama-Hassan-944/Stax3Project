using UnityEngine;
using UnityEngine.UI;
using TMPro;
using sy.Networking;
using sy.Data;

namespace sy.UI
{
    public class MultiplayerUIManager : MonoBehaviour
    {
        #region Fields

        public GameObject multiplayerMenu;
        public GameObject mainMenu;

        [Header("Room Panels")]
        public GameObject createRoomPanel;
        public GameObject joinRoomPanel;
        public GameObject lobbyPanel;

        [Header("Create Room Fields")]
        public Button createRoomConfirm;
        public TMP_InputField lobbyName;
        private bool _lobbyIsValid;

        [Header("Join Room Fields")]
        public Button joinRoomConfirm;

        [Header("Leave Room")]
        public Button leaveRoomButton;

        [Header("Network")]
        public LobbyManager lobbyManager;

        #endregion

        #region Unity Events

        private void Start()
        {
            createRoomConfirm.onClick.AddListener(() => TryCreateLobby());
            joinRoomConfirm.onClick.AddListener(() => TryJoinLobby());
            leaveRoomButton.onClick.AddListener(() => TryLeaveLobby());
        }

        #endregion

        #region Room

        public void onLobbyValueChanged(string name)
        {
            ServerInfo.LobbyName = name;
            createRoomConfirm.interactable = !string.IsNullOrEmpty(name);
        }

        public void onJoinRoomValueChanged(string name)
        {
            ServerInfo.LobbyName = name;
            joinRoomConfirm.interactable = !string.IsNullOrEmpty(name);
        }

        public void ValidateLobby()
        {
            _lobbyIsValid = string.IsNullOrEmpty(ServerInfo.LobbyName) == false;
        }

        public void TryCreateLobby()
        {
            ValidateLobby();
            if (_lobbyIsValid)
            {
                lobbyManager.CreateLobby();
                _lobbyIsValid = false;
            }
        }

        public void TryJoinLobby()
        {
            ValidateLobby();
            if (_lobbyIsValid)
            {
                lobbyManager.JoinLobby();
                _lobbyIsValid = false;
            }
        }
        
        public void TryLeaveLobby()
        {
            lobbyManager.LeaveLobby();
        }

        #endregion

        #region UI
        public void OpenMultiplayerMenu()
        {
            multiplayerMenu.SetActive(true);
            mainMenu.SetActive(false);
        }

        public void CloseMultiplayerMenu()
        {
            multiplayerMenu.SetActive(false);
            mainMenu.SetActive(true);
        }

        public void OpenJoinRoom()
        {
            joinRoomPanel.SetActive(true);
        }
        public void CloseJoinRoom()
        {
            joinRoomPanel.SetActive(false);
        }

        public void OpenCreateRoom()
        {
            lobbyName.text = ServerInfo.LobbyName;
            createRoomPanel.SetActive(true);
        }
        public void CloseCreateRoom()
        {
            createRoomPanel.SetActive(false);
        }

        public void OpenLobby()
        {
            lobbyPanel.SetActive(true);
        }

        public void CloseLobby()
        {
            lobbyPanel.SetActive(false);
        }

        #endregion
    }
}
