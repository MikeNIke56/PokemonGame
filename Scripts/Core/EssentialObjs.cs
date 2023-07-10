using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjs : MonoBehaviour
{
    public GameObject obj;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
