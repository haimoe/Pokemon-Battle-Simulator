using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Menu, Battle}

public class GameController : MonoBehaviour
{
    [SerializeField] MenuController menuController;
    [SerializeField] BattleSystem battleSystem;

    GameState state;

    private void Update()
    { 
        if (state == GameState.Menu)
        {
            
        }

        else if (state == GameState.Battle)
        {

        }
    }
}
