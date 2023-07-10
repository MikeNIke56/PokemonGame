using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask scenemoveLayer;
    [SerializeField] LayerMask triggersLayer;

    public static GameLayers I { get; set; }

    private void Awake()
    {
        I = this;
    }

    public LayerMask SolidLayer
    {
        get => solidLayer;
    }
    public LayerMask GrassLayer
    {
        get => grassLayer;
    }
    public LayerMask InteracbleLayer
    {
        get => interactableLayer;
    }
    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }
    public LayerMask FovLayer
    {
        get => fovLayer;
    }
    public LayerMask SceneMoveLayer
    {
        get => scenemoveLayer;
    }
    public LayerMask TriggerableLayers
    {
        get => grassLayer | fovLayer | scenemoveLayer | triggersLayer;
    }
}
