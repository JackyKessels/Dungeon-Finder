using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShortMessage : MonoBehaviour
{
    private TextMeshProUGUI textMessage;
    private float fade;
    private float fadeTimer;
    private float speed;

    private Color textColor;

    private void Awake()
    {
        textMessage = transform.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        transform.position += new Vector3(0, speed) * Time.deltaTime;

        fadeTimer -= Time.deltaTime;

        if (fadeTimer < 0)
        {
            textColor.a -= fade * Time.deltaTime;
            textMessage.color = textColor;

            if (textColor.a < 0)
                Destroy(gameObject);
        }
    }

    public static ShortMessage SendMessage(Vector3 position, string text, int fontSize, Color color)
    {
        Transform transform = Instantiate(GameAssets.i.shortMessage, position, Quaternion.identity);
        transform.SetParent(GameObject.Find("Short Message Container").transform);

        ShortMessage message = transform.GetComponent<ShortMessage>();
        message.Setup(text, fontSize, color);

        return message;
    }

    public void Setup(string text, int fontSize, Color color)
    {
        textMessage.text = text;
        textMessage.fontSize = fontSize;
        textColor = color;
        textMessage.color = textColor;

        fade = 2f;
        fadeTimer = 0.5f;
        speed = 100f;
    }
}
