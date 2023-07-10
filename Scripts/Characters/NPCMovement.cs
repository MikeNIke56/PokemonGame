using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] Dialogue dialogue;
    ALLCharmovement aLLCharmovement;

    private void Awake()
    {
        aLLCharmovement = GetComponent<ALLCharmovement>();
    }
    
    public void Interact()
    {
        StartCoroutine(aLLCharmovement.Move(new Vector2()));
    }
}
