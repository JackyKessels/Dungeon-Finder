using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Collection", menuName = "World/Shop/Shop Collection")]
public class LevelBracketShopCollection : ScriptableObject
{
    public List<ShopCollectionBracket> shopCollectionBrackets;

    public List<ItemObject> GetCorrectBracketItemObjects(int level)
    {
        for (int i = 0; i < shopCollectionBrackets.Count; i++)
        {
            if (shopCollectionBrackets[i].ValidBracket(level))
                return shopCollectionBrackets[i].itemObjects;
        }

        return null;
    }
}

[System.Serializable]
public class ShopCollectionBracket
{
    public int minimumLevel;
    public int maximumLevel;

    public List<ItemObject> itemObjects;

    public bool ValidBracket(int level)
    {
        if (level >= minimumLevel && level <= maximumLevel)
            return true;

        return false;
    }
}
