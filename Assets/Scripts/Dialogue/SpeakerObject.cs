using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerObject : MonoBehaviour
{
    public Image image;
    public new TextMeshProUGUI name;
    public TextMeshProUGUI content;
    public UnitObject speaker;

    public void Setup(UnitObject unitObject)
    {
        speaker = unitObject;
        image.sprite = speaker.sprite;
        image.rectTransform.sizeDelta = new Vector2(image.sprite.rect.width, image.sprite.rect.height);
        name.text = GeneralUtilities.GetFullUnitName(speaker);
        content.text = "";
    }

    IEnumerator TypeSentence(string sentence)
    {
        content.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            content.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
    }

    public void SetText(string text)
    {
        StopAllCoroutines();
        StartCoroutine(TypeSentence(text));
    }

    public void Show(bool show)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(show);
        }
    }

    public void SetActiveSpeaker()
    {
        image.color = new Color(1f, 1f, 1f);
        content.color = new Color(1f, 1f, 1f);
    }

    public void SetInactiveSpeaker()
    {
        image.color = new Color(0.5f, 0.5f, 0.5f);
        content.color = new Color(0.5f, 0.5f, 0.5f);
    }
}
