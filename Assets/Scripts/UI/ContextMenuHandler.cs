using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ContextMenuHandler : MonoBehaviour
{
    #region Singleton
    public static ContextMenuHandler Instance { get; private set; }

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

    public GameObject contextMenu;
    public GameObject elementPrefab;

    private float width;
    private int totalElements;

    [HideInInspector] public bool overMenu;

    private void Update()
    {
        if (Input.GetMouseButton(0) && !overMenu)
        {
            HideContextMenu();
        }
    }

    public void ShowContextMenu(List<ContextMenuElement> elements, Vector3 position)
    {
        ObjectUtilities.ClearContainer(contextMenu);

        contextMenu.SetActive(true);

        SetupContextMenu(elements, position);
    }

    public void HideContextMenu()
    {
        contextMenu.SetActive(false);
    }

    private void SetupContextMenu(List<ContextMenuElement> elements, Vector2 position)
    {
        SetPosition(position);

        totalElements = 0;

        foreach (ContextMenuElement element in elements)
        {
            GameObject obj = ObjectUtilities.CreateSimplePrefab(elementPrefab, contextMenu);

            ContextMenuElementObject elementObject = obj.GetComponent<ContextMenuElementObject>();
            elementObject.Setup(element.text, element.health);

            if (totalElements == 0)
                elementObject.SetSmallCaps();

            Button button = obj.GetComponent<Button>();

            if (element.interactable)
            {
                button.interactable = element.interactable;

                if (element.action != null)
                {
                    button.onClick.AddListener(delegate { element.action(element.index, contextMenu); });
                }
                else
                {
                    button.onClick.AddListener(HideContextMenu);
                }
            }
            else
            {
                var newColorBlock = button.colors;
                newColorBlock.disabledColor = new Color(1, 1, 1, 1);
                button.colors = newColorBlock;
            
                elementObject.ColorText(element.color);
            }

            //width = obj.GetComponentInChildren<TextMeshProUGUI>().preferredWidth > width ? obj.GetComponentInChildren<TextMeshProUGUI>().preferredWidth : width;

            totalElements++;
        }
    }

    public void SetPosition(Vector3 position)
    {
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;

        float heightRatio = screenHeight / Screen.currentResolution.height;
        float widthRatio = screenWidth / Screen.currentResolution.width;

        // Object height scaled with screen height
        float contextMenuHeight = GetDimensions().y * heightRatio;
        // Object width scaled with screen width
        float contextMenuWidth = GetDimensions().x * widthRatio;

        Vector3 tempPos = position;

        if (tempPos.y + contextMenuHeight > screenHeight)
        {
            tempPos = new Vector3(tempPos.x, screenHeight - contextMenuHeight, tempPos.z);
        }

        if (tempPos.x + contextMenuWidth > screenWidth)
        {
            tempPos = new Vector3(screenWidth - contextMenuWidth, tempPos.y, tempPos.z);
        }

        contextMenu.transform.position = tempPos;
    }

    public Vector2 GetDimensions()
    {
        int x = 300 + 8; // TextWidth + Padding
        int y = totalElements * 48 + 8;

        return new Vector2(x, y);
    }
}

[System.Serializable]
public class ContextMenuElement
{
    public string text;
    public string health = "";
    public int index;
    public Action<int, GameObject> action;
    public bool interactable;
    public string color;

    public ContextMenuElement(Hero hero, int index, Action<int, GameObject> action)
    {     
        text = hero.heroObject.name;
        health = hero.statsManager.currentHealth + " / " + hero.statsManager.GetAttributeValue(AttributeType.Health);
        this.index = index;
        this.action = action;
        this.interactable = true;
    }

    public ContextMenuElement(string text, int index, Action<int, GameObject> action)
    {
        this.text = text;
        this.index = index;
        this.action = action;
        this.interactable = true;
    }

    public ContextMenuElement(string text)
    {
        this.text = text;
        this.interactable = true;
    }

    public ContextMenuElement(string text, bool interactable, string color)
    {
        this.text = text;
        this.interactable = interactable;
        this.color = color;
    }
}
