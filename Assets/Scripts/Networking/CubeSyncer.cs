using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CubeSyncer : MonoBehaviour
{
    public GameObject Red_Glow;
    public GameObject Golden_Glow;

    private bool glowGolden = false;
    private bool glowRed = false;

    [Header("Block ID")]
    public BlockID ID;
    // public PhotonView pv;
    public UnityAction action;

    void Start()
    {
        // pv = GetComponent<PhotonView>();
    }

    //void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if(stream.IsWriting)
    //    {
    //        stream.SendNext(glowGolden);
    //        stream.SendNext(glowRed);
    //    }
    //    if(stream.IsReading)
    //    {
    //        SetGolden((bool)stream.ReceiveNext());
    //        SetRed((bool)stream.ReceiveNext());
    //    }
    //}

    public void SetGolden(bool status/*, Player player = null*/)
    {
        //if (player != null && !pv.IsMine)
        //{
        //    this.TransferOwnerShip(player);
        //}
        //glowGolden = status;
        //    Golden_Glow.SetActive(glowGolden);
        
    }
    public void SetRed(bool status/*, Player player = null*/)
    {
        //if(player != null && !pv.IsMine)
        //{
        //    this.TransferOwnerShip(player);
        //}
        //    glowRed = status;
        //    Red_Glow.SetActive(glowRed);   
    }
   
    public void TransferOwnerShip(/*Player player*/)
    {
        // pv.TransferOwnership(player);
    }
    public void OnActionTaken()
    {
        if(action != null)
            action();
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
    H1, H2, H3, H4, H5, H6, H7, H8, none
}
