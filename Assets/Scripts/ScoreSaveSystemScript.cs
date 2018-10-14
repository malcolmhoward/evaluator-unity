using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSaveSystemScript : MonoBehaviour {
    public Button button_obj;
    public string player_name;
    private Toggle[] toggle_list;
    public int points_earned;
    public int points_possible;
    public Text player_score_obj;
    public string player_score_text;
    public TextAsset save_file;
    public TextFileHandler save_file_handler;

	// Use this for initialization
	void Start () {
		player_name = GameObject.FindGameObjectsWithTag("PlayerName")[0].GetComponent<Text>().text;
        player_score_obj = GameObject.FindGameObjectsWithTag("PlayerScore")[0].GetComponent<Text>();
        Debug.Log(player_name);
        toggle_list = GameObject.FindObjectsOfType<Toggle>();
        foreach (Toggle toggle in toggle_list)
        {
            //Add listener for when the state of the Toggle changes, to take action
            toggle.onValueChanged.AddListener(delegate {
                UpdateScoreText();
            });
        }
        UpdateScoreText();
        save_file_handler = new TextFileHandler("Assets/Resources/player_scores.csv");
        button_obj = this.GetComponent<Button>();
        button_obj.onClick.AddListener(SaveAndReset);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateScoreText()
    {
        points_possible = toggle_list.Length;
        points_earned = 0;
        foreach (Toggle toggle in toggle_list)
        {
            if (toggle.isOn)
            {
                points_earned += 1;
            }
        }
        player_score_text = string.Format("{0}/{1}", points_earned, points_possible);
        Debug.Log(player_score_text);
        player_score_obj.text = player_score_text;
    }

    public void SaveAndReset()
    {
        // Capture current player_name and player_score, and write it to a new row the save_file CSV, along with the timestamp
        // Afterwards, reset the game to the initial state (via the game controller).
        //UpdateScoreText();
        string written_string = string.Format("{0}, {1}, {2}", player_name, player_score_text, System.DateTime.UtcNow.ToString("HH:mm:ss dd MMMM, yyyy"));
        save_file_handler.WriteString(written_string);
    }
}

public class Player
{
    public int id;
    public string first_name;
    public string last_name;
    public string display_name;

    // TODO: Overload this constructor so that all arguments aren't needed everytime this method is called
    public Player(string new_display_name = "", string new_first_name = "", string new_last_name = "", int new_id=0)
    {
        if (new_first_name != "")
        {
            this.first_name = new_first_name;
        }

        if (new_last_name != "")
        {
            this.last_name = new_last_name;
        }

        if (new_display_name != "")
        {
            this.display_name = new_display_name;
        }
        else
        {
            string temp_display_name = this.first_name + " " + this.last_name;
            if (temp_display_name != " ")
            {
                this.display_name = temp_display_name;
            }
        }

        if (new_id != 0)
        {
            this.SetID(new_id);
        }
    }

    public void SetID(int new_id)
    {
        this.id = new_id;
    }
}
