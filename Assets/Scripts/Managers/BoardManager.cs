using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class BoardManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform playerContent;
    //    public PhotonView pv;
    //    public GameObject EndTurn;

    [Header("Board Rows")]
    public List<GameObject> R1;
    public List<GameObject> R2;
    public List<GameObject> R3;
    public List<GameObject> R4;
    public List<GameObject> R5;
    public List<GameObject> R6;
    public List<GameObject> R7;
    public List<GameObject> R8;
        public List<PlayerController> players;
        public List<NetworkClient> clients;
    //    public List<CubeSyncer> ActiveSyncers;
    //    public GameObject InGameUI;
    //    public List<GameObject> GameMoves;
    //    public GameObject winUI;
    //    public TMPro.TMP_Text status;

        [SerializeField] public List<List<GameObject>> Board;
    //    private List<CharacterController> ABILITY_HISTORY;
    //    public bool RESET_ABILITY;
    //    private CharacterController ABILITY_CREATURE;

    public static BoardManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        players = new List<PlayerController>();
        // ActiveSyncers = new List<CubeSyncer>();
    }

    private void Start()
    {
        Board = new List<List<GameObject>>();
        // ABILITY_HISTORY = new List<CharacterController>();
        //if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
        //{
        //    GameObject player = Instantiate(PlayerPrefab, PlayerContent.position, Quaternion.identity);
        //    player.GetComponent<NetworkObject>().Spawn();
        //}
        PopulateBoard();
    }

    //    public void ResetAbilityData()
    //    {
    //        foreach(var c in ABILITY_HISTORY)
    //        {
    //            c.characterProps.Defence -= ABILITY_CREATURE.characterProps.Power;
    //            c.SetDefence(c.characterProps.Defence);
    //        }

    //        ABILITY_HISTORY = new List<CharacterController>();
    //        ABILITY_CREATURE = null;
    //        RESET_ABILITY = false;
    //    }

    //    [PunRPC]
    //    public void EndGame(Player p)
    //    {
    //        if (p.IsLocal)
    //            status.text = "You Loose";
    //        else
    //            status.text = "You Win";
    //        winUI.SetActive(true);
    //    }
    private void PopulateBoard()
    {
        Board.Add(R1);
        Board.Add(R2);
        Board.Add(R3);
        Board.Add(R4);
        Board.Add(R5);
        Board.Add(R6);
        Board.Add(R7);
        Board.Add(R8);
    }

    //    public void SetInGameUI(Vector2 pos, UnityAction move, UnityAction attack, UnityAction special, bool attackUsed, bool moveUsed, CharacterObject props, bool LIMIT)
    //    {
    //        InGameUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, pos.y + 100);


    //    // Moves Button
    //        GameMoves[0].GetComponent<Button>().interactable = !moveUsed;
    //        GameMoves[0].GetComponent<Button>().onClick.RemoveAllListeners();
    //        GameMoves[0].GetComponent<Button>().onClick.AddListener(move);

    //    // Attack Button
    //        GameMoves[1].GetComponent<Button>().interactable = !attackUsed && props.Attack_Power > 0;
    //        GameMoves[1].GetComponent<Button>().onClick.RemoveAllListeners();
    //        GameMoves[1].GetComponent<Button>().onClick.AddListener(attack);

    //    // Special Button
    //        GameMoves[2].GetComponent<Button>().interactable = !attackUsed && props.HasSpecial && !LIMIT;
    //        GameMoves[2].GetComponent<Button>().onClick.RemoveAllListeners();
    //        GameMoves[2].GetComponent<Button>().onClick.AddListener(special);

    //        InGameUI.SetActive(true);
    //        CancelInvoke();
    //        Invoke(nameof(DisableHud),2.5f);
    //    }

    //    private void Update()
    //    {
    //        EndTurn.GetComponent<Button>().interactable = EndTurn.GetComponent<PhotonView>().IsMine;
    //    }
    //    void DisableHud()
    //    {
    //        InGameUI.SetActive(false);
    //    }
    //    public GameObject GetBlockWithID(BlockID ID)
    //    {
    //        foreach (var row in Board)
    //        {
    //            foreach (var col in row)
    //            {
    //                if (col.GetComponent<CubeSyncer>().ID == ID)
    //                    return col;
    //            }
    //        }
    //        return null;
    //    }
    //    public Vector2 FindLocation(BlockID ID)
    //    {
    //        for (int row = 0; row < Board.Count; row++)
    //        {
    //            for (int col = 0; col < Board[row].Count; col++)
    //            {
    //                if (Board[row][col].GetComponent<CubeSyncer>().ID == ID)
    //                    return new Vector2(row, col);
    //            }
    //        }
    //        return Vector2.zero;
    //    }
    //    private void ActivateSyncer(CubeSyncer syncer, CharacterNetworked character, Player p, ActionType action, CharacterController othercharacter = null)
    //    {
    //        ActiveSyncers.Add(syncer);

    //        switch (action) {

    //            case ActionType.Move:
    //                syncer.action = () => { character.Move(syncer.ID); ResetActiveSyncers(); character.MoveUsed = true; };
    //                syncer.SetGolden(true, p);
    //                break;
    //            case ActionType.Attack:
    //                syncer.action = () => { othercharacter.pv.RPC("CalculateDamage", RpcTarget.All, character.character_controller.characterProps.Attack_Power,false); ResetActiveSyncers(); character.AttackUsed = true; };
    //                syncer.SetRed(true, p);
    //                break;
    //        }
    //}

    //    public void CalculateMove(Vector2 location, CharacterNetworked character, Player p, ActionType action)
    //    {
    //        int _row = (int)location.x;
    //        int _col = (int)location.y;
    //        int DEPTH = character.character_controller.characterProps.Movement_Range;
    //        ResetActiveSyncers();

    //        for (int i = 1; i <= DEPTH; i++)
    //        {
    //            switch(character.character_controller.characterProps.Movement)
    //                {
    //                    case CharacterObject.MovementType.Plus:

    //                    int forward = _row + i;
    //                    int back = _row - i;
    //                    int left = _col - i;
    //                    int right = _col + i;

    //                    //forward
    //                    if (forward < 8)
    //                    {
    //                        var syncer = Board[_row + i][_col].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerMoveBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action);
    //                    }
    //                    //backward
    //                    if (back >= 0)
    //                    {
    //                        var syncer = Board[_row - i][_col].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerMoveBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action);
    //                    }
    //                    //left
    //                    if (left >= 0)
    //                    {
    //                        var syncer = Board[_row][_col - i].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerMoveBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action);
    //                    }
    //                    //right
    //                    if (right < 8)
    //                    {
    //                        var syncer = Board[_row][_col + i].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerMoveBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action);
    //                    }
    //                    break;

    //                case CharacterObject.MovementType.Omni_Directional:
    //                    Vector2 fr = new Vector2((_row + i), (_col + DEPTH));
    //                    Vector2 fl = new Vector2((_row + i), (_col - DEPTH));
    //                    Vector2 dr = new Vector2((_row - i), (_col + DEPTH));
    //                    Vector2 dl = new Vector2((_row - i), (_col - DEPTH));
    //                    int l = _col - i;
    //                    int r = _col + i;


    //                    //left
    //                    if (l >= 0)
    //                    {
    //                        var syncer = Board[_row][l].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerMoveBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action);
    //                    }
    //                    //right
    //                    if (r < 8)
    //                    {
    //                        var syncer = Board[_row][r].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerMoveBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action);
    //                    }

    //                    // forward right
    //                    if (fr.x < 8 && fr.y < 8)
    //                    {
    //                        for (int j = _col; j <= fr.y; j++)
    //                        {
    //                            var syncer = Board[(int)fr.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerMoveBlockValid(syncer.ID))
    //                                ActivateSyncer(syncer, character, p, action);
    //                        }
    //                    }
    //                    // forward left
    //                    if (fl.x < 8 && fl.y >= 0)
    //                    {
    //                        for (int j = _col - 1; j >= fl.y; j--)
    //                        {
    //                            var syncer = Board[(int)fl.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerMoveBlockValid(syncer.ID))
    //                                ActivateSyncer(syncer, character, p, action);
    //                        }
    //                    }
    //                    // back right
    //                    if (dr.x >= 0 && dr.y < 8)
    //                    {
    //                        for (int j = _col; j <= dr.y; j++)
    //                        {
    //                            var syncer = Board[(int)dr.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerMoveBlockValid(syncer.ID))
    //                                ActivateSyncer(syncer, character, p, action);
    //                        }
    //                    }
    //                    // down left
    //                    if (dl.x >= 0 && dl.y >= 0)
    //                    {
    //                        for (int j = _col - 1; j >= dl.y; j--)
    //                        {
    //                            var syncer = Board[(int)dl.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerMoveBlockValid(syncer.ID))
    //                                ActivateSyncer(syncer, character, p, action);
    //                        }
    //                    }
    //                    break;

    //                case CharacterObject.MovementType.Diagnole:
    //                    Vector2 ur = new Vector2((_row + i), (_col + i));
    //                    Vector2 ul = new Vector2((_row + i), (_col - i));
    //                    Vector2 br = new Vector2((_row - i), (_col + i));
    //                    Vector2 bl = new Vector2((_row - i), (_col - i));

    //                    // Upper right
    //                    if (ur.x < 8 && ur.y < 8)
    //                    {
    //                        var syncer = Board[(int)ur.x][(int)ur.y].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerMoveBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action);
    //                    }

    //                    // Upper left
    //                    if (ul.x < 8 && ul.y >= 0)
    //                    {
    //                        var syncer = Board[(int)ul.x][(int)ul.y].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerMoveBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action);
    //                    }

    //                    // Down right
    //                    if (br.x >= 0 && br.y < 8)
    //                    {
    //                        var syncer = Board[(int)br.x][(int)br.y].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerMoveBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action);
    //                    }

    //                    // Down left
    //                    if (bl.x >= 0 && bl.y >= 0)
    //                    {
    //                        var syncer = Board[(int)bl.x][(int)bl.y].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerMoveBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action);
    //                    }

    //                    break;
    //            }

    //        }


    //    }
    //    public void CalculateAttack(Vector2 location, CharacterNetworked character, Player p, ActionType action)
    //    {
    //        int _row = (int)location.x;
    //        int _col = (int)location.y;
    //        int DEPTH = character.character_controller.characterProps.Attack_Range;
    //        ResetActiveSyncers();

    //        for (int i = 1; i <= DEPTH; i++)
    //        {
    //            switch (character.character_controller.characterProps.Attack)
    //            {
    //                case CharacterObject.AttackType.Omni_Directional:
    //                    Vector2 fr = new Vector2((_row + i), (_col + DEPTH));
    //                    Vector2 fl = new Vector2((_row + i), (_col - DEPTH));
    //                    Vector2 dr = new Vector2((_row - i), (_col + DEPTH));
    //                    Vector2 dl = new Vector2((_row - i), (_col - DEPTH));
    //                    int l = _col - i;
    //                    int r = _col + i;
    //                    int f = _row + 1; 
    //                    int b = _row - 1;

    //                    // forward
    //                    if (f < 8)
    //                    {
    //                        var syncer = Board[f][_col].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerAttackBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action, ReturnAttackedPlayer(syncer.ID));
    //                    }

    //                    // back 
    //                    if (b >= 0)
    //                    {
    //                        var syncer = Board[b][_col].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerAttackBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action, ReturnAttackedPlayer(syncer.ID));
    //                    }

    //                    //left
    //                    if (l >= 0)
    //                    {
    //                        var syncer = Board[_row][l].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerAttackBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action, ReturnAttackedPlayer(syncer.ID));

    //                    }
    //                    //right
    //                    if (r < 8)
    //                    {
    //                        var syncer = Board[_row][r].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerAttackBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action, ReturnAttackedPlayer(syncer.ID));
    //                    }

    //                    // forward right
    //                    if (fr.x < 8 && fr.y < 8)
    //                    {
    //                        for (int j = _col; j <= fr.y; j++)
    //                        {
    //                            var syncer = Board[(int)fr.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerAttackBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action, ReturnAttackedPlayer(syncer.ID));
    //                        }
    //                    }
    //                    // forward left
    //                    if (fl.x < 8 && fl.y >= 0)
    //                    {
    //                        for (int j = _col - 1; j >= fl.y; j--)
    //                        {
    //                            var syncer = Board[(int)fl.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerAttackBlockValid(syncer.ID))
    //                            ActivateSyncer(syncer, character, p, action, ReturnAttackedPlayer(syncer.ID));

    //                        }
    //                    }
    //                    // back right
    //                    if (dr.x >= 0 && dr.y < 8)
    //                    {
    //                        for (int j = _col; j <= dr.y; j++)
    //                        {
    //                            var syncer = Board[(int)dr.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerAttackBlockValid(syncer.ID))
    //                                ActivateSyncer(syncer, character, p, action,ReturnAttackedPlayer(syncer.ID));
    //                        }
    //                    }
    //                    // down left
    //                    if (dl.x >= 0 && dl.y >= 0)
    //                    {
    //                        for (int j = _col - 1; j >= dl.y; j--)
    //                        {
    //                            var syncer = Board[(int)dl.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerAttackBlockValid(syncer.ID))
    //                                ActivateSyncer(syncer, character, p, action, ReturnAttackedPlayer(syncer.ID));
    //                        }
    //                    }
    //                    break;
    //                case CharacterObject.AttackType.None:
    //                    return;

    //            }

    //        }


    //    }
    //    public void CalculateAbiltiy(Vector2 location, CharacterNetworked character, Player p, ActionType action)
    //    {
    //        int _row = (int)location.x;
    //        int _col = (int)location.y;
    //        int DEPTH = character.character_controller.characterProps.Power_Range;
    //        ResetActiveSyncers();

    //        switch(character.character_controller.characterProps.Abillity)
    //        {
    //            case CharacterObject.AbillityType.Heal:
    //                if (character.character_controller.characterProps.Health >= character.character_controller.characterProps.Power_Range)
    //                    return;
    //                else
    //                {
    //                    int _health = character.character_controller.characterProps.Health + character.character_controller.characterProps.Power;

    //                    if (_health >= character.character_controller.characterProps.Power_Range)
    //                        _health = character.character_controller.characterProps.Power_Range;

    //                    character.character_controller.characterProps.Health = _health;
    //                    character.character_controller.SetHealth(character.character_controller.characterProps.Health);
    //                    character.pv.RPC("AbilityVFX", RpcTarget.All);
    //                    character.AttackUsed = true;
    //                }
    //                break;

    //            case CharacterObject.AbillityType.Increase_Defence:
    //                ABILITY_CREATURE = character.character_controller;
    //                RESET_ABILITY = true;
    //                for (int i = 1; i <= DEPTH; i++)
    //                {
    //                    Vector2 fr = new Vector2((_row + i), (_col + DEPTH));
    //                    Vector2 fl = new Vector2((_row + i), (_col - DEPTH));
    //                    Vector2 dr = new Vector2((_row - i), (_col + DEPTH));
    //                    Vector2 dl = new Vector2((_row - i), (_col - DEPTH));
    //                    int l = _col - i;
    //                    int r = _col + i;
    //                    int f = _row + i;
    //                    int b = _row - i;
    //                    //left
    //                    if (l >= 0)
    //                    {
    //                        var syncer = Board[_row][l].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerSpecialBlockValid(syncer.ID))
    //                            IncreaseDefence(syncer.ID, character);

    //                    }
    //                    //right
    //                    if (r < 8)
    //                    {
    //                        var syncer = Board[_row][r].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerSpecialBlockValid(syncer.ID))
    //                            IncreaseDefence(syncer.ID, character);
    //                    }
    //                    //up
    //                    if (f < 8)
    //                    {
    //                        var syncer = Board[f][_col].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerSpecialBlockValid(syncer.ID))
    //                            IncreaseDefence(syncer.ID, character);

    //                    }
    //                    // back
    //                    if (b >= 0)
    //                    {
    //                        var syncer = Board[b][_col].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerSpecialBlockValid(syncer.ID))
    //                            IncreaseDefence(syncer.ID, character);

    //                    }
    //                    // forward right
    //                    if (fr.x < 8 && fr.y < 8)
    //                    {
    //                        for (int j = _col; j <= fr.y; j++)
    //                        {
    //                            var syncer = Board[(int)fr.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerSpecialBlockValid(syncer.ID))
    //                                IncreaseDefence(syncer.ID, character);
    //                        }
    //                    }
    //                    // forward left
    //                    if (fl.x < 8 && fl.y >= 0)
    //                    {
    //                        for (int j = _col - 1; j >= fl.y; j--)
    //                        {
    //                            var syncer = Board[(int)fl.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerSpecialBlockValid(syncer.ID))
    //                                IncreaseDefence(syncer.ID, character);

    //                        }
    //                    }
    //                    // back right
    //                    if (dr.x >= 0 && dr.y < 8)
    //                    {
    //                        for (int j = _col; j <= dr.y; j++)
    //                        {
    //                            var syncer = Board[(int)dr.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerSpecialBlockValid(syncer.ID))
    //                                IncreaseDefence(syncer.ID, character);
    //                        }
    //                    }
    //                    // down left
    //                    if (dl.x >= 0 && dl.y >= 0)
    //                    {
    //                        for (int j = _col - 1; j >= dl.y; j--)
    //                        {
    //                            var syncer = Board[(int)dl.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerSpecialBlockValid(syncer.ID))
    //                                IncreaseDefence(syncer.ID, character);
    //                        }
    //                    }


    //                }
    //                break;

    //            case CharacterObject.AbillityType.True_Damage:
    //                for (int i = 1; i <= DEPTH; i++)
    //                {
    //                    Vector2 fr = new Vector2((_row + i), (_col + DEPTH));
    //                    Vector2 fl = new Vector2((_row + i), (_col - DEPTH));
    //                    Vector2 dr = new Vector2((_row - i), (_col + DEPTH));
    //                    Vector2 dl = new Vector2((_row - i), (_col - DEPTH));
    //                    int l = _col - i;
    //                    int r = _col + i;
    //                    //left
    //                    if (l >= 0)
    //                    {
    //                        var syncer = Board[_row][l].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerAttackBlockValid(syncer.ID))
    //                            TrueDamage(syncer.ID, character);

    //                    }
    //                    //right
    //                    if (r < 8)
    //                    {
    //                        var syncer = Board[_row][r].GetComponent<CubeSyncer>();
    //                        if (CheckPlayerAttackBlockValid(syncer.ID))
    //                            TrueDamage(syncer.ID, character);
    //                    }

    //                    // forward right
    //                    if (fr.x < 8 && fr.y < 8)
    //                    {
    //                        for (int j = _col; j <= fr.y; j++)
    //                        {
    //                            var syncer = Board[(int)fr.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerAttackBlockValid(syncer.ID))
    //                                TrueDamage(syncer.ID, character);
    //                        }
    //                    }
    //                    // forward left
    //                    if (fl.x < 8 && fl.y >= 0)
    //                    {
    //                        for (int j = _col - 1; j >= fl.y; j--)
    //                        {
    //                            var syncer = Board[(int)fl.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerAttackBlockValid(syncer.ID))
    //                                TrueDamage(syncer.ID, character);

    //                        }
    //                    }
    //                    // back right
    //                    if (dr.x >= 0 && dr.y < 8)
    //                    {
    //                        for (int j = _col; j <= dr.y; j++)
    //                        {
    //                            var syncer = Board[(int)dr.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerAttackBlockValid(syncer.ID))
    //                                TrueDamage(syncer.ID, character);
    //                        }
    //                    }
    //                    // down left
    //                    if (dl.x >= 0 && dl.y >= 0)
    //                    {
    //                        for (int j = _col - 1; j >= dl.y; j--)
    //                        {
    //                            var syncer = Board[(int)dl.x][j].GetComponent<CubeSyncer>();
    //                            if (CheckPlayerAttackBlockValid(syncer.ID))
    //                                TrueDamage(syncer.ID, character);
    //                        }
    //                    }
    //                }
    //                break;
    //            case CharacterObject.AbillityType.GainXP:
    //                foreach (var player in players)
    //                    if(player.pv.IsMine)
    //                    {
    //                        player.XP += character.character_controller.characterProps.Power;
    //                        player.UpdateXp(player.XP);
    //                        character.AttackUsed = true;
    //                    }
    //                    break;
    //        }
    //    }

    //    private void IncreaseDefence(BlockID ID, CharacterNetworked character)
    //    {
    //        var _temp = ReturnSpecialPlayer(ID);
    //        ABILITY_HISTORY.Add(_temp);
    //        _temp.characterProps.Defence += character.character_controller.characterProps.Power;
    //        _temp.SetDefence(_temp.characterProps.Defence);
    //        _temp.body.GetComponent<CharacterNetworked>().pv.RPC("AbilityVFX", RpcTarget.All);
    //        character.AttackUsed = true;
    //       // character.LIMIT_SPECIAL = true;
    //    }

    //    private void TrueDamage(BlockID ID, CharacterNetworked character)
    //    {
    //        var _temp = ReturnAttackedPlayer(ID);
    //        _temp.pv.RPC("CalculateDamage", RpcTarget.All,character.character_controller.characterProps.Power ,true);
    //        character.AttackUsed = true;
    //    }

    //    public void ResetActiveSyncers()
    //    {
    //        InGameUI.SetActive(false);
    //        for (int x = ActiveSyncers.Count - 1; x > -1; x--)
    //        {
    //            ActiveSyncers[x].SetGolden(false);
    //            ActiveSyncers[x].SetRed(false);
    //            ActiveSyncers.RemoveAt(x);
    //        }
    //    }

    //    public enum ActionType
    //    {
    //        Move = 1,
    //        Attack = 2,
    //        Special = 3

    //    }

    //    private bool CheckPlayerMoveBlockValid(BlockID ID)
    //    {
    //        foreach(var p in players)
    //            foreach(var c in p.characters)
    //                if(c.blockID == ID)
    //                    return false;
    //        return true;
    //    }
    //    private bool CheckPlayerAttackBlockValid(BlockID ID)
    //    {
    //        foreach (var p in players)
    //            foreach (var c in p.characters)
    //                if (c.blockID == ID && !c.pv.IsMine)
    //                    return true;
    //        return false;
    //    }
    //    private bool CheckPlayerSpecialBlockValid(BlockID ID)
    //    {
    //        foreach (var p in players)
    //            foreach (var c in p.characters)
    //                if (c.blockID == ID && c.pv.IsMine)
    //                    return true;
    //        return false;
    //    }

    //    private CharacterController ReturnSpecialPlayer(BlockID ID)
    //    {
    //        foreach (var p in players)
    //            foreach (var c in p.characters)
    //                if (c.blockID == ID)
    //                    return c;
    //        return null;
    //    }
    //    private CharacterController ReturnAttackedPlayer(BlockID ID)
    //    {
    //        foreach (var p in players)
    //            foreach (var c in p.characters)
    //                if (c.blockID == ID && !c.pv.IsMine)
    //                    return c;
    //        return null;
    //    }

    //    public void ResetPlayerMovesData()
    //    {
    //        foreach (var p in players)
    //            foreach (var c in p.characters)
    //                if(c.pv.IsMine)
    //                {
    //                    c.body.GetComponent<CharacterNetworked>().AttackUsed = false;
    //                    c.body.GetComponent<CharacterNetworked>().MoveUsed = false;
    //                }
    //    }
    //    public void RewardPlayerKillXP(int _xp)
    //    {
    //        if (!EndTurn.GetComponent<PhotonView>().IsMine)
    //            return;

    //        foreach (var p in players)
    //            if (p.pv.IsMine)
    //                p.pv.RPC("RewardPlayerXP", RpcTarget.All, _xp);
    //    }
    //    public void OnHome()
    //    {
    //        PhotonNetwork.LeaveRoom(this);
    //        SceneManager.LoadScene("Home");
    //    }
}
