using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using sy.Data;
using Unity.VisualScripting;
using Unity.Collections;

public class PlayerController : NetworkBehaviour
{
    public NetworkVariable<NetworkString> userName = new NetworkVariable<NetworkString>();
    public TMP_Text playerName;
    public Transform characterContent;
    public List<CharacterController> characters;
    public GameObject characterPrefab;
    public NetworkObject networkObject;


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(4f);
        if (networkObject == null)
        {
            networkObject = gameObject.GetComponent<NetworkObject>();
        }

        var spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").GetComponent<Transform>();
        this.transform.SetParent(spawnPoint, false);
        BoardManager.instance.players.Add(this);

        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            if (networkObject.IsOwner)
            {
                this.transform.SetAsFirstSibling();
            }

            else
            {
                this.transform.SetAsLastSibling();
            }
        }

        StartCoroutine(InitPlayerCharacters());
    }

    public override void OnNetworkSpawn()
    {
        userName.Value = "Player " + OwnerClientId + 1;
        UpdatePlayerUI();
    }

    public void UpdatePlayerUI()
    {
        playerName.text = userName.Value;
    }
    
    public IEnumerator InitPlayerCharacters()
    {
        yield return new WaitForSeconds(0.25f);
        InitCharacter(0, 0);
        yield return new WaitForSeconds(0.25f);
        InitCharacter(1, 1);
    }

    void InitCharacter(int ID, int index)
    {
        characters[index].Init(this, ID, index);
    }

    //    public void CheckGameEnd()
    //    {
    //        if (characters.Count <= 0)
    //            Invoke(nameof(EndGame), 1f);

    //    }
    //    private void EndGame()
    //    {
    //        BoardManager.Instance.pv.RPC("EndGame", RpcTarget.All, PhotonNetwork.LocalPlayer);
    //        AssignXP();
    //    }
}

public struct NetworkString : INetworkSerializable
{
    public FixedString32Bytes info;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref info);
    }

    public override string ToString()
    {
        return info.ToString();
    }

    public static implicit operator string(NetworkString s) => s.ToString();
    public static implicit operator NetworkString(string s) => new NetworkString()
    {
        info = new FixedString32Bytes(s)
    };
}
