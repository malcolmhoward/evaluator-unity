using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSaveSystemScript : MonoBehaviour {
    public Button button_obj;
    public string player_name;
    public string player_score;
    public Time timestamp;
    public TextAsset save_file;
    public TextFileHandler save_file_handler;

	// Use this for initialization
	void Start () {
		player_name = GameObject.FindGameObjectsWithTag("PlayerName")[0].GetComponent<Text>().text;
        Debug.Log(player_name);
		player_score = GameObject.FindGameObjectsWithTag("PlayerScore")[0].GetComponent<Text>().text;
        Debug.Log(player_score);
        save_file_handler = new TextFileHandler("Assets/Resources/player_scores.csv");
        button_obj = this.GetComponent<Button>();
        button_obj.onClick.AddListener(SaveAndReset);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SaveAndReset()
    {
        // Capture current player_name and player_score, and write it to a new row the save_file CSV, along with the timestamp
        // Afterwards, reset the game to the initial state (via the game controller).
    }
}
