using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

public class CubeSyncer : NetworkBehaviour
{
    public GameObject redGlow;
    public GameObject goldenGlow;
    public BlockID ID;
    public UnityAction action;

    private NetworkVariable<bool> glowGolden = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> glowRed = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        UpdateGlowState();
    }

    private void UpdateGlowState()
    {
        Debug.LogError("My Cube id is: " + ID.ToString());
        Debug.Log($"Updating glow state. Golden: {glowGolden.Value}");
        goldenGlow.SetActive(glowGolden.Value);
        redGlow.SetActive(glowRed.Value);
    }

    public void SetGolden(bool status)
    {
        Debug.Log($"SetGolden called");
        if (IsOwner)
        {
            Debug.Log($"SetGolden called. New status: {status}");
            glowGolden.Value = status;
            UpdateGlowState();
        }
    }

    public void SetRed(bool status)
    {
        Debug.Log($"SetRed called");
        if (IsOwner)
        {
            glowRed.Value = status;
        }
    }

    public void OnActionTaken()
    {
        if (action != null)
        {
            action();
        }
    }
}

[Serializable]
public enum BlockID
{
    A1, A2, A3, A4, A5, A6, A7, A8,
    B1, B2, B3, B4, B5, B6, B7, B8,
    C1, C2, C3, C4, C5, C6, C7, C8,
    D1, D2, D3, D4, D5, D6, D7, D8,
    E1, E2, E3, E4, E5, E6, E7, E8,
    F1, F2, F3, F4, F5, F6, F7, F8,
    G1, G2, G3, G4, G5, G6, G7, G8,
    H1, H2, H3, H4, H5, H6, H7, H8,
}
