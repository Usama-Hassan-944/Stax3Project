using sy.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class BoardManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject endTurnButton;

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
    public List<CubeSyncer> activeSyncers;
    public GameObject inGameUI;
    public List<GameObject> gameMoves;
    public GameObject winUI;
    public TextMeshProUGUI statusText;

    [SerializeField] public List<List<GameObject>> Board;
    public NetworkObject networkObject;
    public static BoardManager instance;

    public enum ActionType
    {
        Move = 1,
        Attack = 2,
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Board = new List<List<GameObject>>();
        players = new List<PlayerController>();
        activeSyncers = new List<CubeSyncer>();
        PopulateBoard();
        if (networkObject == null)
        {
            networkObject = GetComponent<NetworkObject>();
        }
    }

    private void Update()
    {
        if (endTurnButton)
        {
            if (endTurnButton.GetComponent<NetworkObject>())
            {
                endTurnButton.GetComponent<Button>().interactable = endTurnButton.GetComponent<NetworkObject>().IsOwner;
            }
        }
    }

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

    public void SetInGameUI(Vector2 pos, UnityAction move, UnityAction attack, bool attackUsed, bool moveUsed, CharacterObject characterObj)
    {
        inGameUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, pos.y + 100);

        // Move Button
        gameMoves[0].GetComponent<Button>().interactable = !moveUsed;
        gameMoves[0].GetComponent<Button>().onClick.RemoveAllListeners();
        gameMoves[0].GetComponent<Button>().onClick.AddListener(move);

        // Attack Button
        gameMoves[1].GetComponent<Button>().interactable = !attackUsed && characterObj.Attack_Power > 0;
        gameMoves[1].GetComponent<Button>().onClick.RemoveAllListeners();
        gameMoves[1].GetComponent<Button>().onClick.AddListener(attack);

        inGameUI.SetActive(true);
        CancelInvoke();
        Invoke(nameof(DisableHud), 2.5f);
    }

    void DisableHud()
    {
        inGameUI.SetActive(false);
    }

    public void ResetPlayerMovesData()
    {
        foreach (var p in players)
        {
            foreach (var c in p.characters)
            {
                if (c != null && c.body != null)
                {
                    if (c.body.GetComponent<CharacterNetworked>().networkObject.IsOwner)
                    {
                        c.body.GetComponent<CharacterNetworked>().isAttackUsed = false;
                        c.body.GetComponent<CharacterNetworked>().isMoveUsed = false;
                    }
                }
            }
        }
    }

    public GameObject GetBlockWithID(BlockID ID)
    {
        foreach (var row in Board)
        {
            foreach (var col in row)
            {
                if (col.GetComponent<CubeSyncer>().ID == ID)
                {
                    return col;
                }
            }
        }

        return null;
    }

    public Vector2 FindLocation(BlockID ID)
    {
        for (int row = 0; row < Board.Count; row++)
        {
            for (int col = 0; col < Board[row].Count; col++)
            {
                if (Board[row][col].GetComponent<CubeSyncer>().ID == ID)
                {
                    return new Vector2(row, col);
                }
            }
        }

        return Vector2.zero;
    }

    public void CalculateMove(Vector2 location, CharacterNetworked character, ulong id, ActionType action)
    {
        int _row = (int)location.x;
        int _col = (int)location.y;
        int loopTraversals = character.characterController.characterObj.Movement_Range;
        ResetActiveSyncers();

        for (int i = 1; i <= loopTraversals; i++)
        {
            switch (character.characterController.characterObj.Movement)
            {
                case CharacterObject.MovementType.Plus:
                    {
                        int forward = _row + i;
                        int back = _row - i;
                        int left = _col - i;
                        int right = _col + i;

                        //forward
                        if (forward < 8)
                        {
                            var syncer = Board[_row + i][_col].GetComponent<CubeSyncer>();
                            if (CheckPlayerMoveBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action);
                            }
                        }

                        //backward
                        if (back >= 0)
                        {
                            var syncer = Board[_row - i][_col].GetComponent<CubeSyncer>();
                            if (CheckPlayerMoveBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action);
                            }
                        }

                        //left
                        if (left >= 0)
                        {
                            var syncer = Board[_row][_col - i].GetComponent<CubeSyncer>();
                            if (CheckPlayerMoveBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action);
                            }
                        }

                        //right
                        if (right < 8)
                        {
                            var syncer = Board[_row][_col + i].GetComponent<CubeSyncer>();
                            if (CheckPlayerMoveBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action);
                            }
                        }
                        break;
                    }

                case CharacterObject.MovementType.Omni_Directional:
                    {
                        Vector2 fr = new Vector2((_row + i), (_col + loopTraversals));
                        Vector2 fl = new Vector2((_row + i), (_col - loopTraversals));
                        Vector2 dr = new Vector2((_row - i), (_col + loopTraversals));
                        Vector2 dl = new Vector2((_row - i), (_col - loopTraversals));
                        int l = _col - i;
                        int r = _col + i;

                        //left
                        if (l >= 0)
                        {
                            var syncer = Board[_row][l].GetComponent<CubeSyncer>();
                            if (CheckPlayerMoveBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action);
                            }
                        }

                        //right
                        if (r < 8)
                        {
                            var syncer = Board[_row][r].GetComponent<CubeSyncer>();
                            if (CheckPlayerMoveBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action);
                            }
                        }

                        // forward right
                        if (fr.x < 8 && fr.y < 8)
                        {
                            for (int j = _col; j <= fr.y; j++)
                            {
                                var syncer = Board[(int)fr.x][j].GetComponent<CubeSyncer>();
                                if (CheckPlayerMoveBlockValid(syncer.ID))
                                {
                                    ActivateSyncer(syncer, character, id, action);
                                }
                            }
                        }

                        // forward left
                        if (fl.x < 8 && fl.y >= 0)
                        {
                            for (int j = _col - 1; j >= fl.y; j--)
                            {
                                var syncer = Board[(int)fl.x][j].GetComponent<CubeSyncer>();
                                if (CheckPlayerMoveBlockValid(syncer.ID))
                                {
                                    ActivateSyncer(syncer, character, id, action);
                                }
                            }
                        }

                        // back right
                        if (dr.x >= 0 && dr.y < 8)
                        {
                            for (int j = _col; j <= dr.y; j++)
                            {
                                var syncer = Board[(int)dr.x][j].GetComponent<CubeSyncer>();
                                if (CheckPlayerMoveBlockValid(syncer.ID))
                                {
                                    ActivateSyncer(syncer, character, id, action);
                                }
                            }
                        }

                        // down left
                        if (dl.x >= 0 && dl.y >= 0)
                        {
                            for (int j = _col - 1; j >= dl.y; j--)
                            {
                                var syncer = Board[(int)dl.x][j].GetComponent<CubeSyncer>();
                                if (CheckPlayerMoveBlockValid(syncer.ID))
                                {
                                    ActivateSyncer(syncer, character, id, action);
                                }
                            }
                        }

                        break;
                    }

                case CharacterObject.MovementType.Diagnole:
                    {
                        Vector2 ur = new Vector2((_row + i), (_col + i));
                        Vector2 ul = new Vector2((_row + i), (_col - i));
                        Vector2 br = new Vector2((_row - i), (_col + i));
                        Vector2 bl = new Vector2((_row - i), (_col - i));

                        // Upper right
                        if (ur.x < 8 && ur.y < 8)
                        {
                            var syncer = Board[(int)ur.x][(int)ur.y].GetComponent<CubeSyncer>();
                            if (CheckPlayerMoveBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action);
                            }
                        }

                        // Upper left
                        if (ul.x < 8 && ul.y >= 0)
                        {
                            var syncer = Board[(int)ul.x][(int)ul.y].GetComponent<CubeSyncer>();
                            if (CheckPlayerMoveBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action);
                            }
                        }

                        // Down right
                        if (br.x >= 0 && br.y < 8)
                        {
                            var syncer = Board[(int)br.x][(int)br.y].GetComponent<CubeSyncer>();
                            if (CheckPlayerMoveBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action);
                            }
                        }

                        // Down left
                        if (bl.x >= 0 && bl.y >= 0)
                        {
                            var syncer = Board[(int)bl.x][(int)bl.y].GetComponent<CubeSyncer>();
                            if (CheckPlayerMoveBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action);
                            }
                        }

                        break;
                    }
            }
        }
    }

    public void ResetActiveSyncers()
    {
        inGameUI.SetActive(false);
        for (int x = activeSyncers.Count - 1; x > -1; x--)
        {
            activeSyncers[x].SetGolden(false);
            activeSyncers[x].SetRed(false);
            activeSyncers.RemoveAt(x);
        }
    }

    private bool CheckPlayerMoveBlockValid(BlockID ID)
    {
        foreach (var p in players)
        {
            foreach (var c in p.characters)
            {
                if (c.blockID == ID)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void ActivateSyncer(CubeSyncer syncer, CharacterNetworked character, ulong id, ActionType action, CharacterController othercharacter = null)
    {
        activeSyncers.Add(syncer);
        switch (action)
        {
            case ActionType.Move:
                {
                    syncer.action = () =>
                    {
                        character.Move(syncer.ID);
                        ResetActiveSyncers();
                        character.isMoveUsed = true;
                    };
                    syncer.SetGolden(true);
                    break;
                }
            case ActionType.Attack:
                {
                    othercharacter.body.GetComponent<CharacterNetworked>().mySprite.raycastTarget = false;
                    othercharacter.body.GetComponent<CharacterNetworked>().mySprite.maskable = false;
                    syncer.action = () => 
                    {
                        ulong targetUid = othercharacter.body.GetComponent<CharacterNetworked>().networkObject.NetworkObjectId;
                        // othercharacter.CalculateDamage(character.characterController.characterObj.Attack_Power, targetUid);
                        othercharacter.CalculateDamageServerRpc(character.characterController.characterObj.Attack_Power, targetUid);
                        ResetActiveSyncers();
                        character.isAttackUsed = true;
                        othercharacter.body.GetComponent<CharacterNetworked>().mySprite.raycastTarget = true;
                        othercharacter.body.GetComponent<CharacterNetworked>().mySprite.maskable = true;
                    };

                    syncer.SetRed(true);
                    break;
                }
        }
    }

    public void CalculateAttack(Vector2 location, CharacterNetworked character, ulong id, ActionType action)
    {
        int _row = (int)location.x;
        int _col = (int)location.y;
        int loopTraversal = character.characterController.characterObj.Attack_Range;
        ResetActiveSyncers();

        for (int i = 1; i <= loopTraversal; i++)
        {
            switch (character.characterController.characterObj.Attack)
            {
                case CharacterObject.AttackType.Omni_Directional:
                    {
                        Vector2 fr = new Vector2((_row + i), (_col + loopTraversal));
                        Vector2 fl = new Vector2((_row + i), (_col - loopTraversal));
                        Vector2 dr = new Vector2((_row - i), (_col + loopTraversal));
                        Vector2 dl = new Vector2((_row - i), (_col - loopTraversal));
                        int l = _col - i;
                        int r = _col + i;
                        int f = _row + 1;
                        int b = _row - 1;

                        // forward
                        if (f < 8)
                        {
                            var syncer = Board[f][_col].GetComponent<CubeSyncer>();
                            if (CheckPlayerAttackBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action, ReturnAttackedPlayer(syncer.ID));
                            }
                        }

                        // back 
                        if (b >= 0)
                        {
                            var syncer = Board[b][_col].GetComponent<CubeSyncer>();
                            if (CheckPlayerAttackBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action, ReturnAttackedPlayer(syncer.ID));
                            }
                        }

                        //left
                        if (l >= 0)
                        {
                            var syncer = Board[_row][l].GetComponent<CubeSyncer>();
                            if (CheckPlayerAttackBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action, ReturnAttackedPlayer(syncer.ID));
                            }
                        }

                        //right
                        if (r < 8)
                        {
                            var syncer = Board[_row][r].GetComponent<CubeSyncer>();
                            if (CheckPlayerAttackBlockValid(syncer.ID))
                            {
                                ActivateSyncer(syncer, character, id, action, ReturnAttackedPlayer(syncer.ID));
                            }
                        }

                        // forward right
                        if (fr.x < 8 && fr.y < 8)
                        {
                            for (int j = _col; j <= fr.y; j++)
                            {
                                var syncer = Board[(int)fr.x][j].GetComponent<CubeSyncer>();
                                if (CheckPlayerAttackBlockValid(syncer.ID))
                                {
                                    ActivateSyncer(syncer, character, id, action, ReturnAttackedPlayer(syncer.ID));
                                }
                            }
                        }

                        // forward left
                        if (fl.x < 8 && fl.y >= 0)
                        {
                            for (int j = _col - 1; j >= fl.y; j--)
                            {
                                var syncer = Board[(int)fl.x][j].GetComponent<CubeSyncer>();
                                if (CheckPlayerAttackBlockValid(syncer.ID))
                                {
                                    ActivateSyncer(syncer, character, id, action, ReturnAttackedPlayer(syncer.ID));
                                }
                            }
                        }

                        // back right
                        if (dr.x >= 0 && dr.y < 8)
                        {
                            for (int j = _col; j <= dr.y; j++)
                            {
                                var syncer = Board[(int)dr.x][j].GetComponent<CubeSyncer>();
                                if (CheckPlayerAttackBlockValid(syncer.ID))
                                {
                                    ActivateSyncer(syncer, character, id, action, ReturnAttackedPlayer(syncer.ID));
                                }
                            }
                        }

                        // down left
                        if (dl.x >= 0 && dl.y >= 0)
                        {
                            for (int j = _col - 1; j >= dl.y; j--)
                            {
                                var syncer = Board[(int)dl.x][j].GetComponent<CubeSyncer>();
                                if (CheckPlayerAttackBlockValid(syncer.ID))
                                {
                                    ActivateSyncer(syncer, character, id, action, ReturnAttackedPlayer(syncer.ID));
                                }
                            }
                        }

                        break;
                    }

                case CharacterObject.AttackType.None:
                    {
                        return;
                    }
            }
        }
    }

    private bool CheckPlayerAttackBlockValid(BlockID ID)
    {
        foreach (var p in players)
        {
            foreach (var c in p.characters)
            {
                if (c.blockID == ID && !c.body.GetComponent<CharacterNetworked>().networkObject.IsOwner)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private CharacterController ReturnAttackedPlayer(BlockID ID)
    {
        foreach (var p in players)
        {
            foreach (var c in p.characters)
            {
                if (c.blockID == ID && !c.body.GetComponent<CharacterNetworked>().networkObject.IsOwner)
                {
                    return c;
                }
            }
        }

        return null;
    }

    public void SetWinPanel(string msg)
    {
        statusText.text = msg;
        winUI.SetActive(true);
    }
}