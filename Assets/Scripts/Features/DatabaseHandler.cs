using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseHandler : MonoBehaviour
{
    #region Singleton
    public static DatabaseHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Instance already exists.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    public ItemDatabaseObject itemDatabase;
    public AbilityDatabaseObject abilityDatabase;
    public GlyphDatabaseObject glyphDatabase;
}
