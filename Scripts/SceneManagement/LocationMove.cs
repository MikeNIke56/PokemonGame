using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LocationMove : MonoBehaviour, IPlayerTrigger
{
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
        StartCoroutine(ChangeLocation());
    }

    IEnumerator ChangeLocation()
    {
        GameController.Instance.PauseGame(true);
        yield return fader.FadeIn(.5f);

        var dstPortal = FindObjectsOfType<LocationMove>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.CharMovement.SetPositionAndSnapToTile(dstPortal.placeToTeleport.position);

        yield return fader.FadeOut(.5f);
        GameController.Instance.PauseGame(false);
    }
    public Transform PlaceToTeleport => placeToTeleport;
}