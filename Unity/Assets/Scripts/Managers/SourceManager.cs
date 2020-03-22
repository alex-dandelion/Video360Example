using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SourceManager : MonoBehaviour
{
    public bool useFake = false;
    public float delayTime = 2;

    private string path = "/Documents/";
    private Dictionary<string, VideoSourceModel> sources = new Dictionary<string, VideoSourceModel>();
    [SerializeField] private TextAsset textAsset;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        string catalogPath = Application.persistentDataPath + path;

        if (!Directory.Exists(catalogPath))
        {
            Directory.CreateDirectory(catalogPath);
        }

        string configPath = catalogPath + "config.json";
        if (File.Exists(configPath))
        {
            string stringToParce = File.ReadAllText(configPath);
            sources = JsonConvert.DeserializeObject<Dictionary<string, VideoSourceModel>>(stringToParce);
            CheckPoses();
        }
        else 
        {
            sources = JsonConvert.DeserializeObject<Dictionary<string, VideoSourceModel>>(textAsset.text);
            SaveData();
        }

        delayTime = PlayerPrefs.GetFloat("delayTime", 2);
    }

    public void SaveData()
    {
        string configPath = Application.persistentDataPath + path + "config.json";
        Debug.Log($"save data {JsonConvert.SerializeObject(sources)}" );
        File.WriteAllText(configPath, JsonConvert.SerializeObject (sources));

        PlayerPrefs.SetFloat( "delayTime", delayTime );
    }

    public Dictionary<string, VideoSourceModel> GetAllVideoModels() 
    {
        return sources;
    }

    public string GetAudioURL()
    {
        return PlayerPrefs.GetString("audioURL", "");
    }

    public void SaveAudioURL(string audio) 
    {
        PlayerPrefs.SetString("audioURL", audio);
    }

    public bool AddItem(VideoSourceModel item) 
    {
        if (sources.ContainsKey(item.id))
            return false;

        sources.Add(item.id, item);
        CheckPoses();
        return true;
    }

    private void CheckPoses()
    {
        int camCount = sources.Values.Count;
        foreach (VideoSourceModel checkItem in sources.Values)
        {
            if (checkItem.points == null)
                checkItem.points = new List<CamPoseModel>();
            while (checkItem.points.Count < camCount)
            {
                float pos = -9 + checkItem.points.Count * 6;
                checkItem.points.Add(new CamPoseModel() { id = checkItem.points.Count.ToString(), position = new Vector3(pos, -3.0f, 20.6f) });
            }
        }
    }

    public bool RemoveItem(VideoSourceModel item) 
    {
        bool result = sources.Remove(item.id);
        Debug.Log($"remove item at {item.id} with result: { result} ");
        if (!result)
            return false;

        foreach (VideoSourceModel checkItem in sources.Values)
        {

            foreach (CamPoseModel camPoseModel in checkItem.points) 
            {
                if (camPoseModel.id == item.id) 
                {
                    checkItem.points.Remove(camPoseModel);
                    return true;
                }

            }
        }
        return true;
    }
}
