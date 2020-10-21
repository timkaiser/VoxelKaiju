using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;


[Serializable]
public class HighscoreData
{
    [SerializeField]
    string name = "";
    [SerializeField]
    int score = 0;


    public int getScore() { return score; }
    public string getName() { return name; }

    public HighscoreData(int score, string name)
    {
        this.score = score;
        this.name = name;
    }
}

[Serializable]
public class HighscoreDataset
{
    [SerializeField]
    private HighscoreData[] highscoreData = new HighscoreData[0];

    /// <summary>
    /// Get the list of all Player Attacks
    /// </summary>
    public List<HighscoreData> GetSortedHighscoreData ()
    {
        Array.Sort(highscoreData, delegate (HighscoreData x, HighscoreData y) { return -x.getScore().CompareTo(y.getScore()); });
        return new List<HighscoreData>(highscoreData);
    }

    public void AddData(HighscoreData data)
    {
        HighscoreData[] newData = new HighscoreData[highscoreData.Length + 1];
        for (int i = 0; i < highscoreData.Length; i++)
            newData[i] = highscoreData[i];
        newData[highscoreData.Length] = data;
        highscoreData = newData;
    }
}


[Serializable]
public class HighscoreDataList
{
    private static HighscoreDataset highscoreDataset;

    public static HighscoreDataset InitializeHighscores()
    {
        string jsonFile = Resources.Load<TextAsset>("Data/HighscoreData").text;
        highscoreDataset = JsonUtility.FromJson<HighscoreDataset>(jsonFile);

        int lengthList = highscoreDataset.GetSortedHighscoreData().Count;

        Debug.Log("Character Attacks Loaded: " + lengthList);
        return highscoreDataset;
    }

    public static void SerializeHighscores(HighscoreDataset dataset)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/HighscoreData");
        string newData = JsonUtility.ToJson(dataset);
        if(File.Exists("Assets/Resources/Data/HighscoreData.json"))
        {
            var sr = File.CreateText("Assets/Resources/Data/HighscoreData.json");
            sr.Write(newData);
            sr.Close();
        }
    }
}
