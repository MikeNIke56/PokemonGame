using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.CanvasScaler;

public class CRBar : MonoBehaviour
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;

    [SerializeField] float waitTime;
    [SerializeField] float staggerAmnt;

    [SerializeField] BattleSystem battleSystem;
    [SerializeField] public Units pokIcon;
    public Units[] pokemonUIList = new Units[2];


    public PokemonInfo attackingPokemon;
    public PokemonInfo defendingPokemon;

    public Image img;


    private void Awake()
    {
        img = GetComponent<Image>();
    }


    void StartList()
    {
        //clear existing item in list

        if(this.gameObject.GetComponentsInChildren<Units>().Length > 0)
        {
            foreach (Units unit in this.gameObject.GetComponentsInChildren<Units>())
                Destroy(unit.gameObject);
        }
        

        for (int i = 0; i < battleSystem.activePokemon.Length; i++)
        {
            var pokUIObj = Instantiate(pokIcon, startPoint);
            pokUIObj.SetData(battleSystem.activePokemon[i]);

            pokemonUIList[i] = pokUIObj;
        }
    }

    public void Move()
    {
        foreach (var unit in pokemonUIList)
        {
            unit.UpdateSpeed(unit.Pok);
            unit.transform.position = Vector3.MoveTowards(unit.transform.position, endPoint.position, Time.deltaTime * unit.SpeedRef/staggerAmnt);

            if (unit.transform.position.x == endPoint.position.x)
            {
                unit.isTurn = true;
                UnitAction(unit);
            }
        }
    }

    public void ExtraTurn(Units unit)
    {
        unit.isTurn = true;
        UnitAction(unit);
    }

    public void UnitAction(Units unit)
    {
        attackingPokemon = unit.Pok;

        foreach (var pokemon in pokemonUIList)
        {
            if (pokemon.Pok != attackingPokemon)
            {
                defendingPokemon = pokemon.Pok;
            }
        }     

        ResetUnit(unit);      
        unit.isTurn = false;    
    }

    public void UpdateList(PokemonInfo nextPok, int num)
    {
        pokemonUIList[num].gameObject.SetActive(false);

        var pokUIObj = Instantiate(pokIcon, startPoint);
        pokUIObj.SetData(nextPok);
        pokemonUIList[num] = pokUIObj;
        pokemonUIList[num].SetSpeed();

        ResetUnit(pokemonUIList[num]);
    }
    public void UpdateListMegaEn(PokemonInfo nextPok)
    {
        Destroy(pokemonUIList[0].gameObject);

        var pokUIObj = Instantiate(pokIcon, startPoint);
        pokUIObj.SetData(nextPok);
        pokemonUIList[0] = pokUIObj;
        pokemonUIList[0].SetSpeed();

    }
    public void UpdateListMegaPlayer(PokemonInfo nextPok)
    {
        Destroy(pokemonUIList[1].gameObject);

        var pokUIObj = Instantiate(pokIcon, startPoint);
        pokUIObj.SetData(nextPok);
        pokemonUIList[1] = pokUIObj;
        pokemonUIList[1].SetSpeed();

    }

    void ResetUnit(Units unit)
    {
        unit.transform.position = startPoint.position;
    }

    public void PauseUnits()
    {
        foreach (var unit in pokemonUIList)
        {
            unit.Pause();
        }
    }

    public void UnPauseUnits()
    {
        attackingPokemon = null;
        defendingPokemon = null;

        foreach (var unit in pokemonUIList)
        {
            unit.UnPause();
        } 
    }

    public void Tranparent()
    {
        var imgVal = img.color.a;
        imgVal = 0;
    }
    public void UnTranparent()
    {
        var imgVal = img.color.a;
        imgVal = 1;
    }

    void OnDisable()
    {
        foreach (Units unit in this.gameObject.GetComponentsInChildren<Units>())
            Destroy(unit.gameObject);
    }

    void OnEnable()
    {
        StartList();
    }


    public PokemonInfo AttackingPokemon => attackingPokemon;
    public PokemonInfo DefendingPokemon => defendingPokemon;
}
