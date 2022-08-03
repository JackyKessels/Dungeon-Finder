using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QueueManager : MonoBehaviour
{
    #region Singleton

    public static QueueManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 instance of QueueManager found!");
            return;
        }

        Instance = this;
    }

    #endregion

    public GameObject orderObject;

    public float width;
    public float height;

    public Unit[] queueList;
    public List<Unit> speedList;
    public List<GameObject> iconsList;

    public int queueTurn;
    public int queueSize;

    private void Start()
    {
        speedList = new List<Unit>();
        iconsList = new List<GameObject>();
    }

    public void Setup()
    {
        queueTurn = 0;
        queueSize = 0;

        speedList = speedList.OrderByDescending(x => x.statsManager.GetAttributeValue((int)AttributeType.Speed)).ToList();

        Refresh(true);
    }

    public void Refresh(bool refreshTurn)
    {
        queueSize = speedList.Count;
        queueList = new Unit[queueSize];

        ClearIcons();

        List2Array();

        foreach (Unit u in queueList)
        {
            if (refreshTurn)
                u.hasTurn = true;

            AddToOrder(u);
            SetIcon(u);
        }
    }

    public void RemoveQueue()
    {
        ClearLists();
        ClearIcons();
    }

    public void AddToSpeedList(Unit u)
    {
        speedList.Add(u);
    }

    public void RemoveFromSpeedList(Unit u)
    {
        speedList.Remove(u);
    }

    public void List2Array()
    {
        for (int i = 0; i < queueList.Length; i++)
        {
            queueList[i] = speedList[i];
        }
    }

    public void ClearLists()
    {
        for (int i = 0; i < queueList.Length; i++)
        {
            queueList[i] = null;
        }

        speedList.Clear();
    }

    // Queue Icons Logic //

    public void ClearIcons()
    {
        ObjectUtilities.ClearContainer(orderObject);

        iconsList.Clear();
    }

    public void AddToOrder(Unit u)
    {
        GameObject newObj = new GameObject();                                       // Create the GameObject.
        Image newImage = newObj.AddComponent<Image>();                              // Add the Image Component script.
        newImage.sprite = u.icon;                                               // Set the Sprite of the Image Component on the new GameObject.
        newObj.GetComponent<RectTransform>().SetParent(orderObject.transform);       // Assign the newly created Image GameObject as a Child of the Parent Panel.
        newObj.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);// Change size of image.

        if (u.isEnemy)
            newObj.GetComponent<RectTransform>().localScale = new Vector3(-1, 1, 1);
        else
            newObj.GetComponent<RectTransform>().localScale = Vector3.one;

        newObj.SetActive(true);                                                     // Activate the GameObject.
        iconsList.Add(newObj);
        u.orderIcon = newObj;
    }

    public void SetIcon(Unit u)
    {
        if (u == null)
            return;
        
        if (u.hasTurn)
        {
            u.orderIcon.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
        }
        else
        {
            u.orderIcon.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
        }
    }

    // Extra //

    public Hero GetFastestHero()
    {
        for (int i = 0; i < speedList.Count; i++)
        {
            if (!speedList[i].isEnemy)
                return (Hero)speedList[i];
        }
        return null;
    }


    /*
    public void AddGlow(int i)
    {
        orderGroup.transform.GetChild(i).gameObject.SetActive(false);
        
        GameObject targetIcon = orderGroup.transform.GetChild(i).gameObject;

        GameObject glowIcon = new GameObject();
        Image iconImage = glowIcon.AddComponent<Image>();
        iconImage.sprite = iconGlow;
        glowIcon.GetComponent<RectTransform>().SetParent(targetIcon.transform);
        glowIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 64);
        iconImage.transform.position = targetIcon.transform.position;
        glowIcon.SetActive(true);
        
    }*/
}
