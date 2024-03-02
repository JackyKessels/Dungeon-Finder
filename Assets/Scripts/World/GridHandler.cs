using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    public GameObject locationPrefab;
    public GameObject lineRendererPrefab;
    public GameObject locationContainer;
    public GameObject pathsContainer;

    public Location startLocation;
    public Transform initialPosition;

    [Header("Overall Grid Dimensions")]
    public int columns;
    public int rows;

    public float totalWidth;
    public float totalHeight;

    [Header("Grid Customization")]
    public int startingPoints;
    public int endPoints;
    public int removalThreshold;

    private Location[,] locations;
    private int removeCounter = 1;

    private List<Path> possiblePaths = new List<Path>();
    private List<Path> validPaths = new List<Path>();

    private float backgroundWidthIncrease;

    private List<int> bossColumns;

    public void GenerateFloor(Dungeon dungeon, Floor floor)
    {
        if (dungeon.floors.Count == 0 || floor.columns == 0 || floor.rows == 0)
        {
            Debug.Log("Invalid Dungeon");
            return;
        }

        int counter = 0;

        while (counter > 1000 | RerollFloor(CreateFloor(dungeon, floor), floor))
        {
            counter++;
            Debug.Log($"Generated new floor layout after {counter} tries.");
        }
    }

    private bool RerollFloor(int gridCount, Floor floor)
    {
        if (gridCount == -1)
            return false;

        // If conditions not met, reroll the floor
        if (floor.minimumLocations <= gridCount &&
            gridCount <= floor.maximumLocations &&
            CountStartingPoints() == floor.startingPoints)
        {
            return false; // DO NOT REROLL: All conditions are met
        }
        else
        {
            return true; // REROLL: Not all conditions are met 
        }      
    }

    private int CreateFloor(Dungeon dungeon, Floor floor)
    {
        int floorLevel = DungeonManager.GetDungeonLevel(floor);

        columns = floor.columns;
        rows = floor.rows;
        startingPoints = floor.startingPoints;
        endPoints = floor.endPoints;
        removalThreshold = floor.removalThreshold;

        Debug.Log($"Generated floor: {floor.name}, in dungeon: {dungeon.name}.");

        // Lock first locations
        startLocation.SetVisited();

        // Setup for array and fill the map with prefabs
        CreateGrid(floor.addOffset);

        // Set the amount of locations in the first column
        SetLocationAmount(0, startingPoints);

        // Set the amount of locations in the last column
        if (floor.forceBossCenter && floor.endPoints == 1)
            CenterEnd(floor.rows);
        else
            SetLocationAmount(columns - 1, endPoints);

        // Remove random locations
        RemoveRandomLocations(removalThreshold);

        // Create connections between the remaining locations
        CreateConnections();

        // Remove all dead-end routes
        while (removeCounter > 0)
        {
            RemoveConnectionlessLocations();
        }

        // Remove the crossed paths
        RemoveCrossedPaths();

        // Create paths between locations
        SetupPaths();

        // Turn the last row into boss fights
        AddBosses(floor.forceBossCenter);

        // Add elites to the map
        AddNewType(floor.eliteCount, LocationType.Elite, false);

        // Add treasures to the map
        AddNewType(floor.treasureCount, LocationType.Treasure, false);

        // Add campfires to the map
        AddNewType(floor.campfireCount, LocationType.Campfire, false);

        // Add mysteries to the map
        AddNewType(floor.mysteryCount, LocationType.Mystery, false);

        // Add enemies to battle/elite/boss locations
        AddEnemiesToLocations(floor, floorLevel);

        // Update the pan limitations for the camera
        backgroundWidthIncrease = GameManager.Instance.cameraScript.UpdatePanLimits((int)initialPosition.position.x, GetLastColumnX());

        // Return the total number of locations in the current map
        return CountGrid();
    }

    private int CountGrid()
    {
        int total = 0;

        foreach (Location location in locations)
        {
            if (location != null)
                total++;
        }

        return total;
    }

    public Location GetLocation(int x, int y)
    {
        return locations[x, y];
    }

    public float GetWidthIncrease()
    {
        return backgroundWidthIncrease;
    }

    public Location GetRandomFloorLocation(bool excludeBoss)
    {
        int counter = 0;
        Location randomLocation = null;

        int bossColumn = excludeBoss ? 1 : 0;

        while(randomLocation == null || counter < 1000)
        {
            int row = Random.Range(0, rows);
            int column = Random.Range(0, columns - bossColumn);

            randomLocation = locations[column, row];
            counter++;
        }

        return randomLocation;
    }

    private List<Location> GetBattles(bool includeFirstColumn)
    {
        List<Location> battles = new List<Location>();

        foreach (Location location in locations)
        {
            if (location != null && location.locationType == LocationType.Battle)
            {
                if (includeFirstColumn)
                {
                    battles.Add(location);
                }
                else
                {
                    if (location.x != 0)
                        battles.Add(location);
                }
            }
        }

        return battles;
    }

    private void CreateGrid(bool addOffset)
    {
        if (columns == 0 || rows == 0)
        {
            return;
        }

        locations = new Location[columns, rows];

        ObjectUtilities.ClearContainer(locationContainer);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float height;
                float offsetY;
                float columnWidth;

                if (rows == 1)
                {
                    height = totalHeight / 2;
                    offsetY = 0;
                }
                else
                {
                    height = totalHeight / (rows - 1);
                    offsetY = totalHeight / 2;
                }

                if (columns <= 8)
                    columnWidth = totalWidth / columns;
                else
                    columnWidth = 2;

                Vector3 position = new Vector3(initialPosition.position.x + col * columnWidth, initialPosition.position.y + row * height - offsetY, 0);
                // Randomly offset the positions to give a more natural look
                if (addOffset)
                    position = NoiseOffset(position);

                GameObject locationObject = Instantiate(locationPrefab);
                locationObject.transform.parent = locationContainer.transform;
                locationObject.transform.position = position;
                locationObject.name = "Location [" + col + "," + row + "]";

                Location location = locationObject.GetComponent<Location>();
                locations[col, row] = location;
                location.x = col;
                location.y = row;

                location.SetLocationType(LocationType.Battle);
            }
        }
    }

    private void RemoveRandomLocations(int threshold)
    {
        for (int i = 1; i < columns - 1; i++)
        {
            RemoveLocations(i, threshold, true);
        }
    }

    private void RemoveCrossedPaths()
    {
        foreach (Location location in locations)
        {
            FixCrossedPaths(location);
        }
    }

    private void SetLocationAmount(int column, int amount)
    {
        RemoveLocations(column, rows - amount, false);
    }

    private void CreateConnections()
    {
        foreach (Location location in locations)
        {
            CreateConnection(location);
        }

        for (int i = 0; i < rows; i++)
        {
            if (locations[0, i] != null)
                ConnectLocations(startLocation, locations[0, i]);
        }
    }

    public bool IsReachable(Location currentLocation, Location targetLocation)
    {
        if (targetLocation == null)
            return false;

        if (currentLocation == targetLocation)
            return true;

        if (targetLocation.x < currentLocation.x)
            return false;

        if (targetLocation.x == currentLocation.x && targetLocation.y != currentLocation.y)
            return false;

        return false;
    }

    public void RefreshLocations(Location currentLocation)
    {
        foreach (Location location in locations)
        {
            if (location != null)
                location.LockLocation(true);
        }

        foreach (Location location in GetAllReachableLocations(currentLocation, new List<Location>()))
        {
            location.LockLocation(false);
        }
    }

    private List<Location> GetAllReachableLocations(Location targetLocation, List<Location> reachableLocations)
    {
        List<Location> rightConnections = targetLocation.GetRightConnectedLocations();

        if (rightConnections.Count > 0)
        {
            if (!LocationInList(targetLocation, reachableLocations))
                reachableLocations.Add(targetLocation);

            foreach (Location location in rightConnections)
            {
                if (!LocationInList(location, reachableLocations))
                    reachableLocations.Add(location);

                GetAllReachableLocations(location, reachableLocations);
            }
        }

        return reachableLocations;
    }

    private bool LocationInList(Location location, List<Location> targetList)
    {
        if (targetList.Count == 0 || location == null)
            return false;

        for (int i = 0; i < targetList.Count; i++)
        {
            if (location.x == targetList[i].x && location.y == targetList[i].y)
                return true;
        }

        return false;
    }

    public void LockUnreachableLocations(Location currentLocation)
    {
        // Return if at the last column
        if (currentLocation.x >= columns - 1)
            return;

        List<Location> lockedLocations = new List<Location>();

        // Locks the specific column number
        for (int i = 0; i < rows; i++)
        {
            if (locations[currentLocation.x, i] != null && !locations[currentLocation.x, i].locked)
            {
                lockedLocations.Add(locations[currentLocation.x, i]);
                locations[currentLocation.x, i].LockLocation(true);
            }
        }

        lockedLocations.Remove(currentLocation);
        currentLocation.locked = false;

        for (int i = 0; i < lockedLocations.Count; i++)
        {
            LockConnectedLocations(lockedLocations[i]);
        }

        currentLocation.SetVisited();
    }

    private void LockConnectedLocations(Location _location)
    {
        List<Location> rightConnectedLocations = _location.GetRightConnectedLocations();

        List<Location> unreachableLocations = new List<Location>();

        foreach (Location location in rightConnectedLocations)
        {
            if (SetLocked(location) != null)
            {
                unreachableLocations.Add(location);
            }
        }

        foreach (Location location in unreachableLocations)
        {
            LockConnectedLocations(location);
        }
    }

    private Location SetLocked(Location _location)
    {
        bool unreachable = true;

        foreach (Location location in _location.GetLeftConnectedLocations())
        {
            if (!location.locked)
                unreachable = false;
        }

        if (unreachable)
        {
            _location.LockLocation(true);
            return _location;
        }

        return null;
    }

    private int CountStartingPoints()
    {
        int count = 0;

        foreach (Location location in locations)
        {
            if (location != null && location.x == 0)
            {
                count++;
            }
        }

        return count;
    }

    private void RemoveConnectionlessLocations()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            { 
                if (locations[col, row] != null)
                {
                    if (locations[col, row].connectedLocations.Count == 0)
                    {
                        DestroyLocation(locations[col, row]);
                    }

                    if (!HasLeftConnection(locations[col, row]))
                    {
                        DestroyLocation(locations[col, row]);
                    }

                    if (!HasRightConnection(locations[col, row]))
                    {
                        DestroyLocation(locations[col, row]);
                    }
                }
            }
        }

        removeCounter--;
    }

    private void CreateConnection(Location location)
    {
        if (location != null)
        {
            // Ignore last column
            if (location.x < columns - 1)
            {
                if (location.y < rows - 1)
                {
                    Location n1 = locations[location.x + 1, location.y + 1];
                    ConnectLocations(location, n1);
                }

                Location n2 = locations[location.x + 1, location.y];
                ConnectLocations(location, n2);

                if (location.y > 0)
                {
                    Location n3 = locations[location.x + 1, location.y - 1];
                    ConnectLocations(location, n3);
                }
            }
        }
    }

    private void RemoveLocations(int column, int threshold, bool random)
    {
        if (threshold == 0)
            return;

        int amount;

        if (random)
        {
            amount = Random.Range(1, threshold + 1);
        }
        else
        {
            amount = threshold;
        }

        int removed = 0;

        while (removed < amount)
        {
            int randomRow = Random.Range(0, rows);

            if (locations[column, randomRow] != null)
            {
                DestroyLocation(locations[column, randomRow]);

                removed++;
            }
        }
    }

    private void CenterEnd(int rows)
    {
        // If the row number is odd then take the middle,
        // otherwise randomize the center
        if (rows % 2 == 1)
        {
            int center = (rows - 1) / 2;

            for (int i = 0; i < rows; i++)
            {
                if (i != center)
                {
                    DestroyLocation(locations[columns - 1, i]);
                }
            }
        }
        else
        {
            // Pick 0 or 1 randomly
            int randomCenter = Random.Range(0, 2);

            // If 0 then pick the upper center, otherwise pick the lower center
            int center = randomCenter == 0 ? rows / 2 - 1 : rows / 2;

            for (int i = 0; i < rows; i++)
            {
                if (i != center)
                {
                    DestroyLocation(locations[columns - 1, i]);
                }
            }
        }
    }

    private void ConnectLocations(Location a, Location b)
    {
        if (a != null && b != null)
        {
            a.connectedLocations.Add(b);
            b.connectedLocations.Add(a);
        }
    }

    private void ChangeColumns(List<int> targetColumns, LocationType type)
    {
        if (targetColumns.Count <= 0)
            return;

        for (int i = 0; i < targetColumns.Count; i++)
        {
            ColumnToType(targetColumns[i], type);
        }
    }

    private void AddBosses(bool centered)
    {
        for (int i = 0; i < rows; i++)
        {
            // Change last column to boss
            if (locations[columns - 1, i] != null)
            {
                locations[columns - 1, i].SetLocationType(LocationType.Boss);
            }

            // Change second last column to campfire
            if (locations[columns - 2, i] != null)
            {
                locations[columns - 2, i].SetLocationType(LocationType.Campfire);
            }
        }
    }

    private void AddNewType(int count, LocationType type, bool includeFirstColumn)
    {
        List<Location> openBattles = GetBattles(includeFirstColumn);

        if (openBattles.Count <= 0)
            return;

        for (int i = 0; i < count; i++)
        {
            openBattles = GetBattles(includeFirstColumn);

            if (openBattles.Count <= 0)
                return;

            int randomLocation = Random.Range(0, openBattles.Count);

            openBattles[randomLocation].SetLocationType(type);

            openBattles.RemoveAt(randomLocation);
        }
    }

    private void AddEnemiesToLocations(Floor floor, int level)
    {
        foreach (Location location in locations)
        {
            if (location != null)
            {
                if (location.locationType == LocationType.Battle)
                {
                    location.enemyUnits = Encounter.SetupUnitObjects(floor.trashEncounters, level, level + 1);
                }
                else if (location.locationType == LocationType.Elite)
                {
                    location.enemyUnits = Encounter.SetupUnitObjects(floor.eliteEncounters, level + 1, level + 1);
                }
                else if (location.locationType == LocationType.Boss)
                {
                    location.enemyUnits = Encounter.SetupUnitObjects(floor.bossEncounter, level + 2, level + 2);
                }
            }
        }
    }

    private void ColumnToType(int columnNumber, LocationType type)
    {
        for (int i = 0; i < rows; i++)
        {
            if (locations[columnNumber, i] != null)
            {
                locations[columnNumber, i].SetLocationType(type);
            }
        }
    }

    private int GetLastColumnX()
    {
        int maximumX = 0;

        for (int i = 0; i < rows; i++)
        {
            if (locations[columns - 1, i] != null && locations[columns - 1, i].transform.position.x > maximumX)
            {
                maximumX = (int)locations[columns - 1, i].transform.position.x;
            }
        }

        return maximumX;
    }

    private Vector3 NoiseOffset(Vector3 position)
    {
        float randomXOffset = Random.Range(-0.3f, 0.2f);
        float randomYOffset = Random.Range(-0.3f, 0.2f);

        return new Vector3(position.x + randomXOffset, position.y + randomYOffset, 0);
    }

    private bool HasLeftConnection(Location location)
    {
        if (location == null)
        {
            return false;
        }

        if (location.x == 0)
            return true;

        if (location != null)
        {
            foreach (Location connected in location.connectedLocations)
            {
                if (connected != null)
                {
                    if (connected.x < location.x)
                        return true;
                }
            }
        }

        return false;
    }

    private bool HasRightConnection(Location location)
    {
        if (location == null)
        {
            return false;
        }

        if (location.x == columns - 1)
            return true;

        if (location != null)
        {
            foreach (Location connected in location.connectedLocations)
            {
                if (connected != null)
                {
                    if (connected.x > location.x)
                        return true;
                }
            }
        }

        return false;
    }

    private void FixCrossedPaths(Location location)
    {
        // Do nothing if
        // - Invalid Location
        // - Location is in the last column
        // - Location is in the top row
        if (location == null || location.x >= columns - 1 || location.y >= rows - 1)
            return;

        LocationPosition initialLocation = new LocationPosition(location.x, location.y);
        LocationPosition northLocation = new LocationPosition(initialLocation.x, initialLocation.y + 1);
        LocationPosition eastLocation = new LocationPosition(initialLocation.x + 1, initialLocation.y);
        LocationPosition northEastLocation = new LocationPosition(initialLocation.x + 1, initialLocation.y + 1);

        if ((locations[initialLocation.x, initialLocation.y] != null) &&
            (locations[northLocation.x, northLocation.y] != null) &&
            (locations[eastLocation.x, eastLocation.y] != null) &&
            (locations[northEastLocation.x, northEastLocation.y] != null))
        {
            int random = Random.Range(0, 3);

            // Remove /
            if (random == 0)
            {
                locations[initialLocation.x, initialLocation.y].connectedLocations.Remove(locations[northEastLocation.x, northEastLocation.y]);
                locations[northEastLocation.x, northEastLocation.y].connectedLocations.Remove(locations[initialLocation.x, initialLocation.y]);
            }
            // Remove \
            else if (random == 1)
            {
                locations[northLocation.x, northLocation.y].connectedLocations.Remove(locations[eastLocation.x, eastLocation.y]);
                locations[eastLocation.x, eastLocation.y].connectedLocations.Remove(locations[northLocation.x, northLocation.y]);
            }
            // Remove X
            else
            {
                locations[initialLocation.x, initialLocation.y].connectedLocations.Remove(locations[northEastLocation.x, northEastLocation.y]);
                locations[northEastLocation.x, northEastLocation.y].connectedLocations.Remove(locations[initialLocation.x, initialLocation.y]);
                locations[northLocation.x, northLocation.y].connectedLocations.Remove(locations[eastLocation.x, eastLocation.y]);
                locations[eastLocation.x, eastLocation.y].connectedLocations.Remove(locations[northLocation.x, northLocation.y]);
            }
        }
    }

    private void DestroyLocation(Location location)
    {
        if (location != null)
        {
            foreach (Location connected in location.connectedLocations)
            {
                connected.connectedLocations.Remove(location);
            }

            Destroy(locations[location.x, location.y].gameObject);
            locations[location.x, location.y] = null;

            removeCounter++;
            //Debug.Log("Destroyed: " + location.gameObject.name + " and increased counter.");
        }
    }

    public void SetupPaths()
    {
        ObjectUtilities.ClearContainer(pathsContainer);

        possiblePaths.Clear();
        validPaths.Clear();

        foreach (Location location in locations)
        {
            if (location != null)
            {
                CreateAllConnectedPaths(location);
            }
        }

        FilterValidPaths();

        foreach (Path validPath in validPaths)
        {
            CreatePath(validPath);
        }
    }

    // Creates all possible paths
    private void CreateAllConnectedPaths(Location location)
    {
        foreach (Location connectedLocation in location.connectedLocations)
        {
            Path path = new Path(location.transform.position, connectedLocation.transform.position);
            possiblePaths.Add(path);
        }
    }

    // Adds the unique ones to a seperate list
    private void FilterValidPaths()
    {
        if (possiblePaths.Count > 0)
        {
            for (int i = possiblePaths.Count; i-- > 0;)
            {
                AddValidPath(possiblePaths[i]);
            }
        }
    }

    // Checks if paths aren't duplicate and pass it to a list
    private void AddValidPath(Path checkPath)
    {
        bool isDuplicate = false;

        for (int i = validPaths.Count; i-- > 0;)
        {
            if (IsDuplicate(checkPath, validPaths[i]))
            {
                isDuplicate = true;
            }
        }

        if (!isDuplicate)
            validPaths.Add(checkPath);
    }

    private bool IsDuplicate(Path path_1, Path path_2)
    {
        if ((path_1.start == path_2.end && path_1.end == path_2.start))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Creates the actual visual line
    private void CreatePath(Path path)
    {
        GameObject newLineRenderer = Instantiate(lineRendererPrefab);
        newLineRenderer.transform.SetParent(pathsContainer.transform);

        LineRenderer lineRenderer = newLineRenderer.GetComponent<LineRenderer>();

        float width = lineRenderer.startWidth;
        float distance = Vector2.Distance(path.start, path.end);
        lineRenderer.material.mainTextureScale = new Vector2(distance / width, 1); // distance / width is the correct width for the seperate tiles

        lineRenderer.SetPosition(0, path.start);
        lineRenderer.SetPosition(1, path.end);
    }

    public struct Path
    {
        public Vector2 start;
        public Vector2 end;

        public Path(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }
    }

    public struct LocationPosition
    {
        public int x;
        public int y;

        public LocationPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
