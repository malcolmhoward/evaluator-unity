﻿// Based off of https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-
using UnityEngine;
using UnityEditor;
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

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(this.path, true);
        writer.WriteLine(written_string);
        Debug.Log(string.Format("Value written to save file: {0}", written_string));
        writer.Close();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(this.path); 
        TextAsset asset = Resources.Load("player_score") as TextAsset;

        //Print the text from the file
        Debug.Log(asset.text);
    }

    // Method TextFileHandler.ReadString is not static and cannot be used for menu commands.
    //[MenuItem("Tools/Read file")]
    public void ReadString()
    {
        //string path = "Assets/Resources/test.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(this.path); 
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }

}

//For more information, consult the following documentation:
//    Unity TextAsset script reference documentation
//    Microsoft StreamWriter class documentation
//    Microsoft StreamReader class documentation