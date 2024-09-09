using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager instance;
    public float turnDuration = 95f;

    public NetworkVariable<float> currentTurnTime = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<ulong> currentTurnPlayer = new NetworkVariable<ulong>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (IsServer)
        {
            if (NetworkManager.Singleton.ConnectedClientsList.Count > 0)
            {
                currentTurnPlayer.Value = NetworkManager.Singleton.ConnectedClientsList[0].ClientId;
            }
            currentTurnTime.Value = turnDuration;
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            currentTurnTime.Value -= Time.deltaTime;
            if (currentTurnTime.Value <= 0)
            {
                BoardManager.instance.ResetPlayerMovesData();
                BoardManager.instance.ResetActiveSyncers();
                EndTurnServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRpc()
    {
        if (IsServer)
        {
            var clientList = NetworkManager.Singleton.ConnectedClientsList.ToList();
            int clientCount = clientList.Count;
            if (clientCount == 0)
            {
                return;
            }
            int currentIndex = clientList.FindIndex(client => client.ClientId == currentTurnPlayer.Value);
            int nextTurnIndex = (currentIndex + 1) % clientCount;
            currentTurnPlayer.Value = clientList[nextTurnIndex].ClientId;
            currentTurnTime.Value = turnDuration;
            UpdateOwnership();
        }
    }

    public void UpdateOwnership()
    {
        if (IsServer)
        {
            ulong newOwnerId = currentTurnPlayer.Value;
            BoardManager.instance.endTurnButton.GetComponent<NetworkObject>().ChangeOwnership(newOwnerId);
            foreach (var row in BoardManager.instance.Board)
            {
                foreach (var col in row)
                {
                    col.GetComponent<NetworkObject>().ChangeOwnership(newOwnerId);
                }
            }
        }
    }

    public bool IsCurrentTurn()
    {
        return NetworkManager.Singleton.LocalClientId == currentTurnPlayer.Value;
    }
}
