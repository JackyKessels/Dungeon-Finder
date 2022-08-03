using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Camera currentCamera;

    public bool panEnabled = false;
    public float panSpeed = 20f;
    public float panBorderThickness = 25f;
    [SerializeField] private int panLimitLeft;
    [SerializeField] private int panLimitRight;

    private void Update()
    {
        if (panEnabled)
        {
            Vector3 position = currentCamera.transform.position;

            if (Input.GetKey(KeyCode.A))// || Input.mousePosition.x <= panBorderThickness)
            {
                position.x -= panSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))// || Input.mousePosition.x >= Screen.width - panBorderThickness)
            {
                position.x += panSpeed * Time.deltaTime;
            }

            position.x = Mathf.Clamp(position.x, panLimitLeft, panLimitRight);

            currentCamera.transform.position = position;
        }
    }

    private void Start()
    {
        currentCamera = GameManager.Instance.startCamera;
        currentCamera.enabled = true;
    }

    public void GoToCamera(Camera camera, bool enablePanning)
    {
        currentCamera.gameObject.SetActive(false);
        currentCamera.enabled = false;

        currentCamera = camera;

        currentCamera.gameObject.SetActive(true);
        currentCamera.enabled = true;

        panEnabled = enablePanning;
    }

    public void MoveCamera(Camera camera, Transform targetPosition)
    {
        camera.transform.position = new Vector3(targetPosition.position.x, targetPosition.position.y, -10);
    }

    public float UpdatePanLimits(int left, int right)
    {
        panLimitLeft = left + 7;
        panLimitRight = Mathf.Clamp(right - 7, left + 7, 200);

        // Gives the difference between both pan limits;
        return (float)Math.Abs(panLimitLeft - panLimitRight);
    }
}
