using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, ISavable
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    private Vector2 input;

    private ALLCharmovement charMovement;
    public static PlayerMovement i { get; private set; }

    private void Awake()
    {
        i = this;
        charMovement = GetComponent<ALLCharmovement>();
    }

    public void HandleUpdate()
    {
        if (!charMovement.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //removes diagonal movement
            if (input.x != 0) input.y = 0;

            StartCoroutine(charMovement.Move(input, OnMoveOver));
        }

        if (Input.GetKeyDown(KeyCode.Z))
            StartCoroutine(Interact());

        charMovement.HandleUpdate();
    }

    //tells us what tile in front of the player is interactbale 
    IEnumerator Interact()
    {
        var facingDir = new Vector3(charMovement.Animator.moveX, charMovement.Animator.moveY);
        var interactPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.I.InteracbleLayer);
        if(collider != null)
        {
            yield return collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    IPlayerTrigger currentlyInTrigger;

    private void OnMoveOver()
    {
        IPlayerTrigger collider = null;
        var triggerables = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, charMovement.OffsetY), 0.2f, GameLayers.I.TriggerableLayers);

        foreach (var triggerable in triggerables)
        {
            collider = triggerable.GetComponent<IPlayerTrigger>();
            if(collider != null)
            {
                if (collider == currentlyInTrigger && !collider.TriggerRepeatedly)
                    break;

                collider.OnPlayerTriggered(this);
                currentlyInTrigger = collider;
                break;
            }
        }

        if (triggerables.Count() == 0 || collider != currentlyInTrigger)
            currentlyInTrigger = null;
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y },
            pokemon = GetComponent<PokemonParty>().Pokemon.Select(p => p.GetSaveData()).ToList()
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;
        var pos = saveData.position;
        transform.position = new Vector3(pos[0], pos[1]);

        GetComponent<PokemonParty>().Pokemon = saveData.pokemon.Select(s => new PokemonInfo(s)).ToList();
    }

    public string Name
    {
        get => name;
    }

    public Sprite Sprite
    {
        get => sprite;
    }

    public ALLCharmovement CharMovement => charMovement;
}

[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<PokemonSaveData> pokemon;
}
