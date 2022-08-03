using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Path", menuName = "Unit/Path")]
public class HeroPathObject : ScriptableObject
{
    [Header("[ General ]")]
    public new string name;
    public Sprite icon;

    [Header("[ Starter Path ]")]
    public List<Equipment> baseWeapons;
    public ActiveAbility primaryAbility;
    public ActiveAbility secondaryAbility;
    public PassiveAbility passiveAbility;

    [Header("[ Path Abilities ]")]
    public List<ActiveAbility> activeAbilities;
    public List<PassiveAbility> passiveAbilities;

    [Header("[ Attributes per Level ]")]
    public List<Attribute> attributes;

    public List<ActiveAbility> GetRandomActiveAbilities(int amount)
    {
        List<ActiveAbility> tempList = new List<ActiveAbility>(activeAbilities);
        List<ActiveAbility> newList = new List<ActiveAbility>();

        for (int i = 0; i < amount; i++)
        {
            int randomAbility = Random.Range(0, tempList.Count);

            newList.Add(tempList[randomAbility]);
            tempList.RemoveAt(randomAbility);
        }

        return newList;
    }
}
