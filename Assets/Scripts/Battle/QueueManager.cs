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

    public GameObject orderContainer;
    public GameObject queueIconPrefab;

    public float width;
    public float height;

    public Unit[] queueList;
    public List<Unit> speedList;
    public List<GameObject> iconsList;

    public int queueSize;

    private void Start()
    {
        speedList = new List<Unit>();
        iconsList = new List<GameObject>();
    }

    public void Setup()
    {
        queueSize = 0;

        Refresh(true);
    }

    public void OrderOnSpeed()
    {
        speedList = speedList.OrderByDescending(x => x.statsManager.GetAttributeValue(AttributeType.Speed)).ToList();
    }

    public void Refresh(bool refreshTurn)
    {
        OrderOnSpeed();

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

    public Unit GetNextInOrder()
    {
        for (int i = 0; i < queueList.Length; i++)
        {
            if (queueList[i].hasTurn && !queueList[i].statsManager.isDead)
                return queueList[i];
        }

        return null;
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
        ObjectUtilities.ClearContainer(orderContainer);

        iconsList.Clear();
    }

    public void AddToOrder(Unit u)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(queueIconPrefab, orderContainer);
        obj.name = u.name;

        QueueIconObject queueIcon = obj.GetComponent<QueueIconObject>();
        queueIcon.Setup(u);

        iconsList.Add(obj);
    }

    public void SetIcon(Unit u)
    {
        if (u == null)
            return;

        u.orderIcon.HandleTurn();
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
