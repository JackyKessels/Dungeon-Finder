using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    #region Singleton
    public static EventManager Instance { get; private set; }

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

    private GameManager gameManager;

    [Header("[ Manager Variables ]")]
    public Canvas userInterface;
    public Camera cameraObject;

    [Header("[ Event Specific ]")]
    public SpriteRenderer eventBackgroundRenderer;
    public SpriteRenderer battleBackgroundRenderer;
    public GameObject interactionPrefab;
    public GameObject objectContainer;
    public GameObject interactionPosition;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    //public void SetupSpecialEvent(SpecialEventObject specialEventObject)
    //{
    //    gameManager.cameraScript.GoToCamera(cameraObject, false);
    //    gameManager.EnableUI(userInterface);

    //    SetBackground(specialEventObject.background, false);

    //    CreateSpecialEventInteraction(specialEventObject);

    //    ConfirmationBoxHandler.Instance.SetupConfirmationBox("This is a test for the event manager, continue?",
    //                                                         new List<ConfirmationButton>()
    //                                                        {new ConfirmationButton("Yes", Continue),
    //                                                         new ConfirmationButton("No", Continue) });
    //}

    public void Continue()
    {
        gameManager.GoToRun();
    }

    //private void CreateSpecialEventInteraction(SpecialEventObject specialEventObject)
    //{
    //    GameObject interactionObject = Instantiate(interactionPrefab, interactionPosition.transform.position, Quaternion.identity);
    //    interactionObject.transform.parent = objectContainer.transform;
    //    interactionObject.GetComponent<SpriteRenderer>().sprite = specialEventObject.interactionSprite;
    //    interactionObject.GetComponent<SpriteRenderer>().flipX = true;

    //}

    public void SetBackground(Sprite sprite, bool isBattle)
    {
        if (sprite != null)
        {
            if (isBattle)
            {
                battleBackgroundRenderer.sprite = sprite;
            }
            else
            {
                eventBackgroundRenderer.sprite = sprite;
            }
        }



    }
}
