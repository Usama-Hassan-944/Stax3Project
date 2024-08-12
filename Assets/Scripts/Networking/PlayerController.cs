using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public TMP_Text playerName;
    public Transform characterContent;
    public List<CharacterController> characters;
    public GameObject characterPrefab;
    public NetworkObject networkObject;

    private IEnumerator Start()
    {
        if (networkObject == null)
        {
            networkObject = FindObjectOfType<NetworkObject>();
        }

        yield return new WaitForSeconds(2f);
        var spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").GetComponent<Transform>();
        this.transform.SetParent(spawnPoint, false);
        BoardManager.Instance.players.Add(this);

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

        if (IsLocalPlayer)
        {
            
        }

        else
        {
            
        }
    }
    public void InitLocalPlayer()
    {
        //if (p.IsMasterClient)
        //    this.transform.SetAsFirstSibling();

        //PlayerName.text = p.NickName;
        //UpdateXp((int)p.CustomProperties["XP"]);
        //InitCharacter((int)p.CustomProperties["c1"], 0);
        //InitCharacter((int)p.CustomProperties["c2"], 1);
        //InitCharacter((int)p.CustomProperties["c3"], 2);
        //InitCharacter((int)p.CustomProperties["c4"], 3);

    }

    //    [PunRPC]
    //    public void RewardPlayerXP(int xp)
    //    {
    //          XP += xp;
    //            UpdateXp(XP);
    //    }

    //    void InitCharacter(int ID, int index)
    //    {
    //        characters[index].Init(ID, index);
    //    }
    //    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //    {
    //        if (stream.IsWriting)
    //        {
    //            stream.SendNext(XP);
    //        }
    //        if (stream.IsReading)
    //        {
    //            UpdateXp((int)stream.ReceiveNext());
    //        }
    //    }

    //    public void UpdateXp(int Xp)
    //    {
    //        if (Xp > XpSlider.maxValue)
    //            XpSlider.maxValue *= 10;
    //        XpSlider.DOValue(Xp, 1.5f, true);
    //        PlayerXPText.text = Xp.ToString();

    //    }

    //    public void AssignXP()
    //    {
    //        if(!pv.IsMine)
    //            return;

    //        ClientInfo.XP = XP;
    //    }

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
