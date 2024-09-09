using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using sy.Data;
using Unity.Netcode;
using System.Linq;

public class CharacterController : NetworkBehaviour
{
    [Header("Character UI Elements")]
    public TextMeshProUGUI name;
    public Image icon;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI defenceText;
    public TextMeshProUGUI attackText;
    public Slider healthSlider;
    public Slider defenceSlider;
    public Slider attackSlider;

    private Transform spawnPoint;
    public CharacterObject characterObj;
    public CharacterResource resource;
    public GameObject body;
    private int id;
    public BlockID blockID;
    public PlayerController myPlayerController;
    public bool hasSpawnedCharacters = false;

    [Header("Networked")]
    public NetworkVariable<int> Health = new NetworkVariable<int>(0, writePerm: NetworkVariableWritePermission.Owner, readPerm: NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> AttackPower = new NetworkVariable<int>(0, writePerm: NetworkVariableWritePermission.Owner, readPerm: NetworkVariableReadPermission.Everyone);
    public NetworkVariable<int> Defence = new NetworkVariable<int>(0, writePerm: NetworkVariableWritePermission.Owner, readPerm: NetworkVariableReadPermission.Everyone);


    public override void OnNetworkSpawn()
    {
        Health.OnValueChanged += (oldValue, newValue) => SetHealth(newValue);
        AttackPower.OnValueChanged += (oldValue, newValue) => SetAttack(newValue);
        Defence.OnValueChanged += (oldValue, newValue) => SetDefence(newValue);

    }

    public void Init(PlayerController playerController, int id, int index)
    {
        if (resource == null)
        {
            var _res = GameObject.FindGameObjectsWithTag("CharacterResource")[0].GetComponent<CharacterResource>();
            resource = _res;
        }
        spawnPoint = GameObject.FindGameObjectWithTag("CharacterSpawn").GetComponent<Transform>();

        this.id = id;
        characterObj = resource.FindCharacterWithID(id);

        name.text = characterObj.Name;
        icon.sprite = characterObj.Character_Sprite;

        myPlayerController = playerController;

        if (IsOwner)
        {
            Health.Value = characterObj.Health;
            AttackPower.Value = characterObj.Attack_Power;
            Defence.Value = characterObj.Defence;
        }

        StartCoroutine(SpawnBody(index));
    }
    public void SetHealth(int health)
    {
        healthText.text = health.ToString();
        healthSlider.value = health;
    }

    public void SetAttack(int attack)
    {
        attackText.text = attack.ToString();
        attackSlider.value = attack;
    }

    public void SetDefence(int defence)
    {
        defenceText.text = defence.ToString();
        defenceSlider.value = defence;
    }

    public IEnumerator SpawnBody(int index)
    {
        yield return new WaitForSeconds(1f);
        if (NetworkManager.Singleton.IsServer && !hasSpawnedCharacters)
        {
            GameObject character = Instantiate(BoardManager.instance.playerPrefab, spawnPoint.position, Quaternion.identity);
            NetworkObject networkObject = character.GetComponent<NetworkObject>();
            networkObject.Spawn();

            if (NetworkManager.Singleton.ConnectedClients.ContainsKey(OwnerClientId))
            {
                ulong clientId = NetworkManager.Singleton.ConnectedClients[OwnerClientId].ClientId;
                networkObject.ChangeOwnership(clientId);
            }
            hasSpawnedCharacters = true;
            yield return new WaitForSeconds(2f);
            character.GetComponent<CharacterNetworked>().InitCharacterClientRpc(id, index);
        }

        else if (NetworkManager.Singleton.IsClient && IsOwner)
        {
            RequestSpawnCharacterServerRpc(index);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnCharacterServerRpc(int index, ServerRpcParams rpcParams = default)
    {
        StartCoroutine(SpawnBody(index));
    }

    [ServerRpc(RequireOwnership = false)]
    public void CalculateDamageServerRpc(int damageAmount, ulong uid)
    {
        if (body.GetComponent<CharacterNetworked>().networkObject.NetworkObjectId != uid)
        {
            return;
        }

        int _damage = damageAmount - characterObj.Defence;
        UpdateHealth(_damage, uid);
    }

    public void UpdateHealth(int health, ulong uid)
    {
        if (IsOwner)
        {
            Health.Value -= health;
            SetHealth(Health.Value);
            if (Health.Value <= 0)
            {
                DieServerRpc(uid);
            }

            else
            {
                body.GetComponent<CharacterNetworked>().HealthVFXClientRpc(uid);
            }
        }
    }

    [ServerRpc]
    public void DieServerRpc(ulong uid, ServerRpcParams rpcParams = default)
    {
        if (body.GetComponent<CharacterNetworked>().networkObject.NetworkObjectId != uid)
        {
            return;
        }
        ulong puid;
        int index = 1;
        body.GetComponent<CharacterNetworked>().DieVFXClientRpc(uid);
        puid = body.GetComponent<CharacterNetworked>().playerController.networkObject.NetworkObjectId;
        this.blockID = BlockID.none;
        for (int i = 0; i < body.GetComponent<CharacterNetworked>().playerController.characters.Count; i++)
        {
            if (body.GetComponent<CharacterNetworked>().playerController.characters[i] == this)
            {
                index = i;
            }
        }
        CharacterNetworked c = body.GetComponent<CharacterNetworked>();
        NotifyClientsOfCharacterDestructionClientRpc(puid, index);
        foreach (var player in BoardManager.instance.players)
        {
            if (player.networkObject.IsOwner)
            {
                this.blockID = BlockID.none;
                player.characters.Remove(this);
                player.CheckGameEnd();
            }
        }
        c.networkObject.Despawn();
        Invoke(nameof(Destroy), 0.5f);
    }

    [ClientRpc]
    private void NotifyClientsOfCharacterDestructionClientRpc(ulong puid, int index)
    {
        if (puid == myPlayerController.networkObject.NetworkObjectId)
        {
            if (myPlayerController.characters.Count > index)
            {
                myPlayerController.characters.RemoveAt(index);
                Invoke(nameof(Destroy), 0.5f);
            }
        }
    }

    private void Destroy()
    {
        GameObject.Destroy(this.gameObject);
    }
}
