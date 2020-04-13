using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Main {
    
public class GameController : MonoBehaviour
{
    // These are containing our game data
    // -- They are arrays since we ought to have
    // -- multiple instances of each.
    public Board[] _boards;
    public Player[] _players;
    public GameObject[] _buttons;

    public PlayMakerFSM _fsm;

    public bool game_over;
    private int incoming_count = 0;


    #region GameController Start & update
    void Awake()
    {
        game_over = false;

    }

    void Start()
    {
        _fsm = GameObject.FindWithTag("GameController").GetComponent<PlayMakerFSM>();
    }

    void Update()
    {
        
    }
    #endregion 

    #region Boards and Players Initialization
    public bool InitButtons() 
    {
        _buttons = new GameObject[3];
        _buttons[0] = GameObject.Find("Button-0");
        _buttons[1] = GameObject.Find("Button-1");
        _buttons[2] = GameObject.Find("Button-2");

        RandomizeButtons();

        return true;
    }
    public bool InitBoards(int Rows, int Columns, int Mines)
    {
        // Init the game data arrays
        _boards = new Board[3];
        _boards[0] = new Board(12, 8, 12, 0, true);
        _boards[1] = new Board(12, 8, 12, 1, false);
        _boards[2] = new Board(12, 8, 12, 2, false);
        return true;
    }

    // Initialize n players, h of them being human players
    public bool InitPlayers(int n, int h)
    {
        _players = new Player[2];

        _players[0] = new Player();
            _players[0].UpdateUI();

        _players[1] = new Monster();
            _players[1].UpdateUI();

        return true;
    }
    #endregion

    #region Methods
    public void GameOver() 
    {
        Debug.Log("Game over");
        for(int i = 0; i < _boards.Length; ++i)
        {
            Destroy(_boards[i]._parent);
        }
        for(int i = 0; i < _players.Length; ++i)
        {
            _players[i] = null;
        }

        
    }
    public Vector3 SetOrigin(int m)
    {
        /*
        Vector3 newOrigin = Vector3.zero;
        GameObject terrain = GameObject.Find("Terrain");
        newOrigin.x = terrain.GetComponent<Collider>().bounds.extents.x;
        newOrigin.z = terrain.GetComponent<Collider>().bounds.extents.z;
        Debug.Log(new Vector3(-newOrigin.x, 0, -newOrigin.z));
        Origin = new Vector3(-newOrigin.x, 0, -newOrigin.z);
        */

        Vector3 Origin = new Vector3(-6.1f, 0, -3.9f);
        if(m!=0) Origin.z = Origin.z - 2.25f*m*Origin.z;
        return Origin;
    }
    public void SetBoardDimensions(int ID, bool all = false)
    {
        for(int i = 0; i < 3; ++i)
        {
            _boards[i].SetDimensions(SetOrigin(i));
        }
    }
    public void ClickTile(int ID, Vector2 tile_position)
    {
        if(_boards[ID].Active)
        {
            _boards[ID].ClickTile(tile_position);
            CheckMineCount(ID);
        }
        
    }
    public void SetFlag(int ID, Vector2 tile_position)
    {
        if(_boards[ID].Active) _boards[ID].SetFlag(tile_position); 
    }
    public void Detonate()
    {
        incoming_count = 0;
        for(int i = 0; i < _boards.Length; ++i)
        {
            if( _boards[i].Active ) 
            {
                incoming_count += _boards[i].Detonate();
                CheckMineCount(i);
            }
        }
        Debug.Log("Incoming: "+incoming_count);
    }

    public void CheckMineCount(int ID) 
    {
        if(_boards[ID].CheckMines()) _boards[ID].SetActive(false);
    }
    #endregion

    public void AddBoard(int mines)
    {
        for(int i = 0; i < _boards.Length; ++i)
        {
            if(!_boards[i].Active) {
                _boards[i].Reset(mines);
                RandomizeButtons();
                return;
            }
        }
    }

    public void Reset(int mines)
    {
        for(int i = 0; i<3; i++)
        {
            _boards[i].Reset(mines);
        }
    }

    public void PlayerTakeDamage(int ID, int amount)
    {
        incoming_count--;
        //Debug.Log("Incoming: "+incoming_count);
        if(_players[ID].TakeDamage(amount) <= 0) 
        {
            if(_players[ID].type == PlayerType.HUMAN) 
            {
                PlayMakerUtils.SendEventToGameObjectFsmByName(null, GameObject.FindWithTag("GameController"), "FSM_GameController", 
                                                        "GAMEOVER",null);
            }
            else {
                if(incoming_count <= 0) {
                    _players[ID] = new Monster();
                    _players[ID].UpdateUI();
                }
            }
        }
    }

    public void RandomizeButtons()
    {
        // 12 - 15 - 18 - 21 - 24
        System.Random rand = new System.Random();
        for(int i = 0; i < _buttons.Length; ++i)
        {
            int value = rand.Next(3);

            PlayMakerFSM _btnFSM = _buttons[i].GetComponent<PlayMakerFSM>();
            FsmInt ButtonValue = _btnFSM.FsmVariables.GetFsmInt("Value");

            switch (value)
            {
                case 0:
                    ButtonValue.Value = 12;
                    break;
                case 1:
                    ButtonValue.Value = 15;
                    break;
                case 2:
                    ButtonValue.Value = 18;
                    break;
                case 3:
                    ButtonValue.Value = 21;
                    break;
                case 4:
                    ButtonValue.Value = 24;
                    break;
                default:
                    ButtonValue.Value = 95;
                    break;
            }

            _btnFSM.SendEvent("UPDATE_VALUE");
        }
    }

    public void RevealBoards()
    {
        for(int i = 0; i < _boards.Length; ++i)
        {
            if(_boards[i].Active) _boards[i].OpenAll();
        }
    }

}

}