using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RollCredits : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float timer;
    float maxTimer;

    private void Awake()
    {
        FindAnyObjectByType<EssentialObjs>().gameObject.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(Fader.instance.FadeOut(.5f));
        maxTimer = timer;
    }
    private void Update()
    {
        StartTimer();

        gameObject.transform.position = new Vector2(gameObject.transform.position.x, (gameObject.transform.position.y + speed * Time.deltaTime));
        if (timer <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }

    void StartTimer()
    {
        timer -= Time.deltaTime;
    }
}
