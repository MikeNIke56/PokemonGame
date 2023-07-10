using GDE.GenericSelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PokemonPCSlotUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text lvlText;
    RectTransform rectTransform;
    private void Awake()
    {

    }

    public Text NameText => nameText;
    public Text LvlText => lvlText;

    public float Height => rectTransform.rect.height;

    public void SetData(PokemonInfo pokemon)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = pokemon.Base.GetName().ToString();
        lvlText.text = $"Lvl {pokemon.Level}";
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = GlobalSettings.i.HighlightedColor;
        else
            nameText.color = Color.black;
    }
}

