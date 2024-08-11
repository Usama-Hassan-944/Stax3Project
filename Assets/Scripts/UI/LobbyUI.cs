using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace sy.Networking
{
    public class LobbyUI : MonoBehaviour
    {
        #region Fields

        public static LobbyUI instance;
        public GameObject roomEntry;
        public Transform parent;
        public List<GameObject> instantiatedObjects;
        public Button startGameButton;

        #endregion

        #region Unity Events

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            startGameButton.gameObject.SetActive(false);
            startGameButton.onClick.AddListener(() => StartGame());
        }

        #endregion


        #region Methods

        public void UpdateLobby(Lobby lobby)
        {
            Clear();
            foreach (var item in lobby.Players)
            {
                GameObject go = Instantiate(roomEntry, parent);
                instantiatedObjects.Add(go);
                roomEntry.GetComponent<PlayerRoomEntry>().UpdateEntry(item);
            }

            if (lobby.Players.Count >= 2)
            {
                if (LobbyManager.instance.IsHostOfLobby())
                {
                    startGameButton.gameObject.SetActive(true);
                }
            }
        }

        public void Clear()
        {
            foreach (var item in instantiatedObjects)
            {
                Destroy(item);
            }
            instantiatedObjects.Clear();
        }

        public void StartGame()
        {
            Debug.LogError("StartGame clicked");
            LobbyManager.instance.StartGame();
        }

        #endregion
    }
}