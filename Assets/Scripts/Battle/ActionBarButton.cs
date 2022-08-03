using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hotkeyText;
    public bool interactable = true;
    public bool active = true;

    public void SetInteractable(bool value)
    {
        if (value)
        {
            interactable = true;
            EnableButton();
        }
        else
        {
            interactable = false;
            DisableButton();
        }
    }

    private void DisableButton()
    {
        Image buttonImage = GetComponent<Image>();
        buttonImage.color = new Color(0.5f, 0.5f, 0.5f);
    }

    private void EnableButton()
    {
        Image buttonImage = GetComponent<Image>();
        buttonImage.color = new Color(1f, 1f, 1f);
    }

    public void SetHotkeyText(int index)
    {
        hotkeyText.text = (index + 1).ToString();
    }
}
