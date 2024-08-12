using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
//    [Header("Character UI Elements")]
//    public TMP_Text Name;
//    public Image Icon;
//    public TMP_Text Health;
//    public TMP_Text Defence;
//    public TMP_Text Attack;
//    public Slider HealthSlider;
//    public Slider DefenceSlider;
//    public Slider AttackSlider;

//    public int SpawnIndex;
//    private Transform SpawnPoint;
//    public CharacterObject characterProps;
//    public PhotonView pv;
//    public CharacterResource resource;
//    public GameObject body;
//    private int ID;
//    public BlockID blockID;


//    private void Awake()
//    {
//        if(resource == null)
//        {
//            var _res = GameObject.FindGameObjectsWithTag("CharacterResource")[0].GetComponent<CharacterResource>();
//            resource = _res;
//        }
//        pv = GetComponent<PhotonView>();
//        SpawnPoint = GameObject.FindGameObjectWithTag("CharacterSpawn").GetComponent<Transform>();


//    }
//    public void Init(int ID, int index)
//    {
//        if(pv == null)
//            pv = GetComponent<PhotonView>();
//        this.ID = ID;
//        characterProps = resource.FindCharacterWithID(ID).ShallowCopy();

//        Name.text = characterProps.Name;
//        Icon.sprite = characterProps.Character_Sprite;

//        SetHealth(characterProps.Health);
//        SetAttack(characterProps.Attack_Power);
//        SetDefence(characterProps.Defence);
//        SpawnBody(index);
//    }
//    public PhotonView _pv;
//    private void SpawnBody(int index)
//    {
//        if (!pv.IsMine)
//            return;
//        GameObject character;
//        character = PhotonNetwork.Instantiate("Character", SpawnPoint.position, Quaternion.identity,0);
//        _pv = character.GetComponent<PhotonView>();
//        _pv.RPC("InitCharacter", RpcTarget.All, ID, SpawnIndex, index);

//    }

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
//    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//    {
//        if (characterProps != null)
//        {
//            if (stream.IsWriting)
//            {
//                stream.SendNext(characterProps.Health);
//                stream.SendNext(characterProps.Attack_Power);
//                stream.SendNext(characterProps.Defence);
//            }
//            if (stream.IsReading)
//            {
//                SetHealth((int)stream.ReceiveNext());
//                SetAttack((int)stream.ReceiveNext());
//                SetDefence((int)stream.ReceiveNext());
//            }
//        }
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
//    public void SetHealth(int health)
//    {
//        characterProps.Health = health;
//        Health.text = health.ToString();
//        HealthSlider.DOValue(health, 1, true);

//    }
//    public void SetAttack(int attack)
//    {
//        Attack.text = attack.ToString();
//        AttackSlider.value = attack;
//    }
//    public void SetDefence(int defence)
//    {
//        Defence.text = defence.ToString();
//        DefenceSlider.DOValue(defence, 1, true);

//    }
}
