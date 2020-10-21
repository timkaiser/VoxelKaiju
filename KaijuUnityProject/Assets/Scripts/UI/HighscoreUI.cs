using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreUI : MonoBehaviour
{

    HighscoreDataset dataset;
    Text[] textFields = new Text[5];

    [SerializeField]
    Text text1;
    [SerializeField]
    Text text2;
    [SerializeField]
    Text text3;
    [SerializeField]
    Text text4;
    [SerializeField]
    Text text5;
    [SerializeField]
    GameObject start;

    List<HighscoreData> data;

    // Start is called before the first frame update
    void Awake()
    {
        dataset = HighscoreDataList.InitializeHighscores();
        textFields[0] = text1;
        textFields[1] = text2;
        textFields[2] = text3;
        textFields[3] = text4;
        textFields[4] = text5;
        start.SetActive(true);
        data = dataset.GetSortedHighscoreData();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateUI ()
    {
        data = dataset.GetSortedHighscoreData();
        if (data != null)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (i > 4)
                    break;
                textFields[i].text = data[i].getName() + " : " + data[i].getScore();
            }
        }
    }

    public void AddPlayerResults(HighscoreData newData)
    {
        dataset.AddData(newData);
        UpdateUI();
    }

    public void Save()
    {
        if(dataset == null)
        {
            dataset = HighscoreDataList.InitializeHighscores();
        }
        HighscoreDataList.SerializeHighscores(dataset);
    }
}
