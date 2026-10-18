using UnityEngine;

public class RemainingLoot
{
    public Item item;
    public ItemObject itemObject;
    public int amount;

    public RemainingLoot(ItemDrop itemDrop, int amount)
    {
        item = itemDrop.GetItem();
        itemObject = itemDrop.itemObject;
        this.amount = amount;
    }
}