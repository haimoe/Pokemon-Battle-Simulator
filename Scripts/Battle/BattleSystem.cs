using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;


    BattleState state;
    int currentAction;
    int currentMove;


    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"Kampf 1 - Nutze die effektivste Attacke, um das gegnerische {enemyUnit.Pokemon.Base.PokemonName} zu besiegen.");

        ChooseFirstTurn();
    }

    void ChooseFirstTurn()
    {
        if (playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed)
            PlayerAction();
        else
            StartCoroutine(EnemyMove());
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Wähle eine Aktion aus."));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);

    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;


        var move = playerUnit.Pokemon.Moves[currentMove];
        move.PP--;
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.PokemonName} setzt {move.Base.AttackName} ein!");

        if (move.Base.Category == MoveCategory.Status)
        {
            yield return PerformPlayerEffects(move, playerUnit.Pokemon, enemyUnit.Pokemon);

        }
        else
        {
            var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
            yield return enemyHud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }

        if (/*damageDetails.Fainted*/enemyUnit.Pokemon.HP <= 0 )
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.PokemonName} wurde besiegt. Drücke ESC um zurück in das Hauptmenü zu gelangen.");

            yield return new WaitForSeconds(2f);


        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator PerformPlayerEffects(Move move, Pokemon player, Pokemon enemy)
    {
        var effects = move.Base.Effects;
        if (effects.Boosts != null)
        {
            if (move.Base.Target == MoveTarget.Self)
                playerUnit.Pokemon.ApplyBoosts(effects.Boosts);
            else
                enemyUnit.Pokemon.ApplyBoosts(effects.Boosts);
        }

        yield return ShowStatusChanges(player);
        yield return ShowStatusChanges(enemy);
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Pokemon.GetRandomMove();
        move.PP--;

        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.PokemonName} setzt {move.Base.AttackName} ein!");

        var damageDetails = playerUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.PokemonName} wurde besiegt. Drücke F5 um den Kampf neuzustarten.");

            yield return new WaitForSeconds(2f);


        }
        else
        {
            PlayerAction();
        }

    }

    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }
    

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("Volltreffer!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("Es ist sehr effektiv!");

        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("Es ist nicht sehr effektiv.");


    }

    private void Update()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentAction == 0)
            {

                PlayerMove();

            }
            else if (currentAction == 1)
            {
                Application.Quit();
                Debug.Log("Application was quit.");
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
                        
        }
    }

}
