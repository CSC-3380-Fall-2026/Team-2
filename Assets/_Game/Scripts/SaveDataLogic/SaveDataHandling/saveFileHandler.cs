using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
//Current File Path is iirc: 
//"C:\Users\(USERNAMEHERE)\AppData\LocalLow\ZABCU Games\AtomPhaser-unity"

//Handles the loading and saving of data to a file.
public class saveFileHandler {
    private string saveFilePath = ""; //decide later
    private string saveFileName = ""; //decide later

    //Creates a new instance of the saveFileHandler class, which is used to load and save data to a file.
    public saveFileHandler(string saveFilePath, string saveFileName) {
        this.saveFilePath = saveFilePath;
        this.saveFileName = saveFileName;
    }

    //Loads the save file from the given path and returns the data as a SaveData object. If the file doesn't exist, it will return null.
    public SaveData Load() {
        string fullPath = Path.Combine(saveFilePath, saveFileName);
        SaveData loadedData = null;
        if (File.Exists(fullPath)) {
            try {
                string loadData = "";
                using (FileStream dataStream = new FileStream(fullPath, FileMode.Open)) {
                    using (StreamReader reader = new StreamReader(dataStream)) {
                        loadData = reader.ReadToEnd();  
                    }
                }
                loadedData = JsonUtility.FromJson<SaveData>(loadData);
            } 
            catch (Exception e) {
                Debug.LogError($"Failed to load data: saveFileHandler ln30 {e}");
            }
        }
        else {
            Debug.LogWarning("Save file not found: saveFileHandler ln34 ");
        }
        return loadedData;
    }

    //Saves the given SaveData object to the save file at the specified path.
    public void Save(SaveData data) {
        string fullPath = Path.Combine(saveFilePath, saveFileName);
        try {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string writeData = JsonUtility.ToJson(data, true);
            using (FileStream dataStream = new FileStream(fullPath, FileMode.Create)) {
                using (StreamWriter writer = new StreamWriter(dataStream)) {
                    writer.Write(writeData);
                }
            }
        } 
        catch (Exception e) {
            Debug.LogError($"$Failed to save data: saveFileHandler ln57 {e}");
        }
    }
}