using sy.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CharacterNetworked : NetworkBehaviour
{
    private List<Transform> self_spawn;
    private List<Transform> enemy_spawn;
    public PlayerController player_controller;
    public CharacterController character_controller;
    public const byte MoveUnitsToTargetPositionEventCode = 1;
    public GameObject SPAWN;
    public GameObject dieVFX;
    public GameObject healthVFX;
    public GameObject specialVFX;
    public bool isAttackUsed;
    public bool isMoveUsed;
    public NetworkObject networkObject;

    //private void OnEnable()
    //{
    //    PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    //}

    //private void OnDisable()
    //{
    //    PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    //}

    //private void OnEvent(EventData photonEvent)
    //{

    //    byte eventCode = photonEvent.Code;
    //    if (eventCode == MoveUnitsToTargetPositionEventCode)
    //    {
    //        object[] data = (object[])photonEvent.CustomData;
    //        Vector3 targetPosition = (Vector2)data[0];
    //        BlockID blockID = (BlockID)data[1];
    //        var _pv = (int)data[2];


    //        if (this.pv.ViewID == _pv)
    //        {
    //            character_controller.blockID = blockID;

    //            SPAWN.GetComponent<RectTransform>().DOAnchorPos(new Vector3(targetPosition.x + 150, targetPosition.y, 1), 0.3f)
    //                .SetEase(Ease.Linear);
    //        }
    //    }
    //}

    public override void OnNetworkSpawn()
    {
        var go = GameObject.FindGameObjectWithTag("CharacterSpawn").gameObject.GetComponent<SpawnPoints>();
        networkObject = gameObject.GetComponent<NetworkObject>();

        if (((IsHost || IsServer) && networkObject.IsOwner) || (!(IsServer || IsHost) && !networkObject.IsOwner))
        {
            self_spawn = go.SelfSpawnPoints;
            enemy_spawn = go.EnemySpawnPoints;
        }

        else if (((IsHost || IsServer) && !networkObject.IsOwner) || (!(IsServer || IsHost) && networkObject.IsOwner))
        {
            self_spawn = go.EnemySpawnPoints;
            enemy_spawn = go.SelfSpawnPoints;
        }

        this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    //public void OnActionTaken()
    //{
    //    if (!pv.IsMine)
    //        return;

    //    if (BoardManager.Instance.EndTurn.GetComponent<PhotonView>().IsMine)
    //    {
    //        var _current = BoardManager.Instance.FindLocation(character_controller.blockID);

    //        BoardManager.Instance.SetInGameUI(this.transform.parent.GetComponent<RectTransform>().anchoredPosition,
    //            () =>
    //            {
    //                BoardManager.Instance.CalculateMove(_current, this, PhotonNetwork.LocalPlayer, BoardManager.ActionType.Move);
    //            },
    //            () =>
    //            {
    //                BoardManager.Instance.CalculateAttack(_current, this, PhotonNetwork.LocalPlayer, BoardManager.ActionType.Attack);
    //            },
    //            () =>
    //            {
    //                BoardManager.Instance.CalculateAbiltiy(_current, this, PhotonNetwork.LocalPlayer, BoardManager.ActionType.Attack);
    //            }, AttackUsed, MoveUsed, character_controller.characterProps, LIMIT_SPECIAL);
    //    }
    //}
    //public void Move(BlockID ID)
    //{
    //    if (!pv.IsMine)
    //        return;

    //    byte evCode = 1;
    //    var _block = BoardManager.Instance.GetBlockWithID(ID);

    //    Vector2 vector2 = _block.GetComponent<RectTransform>().anchoredPosition;
    //    object[] content = new object[] { vector2, (int)ID, pv.ViewID };

    //    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    //    PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, SendOptions.SendReliable);
    //}
    //[PunRPC]
    //public void Die()
    //{
    //    dieVFX.SetActive(true);
    //    Invoke(nameof(Destroy),0.5f);
    //}

    //[PunRPC]
    //public void HealthVFX()
    //{
    //    healthVFX.SetActive(true);
    //    Invoke(nameof(DisableAllVFX), 0.5f);
    //}
    //[PunRPC]
    //public void AbilityVFX()
    //{
    //    specialVFX.SetActive(true);
    //    Invoke(nameof(DisableAllVFX), 0.5f);
    //}

    //private void DisableAllVFX()
    //{
    //    healthVFX.SetActive(false);
    //    specialVFX.SetActive(false);
    //    dieVFX.SetActive(false);
    //}
    //private void Destroy()
    //{
    //    GameObject.Destroy(this.gameObject);
    //}

    [ClientRpc]
    public void InitCharacterClientRpc(int id, int index)
    {
        InitCharacter(id, index);
    }

    public void InitCharacter(int id, int index)
    {
        BlockID BLOCK_ID;
        GameObject SPAWN = null;

        CharacterObject characterProps = GameObject.FindGameObjectsWithTag("CharacterResource")[0].GetComponent<CharacterResource>().FindCharacterWithID(id);
        this.GetComponent<Image>().sprite = characterProps.Character_Sprite;
        if (IsServer)
        {
            if (networkObject.IsOwner)
            {
                Debug.LogError("Server: Reparenting character to self_spawn for client id: " + OwnerClientId + ", character id is: " + id + ", spawnIndex is: " + index + ", index is: " + index);
                this.transform.SetParent(self_spawn[index]);
                SPAWN = self_spawn[index].gameObject;
                self_spawn[index].gameObject.name = characterProps.Name;
            }

            else
            {
                Debug.LogError("Server: Reparenting character to enemy_spawn for client id: " + OwnerClientId + ", character id is: " + id + ", spawnIndex is: " + index + ", index is: " + index);
                this.transform.SetParent(enemy_spawn[index]);
                SPAWN = enemy_spawn[index].gameObject;
                enemy_spawn[index].gameObject.name = characterProps.Name;
            }
            this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        else
        {
            StartCoroutine(SetAnchoredPositionCoroutine());
        }

        if (BoardManager.instance.players.Count != 2)
        {
            BoardManager.instance.players.Clear();
            BoardManager.instance.players = FindObjectsOfType<PlayerController>().ToList();

            foreach (var item in BoardManager.instance.players)
            {
                if (item.networkObject == null)
                {
                    item.networkObject = item.GetComponent<NetworkObject>();
                }
            }
        }

        if (networkObject.IsOwner)
        {
            BLOCK_ID = self_spawn[index].gameObject.GetComponent<PlayerLocation>().PlayerCurrentBlock;
            foreach (var p in BoardManager.instance.players)
            {
                if (p.networkObject.IsOwner)
                {
                    player_controller = p;
                }
            }
        }

        else
        {
            BLOCK_ID = enemy_spawn[index].gameObject.GetComponent<PlayerLocation>().PlayerCurrentBlock;
            foreach (var p in BoardManager.instance.players)
            {
                if (!p.networkObject.IsOwner)
                {
                    player_controller = p;
                }
            }
        }

        if (player_controller != null)
        {
            character_controller = player_controller.characters[index];
            character_controller.blockID = BLOCK_ID;
            character_controller.body = this.gameObject;
        }
    }
    public IEnumerator SetAnchoredPositionCoroutine()
    {
        yield return new WaitForSeconds(1f);

        var rectTransform = this.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
