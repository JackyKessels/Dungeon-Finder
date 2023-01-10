using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioSourceExtensions
{
    public static void FadeIn(this AudioSource a, AudioClip clip, float duration)
    {
        a.clip = clip;
        a.Play();
        a.GetComponent<MonoBehaviour>().StartCoroutine(FadeInCore(a, duration));
    }

    private static IEnumerator FadeInCore(AudioSource a, float duration)
    {
        float startVolume = a.volume;
        a.volume = 0;

        while (a.volume <= startVolume)
        {
            a.volume += startVolume * Time.deltaTime / duration;
            yield return new WaitForEndOfFrame();
        }

        a.volume = startVolume;
    }

    public static void FadeOut(this AudioSource a, float duration)
    {
        a.GetComponent<MonoBehaviour>().StartCoroutine(FadeOutCore(a, duration));
    }

    private static IEnumerator FadeOutCore(AudioSource a, float duration)
    {
        float startVolume = a.volume;

        while (a.volume > 0)
        {
            a.volume -= startVolume * Time.deltaTime / duration;
            yield return new WaitForEndOfFrame();
        }

        a.Stop();
        a.volume = startVolume;
    }


}