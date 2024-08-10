using UnityEngine;
using UnityEngine.UI;

public class PlayerRoomEntry : MonoBehaviour
{
    [Header("UI References")]
    public Text PlayerNameText;
    public Button PlayerReadyButton;
    public GameObject PlayerReadyImage;

    private int ownerId;
    private bool isPlayerReady;

    #region UNITY

    public void OnEnable()
    {

    }

    public void Start()
    {
        
    }

    public void OnDisable()
    {

    }

    #endregion

    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        PlayerNameText.text = playerName;
    }

    private void OnPlayerNumberingChanged()
    {
        
    }

    public void SetPlayerReady(bool playerReady)
    {
        PlayerReadyButton.GetComponentInChildren<TMPro.TMP_Text>().text = playerReady ? "Ready!" : "Ready?";
        PlayerReadyImage.SetActive(playerReady);
    }
}
