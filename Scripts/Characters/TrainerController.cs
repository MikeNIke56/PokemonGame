using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class TrainerController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Sprite vsSprite;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] Dialogue dialogue;
    [SerializeField] public Dialogue lostDialogue;
    ALLCharmovement character;

    [SerializeField] AudioClip trainerStartClip;

    bool battleLost = false;
    bool isEliteFour;

    VsScreen vsScreen;
    GameObject player;

    public static TrainerController i { get; private set; }

    private void Awake()
    {
        i = this;
        character = GetComponent<ALLCharmovement>();
    }
    private void Start()
    {
        if(tag == "EliteFour")
            isEliteFour = true;

        SetFovRotation(character.Animator.DefaultDir);
        player = FindObjectOfType<Canvas>().gameObject;
        vsScreen = player.GetComponentInChildren<VsScreen>(true);
    }
    private void Update()
    {
        character.HandleUpdate();
    }
    public IEnumerator Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);
        vsScreen.trainer = this;

        if (!battleLost)
        {
            AudioManager.i.PlayMusic(trainerStartClip);
            yield return DialogueManager.Instance.ShowDialogue(dialogue);

            yield return TrainerCinematic();
            yield return new WaitUntil(() => vsScreen.IsDone == true);
            yield return new WaitForSeconds(1f);
            vsScreen.gameObject.SetActive(false);
            GameController.Instance.StartTrainerBattle(this);
            yield return Fader.instance.FadeOut(1f);
        }
        else
            yield return DialogueManager.Instance.ShowDialogue(lostDialogue);
    }
    public void BattleLost()
    {
        if (isEliteFour == true)
            LeagueCounter.i.count++;
        if(name == "Mom")
            LeagueCounter.i.beatMom = true;      

        battleLost = true;
        fov.gameObject.SetActive(false);
    }

    public IEnumerator TriggerTrainerBattle(PlayerMovement player)
    {
        AudioManager.i.PlayMusic(trainerStartClip);
        vsScreen.trainer = this;

        // shows exclamation 
        exclamation.SetActive(true);
        yield return new WaitForSeconds(.5f);
        exclamation.SetActive(false);

        // walks to player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        // show dialogue
        yield return DialogueManager.Instance.ShowDialogue(dialogue);
        yield return TrainerCinematic();
        yield return new WaitUntil(() => vsScreen.IsDone == true);
        yield return new WaitForSeconds(1f);
        vsScreen.gameObject.SetActive(false);
        GameController.Instance.StartTrainerBattle(this);
        yield return Fader.instance.FadeOut(1f);
    }

    IEnumerator TrainerCinematic()
    {
        yield return Fader.instance.FadePartially(.5f);
        vsScreen.gameObject.SetActive(true);
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public object CaptureState()
    {
        return battleLost;
    }

    public void RestoreState(object state)
    {
        battleLost = (bool)state;

        if(battleLost)
            fov.gameObject.SetActive(false);
    }

    public string Name
    {
        get => name;
    }

    public Sprite Sprite
    {
        get => sprite;
    }
    public Sprite VsSprite
    {
        get => vsSprite;
    }
}