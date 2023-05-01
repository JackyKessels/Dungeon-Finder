using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class SoundUtilities
{
    public static void PlayClick()
    {
        GameManager.Instance.audioSourceSFX.PlayOneShot(GameAssets.i.click);
    }

}
