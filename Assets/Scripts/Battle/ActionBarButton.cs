using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ActionBarButton : MonoBehaviour
{
    public Image icon;
    public TooltipObject tooltip;
    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI hotkeyText;
    public GameObject crossed;

    public bool interactable = true;
    public bool isActive = true;

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

    public void SetHotkeyText(int index)
    {
        hotkeyText.text = (index + 1).ToString();
    }
    
    public void CrossButton(bool active)
    {
        crossed.SetActive(active);
    }

    public void SetupActionBarAbility(Sprite _sprite, Active _active, CurrentState _state)
    {
        icon.sprite = _sprite;
        tooltip.active = _active;
        tooltip.state = _state;

        cooldownText.SetText("");
    }

    public void SetupEmptyActionBarAbility(bool locked)
    {
        Sprite sprite;

        if (locked)
        {
            sprite = GameAssets.i.lockedAbility;
        }
        else
        {
            sprite = GameAssets.i.noAbility;
        }

        SetupActionBarAbility(
            sprite,
            null,
            CurrentState.Battle);
    }

    public void SetButtonCooldown(Active active)
    {
        if (active.IsOnCooldown())
        {
            SetInteractable(false);

            if (active.currentCooldown == ActiveAbility.SINGLE_USE_COOLDOWN)
            {
                CrossButton(true);
                cooldownText.SetText("");
            }
            else
            {
                // Swift ability
                if (active.currentCooldown > active.cooldown)
                {
                    cooldownText.SetText("");
                }
                else
                {
                    cooldownText.SetText(active.currentCooldown.ToString());
                }
            }
        }
        else
        {
            CrossButton(false);
            cooldownText.SetText("");
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


}
