using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Main {


public class Board
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public int Mines { get; set; }
    public int MineCount;
    public bool Active { get; set; }
    public bool First_Click { get; set; }

    protected int ID { get; private set; }

    public Tile[,] Grid { get; private set; }
    public List<Tile> FlaggedTiles;
    public bool Initialized { get; set; }

    public GameObject _parent { get; set;}

    public Board(int id)
    {
        ID = id;
        Active = false;
    }

    public Board(int rows, int cols, int mines, int id, bool active = false)
    {
        if (rows < 1 || cols < 1)
            throw new ArgumentException("Board size can not be less than 1x1");

        Rows = rows;
        Columns = cols;

        Mines = mines;
        MineCount = 0;
        
        ID = id;
        First_Click = true;
        Active = active;

        FlaggedTiles = new List<Tile>();

        _parent = new GameObject();
        _parent.name = "Tile Container:"+ID;
        if(!Active) _parent.SetActive(false);
    }
    
    // Set the board dimension
    //  -- and displays the Tiles with an offset
    public void SetDimensions(Vector3 origin)
    {
        Grid = new Tile[Rows, Columns];
        var tile_number = 0;
        for ( int r = 0; r < Rows; ++r )
        {
            for ( int c = 0; c < Columns; ++c )
            {
                tile_number++;
                GameObject _t = GameObject.Instantiate( Resources.Load("Prefabs/Tile") )
                                 as GameObject;
                _t.transform.parent = _parent.transform;
                _t.name = ID+":Tile_"+tile_number;

                Grid[r, c] = new Tile(new Vector2(r, c), ID, _t);
                _t.transform.position = Grid[r, c].SetPosition(origin);

            }
        }

    }

    public void SetMines(Vector2 tile_position)
    {
        First_Click = false;
        Debug.Log("Initial Click on "+tile_position);

        var rand = new System.Random();
        var placed = 0;
        while(placed < Mines)
        {
            var r = rand.Next() % Rows;
            var c = rand.Next() % Columns;
            Vector2 rc = new Vector2(r,c);
            if( !Grid[r,c].IsMine && !tile_position.Equals(rc) )
            {
                Grid[r,c].SetMine();
                IncreaseSurroundingTilesCount(rc);
                placed++;
            }
        }
    }

    public void ClickTile(Vector2 tile_position)
    {
        if(First_Click) 
        {
            First_Click = false;
            SetMines(tile_position);
        }
        if( Open(tile_position) )
        {
            if(GetTile(tile_position).IsMine) 
            {
                PlayMakerUtils.SendEventToGameObjectFsmByName(null, GameObject.FindWithTag("GameController"), "FSM_GC_PlayerActions", 
                                                        "MINE_CLICKED",null);
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
            if(Grid[x, y].IsMine) MineCount++;
            if ( CheckSurroundings(tile_position) == 0 )
            {
                for( int i = x - 1; i <= x + 1; ++i )
                {
                    for ( int j = y - 1; j <= y + 1; ++j )
                    {
                        if ( IsInside(i, j) ) Open( new Vector2(i, j) );
                    }
                }
            }
            return true;
        }
        return false;
    }

    public int CheckSurroundings( Vector2 tile_position )
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
    public bool IsInside(int r, int c) { return r >= 0 && c >= 0 && r < Rows && c < Columns; }

    public void SetFlag(Vector2 tile_position)
    {
        GetTile(tile_position).SetFlag();
        if( GetTile(tile_position).IsFlag() ) FlaggedTiles.Add( GetTile(tile_position) );
        else 
        {
            List<Tile> flagged = FlaggedTiles.FindAll(item => item.Position.Equals(tile_position));
            Tile t = flagged.Find(item => item.Owner == ID);
            FlaggedTiles.Remove(t);
        }

    }

    public int Detonate()
    {
        var count = 0;
        for(int i = FlaggedTiles.Count - 1; i >=0; --i)
        {
            count += FlaggedTiles[i].Detonate();
            FlaggedTiles[i].Status = TileStatus.OPEN;
            FlaggedTiles.RemoveAt(i);
        }
        MineCount += count;
        return count;
    }

    public void SetActive(bool active)
    {
        Active = active;
        if(!Active)
        {
            Debug.Log("Set inactive "+ID);
        }
    }

    public bool CheckMines()
    {
        //Debug.Log("GRID:"+ID+" -> Mines found: "+MineCount);
        if(Mines == MineCount) return true;
        return false;
    }

    public void Reset(int mines)
    {
        for ( int r = 0; r < Rows; ++r )
        {
            for ( int c = 0; c < Columns; ++c )
            {
                Grid[r,c].Reset();
            }
        }
        FlaggedTiles.Clear();
        First_Click = true;
        SetActive(true);
        MineCount = 0;
        Mines = mines;
        _parent.SetActive(true);
    }

    public void OpenAll()
    {
        for( int i = 0; i < Rows; ++i)
        {
            for( int j = 0; j < Columns; ++j)
            {
                Grid[i,j].Open();
            }
        }
    }

    #region Assessors
    public Tile GetTile(Vector2 tile_position)
    {
        return Grid[(int)tile_position.x, (int)tile_position.y];
    }
    #endregion

}
}