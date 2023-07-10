using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneMove : MonoBehaviour, IPlayerTrigger
{
    [SerializeField]
    int sceneToLoad = -1;
    [SerializeField]
    Transform placeToTeleport;
    [SerializeField]
    DestinationIdentifier destinationPortal;

    PlayerMovement player;
    Fader fader;

    public bool TriggerRepeatedly => false;

    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    public void OnPlayerTriggered(PlayerMovement player)
    {
        this.player = player;
        player.CharMovement.Animator.isMoving = false;
        StartCoroutine(SwitchScene());
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);
        GameController.Instance.PauseGame(true);
        yield return fader.FadeIn(.5f);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        var dstPortal = FindObjectsOfType<SceneMove>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.CharMovement.SetPositionAndSnapToTile(dstPortal.placeToTeleport.position);

        yield return fader.FadeOut(.5f);
        GameController.Instance.PauseGame(false);
        Destroy(gameObject);
    }

    public Transform PlaceToTeleport => placeToTeleport;
}

public enum DestinationIdentifier { A, B, C, D, E }
