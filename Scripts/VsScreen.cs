using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;
using UnityEngine.UI;

public class VsScreen : MonoBehaviour
{
    [SerializeField] Image bgImg;
    [SerializeField] public Image trainerImg;


    bool isDone = false;

    public TrainerController trainer;
    Animator animator;
    AnimatorStateInfo animStateInfo;


    private void Start()
    {
        trainerImg.sprite = trainer.VsSprite;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (animStateInfo.length == 0)
            StartCoroutine(Done());
    }

    IEnumerator Done()
    {
        yield return new WaitForSeconds(1.5f);
        isDone = true;
    }

    void OnEnable()
    {
        trainerImg.sprite = trainer.VsSprite;
    }

    public bool IsDone => isDone;
}
