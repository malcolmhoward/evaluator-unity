// Based off of https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-
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

    [MenuItem("Tools/Write file")]
    static void WriteString()
    {
        //string path = "Assets/Resources/test.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(this.path, true);
        writer.WriteLine("Test");
        writer.Close();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(this.path); 
        TextAsset asset = Resources.Load("test");

        //Print the text from the file
        Debug.Log(asset.text);
    }

    [MenuItem("Tools/Read file")]
    static void ReadString()
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