using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    public GameObject windowToClose;

    public void CloseWindow()
    {
        windowToClose.SetActive(false);
    }

    public void DestroyWindow()
    {
        Destroy(windowToClose);
    }
}
