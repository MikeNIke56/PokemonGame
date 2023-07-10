using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;

    [SerializeField] public Text curHPTextHUD;
    [SerializeField] public Text maxHPTextHUD;

    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color frzColor;
    [SerializeField] Color parColor;
    [SerializeField] Color slpColor;

    PokemonInfo _pokemon;
    Dictionary<ConditionsID, Color> statusColors;


    public void SetData(PokemonInfo pokemon)
    {
        _pokemon = pokemon;

        if (_pokemon != null)
        {
            _pokemon.OnStatusChanged -= SetStatusText;
            _pokemon.OnHPChanged -= UpdateHP;
        } 

        nameText.text = pokemon.Base.Name;
        SetLevel();
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);
        SetExp();

        statusColors = new Dictionary<ConditionsID, Color>()
        {
            {ConditionsID.psn, psnColor },
            {ConditionsID.brn, brnColor },
            {ConditionsID.frz, frzColor },
            {ConditionsID.par, parColor },
            {ConditionsID.slp, slpColor },
        };


        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
        _pokemon.OnHPChanged += UpdateHP;
    }

    public void SetHPText(PokemonInfo pokemon)
    {
        _pokemon = pokemon;

        maxHPTextHUD.text = _pokemon.HP.ToString();
        curHPTextHUD.text = _pokemon.HP.ToString();
    }
    void SetStatusText()
    {
        if (_pokemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _pokemon.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_pokemon.Status.Id];
        }
    }
    public void SetLevel()
    {
        levelText.text = "Lvl  " + _pokemon.Level;
    }
    public void SetExp()
    {
        if (expBar == null) return;
        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }
    public IEnumerator SetExpSmooth(bool reset=false)
    {
        if (expBar == null) yield break;

        if (reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }
    float GetNormalizedExp()
    {
        int currLvlExp = _pokemon.Base.GetExpForLvl(_pokemon.Level);
        int nextLvlExp = _pokemon.Base.GetExpForLvl(_pokemon.Level + 1);

        float normalizedExp = (float)(_pokemon.Exp - currLvlExp) / (nextLvlExp - currLvlExp);
        return Mathf.Clamp01(normalizedExp);
    }
    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }
    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);
    }

    public IEnumerator WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.isUpdating == false);
    }

    public void ClearData()
    {
        if (_pokemon != null)
        {
            _pokemon.OnStatusChanged -= SetStatusText;
            _pokemon.OnHPChanged -= UpdateHP;
        }
    }
}
