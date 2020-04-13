using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Zoo {
    
public class GameController : MonoBehaviour
{
    // These are containing our game data
    // -- They are arrays since we are ought to have
    // -- multiple instances of each.
    public Board[] _boards;
    public Player[] _players;

    public bool game_over;


    #region GameController Start & update
    void Awake()
    {
        game_over = false;

    }

    void Start()
    {

    }

    void Update()
    {
        //if( game_over ) GameOver();
    }
    #endregion 

    #region Boards and Players Initialization
    public bool InitBoards(int Rows, int Columns, int Mines)
    {
        // Init the game data arrays
        _boards = new Board[3];

        for ( int i = 0; i < _boards.Length; ++i)
        {
            _boards[i] = new Board(Rows, Columns, Mines, i);
        }

        return true;
    }

    // Initialize n players, h of them being human players
    public bool InitPlayers(int n, int h)
    {
        _players = new Player[2];
        for ( int i = 0; i < h; ++i) { _players[i] = new Player(); }
        for (int j = h; j < n; ++j) { _players[j] = new Player(); }

        return true;
    }
    #endregion

    #region Methods
    public void GameOver() 
    {
        game_over = true;
        for(int b = 0; b < _boards.Length; ++b)
        {
            Destroy(_boards[b]._parent);
            _boards[b] = null;
        }
        for( int p = 0; p < _players.Length; ++p)
        {
            _players[p] = null;
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
        if(all)
        {
            for( int i = 0; i < _boards.Length; ++i)
            {
                _boards[i].SetDimensions(SetOrigin(i));
            }
        }
        else { _boards[ID].SetDimensions(SetOrigin(ID)); }
    }
    public void ClickTile(int ID, Vector2 tile_position)
    {
        _boards[ID].ClickTile(tile_position);
    }
    public void SetFlag(int ID, Vector2 tile_position)
    {
        _boards[ID].SetFlag(tile_position);
    }
    #endregion

}

}