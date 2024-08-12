//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Timmer : MonoBehaviour
//{
//    public PhotonView pv;
//    float seconds = 95;
//    public TMPro.TMP_Text Timetext;
//    private int turn;


//    private void Update()
//    {
//        seconds -= Time.deltaTime;
//        UpdateTime(seconds);
//    }
//    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//    {
//        if (stream.IsWriting)
//        {
//            stream.SendNext(seconds);
//        }
//        if (stream.IsReading)
//        {
//            UpdateTime((float)stream.ReceiveNext());
//        }
//    }
//    public void OnEndTurn()
//    {
//        if (!BoardManager.Instance.EndTurn.GetComponent<PhotonView>().IsMine)
//            return;

//        pv.RPC("ResetTimmer", RpcTarget.All);
      
//        BoardManager.Instance.ResetPlayerMovesData();
//        BoardManager.Instance.ResetActiveSyncers();
        
//        foreach (var p in BoardManager.Instance.players)
//            if (p.pv.IsMine)
//                p.pv.RPC("RewardPlayerXP", RpcTarget.All, (int)CharacterObject.RewardXP.Turn);

//        if (PhotonNetwork.PlayerListOthers.Length > 0)
//            BoardManager.Instance.EndTurn.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.PlayerListOthers[0]);
     
        
//    }
//    [PunRPC]
//    public void ResetTimmer()
//    {
//        turn += 1;
//        seconds = 95;
//        UpdateTime(seconds);
//        if(turn >= 2  && BoardManager.Instance.RESET_ABILITY)
//        {
//            BoardManager.Instance.ResetAbilityData();
//        }
//    }

//    public void UpdateTime(float time)
//    {
//        if (seconds <= 0)
//        {
//            OnEndTurn();
//            seconds = 95;
//            UpdateTime(seconds);
//        }
//        TimeSpan t = TimeSpan.FromSeconds(seconds);
//        Timetext.text = "Turn Time: "+ String.Format("{0:00}:{1:00}", t.Minutes,t.Seconds);
//    }
//}
