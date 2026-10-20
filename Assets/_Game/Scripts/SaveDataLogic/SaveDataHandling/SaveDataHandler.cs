using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//Save Data Handler, manages all the logic that relating to save files currently.
public class saveDataHandler : MonoBehaviour {

[SerializeField] private string saveName;

//From SaveData.cs Script
private SaveData saveData;
private List<iSaveData> SaveDataObjects;

//Creates a new instance of the saveFileHandler class, which is used to load and save data to a file.
private saveFileHandler saveHandler;
    public static saveDataHandler instance { get; private set; }
    
    private void Awake() {
        if (instance != null) {
            Debug.LogError("Multiple saveDataHandler instances found! SavaDataHandler.cs ln19");
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
            Debug.Log("No save file found, creating new save file: saveDataHandler.cs ln49");
            newSave();
        }

        foreach (iSaveData saveDataObject in SaveDataObjects) {
            saveDataObject.loadData(saveData);
        }
    }

    //Saves the data to the save file.
    public void saveFile() {
        foreach (iSaveData saveDataObject in SaveDataObjects) {
            saveDataObject.saveData(ref saveData);
        }
        saveHandler.Save(saveData);
    }

    private void saveDataPlaceHolder() {
        saveFile();
    }

    //fixed line to a non depreciated version.
    private List<iSaveData> findSaveDataObjects() {
    return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include).OfType<iSaveData>().ToList(); 
    }
}
