using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;
using NUnit.Framework;

//Save Data Handler, manages all the logic that relating to save files currently.
public class saveDataHandler : MonoBehaviour {

[SerializeField] private string saveName;
[SerializeField] private DefaultSaveData defaultSaveData;   

//From SaveData.cs Script
private SaveData saveData;
private List<iSaveData> SaveDataObjects;
private SceneData sceneData;

//Creates a new instance of the saveFileHandler class, which is used to load and save data to a file.
private saveFileHandler saveHandler;
    public static saveDataHandler instance { get; private set; }
    public bool isLoading {get; private set;}

    //Initializes the save manager, checks for duplicates to destroy.
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        saveHandler = new saveFileHandler(
            Application.persistentDataPath,
            saveName
        );

        saveData = saveHandler.Load();

        if (saveData == null)
        {
            newSave();
        }
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
        if (isLoading || mode == LoadSceneMode.Additive) {
            return;
        }        
    
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
    public void newSave()
    {
        saveData = new SaveData();

        if (defaultSaveData != null)
        {
            saveData.sceneData = JsonUtility.FromJson<SceneData>(
                JsonUtility.ToJson(defaultSaveData.sceneData)
            );
        }
    }

    //Calls for the save file to be loaded. if one doesn't exist, it will call "newSave()" to make a file.
    public void loadFile()
    {
        StartCoroutine(loadFileRoutine());
    }
    

    //Saves the data to the save file.
    public void saveFile() {
        if (isLoading) {
            return;
        }
        
        captureSceneState();
        saveHandler.Save(saveData);
    }

    //Captures state before loading next scene.
    public void changeScene(string sceneName) {
        if (isLoading) {
            return;
        }
        
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

    //Selects a checkpoint and writes the checkpoint to the save file.
    public void saveCheckpoint(checkpointIdent checkpoint) {
        if (isLoading || checkpoint == null) {
            return;
        }

        if (!checkpoint.isConfigured()) {
            Debug.LogError("Checkpoint missing information. SaveDataHandler.cs", checkpoint);
            return;
        } 

        string scenePath = checkpoint.gameObject.scene.path;

        if(string.IsNullOrEmpty(scenePath) || !Application.CanStreamedLevelBeLoaded(scenePath)) {
            Debug.LogError("Checkpoint scene must be saved. SaveDataHandler.cs", checkpoint);
            return;
        }

        saveData.checkpointId = checkpoint.checkpointId;
        saveData.checkpointScene = scenePath;
        saveFile();
    }

    //Loads the file, waits for scene changes, then will respawn the player. check hell.
    public IEnumerator loadFileRoutine() {
        if (isLoading) {
            while (isLoading) { 
                yield return null;
            }
            yield break;        
        }
    
    isLoading = true;
    Player player = null;
    bool restorePhysics = false;
    bool prevSimulated = false;

        try {
            SaveData loadedData = saveHandler.Load();
            if (loadedData == null) {
                Debug.LogError("No Savedata Found.");
                yield break;
            }

        if(string.IsNullOrWhiteSpace(loadedData.checkpointId) || string.IsNullOrWhiteSpace(loadedData.checkpointScene)) {
            saveData = loadedData;
            applySceneData();
            Debug.LogError("Save has No Checkpoint Value");
            yield break;
        }

        if(!Application.CanStreamedLevelBeLoaded(loadedData.checkpointScene)) {
            Debug.LogError ("Checkpoint Scene Unavailable: " + loadedData.checkpointScene);
            yield break;
        }

        player = FindAnyObjectByType<Player>();
        
        if (player == null || player.rb == null) {
            Debug.LogError("Can't Respawn, Missing Player or RigidBody.");
            yield break; 
        }

        prevSimulated = player.rb.simulated;
        restorePhysics = true;
        player.rb.linearVelocity = Vector2.zero;
        player.rb.angularVelocity = 0f;
        player.rb.simulated = false;
        saveData = loadedData;

        if (SceneManager.GetActiveScene().path != saveData.checkpointScene) {
            AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(saveData.checkpointScene, LoadSceneMode.Single);
            yield return sceneLoad;
        }

        yield return null;
        checkpointIdent checkpoint = findSavedCheckpoint(saveData.checkpointId, saveData.checkpointScene);
        
        if (checkpoint == null) {
            Debug.LogError("Saved Checkpoint missing or duplicated: " + saveData.checkpointId);
            yield break;
        }

        if (!checkpoint.isConfigured()) {
            Debug.LogError("Saved Checkpoint Missing Spawn or Room References.", checkpoint);
            yield break;
        }

        applySceneData();
        checkpoint.respawn(player);
        }
        
        finally {
            if(restorePhysics && player != null && player.rb != null) {
                player.rb.simulated = prevSimulated;
            }
        
        isLoading = false;
        }
    }
    
    //Finds the active checkpoint that matches the savefile, returns null if it can't.
    private checkpointIdent findSavedCheckpoint(string checkpointId, string scenePath) {
        checkpointIdent match = null;
        checkpointIdent[] checkpoints = FindObjectsByType<checkpointIdent>();

        foreach (checkpointIdent checkpoint in checkpoints) {
            if (!checkpoint.isActiveAndEnabled || checkpoint.gameObject.scene.path != scenePath || checkpoint.checkpointId != checkpointId) {
                continue;
            }

            if (match != null) {
            Debug.LogError("Duplicate ID in the Scene: " + checkpointId, checkpoint);
            return null;
            }

            match = checkpoint;    
        }

        return match;
    }

    //fixed line to a non depreciated version.
    private List<iSaveData> findSaveDataObjects() {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include).OfType<iSaveData>().ToList(); 
    }
}
