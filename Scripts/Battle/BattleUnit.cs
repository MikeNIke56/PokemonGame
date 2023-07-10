using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;


    [SerializeField] GameObject[] animToPlay;
    [SerializeField] Transform animPos;

    public float waittime;

    public IEnumerator PlayTypeAnimation(PokemonType type)
    {
        GameObject animObj = null;
        switch(type)
        {
            case PokemonType.Normal:
                animObj = Instantiate(animToPlay[0], animPos);
                break;
            case PokemonType.Fire:
                animObj = Instantiate(animToPlay[1], animPos);
                break;
            case PokemonType.Water:
                animObj = Instantiate(animToPlay[2], animPos);
                break;
            case PokemonType.Electric:
                animObj = Instantiate(animToPlay[3], animPos);
                break;
            case PokemonType.Psychic:
                animObj = Instantiate(animToPlay[4], animPos);
                break;
            case PokemonType.Fighting:
                animObj = Instantiate(animToPlay[5], animPos);
                break;
            case PokemonType.Grass:
                animObj = Instantiate(animToPlay[6], animPos);
                break;
            case PokemonType.Ground:
                animObj = Instantiate(animToPlay[7], animPos);
                break;
            case PokemonType.Rock:
                animObj = Instantiate(animToPlay[8], animPos);
                break;
            case PokemonType.Flying:
                animObj = Instantiate(animToPlay[9], animPos);
                break;
            case PokemonType.Bug:
                animObj = Instantiate(animToPlay[10], animPos);
                break;
            case PokemonType.Dark:
                animObj = Instantiate(animToPlay[11], animPos);
                break;
            case PokemonType.Ghost:
                animObj = Instantiate(animToPlay[12], animPos);
                break;
            case PokemonType.Steel:
                animObj = Instantiate(animToPlay[13], animPos);
                break;
            case PokemonType.Fairy:
                animObj = Instantiate(animToPlay[14], animPos);
                break;
            case PokemonType.Dragon:
                animObj = Instantiate(animToPlay[15], animPos);
                break;
            case PokemonType.Poison:
                animObj = Instantiate(animToPlay[16], animPos);
                break;
            case PokemonType.Ice:
                animObj = Instantiate(animToPlay[17], animPos);
                break;
        }

        yield return new WaitForSeconds(waittime);
        Destroy(animObj);
        yield return null;
    }

    public bool IsPlayerUnit
    {
        get { return isPlayerUnit; }
        
    }

    public BattleHud Hud
    {
        get { return hud; }
    }

    public PokemonInfo Pokemon { get; set; }

    Image image;
    Vector3 originalPos;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(PokemonInfo pokemonInfo)
    {
        Pokemon = pokemonInfo;
        if (isPlayerUnit)
        {
            hud.SetHPText(pokemonInfo);
            image.sprite = Pokemon.Base.Backsprite;
        }
        else
            image.sprite = Pokemon.Base.Frontsprite;

        hud.gameObject.SetActive(true);
        hud.SetData(pokemonInfo);

        image.color = originalColor;
        transform.localScale = new Vector3(1, 1, 1);
        PlayerEnterAnimation();
    }
    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }
    public void PlayerEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, originalPos.y);

        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
           sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, .25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, .25f));

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, .25f));
    }
    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, .1f));
        sequence.Append(image.DOColor(originalColor, .1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, .5f));
        sequence.Join(image.DOFade(0f, .5f));
    }
    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0, .5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 50f, .5f));
        sequence.Join(transform.DOScale(new Vector3(.3f, .3f, 1f), .5f));

        yield return sequence.WaitForCompletion();
    }
    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1, .5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y, .5f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), .5f));

        yield return sequence.WaitForCompletion();
    }
}
