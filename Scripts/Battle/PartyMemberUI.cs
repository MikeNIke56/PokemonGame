using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] public Text messageText;

    [SerializeField] Text curHPText;
    [SerializeField] Text maxHPText;

    PokemonInfo _pokemon;

    public void Init(PokemonInfo pokemon)
    {
        _pokemon = pokemon;
        UpdateData();
        SetMessage(" ");
        maxHPText.text = _pokemon.HP.ToString();

        _pokemon.OnHPChanged += UpdateData;  
    }

    void UpdateData()
    {
        nameText.text = _pokemon.Base.Name;
        levelText.text = "Lvl" + _pokemon.Level;
        hpBar.SetHP((float)_pokemon.HP / _pokemon.MaxHp); 
        curHPText.text = _pokemon.HP.ToString();
    }


    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = GlobalSettings.i.HighlightedColor;
        else
            nameText.color = Color.black;
    }

    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}
