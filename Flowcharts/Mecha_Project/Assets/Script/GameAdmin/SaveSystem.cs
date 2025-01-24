using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SavePlayer(MechaPlayer player)
    {
        BinaryFormatter formatter = new();
        string path = Application.persistentDataPath + "/player.savedata";
        FileStream stream = new(path, FileMode.Create);

        PlayerData data = new(player);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.savedata";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Open);
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file tidak ditemukan di" +  path);
            return null;
        }
    }
}
