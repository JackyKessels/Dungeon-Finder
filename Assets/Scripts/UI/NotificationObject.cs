using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationObject : MonoBehaviour
{
    public TextMeshProUGUI message;
    public GameObject rewardContainer;
    public Button continueButton;
    private GameObject container;

    public void Update()
    {
        if (KeyboardHandler.ProgressWindow())
        {
            if (continueButton)
                continueButton.onClick?.Invoke();
        }
    }

    public void Setup(GameObject containerObject, string message, float windowWidth, float windowHeight, List<Reward> rewards = null)
    {
        container = containerObject;

        transform.SetParent(containerObject.transform);
        transform.localPosition = new Vector3(0, NotificationsInContainer() * -20);
        name = "Notification";

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(windowWidth, windowHeight - 64);
        rectTransform.localScale = Vector3.one;

        this.message.text = message;

        if (rewards != null)
        {
            foreach (Reward reward in rewards)
            {
                AddRewardButton(reward);
            }
        }

        continueButton.onClick.AddListener(CloseNotification);
    }

    public static void CreateNotification(string message, float windowWidth, float windowHeight, List<Reward> rewards = null)
    {
        GameObject container = GameObject.Find("Notification Container");

        GameObject obj = ObjectUtilities.CreateSimplePrefab(GameAssets.i.notificationPrefab.gameObject, container);

        NotificationObject notification = obj.GetComponent<NotificationObject>();
        notification.Setup(container, message, windowWidth, windowHeight, rewards);
    }

    private void CloseNotification()
    {
        Destroy(gameObject);
    }

    private int NotificationsInContainer()
    {
        int total = 0;

        foreach (Transform child in container.transform)
        {
            total++;
        }

        return total;
    }

    private void AddRewardButton(Reward reward)
    {
        GameObject obj = ObjectUtilities.CreateSimplePrefab(GameAssets.i.rewardPrefab, rewardContainer);

        RewardObject r = obj.GetComponent<RewardObject>();
        r.SetData(reward);
    }
}