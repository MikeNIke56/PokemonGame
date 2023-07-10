using GDE.GenericSelectionUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuSelection : MonoBehaviour
{
    [SerializeField] List<Text> menuUIList;
    int currentItem = 0;
    float selectionTimer = 0.2f;

    public static bool isHard { get; protected set; }

    private void Awake()
    {
        isHard = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Start()
    {
        UpdateList();
    }
    private void Update()
    {
        currentItem = Mathf.Clamp(currentItem, 0, menuUIList.Count - 1);
        UpdateTimer();
        float v = Input.GetAxis("Vertical");

        if (selectionTimer == 0 && Mathf.Abs(v) > 0.2)
        {
            currentItem += -(int)Mathf.Sign(v);
            selectionTimer = 0.2f;
            UpdateList();
        }

        if(Input.GetButtonDown("Action"))
        {
            if (currentItem == 0)
            {
                if(SceneManager.GetActiveScene().name == "Menu")
                    SceneManager.LoadScene("DifficultyScene");
                else
                {
                    SceneManager.LoadScene("GamePlay");
                }   
            }  
            else if (currentItem == 1)
            {
                if (SceneManager.GetActiveScene().name == "Menu")
                    Debug.Log("Got to Options Scene");
                else
                {
                    SceneManager.LoadScene("GamePlay");
                    isHard = true;
                }
                    
            }
        }
        if (Input.GetButtonDown("Back"))
        {
            if (SceneManager.GetActiveScene().name == "DifficultyScene" ||
                SceneManager.GetActiveScene().name == "OptionsScene")
                SceneManager.LoadScene("Menu");
        }

    }

    void UpdateList()
    {
        if(currentItem == 0)
        {
            menuUIList[currentItem].color = Color.blue;
            menuUIList[currentItem+1].color = Color.black;
        }
        else
        {
            menuUIList[currentItem].color = Color.blue;
            menuUIList[currentItem-1].color = Color.black;
        }

    }
    void UpdateTimer()
    {
        if (selectionTimer > 0)
        {
            selectionTimer = Mathf.Clamp(selectionTimer - Time.deltaTime, 0, selectionTimer);
        }
    }

}
