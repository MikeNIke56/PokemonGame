using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class Units : MonoBehaviour
{
    [SerializeField] int speedRef;
    [SerializeField] int speedBeforePause;
    [SerializeField] int speedOnPause;
    [SerializeField] int baseSpd;
    public bool isTurn = false;

    public Image img;
    PokemonInfo pok;

    private void Awake()
    {
        var script = GetComponent<Units>();
        script.enabled = true;
    }

    private void Start()
    {
        SetSpeed();
    }
    public void SetSpeed()
    {
        speedBeforePause = speedRef;
    }
    public void Pause()
    {
        speedRef = speedOnPause;
    }
    public void UnPause()
    {
        speedRef = speedBeforePause;
    }
    public void UpdateSpeed(PokemonInfo pokemon)
    {
        speedRef = baseSpd + pokemon.Speed;
        speedBeforePause = speedRef;
    }

    public void SetData(PokemonInfo pokemon)
    {
        pokemon.count = 0;
        pokemon.CalculateStats();
        speedRef = baseSpd + pokemon.Speed;

        float spdPer = speedRef * .05f;
        float speedDif = Random.Range(0, spdPer);

        if (Random.Range(0, 2) == 0)
            speedRef += (int)speedDif;
        else
            speedRef -= (int)speedDif;

        if (speedRef <= 0)
            speedRef *= -1;

        img.sprite = pokemon.Base.Frontsprite;
        img.enabled = true;
        pok = pokemon;
    }

    public int SpeedRef => speedRef;
    public PokemonInfo Pok => pok;
}
