using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogueBox : MonoBehaviour
{
    [SerializeReference] int lettersPerSecond;
    [SerializeField] Text dialogueText;
    [SerializeField] public GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject boostSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject boostDetails;
    [SerializeField] GameObject choiceBox;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;
    [SerializeField] List<Text> boostTexts;

    [SerializeField] Text yesText;
    [SerializeField] Text noText;
    [SerializeField] Text ppText;
    [SerializeField] Text ppboostText;
    [SerializeField] Text typeText;

    Color highlightedColor;

    private void Start()
    {
        highlightedColor = GlobalSettings.i.HighlightedColor;
    }
    public void SetDialogue(string dialogue)
    {
        dialogueText.text = dialogue;
    }

    public IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";
        foreach (var letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogueText(bool enabled)
    {
        dialogueText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableBoostSelector(bool enabled)
    {
        boostSelector.SetActive(enabled);
        boostDetails.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }
    public void EnableChoiceBox(bool enabled)
    {
        choiceBox.SetActive(enabled);
    }
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i == selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }

        ppText.text = $"PP {move.PP} / {move.Base.PP}";
        typeText.text = move.Base.Type.ToString();

        if (move.PP == 0)
            ppText.color = Color.red;
        else
            ppText.color = Color.black;
    }

    public void UpdateBoostSelection(int selectedBoost, Boost boost)
    {
        for (int i = 0; i < boostTexts.Count; ++i)
        {
            if (i == selectedBoost)
                boostTexts[i].color = highlightedColor;
            else
                boostTexts[i].color = Color.black;
        }

        ppboostText.text = $"PP {boost.PP} / 1";

        if (boost.PP == 0)
            ppboostText.color = Color.red;
        else
            ppboostText.color = Color.black;
    }
    public void UpdateChoiceBox(bool yesSelected)
    {
        if(yesSelected)
        {
            yesText.color = highlightedColor;
            noText.color = Color.black;
        }
        else
        {
            yesText.color = Color.black;
            noText.color = highlightedColor;
        }
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
                if (i < moves.Count)
                    moveTexts[i].text = moves[i].Base.Name;
                else
                    moveTexts[i].text = "-";
        }
    }
    public void SetBoostNames(List<Boost> boosts)
    {
        for (int i = 0; i < boostTexts.Count; ++i)
        {
            if (i < boosts.Count)
                boostTexts[i].text = boosts[i].BBase.Name;
            else
                boostTexts[i].text = "-";
        }
    }
}
