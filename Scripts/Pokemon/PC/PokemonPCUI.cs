using GDE.GenericSelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class PokemonPCUI : MonoBehaviour
{
    [SerializeField] GameObject pcList;
    [SerializeField] PokemonPCSlotUI pokemonSlotUI;
    [SerializeField] PokemonParty party;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [SerializeField] PartyScreen partyScreen;
    public PCList list;

    List<PokemonPCSlotUI> pokemonUIList;
    [SerializeField]RectTransform itemListRect;

    public int itemsInViewport = 18;

    [SerializeField] Text nameText;
    [SerializeField] Text lvlText;
    [SerializeField] Image img;

    PokemonInfo chosenPok;
    int selection = 0;
    float selectionTimer = 0.2f;
    public int offset;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateList();
    }


    private void Start()
    {

    }

    void UpdateList()
    {
        //clear existing item in list
        foreach (Transform slots in pcList.transform)
            Destroy(slots.gameObject);

        pokemonUIList = new List<PokemonPCSlotUI>();
        var pokemonAry = list.pokemonList;

        //create new pokemon slot
        foreach (var pokemon in pokemonAry)
        {
            var slotUIObj = Instantiate(pokemonSlotUI, pcList.transform);
            slotUIObj.SetData(pokemon);

            pokemonUIList.Add(slotUIObj);
            slotUIObj.gameObject.GetComponent<Image>().enabled = true;

            Text[] texts = slotUIObj.gameObject.GetComponentsInChildren<Text>();
            nameText = texts[0];
            lvlText = texts[1];

            nameText.enabled = true;
            lvlText.enabled = true;
            img.enabled = true;
        }

        HandleScrolling();
        UpdateMemberSelection(selection);
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevSelection = selection;

        selection = Mathf.Clamp(selection, 0, pokemonUIList.Count - 1);
        UpdateTimer();
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        if (selectionTimer == 0 && Mathf.Abs(v) > 0.2)
        {
            selection += -(int)Mathf.Sign(v)*3;
            selectionTimer = 0.2f;
            UpdateList();
        }
        else if (selectionTimer == 0 && Mathf.Abs(h) > 0.2)
        {
            selection += (int)Mathf.Sign(h);
            selectionTimer = 0.2f;
            UpdateList();
        }

        if (selection != prevSelection)
        {
            UpdateMemberSelection(selection);
            HandleScrolling();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            chosenPok = list.pokemonList[selection];
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < pokemonUIList.Count; i++)
        {
            if (i == selectedMember)
                pokemonUIList[i].SetSelected(true);
            else
                pokemonUIList[i].SetSelected(false);
        }
    }

    void HandleScrolling()
    {
        if (pokemonUIList.Count <= itemsInViewport) return;

        float scrollpos = Mathf.Clamp(selection - itemsInViewport/2, 0, selection) * (pokemonUIList[0].Height/1.75f) - offset;

        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollpos);

        bool showUpArrow = selection > itemsInViewport / 2;
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selection + itemsInViewport / 2 < pokemonUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }

    void UpdateTimer()
    {
        if (selectionTimer > 0)
        {
            selectionTimer = Mathf.Clamp(selectionTimer - Time.deltaTime, 0, selectionTimer);
        }
    }


    public PokemonInfo ChosenPok => chosenPok;
}
