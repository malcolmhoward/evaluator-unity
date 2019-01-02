using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class ScoreSaveSystemScript : MonoBehaviour {
    public Button save_and_reset_button;
    public string player_name;
    private Toggle[] toggle_list;
    // toggle_name_dictionary is a dictionary of unique toggle names as keys, and their matching toggle objects as values
    // This is a very fast way to retrieve a toggle based on its name
    public Dictionary<string, Toggle> toggle_name_dictionary = new Dictionary<string, Toggle>();
    public Dictionary<string, List<string>> toggle_state_dictionary = new Dictionary<string, List<string>>();
    public List<string> checked_toggle_list, unchecked_toggle_list;
    public int points_earned;
    public int points_possible;
    public Text player_score_obj;
    public string player_score_text;
    public TextAsset save_file;
    public TextFileHandler save_file_handler;
    public string score_summary_json;
    public string serialized_score_entry;
    public PlayerScoreEntry current_score_entry;
    public List<PlayerScoreEntry> saved_score_entry_list;
    public int current_score_entry_index;
    public Player current_player;
    public List<Player> player_list;

    // Use this for initialization
    void Start () {
        this.save_file_handler = new TextFileHandler("Assets/Resources/player_scores.txt");
        this.LoadSavedScores();
        this.InitPlayer();
        // Create the toggle_list before doing anything else related to the toggle_list
        this.toggle_list = GameObject.FindObjectsOfType<Toggle>();
        this.InitToggleNameDictionary();
        this.InitToggleStateDictionary();
        foreach (Toggle toggle in this.toggle_list)
        {
            //Add listener for when the state of the Toggle changes, to take action
            toggle.onValueChanged.AddListener(delegate {
                UpdateScoreText();
            });
        }
        this.UpdateScoreText();
        // TODO: Add a save and reset button to the scene
        //this.save_and_reset_button = this.GetComponent<Button>();
        //this.save_and_reset_button.onClick.AddListener(SaveAndReset);
	}

    // Update is called once per frame
    void Update()
    {
        // TODO: Figure out why this doesn't work when runnning a standalone game build outside of the editor
        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            Debug.Log("CTRL + SHIFT hotkey pressed.");
            //if (Input.GetKeyUp(KeyCode.R))
            //{
            //    // CTRL + SHIFT + R
            //    Debug.Log("CTRL + SHIFT + R hotkey pressed.");
            //    Debug.Log("R hotkey pressed.");
            //    SaveAndResetGame();
            //}
            //else if (Input.GetKeyUp(KeyCode.L))
            //{
            //    // CTRL + SHIFT + L
            //    // This is just a placeholder hotkey used test specific functions on command
            //    Debug.Log("CTRL + SHIFT + L hotkey pressed.");
            //    Debug.Log("L hotkey pressed.  Reset the scene to the state of the last saved score.");
            //    string loaded_score_summary_string = this.current_score_entry.score_summary;
            //    Dictionary<string, List<string>> loaded_score_summary = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(loaded_score_summary_string);
            //    this.LoadScore(loaded_score_summary: loaded_score_summary, loaded_score_summary_index: this.current_score_entry_index);
            //}

            // We use GetKeyUp() here instead of GetKey() or GetKeyDown() so that we only watch for the moment when keys are released
            // Otherwise, the following logic might fire for every frame that all of the relevant keys are pressed
            // That is the opposite of what we want in this case
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                Debug.Log("LeftArrow or RightArrow hotkey pressed.");
                // CTRL + SHIFT + LeftArrow or CTRL + SHIFT + RightArrow
                // This is just a placeholder hotkey used test specific functions on command
                string next_score_location = "left";
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    Debug.Log("CTRL + SHIFT + LeftArrow hotkey pressed.");
                    //Debug.Log("LeftArrow hotkey pressed. Reset the scene to the state of the previously saved score.");
                }
                else if (Input.GetKeyUp(KeyCode.RightArrow))
                {
                    Debug.Log("CTRL + SHIFT + RightArrow hotkey pressed.");
                    //Debug.Log("RighttArrow hotkey pressed. Reset the scene to the state of the next saved score.");
                    next_score_location = "right";
                }

                if (this.saved_score_entry_list.Count > 0)
                {
                    this.LoadScore(next_score_location: next_score_location);
                }
                else
                {
                    Debug.Log("Warning: You're attempting to load a saved score when one is not available.  Try saving first with Control+Shift+R...");
                }
            }
        }
    }

    //void OnGUI()
    //{
    //    // TODO: Figure out why this doesn't work when runnning a standalone game build outside of the editor
    //    Event e = Event.current;
    //    // Only check one key release (i.e. EventType.KeyUp) instead of key press to prevent this from executing multiple times
    //    if (e.type == EventType.KeyUp)
    //    {
    //        Debug.Log("Some key pressed.");
    //        if (e.shift && e.control)
    //        {
    //            Debug.Log("CTRL + SHIFT hotkey pressed.");
    //            if (e.keyCode == KeyCode.R)
    //            {
    //                // CTRL + SHIFT + R
    //                Debug.Log("CTRL + SHIFT + R hotkey pressed.");
    //                Debug.Log("R hotkey pressed.");
    //                SaveAndResetGame();
    //            }
    //        }
    //        else if (e.keyCode == KeyCode.LeftArrow || e.keyCode == KeyCode.RightArrow)
    //        {
    //            Debug.Log("LeftArrow or RightArrow hotkey pressed.");
    //            // CTRL + SHIFT + LeftArrow or CTRL + SHIFT + RightArrow
    //            // This is just a placeholder hotkey used test specific functions on command
    //            string next_score_location = "left";
    //            if (e.keyCode == KeyCode.LeftArrow)
    //            {
    //                // Debug.Log("CTRL + SHIFT + LeftArrow hotkey pressed.");
    //                Debug.Log("LeftArrow hotkey pressed.");
    //            }
    //            else if (e.keyCode == KeyCode.RightArrow)
    //            {
    //                // Debug.Log("CTRL + SHIFT + RighttArrow hotkey pressed.");
    //                Debug.Log("RighttArrow hotkey pressed.");
    //                next_score_location = "right";
    //            }
    //            // Debug.Log("CTRL + SHIFT + L hotkey pressed.");
    //            // Debug.Log("L hotkey pressed.  Reset the scene to the state of the last saved score.");
    //            if (this.saved_score_entry_list.Count > 0)
    //            {
    //                this.LoadScore(next_score_location: next_score_location);
    //            }
    //            else
    //            {
    //                Debug.Log("Warning: You're attempting to load a saved score when one is not available.  Try saving first with Control+Shift+R...");
    //            }
    //        }
    //    }
    //}

    public void LoadSavedScores()
    {
        Debug.Log("LoadSavedScores() called");
        this.saved_score_entry_list = new List<PlayerScoreEntry>();
        string file_contents_string = this.save_file_handler.Read();
        List<string> file_contents_list = this.save_file_handler.ParseFileContents(file_contents_string:file_contents_string);
        foreach (string saved_score_entry_json in file_contents_list)
        {
            if (saved_score_entry_json != "")
            {
                PlayerScoreEntry loaded_score_entry = JsonConvert.DeserializeObject<PlayerScoreEntry>(saved_score_entry_json);
                this.saved_score_entry_list.Add(loaded_score_entry);
            }
        }

        // Default the score entry index to the size of the list for use in previous/next score navigation
        // The LoadScore() logic will adjust the index appropriately to prevent index out of bounds errors
        this.current_score_entry_index = this.saved_score_entry_list.Count;
    }

    public void SaveAndResetGame()
    {
        Debug.Log("SaveAndResetGame() called");
        this.SaveScore();
        this.ResetScore();
    }

    public void UpdateScoreText()
    {
        points_possible = this.toggle_list.Length;
        points_earned = 0;
        foreach (Toggle toggle in this.toggle_list)
        {
            if (toggle.isOn)
            {
                points_earned += 1;
            }
        }
        this.player_score_text = string.Format("{0}/{1}", points_earned, points_possible);
        // Debug.Log(this.player_score_text);
        if (this.player_score_obj != null)
        {
            this.player_score_obj.text = player_score_text;
        }
    }

    public void SaveAndReset()
    {
        // Capture current player_name and player_score, and write it to a new row the save_file CSV, along with the timestamp
        // Afterwards, reset the game to the initial state (via the game controller).
        //UpdateScoreText();
        string written_string = string.Format("{0}, {1}, {2}", player_name, player_score_text, System.DateTime.UtcNow.ToString("HH:mm:ss dd MMMM, yyyy"));
        save_file_handler.WriteString(written_string);
    }

    public void ResetScore()
    {
        Debug.Log("Now reseting the score, score text, and toggle states...");
        // Set all of the toggle objects to off, and the call UpdateScoreText
        this.points_earned = 0;
        this.UpdateScoreText();
        // Reset the toggle_name_dictionary to its initial value
        this.InitToggleNameDictionary(force_reset:true);
    }

    public void UpdateToggleStateDictionary()
    {
        foreach (Toggle current_toggle in this.toggle_list)
        {
            string toggle_name_string = current_toggle.name;
            bool toggle_state = current_toggle.isOn;
            string toggle_state_string = "unchecked";
            if (toggle_state == true)
            {
                toggle_state_string = "checked";
            }
            this.toggle_state_dictionary[toggle_state_string].Add(toggle_name_string);
        }
        this.score_summary_json = JsonConvert.SerializeObject(this.toggle_state_dictionary);
    }

    public void SaveScore()
    {

        Debug.Log("Now saving score...");
        DateTime timestamp = System.DateTime.UtcNow;
        this.InitToggleStateDictionary(force_reset: true);
        this.UpdateToggleStateDictionary();
        PlayerScoreEntry current_score_entry = new PlayerScoreEntry(new_player: this.current_player, new_timestamp: timestamp, new_score_total: this.points_earned, new_score_summary: this.score_summary_json);
        this.saved_score_entry_list.Add(current_score_entry);
        this.current_score_entry = current_score_entry;
        this.current_score_entry_index = this.saved_score_entry_list.Count - 1;
        // TODO: Implement the following score serialization logic in a SerializeScore() method
        this.serialized_score_entry = JsonConvert.SerializeObject(current_score_entry);
        // TODO: Update DeserializeScore() method to support ScoreEntry instances
        // this.current_score_entry = JsonConvert.DeserializeObject<ScoreEntry>(this.serialized_score_entry);
        this.save_file_handler.WriteString(this.serialized_score_entry);
        // The score has been saved to the score save file
        // Reload the saved scores to reset the saved_score_entry_list
        LoadSavedScores();
    }

    public void LoadScore(Dictionary<string, List<string>> loaded_score_summary=null, int loaded_score_summary_index=0, string next_score_location=null)
    {
        // Reloads the scene with the state of the most recently saved score
        Debug.Log("Now loading: current_score_entry...");

        // Update the current_score_entry_index based on the next_score_location or the loaded_score_summary_index
        if (next_score_location != null)
        {
            // Select the next score index based on the intended direction
            int relative_index_location = 0;
            if (next_score_location == "left")
            {
                relative_index_location = -1;
            }
            else if (next_score_location == "right")
            {
                relative_index_location = 1;
            }
            this.current_score_entry_index += relative_index_location;
        }
        else
        {
            this.current_score_entry_index = loaded_score_summary_index;
        }

        // Prevent IndexOutOfRangeException
        // Adjust the current_score_entry_index to make sure it's within the valid range of saved_score_entry_list
        if (this.current_score_entry_index <= 0)
        {
            // The oldest saved score has been loaded, so loop back to the newest one
            this.current_score_entry_index = this.saved_score_entry_list.Count - 1;
        }
        else if (this.current_score_entry_index >= this.saved_score_entry_list.Count)
        {
            // The newest saved score has been loaded, so loop back to the oldest one
            this.current_score_entry_index = 0;
        }

        Debug.LogFormat("this.current_score_entry_index = {0}", this.current_score_entry_index);
        this.current_score_entry = this.saved_score_entry_list[this.current_score_entry_index];

        // loaded_score_summary is of type Dictionary<string, List<string>>, just like the toggle_name_dictionary
        if (loaded_score_summary == null && this.current_score_entry != null)
        {
            // This method can be optionally called with a score summary instance.  If so, then load that instance's state into the scene.
            // If not, then use the most recently saved score summary, if it exists (i.e. this.current_score_entry).
            string loaded_score_summary_string = this.current_score_entry.score_summary;
            loaded_score_summary = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(loaded_score_summary_string);
        }

        // Make sure there's a loaded score summary before trying to process it.  It's null by default.
        if (loaded_score_summary != null)
        {
            // Reset the score first to prevent the score from duplicating on top of the current score in the scene
            this.ResetScore();
            foreach (KeyValuePair<string, List<string>> toggle_name_list in loaded_score_summary)
            {
                // Debug.Log(string.Format("this is the key: {0}", toggle_name_list.Key));
                string toggle_state_string = toggle_name_list.Key;
                // At this point, toggle_state_string should be either "checked" or "unchecked", depending on the current list
                // We will use toggle_state_string to set the new state of the toggle objects 
                bool toggle_state = false;
                if (toggle_state_string == "checked")
                {
                    toggle_state = true;
                }
                foreach (string toggle_name in toggle_name_list.Value)
                {
                    // Debug.Log(toggle_name);
                    // GameObject.Find() is slow, and apparently searches all available/active scenes
                    // TODO: Create another solution for retrieving the toggle object by name
                    // Toggle current_toggle = GameObject.Find(toggle_name).GetComponent<Toggle>();
                    // this.SetToggleState(current_toggle:current_toggle, is_on:toggle_state); 
                    Toggle current_toggle = this.toggle_name_dictionary[toggle_name];
                    this.SetToggleState(current_toggle:current_toggle, is_on:toggle_state);                    
                }
                // Update the score with the point value of this toggle if it was checked
                this.UpdateScoreText();
            }
        }
        else
        {
            Debug.Log("Warning: You're attempting to load a score when one has not been provided. Try saving first with Control+Shift+R...");
        }
    }

    public void InitToggleNameDictionary(bool force_reset = false)
    {
        bool toggle_dictionary_is_empty = this.toggle_name_dictionary.Count == 0;
        if (toggle_dictionary_is_empty || force_reset)
        {
            this.toggle_name_dictionary = new Dictionary<string, Toggle>();
            foreach (Toggle current_toggle in this.toggle_list)
            {
                string toggle_name_string = current_toggle.name;
                current_toggle.isOn = false;
                this.toggle_name_dictionary[toggle_name_string] = current_toggle;
            }
        }
    }

    public void InitToggleStateDictionary(bool force_reset = false)
    {
        this.checked_toggle_list = new List<string>();
        this.unchecked_toggle_list = new List<string>();
        bool state_dictionary_is_empty = this.toggle_state_dictionary.Count == 0;
        if (state_dictionary_is_empty || force_reset)
        {
            this.toggle_state_dictionary["checked"] = this.checked_toggle_list;
            this.toggle_state_dictionary["unchecked"] = this.unchecked_toggle_list;
        }
    }

    public void SetToggleState(Toggle current_toggle, bool is_on = false)
    {
        // Update the state of the current_toggle, including the image of the Checkmark on the Toggle Background
        current_toggle.isOn = is_on;
    }

    public void InitPlayer()
    {
        this.player_list = new List<Player>();
        this.current_player = new Player(new_first_name: "my_first_name", new_last_name: "my_last_name", new_id: 5);
        this.player_list.Add(current_player);
        //Debug.Log(player_list);
        //Debug.Log(current_player);
        //Debug.Log(current_player.display_name);
        //Debug.Log(current_player.first_name);
        //Debug.Log(current_player.last_name);
        //Debug.Log(current_player.id);
        this.InitPlayerScoreAndNameText();
        Debug.Log("player_list and current_player created successfully!");
    }

    public void InitPlayerScoreAndNameText()
    {
        try
        {
            // this.player_name = GameObject.FindGameObjectsWithTag("PlayerName")[0].GetComponent<Text>().text;
            // this.player_score_obj = GameObject.FindGameObjectsWithTag("PlayerScore")[0].GetComponent<Text>();
            this.player_name = GameObject.FindGameObjectWithTag("PlayerName").GetComponent<Text>().text;
            this.player_score_obj = GameObject.FindGameObjectWithTag("PlayerScore").GetComponent<Text>();
        }
        catch (UnityException ex)
        {
            // UnityException: Tag: PlayerName is not defined.
            // UnityException: Tag: PlayerScore is not defined.
            // Prevent the script from crashing if the 'PlayerName' and 'PlayerScore' tags are not defined
            this.player_name = "Player Name";
            this.player_score_obj = null;
            //Debug.Log(ex.ToString());
            Debug.Log("Either the PlayerName or the PlayerScore tag is not defined, so continue without it.");
        }
        catch (System.NullReferenceException ex)
        {
            // NullReferenceException: Object reference not set to an instance of an object
            // Prevent the script from crashing if the 'PlayerName' and 'PlayerScore' tags are defined, but the necessary objects are not tagged with them
            this.player_name = "Player Name";
            this.player_score_obj = null;
            // Debug.Log(ex.ToString());
            Debug.Log("The PlayerName or the PlayerScore tag is defined, but it's not in use, so continue without them.");
        }
        Debug.Log(this.player_name);
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

public class ScoreEntry
{
    public DateTime timestamp;
    public int score_total;
    public string score_summary;

    public ScoreEntry(DateTime new_timestamp, int new_score_total, string new_score_summary="")
    {
        
        this.SetTimestamp(new_timestamp);
        this.SetScoreTotal(new_score_total);
        if (new_score_summary != "")
        {
            this.SetScoreSummary(new_score_summary);
        }
    }

    public void SetTimestamp(DateTime new_timestamp)
    {
        this.timestamp = new_timestamp;
    }

    public void SetScoreTotal(int new_score_total)
    {
        this.score_total = new_score_total;
    }

    public void SetScoreSummary(string new_score_summary)
    {
        this.score_summary = new_score_summary;
    }
}

public class PlayerScoreEntry : ScoreEntry
{

    //public DateTime timestamp;
    //public int score_total;
    //public string score_summary;
    // ^^^^^ The above attributes are inherited from the parent/base class ScoreEntry.
    // They're commented out here for reference.
    public Player player;


    public PlayerScoreEntry(Player new_player, DateTime new_timestamp, int new_score_total, string new_score_summary = "") : base(new_timestamp, new_score_total, new_score_summary)
    {
        // This is the constructor for the PlayerScoreEntry class, which also immedialtey calls the constructor to the parent/base class (i.e. ScoreEntry)
        Debug.Log("PlayerScoreEntry constructor called");
        this.SetPlayer(new_player);
    }

    public void SetPlayer(Player new_player)
    {
        this.player = new_player;
    }
}
