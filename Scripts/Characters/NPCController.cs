using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] List<Vector2> movePat;
    [SerializeField] float timebtwnpat;

    NPCState state;
    float idletime = 0f;
    int currmovePat = 0;

    ALLCharmovement aLLCharmovement;
    Healer healer;

    private void Awake()
    {
        aLLCharmovement = GetComponent<ALLCharmovement>();
        healer = GetComponent<Healer>();
    }

    private void Update()
    {
        if (state == NPCState.Idle)
        {
            idletime += Time.deltaTime;
            if (idletime > timebtwnpat)
            {
                idletime = 0f;
                if (movePat.Count > 0)
                    StartCoroutine(Walk());
            }
        }
        aLLCharmovement.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldpos = transform.position;

        yield return aLLCharmovement.Move(movePat[currmovePat]);

        if (transform.position != oldpos)
        {
            currmovePat = (currmovePat + 1) % movePat.Count;
        }

        state = NPCState.Idle;
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialogue;
            aLLCharmovement.LookTowards(initiator.position);

            if (healer != null)
            {
                yield return healer.Heal(initiator, dialogue);
            }
            else
                yield return DialogueManager.Instance.ShowDialogue(dialogue);
        }

        idletime = 0f;
        state = NPCState.Idle;
    }

    public enum NPCState { Idle, Walking, Dialogue }
}
