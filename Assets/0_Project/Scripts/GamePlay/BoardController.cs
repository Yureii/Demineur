using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

namespace old {

public enum TileStatus { CLOSED, OPEN, FLAGGED }
public enum PlayerType { HUMAN, IA }

public class BoardController : MonoBehaviour
{
    public Board Board;
    public bool FirstClick;
    public Player _player;
    public Player _bot;

    // Start is called before the first frame update
    void Start()
    {
        FirstClick = true;
        
    }

    // Update is called once per frame
    void Update() {}

    public void InitBoard(int r, int c, int m) 
    {
        Debug.Log("Generating Board");
        Board = new Board(r, c, m);
    }

    // ---------------------
    // BOARD METHODS
    public void SetMines(Vector2 pos) { Board.SetMines(pos, FirstClick); }
    public void Open(Vector2 tile_position) { Board.Open(tile_position); }
    public void SetFlag(Vector2 tile_position) { Board.SetFlag(tile_position); }
    public int DetonateMines() { return Board.DetonateMines(); }


    // ---------------------
    // PLAYER METHODS
    public void InitPlayers() { 
        _player = new Player(10, PlayerType.HUMAN); 
        _bot = new Player(5, PlayerType.IA);
    }
    public void PlayerTakeDamage()
    {
        _player.TakeDamage(0);
    }
    public void PlayerDealDamage()
    {
        _bot.TakeDamage(0);
        if( !_bot.Alive ) _bot = new Player(5, PlayerType.IA);
    }

}


/* ########################################
                CLASS BOARD
######################################### */
public class Board {
    public int Mines;
    public int Rows, Columns;
    public Tile[,] Grid;
    public PlayMakerFSM _boardFSM;
    public GameObject _parent;

    public Board(int rows, int cols, int mines)
    {
        Rows = rows;
        Columns = cols;
        Mines = mines;

        _parent = new GameObject();
        _parent.name = "Tile_Parent";

        // Check if Board values are correct
        if(rows < 1 || cols < 1)
            Debug.Log("Board size too small. Like your pp.");
        if(mines > rows * cols)
            Debug.Log("Too many mines to place. Try lowering the number of mines.");

        SetDimensions(Rows, Columns);

    }

    public void SetDimensions(int rows, int cols) 
    {
        Vector3 origin = new Vector3(-6.15f, 0, -3.95f);
        Grid = new Tile[rows, cols];
        var spacing = 0.1f;

        var tile_number = 1;
        for ( int r = 0; r < rows; ++r) 
        {
            for(int c = 0; c < cols; ++c) 
            {
                Vector2 current_tile_pos = new Vector2( r , c );
                Grid[r,c] = new Tile( current_tile_pos );

                // We instantiate each physical tile 
                GameObject _t = GameObject.Instantiate( Resources.Load("Prefabs/Tile") ) as GameObject;
                // Gives them all the same parent
                _t.transform.parent = _parent.transform;

                _t.name = "Tile "+ tile_number++;
                // ... and we reference each Tile's FSM in our Tile  array
                Grid[r,c]._tileFSM = _t.GetComponent<PlayMakerFSM>();
                FsmVector2 _tile_position = Grid[r,c]._tileFSM.FsmVariables.GetFsmVector2("Position");
                _tile_position.Value = current_tile_pos;

                Vector3 _tile_position_3D = new Vector3( r + spacing*(1+r) , 0, c + spacing*(1 + c) ) + origin;
                // Place it wherever we want
                _t.transform.position = _tile_position_3D;
                Grid[r, c]._tile_position_3D = _tile_position_3D;
            }
        }
    }

    public void SetMines(Vector2 click_position, bool FirstClick)
    {
        FirstClick = false;
        Debug.Log("Initial Click on "+click_position);

        var rand = new System.Random();
        var placed = 0;
        while(placed < Mines)
        {
            var r = rand.Next() % Rows;
            var c = rand.Next() % Columns;
            Vector2 rc = new Vector2(r,c);
            if( !Grid[r,c].IsMine && !click_position.Equals(rc) )
            {
                Grid[r,c].SetMine();
                IncreaseSurroundingTilesCount(new Vector2(r,c));
                placed++;
            }
        }
    }

