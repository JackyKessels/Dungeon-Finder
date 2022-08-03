using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder
{
    private List<Location> openList;
    private List<Location> closedList;

    public List<Location> FindPath(Location startLocation, Location endLocation)
    {
        //openList = new List<Location>() { startLocation };
        //closedList = new List<Location>();

        //foreach (Location location in runManager.Instance.currentRegion.locations)
        //{
        //    location.gCost = float.MaxValue;
        //    location.CalculateFCost();
        //    location.cameFromLocation = null;
        //}

        //startLocation.gCost = 0;
        //startLocation.hCost = CalculateCost(startLocation, endLocation);
        //startLocation.CalculateFCost();

        //while (openList.Count > 0)
        //{
        //    Location currentLocation = GetLowestFCostNode(openList);

        //    if (currentLocation == endLocation)
        //    {
        //        return CalculatePath(endLocation);
        //    }

        //    openList.Remove(currentLocation);
        //    closedList.Add(currentLocation);

        //    foreach (Location connectedLocation in currentLocation.connectedLocations)
        //    {
        //        if (closedList.Contains(connectedLocation))
        //            continue;

        //        float tentativeGCost = currentLocation.gCost + CalculateCost(currentLocation, connectedLocation);

        //        if (tentativeGCost < connectedLocation.gCost)
        //        {
        //            connectedLocation.cameFromLocation = currentLocation;
        //            connectedLocation.gCost = tentativeGCost;
        //            connectedLocation.hCost = CalculateCost(connectedLocation, endLocation);
        //            connectedLocation.CalculateFCost();

        //            if (!openList.Contains(connectedLocation))
        //            {
        //                openList.Add(connectedLocation);
        //            }
        //        }
        //    }
        //}

        return null;
    }

    private List<Location> CalculatePath(Location endLocation)
    {
        List<Location> path = new List<Location>();
        path.Add(endLocation);
        Location currentLocation = endLocation;

        while (currentLocation.cameFromLocation != null)
        {
            path.Add(currentLocation.cameFromLocation);
            currentLocation = currentLocation.cameFromLocation;
        }

        path.Reverse();

        return path;
    }

    private float CalculateCost(Location startLocation, Location connectedLocation)
    {
        return Vector3.Distance(startLocation.transform.position, connectedLocation.transform.position);
    }
    
    private Location GetLowestFCostNode(List<Location> locationList)
    {
        Location lowestFCostLocation = locationList[0];

        for (int i = 0; i < locationList.Count; i++)
        {
            if (locationList[i].fCost < lowestFCostLocation.fCost)
            {
                lowestFCostLocation = locationList[i];
            }
        }

        return lowestFCostLocation;
    }

    public List<Vector3> GetVectorPath(List<Location> locationPath)
    {
        List<Vector3> vectorPath = new List<Vector3>();

        foreach (Location location in locationPath)
        {
            vectorPath.Add(new Vector3(location.transform.position.x, location.transform.position.y, location.transform.position.z));
        }

        return vectorPath;
    }
}
