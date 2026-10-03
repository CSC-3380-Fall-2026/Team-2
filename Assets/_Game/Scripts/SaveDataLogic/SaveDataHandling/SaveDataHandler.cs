using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//Save Data Handling
public class SaveDataHandler : MonoBehaviour {

[SerializeField] private string saveName;

//From SaveData.cs Script
private SaveData saveData;
private List<intSaveData> SaveDataObjects;

//Creates a new instance of the saveFileHandler class, which is used to load and save data to a file.
private saveFileHandler saveHandler;
    public static SaveDataHandler instance { get; private set; }
    
    private void Awake() {
        if (instance != null) {
            Debug.LogError("Multiple SaveDataHandler instances found! SavaDataHandler.cs ln19");
        }
        else {
            instance = this;
        
        }
    } 

    //Loads the save file on game start.
    private void Start(){
        this.saveHandler = new saveFileHandler(Application.persistentDataPath, saveName);
        this.SaveDataObjects = findSaveDataObjects();
        loadSave();
    }
    

    //Calls for the creation of a new save file.
    public void newSave() {
        this.saveData = new SaveData();
    }

    //Calls for the save file to be loaded. if one doesn't exist, it will call "newSave()" to make a file.
    public void loadSave() {
        this.saveData = saveHandler.Load();

        if (this.saveData == null) {
            Debug.Log("No save file found, creating new save file: SaveDataHandler.cs ln49");
            newSave();
        }

        foreach (intSaveData saveDataObject in SaveDataObjects) {
            saveDataObject.loadData(saveData);
        }
    }

    //Saves the data to the save file.
    public void saveFile() {
        foreach (intSaveData saveDataObject in SaveDataObjects) {
            saveDataObject.saveData(ref saveData);
        }
    
        saveHandler.Save(saveData);
    }

    private void saveDataPlaceHolder() {
        saveFile();
    }

    private List<intSaveData> findSaveDataObjects() {
    IEnumerable<intSaveData> saveDataObjects = FindObjectsOfType<MonoBehaviour>().OfType<intSaveData>();
    return new List<intSaveData>(saveDataObjects); 
    }
}
