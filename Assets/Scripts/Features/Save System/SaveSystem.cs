using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveTeam(Team team)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "/team.psd");

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            TeamData data = new TeamData(team);

            formatter.Serialize(stream, data);
        }
    }

    public static TeamData LoadTeam()
    {
        string path = Path.Combine(Application.persistentDataPath, "/team.psd");

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
            Debug.Log("No save file. :(");
            return null;
        }
    }
}
