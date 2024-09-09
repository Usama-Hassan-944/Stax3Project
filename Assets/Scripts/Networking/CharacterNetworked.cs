using sy.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterNetworked : NetworkBehaviour
{
    public Image mySprite;
    private List<Transform> selfSpawn;
    private List<Transform> enemySpawn;
    public PlayerController playerController;
    public CharacterController characterController;
    public GameObject SPAWN;
    public GameObject dieVFX;
    public GameObject healthVFX;
    public bool isAttackUsed;
    public bool isMoveUsed;
    public BlockID blockId;
    public NetworkObject networkObject;

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
        if (!IsOwner)
        {
            return;
        }

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

    public void Move(BlockID ID)
    {
        if (!IsOwner)
        {
            return;
        }

        Vector2 targetPosition = BoardManager.instance.GetBlockWithID(ID).GetComponent<RectTransform>().anchoredPosition;
        MoveServerRpc(targetPosition, ID, (int)networkObject.NetworkObjectId); 
    }

    [ServerRpc]
    public void MoveServerRpc(Vector2 targetPosition, BlockID blockID, int uid)
    {
        if (networkObject.NetworkObjectId == (ulong)uid)
        {
            MoveCharacter(targetPosition, blockID);
            MoveClientRpc(targetPosition, blockID, uid);
        }
    }

    private void MoveCharacter(Vector2 targetPosition, BlockID blockID)
    {
        this.blockId = blockID;
        characterController.blockID = blockID;

        if (SPAWN != null)
        {
            SPAWN.GetComponent<RectTransform>().DOAnchorPos(new Vector3(targetPosition.x + 150, targetPosition.y, 1), 0.3f).SetEase(Ease.Linear);
        }

        else
        {
            Debug.LogError("SPAWN object is null.");
        }
    }

    [ClientRpc]
    private void MoveClientRpc(Vector2 targetPosition, BlockID blockID, int uid)
    {
        if (networkObject.NetworkObjectId == (ulong)uid)
        {
            MoveCharacter(targetPosition, blockID);
        }
    }

    [ClientRpc]
    public void DieVFXClientRpc(ulong uid)
    {
        if (networkObject.NetworkObjectId == uid)
        {
            Debug.Log("DieVFXClientRpc triggered. Activating dieVFX.");
            dieVFX.SetActive(true);
        }
    }

    [ClientRpc]
    public void HealthVFXClientRpc(ulong uid)
    {
        if (networkObject.NetworkObjectId == uid)
        {
            Debug.Log("HealthVFXClientRpc triggered. Activating healthVFX.");
            healthVFX.SetActive(true);
            Invoke(nameof(DisableAllVFX), 0.5f);
        }
    }

    private void DisableAllVFX()
    {
        healthVFX.SetActive(false);
        dieVFX.SetActive(false);
    }

    private void End()
    {
        NetworkObject.Despawn();
    }
}
