using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Main {


public class Player : MonoBehaviour
{

    public int health;
    public HealthBar healthBar;
    public int maxHealth = 5;
    public bool dead = false;
    public PlayMakerFSM _fsm;
    public PlayerType type;

    void Start()
    {
        
        healthBar = GameObject.FindWithTag("PlayerUI").GetComponent<HealthBar>();
        type = PlayerType.HUMAN;

        Init();
    }
    public Player() 
    {        
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
         
        PlayMakerUtils.SendEventToGameObjectFsmByName(null, GameObject.FindWithTag("Player"), "FSM_Player_UI", 
                                                "DIE",null);

    }

    public virtual void UpdateUI()
    {
        PlayMakerUtils.SendEventToGameObjectFsmByName(null, GameObject.FindWithTag("Player"), "FSM_Player_UI", 
                                                "UPDATE_HITPOINTS",null);
        healthBar.SetHealth(health);

    }

    public void Init()
    {
        health = maxHealth; 
        healthBar.SetMaxHealth(health);
    }



}

public enum PlayerType { HUMAN, BOT }

}
