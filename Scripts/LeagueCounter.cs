using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.SceneManagement;

public class LeagueCounter : MonoBehaviour
{
    public int count = 0;
    [SerializeField] GameObject mom;
    public Transform spawnLoc;
    public bool beatMom = false;

    public static LeagueCounter i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public void CheckForChamp()
    {
        if (count == 4)
        {
            StartCoroutine(ChampionEntrance());
            count = 0;
        }
    }

    public void CheckForLoss()
    {
        if(BattleSystem.i.didFail == true)
        {
            StartCoroutine(RestartPlayer());
        }
    }

    public void CheckForBeatGame()
    {
        if (beatMom == true)
        {
            StartCoroutine(BeatGame());
        }
    }

    IEnumerator ChampionEntrance()
    {
        yield return Fader.instance.FadeIn(1f);
        yield return new WaitForSeconds(2);
        mom.SetActive(true);
        yield return Fader.instance.FadeOut(1f);
    }

    IEnumerator RestartPlayer()
    {
        GameController.Instance.PauseGame(true);
        yield return Fader.instance.FadeIn(1f);
        yield return new WaitForSeconds(2);    
        FindObjectOfType<PlayerMovement>().transform.position = spawnLoc.position;
        yield return Fader.instance.FadeOut(1f);
        GameController.Instance.PauseGame(false);
        BattleSystem.i.didFail = false;
    }

    IEnumerator BeatGame()
    {
        yield return DialogueManager.Instance.ShowDialogue(mom.GetComponent<TrainerController>().lostDialogue);
        yield return Fader.instance.FadeIn(1f);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(11);
    }
}
