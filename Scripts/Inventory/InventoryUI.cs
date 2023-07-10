using GDE.GenericSelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, MoveToForget, Busy }

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image icon;
    [SerializeField] Text itemDescription;
    [SerializeField] Text categoryText;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveForgetSelection moveForgetSelection;



    Action<ItemBase> onItemUsed;

    int selectedItem = 0;
    int selectedCategory = 0;

    MovesBase moveToLearn;

    InventoryUIState state;

    [SerializeField] int itemsInViewport;

    List<ItemSlotUI> slotUIList;
    Inventory inventory;
    [SerializeField] RectTransform itemListRect;

    public int offset;
    float selectionTimer = 0.2f;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Start()
    {
        UpdateList();
        inventory.OnUpdated += UpdateList;
    }

    void UpdateList()
    {
        //clear existing item in list
        foreach(Transform slots in itemList.transform)
            Destroy(slots.gameObject);

        slotUIList = new List<ItemSlotUI>();

        //create new item slot
        foreach(var itemslot in inventory.GetSlotsByCategory(selectedCategory))
        {
           var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
           slotUIObj.SetData(itemslot);

           slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed=null)
    {
        this.onItemUsed = onItemUsed;

        if (state == InventoryUIState.ItemSelection)
        {
            int prevSelection = selectedItem;
            int prevCategory = selectedCategory;

            if (Input.GetKeyDown(KeyCode.RightArrow))
                ++selectedCategory;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                --selectedCategory;

            if (selectedCategory > Inventory.ItemCategories.Count - 1)
                selectedCategory = 0;
            else if (selectedCategory < 0)
                selectedCategory = Inventory.ItemCategories.Count - 1;

            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);
            UpdateTimer();
            float v = Input.GetAxis("Vertical");

            if (selectionTimer == 0 && Mathf.Abs(v) > 0.2)
            {
                selectedItem += -(int)Mathf.Sign(v);
                selectionTimer = 0.2f;
                UpdateList();
            }
            if (prevCategory != selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateList();
            }
            else if (prevSelection != selectedItem)
            {
                UpdateItemSelection();
                HandleScrolling();
            }

            if (Input.GetKeyDown(KeyCode.Z))
                StartCoroutine(ItemSelected());
            else if (Input.GetKeyDown(KeyCode.X))
                onBack?.Invoke();
        }
        else if (state == InventoryUIState.PartySelection)
        {

            Action onSelected = () =>
            {
                StartCoroutine(UseItem());
            };

            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };

            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
        else if (state == InventoryUIState.MoveToForget)
        {
            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            };

            moveForgetSelection.HandleMoveSelection(onMoveSelected);
        }
    }

    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);

        if (GameController.Instance.State == GameState.Battle)
        {
            // In Battle
            if (!item.CanUseInBattle)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"This item cannot be used in battle");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
            else
            {
                UseItem();
            }
        }
        else
        {
            // Outside Battle
            if (!item.CanUseOutsideBattle)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"This item cannot be used outside battle");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }

        if (selectedCategory == (int)ItemCategory.Items || selectedCategory == (int)ItemCategory.Tms)
        {
            OpenPartyScreen();

            if (item is TMItem)
                partyScreen.ShowIfTmUsable(item as TMItem);
        }
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        yield return HandleTmItems();

        var item = inventory.GetItem(selectedItem, selectedCategory);
        var pokemon = partyScreen.SelectedMember;

        // Handle Evolution/Mega Items
        if (item is EvoItem)
        {
            var evolution = pokemon.CheckForEvolution(item);
            if (evolution != null && BattleSystem.i.didMegaPlay == false)
            {
                yield return EvolutionManager.i.Evolve(pokemon, evolution);
                BattleSystem.i.didMegaPlay = true;
                BattleSystem.i.SetupMegaBattlePlayer();

            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogueText($"It won't have any effect!");
                ClosePartyScreen();
                yield break;
            }
        }

        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if (usedItem != null)
        {
            if (usedItem is RecoveryItems)
                yield return DialogueManager.Instance.ShowDialogueText($"You used a/an{usedItem.Name}");

            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            if (selectedCategory == (int)ItemCategory.Items)
                yield return DialogueManager.Instance.ShowDialogueText($"It won't have any effect!");
        }

        ClosePartyScreen();
    }

    IEnumerator HandleTmItems()
    {
        var tmItem = inventory.GetItem(selectedItem, selectedCategory) as TMItem;
        if (tmItem == null)
            yield break;

        var pokemon = partyScreen.SelectedMember;

        if (pokemon.HasMove(tmItem.Move))
        {
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.Base.Name} already know {tmItem.Move.Name}");
            yield break;
        }

        if (!tmItem.CanBeTaught(pokemon))
        {
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.Base.Name} can't learn {tmItem.Move.Name}");
            yield break;
        }

        if (pokemon.Moves.Count < 4)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.Base.Name} learned {tmItem.Move.Name}");
        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.Base.Name} is trying to learn {tmItem.Move.Name}");
            yield return DialogueManager.Instance.ShowDialogueText($"But it cannot learn more than 4 moves");
            yield return ChooseMoveToForget(pokemon, tmItem.Move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
        }
    }


    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);

        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }

        if (slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            icon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }

        HandleScrolling();
    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewport) return;


        float scrollpos = Mathf.Clamp(selectedItem - itemsInViewport/2, 0, selectedItem) * (slotUIList[0].Height * 2) - offset;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollpos);

        bool showUpArrow = selectedItem > itemsInViewport / 2;
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItem + itemsInViewport / 2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }
    void ResetSelection()
    {
        selectedItem = 0;
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);
        icon.sprite = null;
        itemDescription.text = "";
    }
    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);

        gameObject.GetComponent<CanvasRenderer>().SetAlpha(1);

        foreach (var rend in gameObject.GetComponentsInChildren<CanvasRenderer>())
            rend.SetAlpha(0);
    }

    IEnumerator ChooseMoveToForget(PokemonInfo pokemon, MovesBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogueManager.Instance.ShowDialogueText($"Choose a move you want to forget", true, false);
        moveForgetSelection.gameObject.SetActive(true);
        moveForgetSelection.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = InventoryUIState.MoveToForget;
    }

    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        partyScreen.ClearMemberSlotMessages();
        partyScreen.gameObject.SetActive(false);

        gameObject.GetComponent<CanvasRenderer>().SetAlpha(1);

        foreach (var rend in gameObject.GetComponentsInChildren<CanvasRenderer>())
            rend.SetAlpha(1);
    }

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var pokemon = partyScreen.SelectedMember;

        DialogueManager.Instance.CloseDialog();
        moveForgetSelection.gameObject.SetActive(false);
        if (moveIndex == 4)
        {
            // Don't learn the new move
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.Base.Name} did not learn {moveToLearn.Name}");
        }
        else
        {
            // Forget the selected move and learn new move
            var selectedMove = pokemon.Moves[moveIndex].Base;
            yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.Base.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}");

            pokemon.Moves[moveIndex] = new Move(moveToLearn);
        }

        ClosePartyScreen();
    }

    void UpdateTimer()
    {
        if (selectionTimer > 0)
        {
            selectionTimer = Mathf.Clamp(selectionTimer - Time.deltaTime, 0, selectionTimer);
        }
    }

    public ItemBase SelectedItem => inventory.GetItem(selectedItem, selectedCategory);
    public int SelectedCategory => selectedCategory;
}
