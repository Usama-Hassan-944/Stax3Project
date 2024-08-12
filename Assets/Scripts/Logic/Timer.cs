using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Timer : NetworkBehaviour
{
    public TextMeshProUGUI timeText;

    private void Update()
    {
        if (TurnManager.instance != null)
        {
            UpdateTime(TurnManager.instance.currentTurnTime.Value);
        }
    }

    public void OnEndTurnButtonClicked()
    {
        // BoardManager.instance.ResetPlayerMovesData();
        BoardManager.instance.ResetActiveSyncers();
        if (TurnManager.instance.IsCurrentTurn())
        {
            TurnManager.instance.EndTurnServerRpc();
        }
    }

    public void UpdateTime(float seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        timeText.text = "Turn Time: " + String.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
    }
}
