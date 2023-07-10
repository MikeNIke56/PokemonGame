using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TallGrass : MonoBehaviour, IPlayerTrigger
{
    public void OnPlayerTriggered(PlayerMovement player)
    {
        if (UnityEngine.Random.Range(1, 101) <= 10)
        {
            player.CharMovement.Animator.isMoving = false;
            GameController.Instance.StartBattle(BattleTrigger.LongGrass);
        }
    }
    public bool TriggerRepeatedly => true;
}
