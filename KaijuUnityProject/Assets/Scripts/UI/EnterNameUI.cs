using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterNameUI : MonoBehaviour
{
    InputField input;
    [SerializeField]
    GameObject highscoreUI;

    // Start is called before the first frame update
    void Awake()
    {
        input = transform.GetComponentInChildren<InputField>();
        highscoreUI = UIManager.Instance.GetHighscorePanel;
    }

    public void Continue()
    {
        highscoreUI.SetActive(true);
        int score = Mathf.RoundToInt( GameManager.Instance.GetComponent<Highscore>().GetScore());
        string name = input.text;
        HighscoreData data = new HighscoreData(score,name);
        highscoreUI.GetComponent<HighscoreUI>().AddPlayerResults(data);
        input.text = "";
        this.gameObject.SetActive(false);
    }

}
