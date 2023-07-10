using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialogueBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Text dialogueText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialogue;
    public event Action OnCloseDialogue;
    public bool IsShowing { get; private set; }
   
    public static DialogueManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator ShowDialogueText(string text, bool waitForInput=true, bool autoClose=true, List<string> choices = null, Action<int> onSelected = null)
    {
        IsShowing = true;

        dialogueBox.SetActive(true);
        OnShowDialogue?.Invoke();

        AudioManager.i.PlaySfx(AudioId.UISelect);
        yield return TypeDialogue(text);

        if(waitForInput)
        {
           yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onSelected);
        }

        if (autoClose)
        {
            CloseDialog();
        }
        OnCloseDialogue?.Invoke();
    }

    public void CloseDialog()
    {
        dialogueBox.SetActive(false);
        IsShowing = false;
    }

    public IEnumerator ShowDialogue(Dialogue dialogue, Action onFinished=null, List<string> choices = null, Action<int> onSelected=null)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialogue?.Invoke();

        IsShowing = true;

        dialogueBox.SetActive(true);

        foreach(var line in dialogue.Lines)
        {
            AudioManager.i.PlaySfx(AudioId.UISelect);
            yield return TypeDialogue(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        if(choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onSelected);
        }

        dialogueBox.SetActive(false);
        IsShowing = false;
        OnCloseDialogue?.Invoke();
    }

    public void HandleUpdate()
    {

    }

    public IEnumerator TypeDialogue(string line)
    {
        dialogueText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }
}
