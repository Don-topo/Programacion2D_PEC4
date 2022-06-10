
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class FileManager
{
    private static readonly string path = Application.persistentDataPath + "/GameInfo.txt";

    public static Collaborators LoadCollaborators(TextAsset textAsset)
    {
        Collaborators collaborators = JsonUtility.FromJson<Collaborators>(textAsset.text);
        return collaborators;
    }
    
    public static GameInfo LoadGameConfig()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GameInfo gameInfo = formatter.Deserialize(stream) as GameInfo;
            stream.Close();
            return gameInfo;
        }
        else
        {
            return new GameInfo(100f, 100f, "Tutorial");
        }
    }

    public static void SaveGameConfig(GameInfo gameInfo)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, gameInfo);
        stream.Close();
    }

    public static bool CheckIfExistSavedData()
    {
        return File.Exists(path);
    }


}
