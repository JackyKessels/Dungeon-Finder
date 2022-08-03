using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathPassive : MonoBehaviour
{
    [SerializeField] Image passiveImage;
    [SerializeField] Button passiveButton;
    [SerializeField] TooltipObject passiveTooltip;

    private Passive passive;

    public bool learned = false;
    public int index;
    
    public void Setup(PassiveAbility _passiveAbility, int _index, bool _learned, HeroPathGameObject _heroPathGameObject)
    {
        passive = new Passive(_passiveAbility, 1);

        learned = _learned;
        index = _index;

        passiveImage.sprite = passive.passiveAbility.icon;
        passiveTooltip.passive = passive;
        passiveTooltip.state = CurrentState.HeroInformation;

        passiveButton.onClick.AddListener(delegate { PassiveButton(_heroPathGameObject); });

        UpdateStatus();
    }
    
    private void PassiveButton(HeroPathGameObject heroPathGameObject)
    {
        // Unlearn
        if (learned)
        {
            learned = false;

            heroPathGameObject.heroPath.unlockedPassives[index] = false;

            passive.DeactivatePassive(heroPathGameObject.hero);
        }
        // Learn
        else
        {
            if (heroPathGameObject.heroPath.CanUnlockPassive())
            {
                learned = true;

                heroPathGameObject.heroPath.unlockedPassives[index] = true;

                passive.ActivatePassive(heroPathGameObject.hero);
            }
            else
            {
                ShortMessage.SendMessage(Input.mousePosition, "Cannot unlock passive.", 24, Color.red);
            }
        }

        heroPathGameObject.UpdateCounter();

        InventoryManager.Instance.UpdateCharacterAttributes(heroPathGameObject.hero, -1);

        UpdateStatus();
    }

    private void UpdateStatus()
    {
        if (learned)
        {
            passiveImage.color = new Color(1.0f, 1.0f, 1.0f);
        }
        else
        {
            passiveImage.color = new Color(0.5f, 0.5f, 0.5f);
        }
    }
}
