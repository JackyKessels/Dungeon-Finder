using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationBox : MonoBehaviour
{
    public TextMeshProUGUI titleObject;
    public GameObject buttonsContainer;
    public Button buttonPrefab;

    [HideInInspector] public int result = 0;

    public void Setup(string title, List<ConfirmationButton> buttons)
    {
        titleObject.text = title;

        for (int i = 0; i < buttons.Count; i++)
        {
            AddButton(buttons[i], i + 1);
        }
    }

    public void AddButton(ConfirmationButton button, int index)
    {
        Button b = Instantiate(buttonPrefab, buttonsContainer.transform.position, Quaternion.identity);
        b.transform.SetParent(buttonsContainer.transform);
        b.transform.localScale = Vector3.one;
        b.GetComponentInChildren<TextMeshProUGUI>().text = button.name;
        b.onClick.AddListener(delegate { OnButtonPress(button, index); });
    }

    public void OnButtonPress(ConfirmationButton button, int index)
    {
        result = index;

        button.function?.Invoke();
    }
}

public class ConfirmationButton
{
    public string name;
    public Action function;

    public ConfirmationButton(string name, Action function)
    {
        this.name = name;
        this.function = function;
    }
}
