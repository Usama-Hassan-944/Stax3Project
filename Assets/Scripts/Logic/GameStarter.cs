using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace sy.Networking
{
    public class GameStarter : MonoBehaviour
    {
        #region Fields

        public static GameStarter instance;

        #endregion

        #region Unity Events

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        #endregion

        #region Methods

        public void OnStartGameButtonClicked()
        {
            if (LobbyManager.instance.IsHostOfLobby())
            {
                NetworkManager.Singleton.StartHost();
                StartGameServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void StartGameServerRpc()
        {
            StartGameClientRpc();
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }

        [ClientRpc]
        private void StartGameClientRpc()
        {
            SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }

        #endregion
    }
}