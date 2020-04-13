using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Main {
public class Monster : Player 
{

    void Start()
    {
        maxHealth = 10;
        type = PlayerType.BOT;
        
        healthBar = GameObject.FindWithTag("BotUI").GetComponent<HealthBar>();
        Init();
        Debug.Log("New monster");
    }
    public Monster()
    {
        
    }

    public override void Death()
    {
        health = 0;
        Debug.Log("BOT DEAD");
         
        PlayMakerUtils.SendEventToGameObjectFsmByName(null, GameObject.FindWithTag("Bot"), "FSM_Bot_UI", 
                                                "DIE",null);

    }


    public override void UpdateUI()
    {
        PlayMakerUtils.SendEventToGameObjectFsmByName(null, GameObject.FindWithTag("Bot"), "FSM_Bot_UI", 
                                                "UPDATE_HITPOINTS",null);
        healthBar.SetHealth(health);

    }

    
}
}