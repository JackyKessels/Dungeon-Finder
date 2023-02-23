using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Database", menuName = "Systems/Ability Database Object")]
public class AbilityDatabaseObject : ScriptableObject
{
    public List<AbilityObject> abilityObjects;

    [ContextMenu("Update IDs")]
    public void UpdateId()
    {
        List<AbilityObject> noDupes = new List<AbilityObject>(new HashSet<AbilityObject>(abilityObjects));
        noDupes.RemoveAll(item => item == null);
        abilityObjects.Clear();
        abilityObjects.AddRange(noDupes);

        for (int i = 0; i < abilityObjects.Count; i++)
        {
            if (abilityObjects[i].id != i)
                abilityObjects[i].id = i;
        }
    }
}