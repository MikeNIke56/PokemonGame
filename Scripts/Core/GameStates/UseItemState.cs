using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDEUtils.StateMachine;
using System.Linq;

public class UseItemState : State<GameController>
{
    /*[SerializeField] InventoryUI inventoryUI;
    [SerializeField] PartyScreen partyScreen;
    Inventory inventory;
    public static UseItemState i { get; private set; }

    private void Awake()
    {
        i = this;
        inventory = Inventory.GetInventory();
    }

    GameController gC;
    public override void Enter(GameController owner)
    {
        gC = owner;
        StartCoroutine(UseItem());
    }
    IEnumerator UseItem()
    {
        var item = inventoryUI.SelectedItem;
        var pokemon = partyScreen.SelectedMember;

        if (item is TMItem)
            yield return HandleTMs();
        else
        {
            if (item is EvoItem)
            {
                var evolution = pokemon.CheckForEvolution(item);
                if (evolution != null)
                {
                    yield return EvolutionManager.i.Evolve(pokemon, evolution);
                }
                else
                {
                    yield return DialogueManager.Instance.ShowDialogueText($"The item had no effect!");
                    gC.StateMachine.Pop();
                    yield break;
                }
            }
        }

        var usedItem = inventory.UseItem(item, partyScreen.SelectedMember);

        if (usedItem != null)
        {
            if (usedItem is RecoveryItems)
                yield return DialogueManager.Instance.ShowDialogueText($"You used a(n) {usedItem.Name}.");
        }
        else
        {
            if (inventoryUI.SelectedCategory == (int)ItemCategory.Items)
                yield return DialogueManager.Instance.ShowDialogueText($"The item had no effect!");
        }

        gC.StateMachine.Pop();
    }

    IEnumerator HandleTMs()
    {
        var tmItem = inventoryUI.SelectedItem as TMItem;
        if (tmItem == null)
            yield break;

        var pokemon = partyScreen.SelectedMember;

        if (pokemon.HasMove(tmItem.Move))
        {
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.Base.Name} already knows {tmItem.Move.Name}!");
            yield break;
        }

        if (!tmItem.CanBeTaught(pokemon))
        {
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.Base.Name} can't learn {tmItem.Move.Name}!");
            yield break;
        }

        if (pokemon.Moves.Count < 4)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.Base.Name} learned {tmItem.Move.Name}!");
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.Base.Name} is trying to learn {tmItem.Move.Name}.");
            yield return DialogueManager.Instance.ShowDialogueText($"But it can't learn more than 4 moves.");
            yield return DialogueManager.Instance.ShowDialogueText($"Select a move to be forgotten.");

            MoveForgetState.i.NewMove = tmItem.Move;
            MoveForgetState.i.CurrentMoves = pokemon.Moves.Select(m => m.Base).ToList();
            yield return gC.StateMachine.PushAndWait(MoveForgetState.i);

            int moveIndex = MoveForgetState.i.Selection;
            if (moveIndex == 4 || moveIndex == -1)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.Base.Name} did not learn {tmItem.Move.Name}");
            }
            else
            {
                var selectedMove = pokemon.Moves[moveIndex].Base;
                yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.Base.Name} forgot {selectedMove.Name} and learned {tmItem.Move.Name} instead.");

                pokemon.Moves[moveIndex] = new Move(tmItem.Move);
            }
        }
    }*/
}
