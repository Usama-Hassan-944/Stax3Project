using sy.Networking;
using System;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRoomEntry : MonoBehaviour
{
    #region Fields

    [Header("UI References")]
    public Text playerNameText;
    public Button playerReadyButton;
    public GameObject playerReadyImage;
    public string id;

    private int ownerId;
    private bool isPlayerReady = false;

    #endregion

    #region Unity Events

    private void Start()
    {
        // playerReadyButton.onClick.AddListener(() => UpdateReadyStatus());
    }

    #endregion

    #region Methods

    public void UpdateReadyStatus()
    {
        isPlayerReady = !isPlayerReady;
    }

    public void UpdateEntry(Player player)
    {
        id = player.Id;
        playerNameText.text = player.Data["PlayerName"].Value;
        if (player.Data["Status"].Value == "0")
        {
            isPlayerReady = false;
        }

        else
        {
            isPlayerReady = true;
        }

        // UpdateReadyUI(player);
    }

    public void UpdateReadyUI(Player player)
    {
        if (player.Id == AuthenticationService.Instance.PlayerId)
        {
            playerReadyButton.gameObject.SetActive(true);
            if (isPlayerReady)
            {
                playerReadyImage.gameObject.SetActive(isPlayerReady);
                playerReadyButton.interactable = !isPlayerReady;
            }
        }
    }

    #endregion

}
