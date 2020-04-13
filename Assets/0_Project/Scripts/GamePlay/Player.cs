using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Main {


public class Player
{

    public int health;
    
    public bool dead = false;
    public PlayMakerFSM _fsm;
    public PlayerType type;

    public Player() 
    {
        health = 10;
        type = PlayerType.HUMAN;
        _fsm = GameObject.FindWithTag("Player").GetComponent<PlayMakerFSM>();
    }

    public int TakeDamage(int amount)
    {
        health = health - amount;
        if(health <= 0) Death();

        UpdateUI();

        return health;

    }

    public int GetHP() { return health; }

    public virtual void Death()
    {
        health = 0;
        Debug.Log("HUMAN DEAD");
         
        _fsm.SendEvent("DIE");

    }

    public void UpdateUI()
    {
        FsmInt FSM_hp = _fsm.FsmVariables.GetFsmInt("UI_HitPoints");
        FSM_hp.Value = health;
        _fsm.SendEvent("UPDATE_HITPOINTS");
    }

}

public class Monster : Player 
{
    public Monster()
    {
        type = PlayerType.BOT;
        health = 20;
        _fsm = GameObject.FindWithTag("Bot").GetComponent<PlayMakerFSM>();
        Debug.Log("New monster");
        
    }

    public override void Death()
    {
        health = 0;
        Debug.Log("BOT DEAD");
         
        _fsm.SendEvent("DIE");

    }


}

public enum PlayerType { HUMAN, BOT }

}
