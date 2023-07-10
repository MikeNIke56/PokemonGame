using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCTrigger : MonoBehaviour
{
    [SerializeField] GameObject pcScreen;
    GameObject player;
    float speed;



    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        speed = player.GetComponent<ALLCharmovement>().moveSpeed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.GetComponent<CharacterAnimator>().enabled = false;
        player.GetComponent<ALLCharmovement>().moveSpeed = 0;

        if (collision.gameObject == player)
        {
            pcScreen.SetActive(true);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pcScreen.SetActive(false);
            player.GetComponent<CharacterAnimator>().enabled = true;
            player.GetComponent<ALLCharmovement>().moveSpeed = speed;
        }
    }
}
