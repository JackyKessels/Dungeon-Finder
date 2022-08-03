using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfirmationBoxHandler : MonoBehaviour
{
    #region Singleton
    public static ConfirmationBoxHandler Instance { get; private set; }

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

    public GameObject boxContainer;
    public ConfirmationBox boxPrefab;

    public void SetupConfirmationBox(string boxName, List<ConfirmationButton> buttons)
    {
        StartCoroutine(CreateConfirmationBox(boxName, buttons));
    }

    IEnumerator CreateConfirmationBox(string boxName, List<ConfirmationButton> buttons)
    {
        ConfirmationBox confirmationBox = Instantiate(boxPrefab, boxContainer.transform.position, Quaternion.identity);
        confirmationBox.transform.SetParent(boxContainer.transform);
        confirmationBox.transform.localScale = Vector3.one;

        confirmationBox.Setup(boxName, buttons);

        while (confirmationBox.result == 0)
            yield return null;

        if (confirmationBox.result > 0)
        {
            Destroy(confirmationBox.gameObject);
        }
    }
}
