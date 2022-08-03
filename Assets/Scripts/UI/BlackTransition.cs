using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackTransition : MonoBehaviour
{
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;

    public float fadeDuration;
    public bool fadeIn;

    private float fadeSpeed;

    private void Start()
    {
        fadeSpeed = Time.deltaTime / fadeDuration;
    }

    private void Update()
    {
        fadeDuration -= Time.deltaTime;

        if (fadeIn)
        {

            canvasGroup.alpha -= fadeSpeed;
        }
        else
        {
            canvasGroup.alpha += fadeSpeed;
        }

        if (fadeDuration <= 0)
        {
            Destroy(gameObject);
        }
            
    }
}
