using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;
    private DungeonManager runManager;

    [SerializeField] private float moveSpeed = 5f;

    public bool isMoving = false;
    public bool movementLocked = false; // Unlock on completion of event

    public Location currentLocation;

    public List<Vector3> pathVectorList;
    private int currentPathIndex;

    private void Start()
    {
        gameManager = GameManager.Instance;
        runManager = DungeonManager.Instance;

        pathVectorList = null;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (transform.position != targetPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                currentPathIndex++;

                // Reached destination
                if (currentPathIndex >= pathVectorList.Count)
                {
                    StopMoving();
                    isMoving = false;

                    runManager.locationInformationObject.SetActive(true);
                    runManager.SetupLocationInformation(currentLocation);
                    runManager.gridHandler.LockUnreachableLocations(currentLocation);
                }
            }
        }
    }

    // Moves along the path that is calculated by the pathfinder
    public void MovePath(List<Vector3> path, Location endLocation)
    {
        isMoving = true;

        currentPathIndex = 0;
        pathVectorList = path;

        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }

        currentLocation = endLocation;
    }

    // Moves 1 location into the direction of the selected location
    public void MoveToLocation(Location targetLocation)
    {
        isMoving = true;
        movementLocked = true;

        currentPathIndex = 0;
        pathVectorList = new List<Vector3>() { targetLocation.transform.position };

        currentLocation = targetLocation;
    }

    private void StopMoving()
    {
        pathVectorList = null;
    }

    public void SetCurrentLocation(Location location)
    {
        currentLocation = location;
        transform.position = location.transform.position;
    }
}
