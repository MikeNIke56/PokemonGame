using GDE.GenericSelectionUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonSummary : SelectionUI<TextSlot>
{
    [SerializeField] Image pokImg;
    [SerializeField] Text nameText;
    [SerializeField] Text atkText;
    [SerializeField] Text defText;
    [SerializeField] Text spatkText;
    [SerializeField] Text spdefText;
    [SerializeField] Text spdText;
    [SerializeField] Text dodgeText;
    [SerializeField] Text guardText;
    [SerializeField] Text dmgRedText;
    [SerializeField] Text buatkText;
    [SerializeField] Text budefText;
    [SerializeField] Text buspatkText;
    [SerializeField] Text buspdefText;
    [SerializeField] Text buspdText;
    [SerializeField] Text fieldText;

    [SerializeField] Text[] moves;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowSummary(PokemonInfo pokemon)
    {
        if(GameController.Instance.State == GameState.Battle)
        {
            if(pokemon.count == 0)
            {
                pokemon.CalculateStats();
                pokemon.count = 1;
            }
            else
            {
                if(pokemon == BattleSystem.i.playerUnit.Pokemon)
                {
                    pokemon.count = 1;
                    pokemon.CalculateStats();
                }
            }
        }
        else if (GameController.Instance.State == GameState.FreeRoam)
        {
            pokemon.count = 0;
            pokemon.CalculateStats();
        }


        pokImg.sprite = pokemon.Base.Frontsprite;
        nameText.text = $"{pokemon.Base.Name}";
        atkText.text = $"Attack: {pokemon.Attack}";
        defText.text = $"Defense: {pokemon.Defense}";
        spatkText.text = $"SpAtk: {pokemon.SpAttack}";
        spdefText.text = $"SpDef: {pokemon.SpDefense}";
        spdText.text = $"Speed: {pokemon.Speed}";
        dodgeText.text = $"Dodge%: {pokemon.Base.DodgeChance}";
        guardText.text = $"Guard%: {pokemon.Base.GuardChance}";
        dmgRedText.text = $"Damage Reduction%: {pokemon.Base.DmgReductionAmnt}";
        buatkText.text = $"BUAtk%: {pokemon.Base.BuildUpAmntAtk}";
        budefText.text = $"BUDef%: {pokemon.Base.BuildUpAmntDef}";
        buspatkText.text = $"BUSpAtk%: {pokemon.Base.BuildUpAmntSpAtk}";
        buspdefText.text = $"BUSpDef%: {pokemon.Base.BuildUpAmntSpDef}";
        buspdText.text = $"BUSpd%: {pokemon.Base.BuildUpAmntSpd}";
        fieldText.text = $"Field Effect: {pokemon.Base.FieldEffect}";

        ClearMoves();

        for (int i = 0; i < pokemon.GetMoves().Count; i++)
        {
            if (pokemon.GetMoves()[i] != null)
                moves[i].text = pokemon.GetMoves()[i].ToString();
            else
                moves[i].text = "-";
        }
    }

    void ClearMoves()
    {
        for (int i = 0; i < 4; i++)
        {
             moves[i].text = "-";
        }
    }

}
