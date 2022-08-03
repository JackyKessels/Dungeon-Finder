using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SellType
{
    Items,
    Abilities
}

public class SellSystem : MonoBehaviour
{
    [Header("[ Sell Settings ]")]
    public SellType sellType = SellType.Items;
    public float sellFraction = 0.5f;

    [Header("[ Editor Objects ]")]
    public GameObject sellSlotObject;
    public InventorySlot inventorySlot = null;
    public Active active = null;
    public TextMeshProUGUI sellAmount;

    private SellSlot sellSlot;

    private void Start()
    {
        sellSlot = sellSlotObject.AddComponent<SellSlot>();
        sellSlot.Setup(this);
    }

    public void UpdateSystem(InventorySlot slot = null, Active active = null)
    {
        inventorySlot = slot;
        this.active = active;

        if (inventorySlot != null)
        {
            sellSlotObject.GetComponent<Image>().sprite = slot.ItemObject.icon;
            sellSlotObject.GetComponent<TooltipObject>().itemObject = slot.ItemObject;
            sellSlotObject.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount == 1 ? "" : slot.amount.ToString("n0");

            if (sellAmount != null)
                sellAmount.text = GetSellValue(slot).ToString();
        }
        else if (this.active != null)
        {
            sellSlotObject.GetComponent<Image>().sprite = this.active.activeAbility.icon;
            sellSlotObject.GetComponent<TooltipObject>().active = this.active;
            sellSlotObject.GetComponentInChildren<TextMeshProUGUI>().text = "";

            if (sellAmount != null)
                sellAmount.text = GetSellValue(active).ToString();
        }
        else
        {
            sellSlotObject.GetComponent<Image>().sprite = GameAssets.i.emptySlot;
            sellSlotObject.GetComponent<TooltipObject>().itemObject = null;
            sellSlotObject.GetComponent<TooltipObject>().active = new Active();
            sellSlotObject.GetComponentInChildren<TextMeshProUGUI>().text = "";

            if (sellAmount != null)
                sellAmount.text = "";
        }
    }

    public void SellItem()
    {
        if (!sellSlot.filled)
        {
            return;
        }

        if (sellType == SellType.Items && inventorySlot.ItemObject.sellable)
        {
            Currency currency = new Currency(CurrencyType.Gold, GetSellValue(inventorySlot));

            inventorySlot.RemoveItem();

            GameManager.Instance.currencyHandler.IncreaseCurrency(currency);

            sellSlot.filled = false;

            UpdateSystem();
        }
        else if (sellType == SellType.Abilities && active.activeAbility.sellable)
        {
            if (active.owner.spellbook.abilityCollection.Count > 4)
            {
                Currency currency = new Currency(CurrencyType.Spirit, GetSellValue(active));

                active.owner.spellbook.UnlearnAbility(active);

                active = null;

                GameManager.Instance.currencyHandler.IncreaseCurrency(currency);

                sellSlot.filled = false;

                TownManager.Instance.enchanter.SetupShop();

                UpdateSystem();
            }
            else
            {
                NotificationObject.SendNotification("Can only sell abilities when you have more than 4.");
            }
        }
    }

    private int GetSellValue(InventorySlot slot)
    {
        int value = inventorySlot.ItemObject.value;

        return (int)(value * sellFraction);
    }

    private int GetSellValue(Active activeAbility)
    {
        float value = Shop.GetAbilityCost(activeAbility);

        return (int)(value * sellFraction);
    }
}

public class SellSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SellSystem sellSystem;
    public InventorySlot inventorySlot;
    public Active activeAbility;

    public bool filled = false;

    public void Setup(SellSystem ss)
    {
        sellSystem = ss;
    }

    public void UpdateSlot(InventorySlot slot)
    {
        inventorySlot = slot;

        sellSystem.UpdateSystem(inventorySlot, null);
        filled = true;
    }

    public void UpdateSlot(Active active)
    {
        activeAbility = active;

        sellSystem.UpdateSystem(null, activeAbility);
        filled = true;
    }

    public void ClearSlot()
    {
        sellSystem.UpdateSystem();
        filled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseData.slotHoveredOver = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseData.slotHoveredOver = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ClearSlot();
        }
    }
}