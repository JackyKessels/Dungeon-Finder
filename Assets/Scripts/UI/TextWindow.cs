using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextWindow : MonoBehaviour
{
    public TextMeshProUGUI title;
    public RectTransform contentContainer;
    public TextMeshProUGUI content;
    public Button continueButton;

    public void Update()
    {
        if (KeyboardHandler.ProgressWindow())
        {
            if (continueButton)
            {
                continueButton.onClick?.Invoke();
            }
        }
    }

    public void Setup(string titleText, string contentText, float windowWidth, float windowHeight)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(windowWidth, windowHeight);
        rectTransform.localScale = Vector3.one;

        name = titleText;

        // 164 is the height of the continue button + the height of the title container
        contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, windowHeight - 164);

        title.text = titleText;
        content.text = contentText;
    }

    public static void CreateTextWindow(string titleText, string contentText, float width, float height)
    {
        GameObject container = GameObject.Find("Text Window Container");

        GameObject obj = ObjectUtilities.CreateSimplePrefab(GameAssets.i.textWindow.gameObject, container);

        TextWindow textWindow = obj.GetComponent<TextWindow>();
        textWindow.Setup(titleText, contentText, width, height);
    }

    public void ContinueButton()
    {
        Destroy(gameObject);
    }
}
