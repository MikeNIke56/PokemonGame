using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryItem : MonoBehaviour, IPlayerTrigger
{
    [SerializeField] Dialogue dialogue;


    public void OnPlayerTriggered(PlayerMovement player)
    {
        player.CharMovement.Animator.isMoving = false;
        StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
    }
    public bool TriggerRepeatedly => false;
}
