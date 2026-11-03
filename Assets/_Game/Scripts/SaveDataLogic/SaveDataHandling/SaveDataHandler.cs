using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

//Save Data Handler, manages all the logic that relating to save files currently.
public class saveDataHandler : MonoBehaviour {

[SerializeField] private string saveName;

//From SaveData.cs Script
private SaveData saveData;
private List<iSaveData> SaveDataObjects;
private SceneData sceneData;

//Creates a new instance of the saveFileHandler class, which is used to load and save data to a file.
private saveFileHandler saveHandler;
    public static saveDataHandler instance { get; private set; }
    
    //Initializes the save manager, checks for duplicates to destroy.
    private void Awake() {
        if (instance != null && instance != this) {
            Debug.LogError("Multiple saveDataHandler instances found! SavaDataHandler.cs ln19");
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        this.saveHandler = new saveFileHandler(Application.persistentDataPath, saveName);
        saveData = saveHandler.Load() ?? new SaveData();
    } 
    
    //Listens for scene loads, to properly manage save manager.
    private void OnEnable() {
        if (instance == this)
            SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //opposite of the above, stops listening.
    private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Clears shared instances of save managers.
    private void OnDestroy() {
        if (instance == this)
            instance = null;
    }

    //Responds to a completed scene load.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        applySceneData();
    }

    //Pushes saved data into current objects in the scene.
    private void applySceneData() {
        foreach (iSaveData saveDataObject in findSaveDataObjects()) {
            saveDataObject.loadData(saveData);
        }
    }

    //Collects the states of current objects, pushes them into save data.
    private void captureSceneState() {
        foreach (iSaveData saveDataObject in findSaveDataObjects()) {
            saveDataObject.saveData(ref saveData);
        }
    }

    //Calls for the creation of a new save file.
    public void newSave() {
        this.saveData = new SaveData();
    }

    //Calls for the save file to be loaded. if one doesn't exist, it will call "newSave()" to make a file.
    public void loadFile() {
        saveData = saveHandler.Load() ?? new SaveData();
        applySceneData();
    }

    //Saves the data to the save file.
    public void saveFile() {
        captureSceneState();
        saveHandler.Save(saveData);
    }

    //Captures state before loading next scene.
    public void changeScene(string sceneName) {
        captureSceneState();
        SceneManager.LoadScene(sceneName);
    }

    //Debug method for saving and loading data manually pre-checkpoints.
    //MUST BE HOLDING CTRL, O = Save, L = Load
    private void Update() {
        if (instance != this) {
            return;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard.ctrlKey.isPressed) {
            return;
        }
        if (keyboard.oKey.wasPressedThisFrame) {
            Debug.Log("Debug Save Called.");
            saveFile();
        }
        else if (keyboard.lKey.wasPressedThisFrame) {
            Debug.Log("Debug Load Called.");
            loadFile();
        }
    }

    //Debug method which clears the savefile after the killing the game.
    //DELETE THIS SHIT AFTER WE GET FURTHER ALONG IN DEV.
    private void saveDebugClear() {

    }

    //fixed line to a non depreciated version.
    private List<iSaveData> findSaveDataObjects() {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include).OfType<iSaveData>().ToList(); 
    }
}