    public void IncreaseSurroundingTilesCount(Vector2 tile_position)
    {
        var x = (int)tile_position.x;
        var y = (int)tile_position.y;
        for( int i = x - 1; i <= x + 1; ++i )
        {
            for ( int j = y - 1; j <= y + 1; ++j )
            {
                if ( IsInside(i, j) ) Grid[i, j].Value++;
            }
        }
    }

    public bool Open(Vector2 tile_position)
    {
        var x = (int)tile_position.x;
        var y = (int)tile_position.y;
        var open = Grid[x, y].Open();
        if( open ) 
        {
            if ( NumberOfSurroundingMines(tile_position) == 0 )
            {
                for( int i = x - 1; i <= x + 1; ++i )
                {
                    for ( int j = y - 1; j <= y + 1; ++j )
                    {
                        if ( IsInside(i, j) ) Open( new Vector2(i, j) );
                        Debug.Log("Opening tile "+ tile_position);
                    }
                }
            }
            return true;
        }
        return false;
    }

    public int NumberOfSurroundingMines( Vector2 tile_position )
    {
        var x = (int)tile_position.x;
        var y = (int)tile_position.y;
        int mine_count = 0;
        int flag_count = 0;
        for( int i = x - 1; i <= x + 1; ++i )
        {
            for ( int j = y - 1; j <= y + 1; ++j )
            {
                if ( IsInside(i, j) && Grid[i, j].IsMine ) 
                {
                    if( Grid[i, j].IsFlag() ) { flag_count++; }
                    mine_count++;
                }
            }
        }
        return flag_count+mine_count;
    }

    public bool IsInside(int r, int c) { return r >= 0 && c >= 0 && r < Rows && c < Columns; }

    public void SetFlag(Vector2 tile_position)
    {
        Grid[(int)tile_position.x, (int)tile_position.y].SetFlag();
    }

    public int DetonateMines()
    {
        var count = 0;
        for ( int i = 0; i < Rows; ++i )
        {
            for (int j = 0; j < Columns; ++j )
            {
                if ( Grid[i,j].IsFlag() ) 
                {
                    Grid[i,j].Detonate();
                    count++;
                }
            }
        }
        return count;
    }

}



/* ########################################
                CLASS TILE
######################################### */
public class Tile {
    public bool IsMine;
    public TileStatus Status;
    public Vector2 Position;
    public int Value;
    public bool Flag;
    public Vector3 _tile_position_3D;

    public PlayMakerFSM _tileFSM;

    public Tile(Vector2 pos, bool mine = false)
    {
        Flag = false;
        Position = pos;
        Status = TileStatus.CLOSED;
        IsMine = mine;
        Value = 0;

    }

    public void SetMine() 
    {
        IsMine = true;
        _tileFSM.SendEvent("SET_MINE_MATERIAL");
    }

    public bool Open()
    {
        if(Status == TileStatus.CLOSED ) 
        {
            SetStatus( TileStatus.OPEN );
            InitValue();
            //Debug.Log("Opening "+Position);

            // We need to send the tile's own position translated to a Vector3 
            // -- along with the event to trigger the explosion FX at the right
            // -- position in the 3D space.
            FsmEventData tile_position = new FsmEventData();
            tile_position.Vector3Data = _tile_position_3D;

            HutongGames.PlayMaker.Fsm.EventData = tile_position;

            // We also need to set its IsOpen bool value in the FSM
            // -- so we can add some more fxs
            FsmBool tile_isopen = _tileFSM.FsmVariables.GetFsmBool("IsOpen");
            tile_isopen.Value = true;

            if( IsMine ) { 
                _tileFSM.SendEvent( "DESTROY_MINE" );
                _tileFSM.SendEvent( "PLAYER_TAKE_DAMAGE" );
            }
            else _tileFSM.SendEvent( "DESTROY_TILE" );

            return true;
        }
        else return false;
    }

    public void SetStatus(TileStatus status)
    {
        Status = status;
    }

    public bool IsOpen() { return ( Status==TileStatus.OPEN ); }
    public bool IsFlag() { return ( Status==TileStatus.FLAGGED); }

