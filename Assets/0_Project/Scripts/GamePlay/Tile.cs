﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;


namespace Main {

public class Tile
{
    public bool IsMine;
    public TileStatus Status;
    public Vector2 Position;
    public int Value;
    public int Owner;
    public bool Flag;
    public Vector3 _tile_position_3D;
    public GameObject _object;

    public PlayMakerFSM _tileFSM;

    public Tile(Vector2 pos, int owner, GameObject obj,bool mine = false)
    {
        Flag = false;
        Position = pos;
        _object = obj;
        Status = TileStatus.CLOSED;
        IsMine = mine;
        Value = 0;

        _tileFSM = _object.GetComponent<PlayMakerFSM>();
        Owner = owner;

            FsmInt tile_owner = _tileFSM.FsmVariables.GetFsmInt("Owner");
            tile_owner.Value = Owner;
    }

    public void SetMine()
    {
        IsMine = true;
        PlayMakerUtils.SendEventToGameObjectFsmByName(null, _object, "FSM_Tile_Materials", 
                                                        "SET_MINE_MATERIAL",null);
        
    }
    public bool Open()
    {
        if(Status == TileStatus.CLOSED ) 
        {
            SetStatus( TileStatus.OPEN );
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

            if( IsMine ) 
            { 
                PlayMakerUtils.SendEventToGameObjectFsmByName(null, _object, "FSM_Tile_Materials", 
                                                        "DESTROY_MINE",null);
            }
            else
            {
                PlayMakerUtils.SendEventToGameObjectFsmByName(null, _object, "FSM_Tile_Materials", 
                                                        "DESTROY_TILE",null);
                SetValue();
            }
            return true;
        }
        else return false;
    }

    public void SetStatus(TileStatus status)
    {
        Status = status;
    }

    public bool IsOpen() { return ( Status==TileStatus.OPEN ); }
    public bool IsFlag() { return ( Status==TileStatus.FLAGGED ); }

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
                PlayMakerUtils.SendEventToGameObjectFsmByName(null, _object, "FSM_Tile_Materials", "SET_FLAG",null);
            }
            else
            {
                // We also need to set its IsOpen bool value in the FSM
                // -- so we can add some more fxs
                FsmBool tile_isopen = _tileFSM.FsmVariables.GetFsmBool("IsOpen");
                tile_isopen.Value = false;

                Status = TileStatus.CLOSED;
                PlayMakerUtils.SendEventToGameObjectFsmByName(null, _object, "FSM_Tile_Materials", "UNSET_FLAG",null);
            }
            
        }
    }

    public void SetValue()
    {
        // We need to send the value of the tile to its corresponding FSM variable
        // -- so we can display it.
        FsmEventData _tile_value = new FsmEventData();
        _tile_value.IntData = Value;

        HutongGames.PlayMaker.Fsm.EventData = _tile_value;
        PlayMakerUtils.SendEventToGameObjectFsmByName(null, _object, "FSM_Tile_Value", "SET_VALUE",null);
    }

    public int Detonate()
    {
        if(!IsOpen())
        {
            SetFlag();
            if(IsMine)
            {
                PlayMakerUtils.SendEventToGameObjectFsmByName(null, _object, "FSM_Tile_Materials", "MINE_DETONATED",null);
                return 1;
            }
            else 
            {
                PlayMakerUtils.SendEventToGameObjectFsmByName(null, _object, "FSM_Tile_Materials", "WRONG_TILE_FLAGGED",null);
                return 0;
            }
        }
        return 0;
    }

    public Vector3 SetPosition( Vector3 origin )
    {
        var spacing = 0.09f;
        FsmVector2 _tile_position = _tileFSM.FsmVariables.GetFsmVector2("Position");
        _tile_position.Value = Position;

        _tile_position_3D = new Vector3( Position.x + spacing*(1+ Position.x) , 
                                                0,
                                                Position.y + spacing*(1+Position.y) 
                                        );
        _tile_position_3D = _tile_position_3D + origin;
        
        return _tile_position_3D;
    }

    public void SetOwner(int owner) { 
        FsmInt FSM_owner = _tileFSM.FsmVariables.GetFsmInt("Owner");
        FSM_owner.Value = owner;
         
    }

    public void Reset()
    {
        Flag = false;
        Status = TileStatus.CLOSED;
        Value = 0;
        IsMine = false;
        PlayMakerUtils.SendEventToGameObjectFsmByName(null, _object, "FSM_Tile", 
                                                        "RESET_TILE",null);
    }

}


public enum TileStatus { CLOSED, OPEN, FLAGGED }

}