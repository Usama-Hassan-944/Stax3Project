using sy.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class CharacterNetworked : NetworkBehaviour
{
    private List<Transform> selfSpawn;
    private List<Transform> enemySpawn;
    public PlayerController playerController;
    public CharacterController characterController;
    public const byte MoveUnitsToTargetPositionEventCode = 1;
    public GameObject SPAWN;
    public GameObject dieVFX;
    public GameObject healthVFX;
    public GameObject specialVFX;
    public bool isAttackUsed;
    public bool isMoveUsed;
    public BlockID blockId;
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
            selfSpawn = go.SelfSpawnPoints;
            enemySpawn = go.EnemySpawnPoints;
        }

        else if (((IsHost || IsServer) && !networkObject.IsOwner) || (!(IsServer || IsHost) && networkObject.IsOwner))
        {
            selfSpawn = go.EnemySpawnPoints;
            enemySpawn = go.SelfSpawnPoints;
        }

        this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    [ClientRpc]
    public void InitCharacterClientRpc(int id, int index)
    {
        InitCharacter(id, index);
    }

    public void InitCharacter(int id, int index)
    {
        GameObject SPAWN = null;

        CharacterObject characterProps = GameObject.FindGameObjectsWithTag("CharacterResource")[0].GetComponent<CharacterResource>().FindCharacterWithID(id);
        this.GetComponent<Image>().sprite = characterProps.Character_Sprite;
        if (IsServer)
        {
            if (networkObject.IsOwner)
            {
                this.transform.SetParent(selfSpawn[index]);
                SPAWN = selfSpawn[index].gameObject;
                blockId = selfSpawn[index].gameObject.GetComponent<PlayerLocation>().PlayerCurrentBlock;
                selfSpawn[index].gameObject.name = characterProps.Name;
            }

            else
            {
                this.transform.SetParent(enemySpawn[index]);
                SPAWN = enemySpawn[index].gameObject;
                blockId = enemySpawn[index].gameObject.GetComponent<PlayerLocation>().PlayerCurrentBlock;
                enemySpawn[index].gameObject.name = characterProps.Name;
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
                foreach (var p in BoardManager.instance.players)
                {
                    if (p.networkObject.IsOwner)
                    {
                        playerController = p;
                    }
                }
            }

            else
            {
                foreach (var p in BoardManager.instance.players)
                {
                    if (!p.networkObject.IsOwner)
                    {
                        playerController = p;
                    }
                }
            }

            if (playerController != null)
            {
                characterController = playerController.characters[index];
                characterController.blockID = blockId;
                // character_controller.myPlayerController = player_controller;
                characterController.body = this.gameObject;
            }
            this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        else
        {
            if (networkObject.IsOwner)
            {
                blockId = enemySpawn[index].gameObject.GetComponent<PlayerLocation>().PlayerCurrentBlock;
                SPAWN = enemySpawn[index].gameObject;
            }

            else
            {
                blockId = selfSpawn[index].gameObject.GetComponent<PlayerLocation>().PlayerCurrentBlock;
                SPAWN = selfSpawn[index].gameObject;
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
                foreach (var p in BoardManager.instance.players)
                {
                    if (p.networkObject.IsOwner)
                    {
                        playerController = p;
                    }
                }
            }

            else
            {
                foreach (var p in BoardManager.instance.players)
                {
                    if (!p.networkObject.IsOwner)
                    {
                        playerController = p;
                    }
                }
            }

            if (playerController != null)
            {
                characterController = playerController.characters[index];
                characterController.blockID = blockId;
                // character_controller.myPlayerController = player_controller;
                characterController.body = this.gameObject;
            }

            StartCoroutine(SetAnchoredPositionCoroutine());
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

    public void OnActionTaken()
    {
        if (!IsOwner) return;

        if (BoardManager.instance.endTurnButton.GetComponent<NetworkObject>().IsOwner)
        {
            var _current = BoardManager.instance.FindLocation(characterController.blockID);

            BoardManager.instance.SetInGameUI(
                this.transform.parent.GetComponent<RectTransform>().anchoredPosition,
                () => BoardManager.instance.CalculateMove(_current, this, NetworkManager.Singleton.LocalClientId, BoardManager.ActionType.Move),
                () => BoardManager.instance.CalculateAttack(_current, this, NetworkManager.Singleton.LocalClientId, BoardManager.ActionType.Attack),
                isAttackUsed, isMoveUsed, characterController.characterObj);
        }
    }

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

}
