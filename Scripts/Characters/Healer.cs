using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    int selectedChoice = 0;

    public IEnumerator Heal(Transform player, Dialogue dialogue)
    {
        yield return DialogueManager.Instance.ShowDialogue(dialogue, null, new List<string>() { "Yes", "No" }, (choiceIndex) => selectedChoice = choiceIndex);

        if(selectedChoice == 0)
        {
            yield return Fader.instance.FadeIn(1f);

            var party = player.GetComponent<PokemonParty>();
            party.Pokemon.ForEach(p => p.Heal());
            party.PartyUpdated();

            yield return Fader.instance.FadeOut(1f);
        }
        else if (selectedChoice == 1)
        {
            yield return DialogueManager.Instance.ShowDialogueText($"Come back if you change your mind.");
        }
 
    }
}
