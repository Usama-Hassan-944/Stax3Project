using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using sy.Data;
using Unity.Netcode;

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
    public CharacterObject characterProps;
    public CharacterResource resource;
    public GameObject body;
    private int id;
    public BlockID blockID;
    public PlayerController myPlayerController;
    public bool hasSpawnedCharacters = false;

    [Header("Networked")]
    public NetworkVariable<int> Health = new NetworkVariable<int>();
    public NetworkVariable<int> AttackPower = new NetworkVariable<int>();
    public NetworkVariable<int> Defence = new NetworkVariable<int>();


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

        this.myPlayerController = playerController;
        this.id = id;
        characterProps = resource.FindCharacterWithID(id);

        name.text = characterProps.Name;
        icon.sprite = characterProps.Character_Sprite;

        Health.Value = characterProps.Health;
        AttackPower.Value = characterProps.Attack_Power;
        Defence.Value = characterProps.Defence;
        StartCoroutine(SpawnBody(index));
    }

    public IEnumerator SpawnBody(int index)
    {
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

        // _pv.RPC("InitCharacter", RpcTarget.All, ID, SpawnIndex, index);

    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnCharacterServerRpc(int index, ServerRpcParams rpcParams = default)
    {
        StartCoroutine(SpawnBody(index));
    }

    //    [PunRPC]
    //    public void CalculateDamage(int DamageAmmont, bool TrueDamage)
    //    {
    //        int _damage;
    //        if (!TrueDamage)
    //        {
    //            body.GetComponent<CharacterNetworked>().pv.RPC("HealthVFX", RpcTarget.All);
    //            _damage = DamageAmmont - characterProps.Defence;
    //        }
    //        else
    //        {
    //            body.GetComponent<CharacterNetworked>().pv.RPC("AbilityVFX", RpcTarget.All);
    //            _damage = DamageAmmont;
    //        }
    //            this.UpdateHealth(_damage);
    //    }

    //    [PunRPC]
    //    public void Die()
    //    {
    //        body.GetComponent<CharacterNetworked>().pv.RPC("Die", RpcTarget.All);

    //            foreach (var player in BoardManager.Instance.players)       
    //                if (player.pv.IsMine)
    //                {
    //                    this.blockID = BlockID.none;
    //                    player.characters.Remove(this);
    //                    player.CheckGameEnd();
    //                }

    //        Invoke(nameof(Destroy),0.5f);
    //    }
    //    private void Destroy()
    //    {
    //        GameObject.Destroy(this.gameObject);  
    //    }

    //    public void UpdateHealth(int health)
    //    {
    //        characterProps.Health -= health;

    //        if (characterProps.Health > 0)
    //        {
    //            Health.text = characterProps.Health.ToString();
    //            HealthSlider.DOValue(characterProps.Health, 1, true);
    //        }
    //        else
    //        {
    //            BoardManager.Instance.RewardPlayerKillXP((int)characterProps.DeadXP);
    //            pv.RPC("Die", RpcTarget.All);
    //        }
    //    }

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
}
