using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Text;

public static class SaveSystem
{
    public static void SaveGameState(SaveData saveData)
    {
        string path = Application.persistentDataPath + "/renewplanetsave.xml";

        XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
        System.IO.StreamWriter writer = new System.IO.StreamWriter(path);
        serializer.Serialize(writer, saveData);
        writer.Close();

        Debug.Log(path);
    }

    public static void DeleteGameSave()
    {
        string path = Application.persistentDataPath + "/renewplanetsave.xml";
        if(File.Exists(path)) File.Delete(path);
    }

    public static SaveData LoadGameState()
    {
        string path = Application.persistentDataPath + "/renewplanetsave.xml";
        if(File.Exists(path))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            System.IO.StreamReader reader = new System.IO.StreamReader(path);
            SaveData data = (SaveData)serializer.Deserialize(reader); // only difference
            reader.Close();
            return data;
        }
        Debug.LogError("Save file not found in " + path);
        return null;
    }
    
    public static SaveData LoadGameFromFile(TextAsset xmlFile)
    {
        if (xmlFile != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            Stream myLevelStr = new MemoryStream(Encoding.UTF8.GetBytes(xmlFile.text));

            System.IO.StreamReader reader = new StreamReader(myLevelStr);
            SaveData data = (SaveData)serializer.Deserialize(reader); // only difference
            reader.Close();
            return data;
        }
        return null;
    }
}
