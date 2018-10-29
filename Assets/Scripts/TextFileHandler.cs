// Based off of https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class TextFileHandler
{
    // The path of the text file to be used for reading or writing.
    // This file path should be defined during instantiation instead of being hard-coded.
    public string path;    
    
    public TextFileHandler(string file_path) {
        this.path = file_path;
    }

    // Method TextFileHandler.WriteString() is not static and cannot be used for menu commands.
    //[MenuItem("Tools/Write file")]
    public void WriteString(string written_string)
    {
        //string path = "Assets/Resources/test.txt";

        //Write some text to the file at this.path
        StreamWriter writer = new StreamWriter(this.path, true);
        writer.WriteLine(written_string);
        Debug.Log(string.Format("Value written to save file: {0}", written_string));
        writer.Close();

        //Re-import the file to update the reference in the editor
        // AssetDatabase.ImportAsset(this.path); 
        // TextAsset asset = Resources.Load<TextAsset>("player_scores");

        //Print the text from the file
        //Debug.Log(asset.text);  // <-- NullReferenceException: Object reference not set to an instance of an object
    }

    // Method TextFileHandler.ReadString is not static and cannot be used for menu commands.
    //[MenuItem("Tools/Read file")]
    public string ReadString(bool one_line_only=false)
    {
        //string path = "Assets/Resources/test.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(this.path);
        string read_string = "";
        if (one_line_only == true)
        {
            // TODO: Loop through to call ReadLine() for reach line in the reader
            read_string = reader.ReadLine();
        }
        else
        {
            read_string = reader.ReadToEnd();
        }
        
        Debug.Log(read_string);
        reader.Close();

        return read_string;
    }

    public string Read()
    {
        //string path = "Assets/Resources/test.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(this.path);
        string file_contents_string = reader.ReadToEnd();

        Debug.Log(file_contents_string);
        reader.Close();

        return file_contents_string;
    }

    public List<string> ParseFileContents(string file_contents_string)
    {
        // Convert file contents string to an array of strings
        StreamReader reader = new StreamReader(this.path);
        string[] file_contents_arrary = file_contents_string.Split('\n');
        List<string> file_contents_list = new List<string>(file_contents_arrary);
        Debug.Log(file_contents_list);

        return file_contents_list;
    }
}

//For more information, consult the following documentation:
//    Unity TextAsset script reference documentation
//    Microsoft StreamWriter class documentation
//    Microsoft StreamReader class documentation