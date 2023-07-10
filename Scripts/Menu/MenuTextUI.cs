using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuTextUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    RectTransform rectTransform;
    private void Awake()
    {

    }

    public Text NameText => nameText;

    public float Height => rectTransform.rect.height;
}
