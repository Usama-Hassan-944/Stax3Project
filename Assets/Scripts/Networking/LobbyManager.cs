using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using Random = UnityEngine.Random;
using sy.Data;
using sy.UI;
using ParrelSync;
using Unity.VisualScripting;
using static UnityEditor.Progress;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace sy.Networking
{
    public class LobbyManager : MonoBehaviour
    {
        #region Fields

        public static LobbyManager instance;
        public MultiplayerUIManager multiplayerUIManager;
        public NetworkManager networkManager;
        public TestRelay testRelay;
        private Action UpdateLobby;
        public Lobby joinedLobby;
        private float heartbeatTimer;
        private float lobbyUpdateTimer;
        private string joinCode = "";

        #endregion


        #region Unity Events

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void OnEnable()
        {
            UpdateLobby += CheckLobbyUpdate;
        }
        
        private void OnDisable()
        {
            UpdateLobby -= CheckLobbyUpdate;
        }

        private void Update()
        {
            CheckLobbyHeartbeat(); 
            UpdateLobby();
        }

        #endregion

        #region Methods

        public async void Authenticate()
        {
            var options = new InitializationOptions();

#if UNITY_EDITOR
            options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
#endif
            await UnityServices.InitializeAsync(options);
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private async void CheckLobbyUpdate()
        {
            if (joinedLobby != null)
            {
                lobbyUpdateTimer -= Time.deltaTime;
                if (lobbyUpdateTimer < 0f)
                {
                    float lobbyPollTimerMax = 1.1f;
                    lobbyUpdateTimer = lobbyPollTimerMax;
                    joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                    LobbyUI.instance.UpdateLobby(joinedLobby);

                    if (joinedLobby.Data["RelayJoinCode"].Value != "0")
                    {
                        if (!IsHostOfLobby())
                        {
                            LobbyUI.instance.Clear();
                            testRelay.JoinRelay(joinedLobby.Data["RelayJoinCode"].Value);
                        }
                        joinedLobby = null;
                    }
                }
            }
        }

        private async void CheckLobbyHeartbeat()
        {
            if (IsHostOfLobby())
            {
                heartbeatTimer -= Time.deltaTime;
                if (heartbeatTimer < 0f)
                {
                    float heartbeatTimerMax = 15f;
                    heartbeatTimer = heartbeatTimerMax;
                    Debug.Log("Heartbeat Sent");
                    await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                }
            }
        }

        private Player GetPlayer()
        {
            Debug.LogError("Player Name: " + ClientInfo.Username);
            string userName = ClientInfo.Username;
            return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, userName) },
            { "Status", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "0") },
        });
        }

        public async void CreateLobby()
        {
            string lobbyName = "Lobby" + ServerInfo.LobbyName;
            int maxPlayers = 2;
            Player player = GetPlayer();
            PlayerPrefs.SetString("CurrentLobby", lobbyName);

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Player = player,
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
            {
                {"RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, "0" )},
                {"LobbyID", new DataObject(DataObject.VisibilityOptions.Public, PlayerPrefs.GetString("CurrentLobby"),DataObject.IndexOptions.S5)}
            }
            };

            try
            {
                joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
                Debug.Log("Lobby/Room created: " + joinedLobby.Name + ", " + joinedLobby.LobbyCode + ", " + joinedLobby.MaxPlayers);
                multiplayerUIManager.OpenLobby();
                multiplayerUIManager.CloseCreateRoom();
                LobbyUI.instance.UpdateLobby(joinedLobby);
                UpdatePlayerName(ClientInfo.Username);
            }

            catch(LobbyServiceException e)
            {
                Debug.LogError(e);
                multiplayerUIManager.OpenCreateRoom();
            }
        }

        public async void JoinLobby()
        {
            string lobbyName = "Lobby" + ServerInfo.LobbyName;
            Player player = GetPlayer();
            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions();

                options.Filters = new List<QueryFilter> {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GT, value: "0")
            };

                options.Order = new List<QueryOrder> {
                new QueryOrder(asc: false, field: QueryOrder.FieldOptions.Created)
                };

                QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);
                foreach (Lobby lobby in lobbyListQueryResponse.Results)
                {
                    if (lobby.Name == lobbyName)
                    {
                        joinCode = lobby.Id;
                        if (!string.IsNullOrEmpty(joinCode))
                        {
                            try
                            {
                                joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(joinCode, new JoinLobbyByIdOptions
                                {
                                    Player = player
                                });

                                Debug.Log("Lobby/Room joined: " + joinedLobby.Name + ", " + joinedLobby.LobbyCode + ", " + joinedLobby.MaxPlayers);
                                multiplayerUIManager.OpenLobby();
                                multiplayerUIManager.CloseJoinRoom();
                                LobbyUI.instance.UpdateLobby(joinedLobby);
                                UpdatePlayerName(ClientInfo.Username);
                            }

                            catch (LobbyServiceException e)
                            {
                                Debug.LogError(e);
                                multiplayerUIManager.OpenJoinRoom();
                            }
                        }
                    }

                    else
                    {
                        Debug.LogError("Lobby Name does not match!");
                    }
                }
            }

            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public async void LeaveLobby()
        {
            if (joinedLobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                    multiplayerUIManager.CloseLobby();
                    multiplayerUIManager.CloseMultiplayerMenu();
                }

                catch (LobbyServiceException e)
                {
                    Debug.LogError(e);
                }
            }
        }

        public bool IsHostOfLobby()
        {
            if (joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public async void UpdatePlayerName(string userName)
        {
            if (joinedLobby != null)
            {
                try
                {
                    UpdatePlayerOptions options = new UpdatePlayerOptions();
                    options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        "PlayerName", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Public, value: ClientInfo.Username)
                    }
                };

                    string playerId = AuthenticationService.Instance.PlayerId;

                    Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                    joinedLobby = lobby;
                    LobbyUI.instance.UpdateLobby(joinedLobby);
                }

                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }

        public async void UpdatePlayerReadyStatus(bool status)
        {
            string val = "0";
            if (status)
            {
                val = "1";
            }

            if (joinedLobby != null)
            {
                try
                {
                    UpdatePlayerOptions options = new UpdatePlayerOptions();
                    options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        "Status", new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Member, value: val)
                    }
                };

                    string playerId = AuthenticationService.Instance.PlayerId;

                    Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, playerId, options);
                    joinedLobby = lobby;
                    LobbyUI.instance.UpdateLobby(joinedLobby);
                }

                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        }

        public async void StartGame()
        {
            if (IsHostOfLobby())
            {
                try
                {
                    string relayCode = await testRelay.CreateRelay();
                    UpdateLobbyOptions options = new UpdateLobbyOptions
                    {
                        Data = new Dictionary<string, DataObject>
                        {
                            {"RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, relayCode )},
                        }
                    };

                    Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, options);
                    joinedLobby = null;
                    LobbyUI.instance.Clear();
                    NetworkManager.Singleton.StartHost();
                    networkManager.SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
                }

                catch (LobbyServiceException e)
                {
                    Debug.LogError(e);
                }
            }
        }

        #endregion
    }
}