    public void SetFlag()
    {
        if( !IsOpen() )
        {
            
            if(!IsFlag() ) 
            { 
                // We also need to set its IsOpen bool value in the FSM
                // -- so we can add some more fxs
                FsmBool tile_isopen = _tileFSM.FsmVariables.GetFsmBool("IsOpen");
                tile_isopen.Value = true;

                Status = TileStatus.FLAGGED;
                _tileFSM.SendEvent( "SET_FLAG" );
                //Debug.Log("Setting flag at "+Position+" "+Flag);
            }
            else
            {
                // We also need to set its IsOpen bool value in the FSM
                // -- so we can add some more fxs
                FsmBool tile_isopen = _tileFSM.FsmVariables.GetFsmBool("IsOpen");
                tile_isopen.Value = false;

                Status = TileStatus.CLOSED;
                _tileFSM.SendEvent( "UNSET_FLAG" );
                //Debug.Log("Removing flag at "+Position+" "+Flag);
            }
            
        }
    }

    public void InitValue()
    {
        // We need to send the value of the tile to its corresponding FSM variable
        // -- so we can display it.
        FsmEventData _tile_value = new FsmEventData();
        _tile_value.IntData = Value;

        HutongGames.PlayMaker.Fsm.EventData = _tile_value;
        _tileFSM.SendEvent( "SET_VALUE" );
    }

    public int[] Detonate()
    {
        int[] damage = new int[2];
        var dealt = 0;
        var taken = 0;
        if(!IsOpen())
        {
            FsmEventData _tile_position = new FsmEventData();
            _tile_position.Vector3Data = _tile_position_3D;

            HutongGames.PlayMaker.Fsm.EventData = _tile_position;

            if(IsMine)
            {
                if( IsFlag() )
                {
                    _tileFSM.SendEvent( "REVEAL_MINE_FLAGGED" );
                    _tileFSM.SendEvent( "PLAYER_DEAL_DAMAGE" );
                    dealt++;
                }
                else 
                {
                    _tileFSM.SendEvent( "DESTROY_MINE" );
                    _tileFSM.SendEvent( "PLAYER_TAKE_DAMAGE" );
                    taken++;
                }
                Status = TileStatus.OPEN;
            }
            else 
            {
                if( IsFlag() ) {
                    _tileFSM.SendEvent( "WRONG_TILE_FLAGGED" );
                    SetStatus(TileStatus.OPEN);
                    taken++;
                }
                else Open();
            }
        }
        damage[0] = dealt;
        damage[1] = taken;
        return damage;
    }

}

public class Player
{
    public int HitPoints;
    public PlayMakerFSM _playerFSM;
    public PlayMakerFSM _player_UI_FSM;
    public PlayerType Type;
    public bool Alive;

    public Player(int hp, PlayerType type)
    {
        Type = type;
        Alive = true;
        HitPoints = hp;
        // We get both PlayerController and Player_UI FSMs
        if(Type == PlayerType.HUMAN)
        {
            Debug.Log("Human  Player with "+hp+" Hit points created");
            _player_UI_FSM = GameObject.FindWithTag("PlayerUI").GetComponent<PlayMakerFSM>();
        }
        else
        {
            Debug.Log("Generating Bot with "+hp+" Hit points.");
            _player_UI_FSM = GameObject.FindWithTag("BotUI").GetComponent<PlayMakerFSM>();
        }

        UpdateHitPoints();
    }

    public void UpdateHitPoints()
    {
        //We send player's HP through events to update its UIs
        FsmEventData _UI_HitPoints = new FsmEventData();
        _UI_HitPoints.IntData = HitPoints;
        Debug.Log(HitPoints);
        HutongGames.PlayMaker.Fsm.EventData = _UI_HitPoints;

        _player_UI_FSM.SendEvent( "UPDATE_HITPOINTS" );
    }

    public void TakeDamage(int d)
    {
        HitPoints = HitPoints - d;
        if( HitPoints <= 0 ) { HitPoints = 0; Alive = false; }
        UpdateHitPoints();
    }
}

}