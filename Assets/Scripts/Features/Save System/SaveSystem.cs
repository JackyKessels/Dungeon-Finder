using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string FileName(string id)
    {
        return "Save_Slot_" + id + ".dat";
    }

    public static void SaveTeamData(Team team, string id)
    {
        string path = Path.Combine(Application.persistentDataPath, FileName(id));

        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            TeamData data = new TeamData(team);

            formatter.Serialize(stream, data);
        }
    }

    public static TeamData LoadTeamData(string id)
    {
        string path = Path.Combine(Application.persistentDataPath, FileName(id));

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return formatter.Deserialize(stream) as TeamData;
            }
        }
        else
        {
            // GameManager.Instance.Introduction1();

            Debug.Log("No save file. :(");
            return null;
        }
    }

    public static void DeleteTeamData(string id)
    {
        string path = Path.Combine(Application.persistentDataPath, FileName(id));

        if (SaveExists(id))
        {
            File.Delete(path);
        }
    }

    public static bool SaveExists(string id)
    {
        string path = Path.Combine(Application.persistentDataPath, FileName(id));

        return File.Exists(path);
    }

    public static string GetLastSaved(string id)
    {
        string path = Path.Combine(Application.persistentDataPath, FileName(id));

        if (File.Exists(path))
        {
           return File.GetLastWriteTime(path).ToString();
        }
        else
        {
            return "";
        }
    }
}
