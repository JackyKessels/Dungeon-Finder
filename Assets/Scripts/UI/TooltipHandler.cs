﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipHandler : MonoBehaviour
{
    #region Singleton
    public static TooltipHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Instance already exists.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    public GameObject tooltip;
    public TextMeshProUGUI tooltipText;
    public TooltipObject tempTooltip;

    private IHasTooltip lastObj;
    private TooltipObject lastTooltip;
    private Vector3 lastPosition;

    private void Update()
    {
        if (KeyboardHandler.CompareTooltip())
        {
            if (tooltip.activeSelf)
            {
                ShowValuesTooltip();
            }
        }

        if (KeyboardHandler.StopCompareTooltip())
        {
            if (tooltip.activeSelf)
            {
                ShowTooltip(lastObj, lastTooltip, lastPosition);
            }
        }
    }

    public void ShowTooltip(IHasTooltip obj, TooltipObject tooltipInfo, Vector3 position)
    {
        if (obj == null)
            return;

        lastObj = obj;
        lastTooltip = tooltipInfo;
        lastPosition = position;

        tooltip.SetActive(true);
        tooltipText.text = obj.GetCompleteTooltip(tooltipInfo);
        SetPosition(position);
    }

    private void ShowValuesTooltip()
    {
        if (lastObj == null)
            return;

        tempTooltip.SetTooltip(lastTooltip);
        tempTooltip.state = CurrentState.Values;
        tooltipText.text = lastObj.GetCompleteTooltip(tempTooltip);
        SetPosition(lastPosition);
    }

    private void CleanHandler()
    {
        TooltipObject[] tooltipObjects = GetComponents<TooltipObject>();

        if (GetComponent<TooltipObject>() != null)
        {
            foreach (var comp in tooltipObjects)
            {
                Destroy(comp);
            }
        }
    }

    public void ShowTooltip(string s, Vector3 position)
    {
        tooltip.SetActive(true);
        tooltipText.text = s;
        SetPosition(position);
    }

    public float GetWidth()
    {
        float screenWidth = Screen.width;

        float widthRatio = screenWidth / Screen.currentResolution.width;

        return 400 * widthRatio;
    }

    public void SetPosition(Vector3 position)
    {
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;

        float heightRatio = screenHeight / Screen.currentResolution.height;
        float widthRatio = screenWidth / Screen.currentResolution.width;

        // Object height scaled with screen height
        float tooltipHeight = tooltipText.preferredHeight * heightRatio;
        float tooltipPadding = 16 * heightRatio;
        // Object width scaled with screen width
        float tooltipWidth = GetWidth();

        Vector3 tempPos = position;

        if (tempPos.y + tooltipHeight + tooltipPadding > screenHeight)
        {
            tempPos = new Vector3(tempPos.x, screenHeight - tooltipHeight - tooltipPadding, tempPos.z);
        }

        if (tempPos.x + tooltipWidth > screenWidth)
        {
            tempPos = new Vector3(screenWidth - tooltipWidth, tempPos.y, tempPos.z);
        }

        tooltip.transform.position = tempPos;
    }

    public void HideTooltip()
    {
        if (tooltip != null && tooltip.activeSelf)
        {
            tooltip.SetActive(false);
        }
    }
}
