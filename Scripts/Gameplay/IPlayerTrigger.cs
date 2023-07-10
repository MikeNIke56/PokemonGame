using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerTrigger
{
    void OnPlayerTriggered(PlayerMovement player)
    {

    }

    public bool TriggerRepeatedly { get;  }
}
