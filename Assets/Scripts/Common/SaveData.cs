using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveData : MonoBehaviour
{
    private static SaveData _instance;
    public static SaveData Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType(typeof(SaveData)) as SaveData;
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "SaveData Instance" + UnityEngine.Random.Range(0, 1000);
                    _instance = container.AddComponent(typeof(SaveData)) as SaveData;
                    _instance.Load();
                    DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }
    }

    //public static int CurrentServer = 0;
    public PlayerData Data;
    public List<string> DataListToSave = new List<string>();
    // Start is called before the first frame update
    void Start()
    {

    }
    public void Save(List<string> keys)
    {
        foreach (var key in keys)
        {
            if (!DataListToSave.Contains(key)) DataListToSave.Add(key);
        }
        //Save();
    }
    public void Save(params string[] keys)
    {
        foreach (var key in keys)
        {
            if (!DataListToSave.Contains(key)) DataListToSave.Add(key);
        }
        //Save();
    }
    public void AddToSave(string key)
    {
        if (!DataListToSave.Contains(key)) DataListToSave.Add(key);
    }
    public void Save(string key)
    {
        if (!DataListToSave.Contains(key)) DataListToSave.Add(key);
        //Save();
    }
    public void Save()
    {
        //PlayerDataSaver.Save(Data);
        byte[] bytes = SerializeObject(Data);
        // ObscuredPrefs.SetByteArray(Consts.Key_SaveData, bytes);

    }
    public void Load()
    {
        //Data = new PlayerData();

        //try
        //{
        //    Data = PlayerDataSaver.Load();
        //}
        //catch (Exception e)
        //{
        //    Data = new PlayerData();
        //    Debug.LogError("error while loading local data: " + e);
        //}

        // byte[] bytes = ObscuredPrefs.GetByteArray(Consts.Key_SaveData);
        // //bytes = new byte[] { }; // test 
        // Load(bytes);
        // CheckForEmptyData();

        //Data.rankEvMsRcv = "0_0_0_0"; // test 

        //Data = new PlayerData(); // test 
        //Data._id = "6493cc970ea5bcd418949b95"; // test 

    }
    public static byte[] SerializeObject<T>(T serializableObject)
    {
        T obj = serializableObject;

        IFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }
    }

    public static T DeserializeObject<T>(byte[] serilizedBytes)
    {
        IFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream(serilizedBytes))
        {
            return (T)formatter.Deserialize(stream);
        }
    }
    public void Load(byte[] bytes)
    {
        if (bytes.Length == 0) // test 
        {
            Data = new PlayerData();
            Debug.Log("Load nothing");
        }
        else
        {
            //Debug.Log("Load previous! " + bytes.Length);
            Data = DeserializeObject<PlayerData>(bytes);
        }
    }
    public void CheckForEmptyData()
    {


    }

}
