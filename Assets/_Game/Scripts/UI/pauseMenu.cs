using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

//Pause Menu System
public class pauseMenu : MonoBehaviour {
    
    //Variables related to functions associated with the pause menu system.
    [Header("Pause Vars")]
    private static pauseMenu instance;
    public static bool isPaused => instance != null && (instance.paused || instance.returningToMenu);
    private bool paused;
    private bool returningToMenu;
    private float prevTimeScale;
    private bool prevAudioPause;
    private bool prevCursorVisibility;
    private CursorLockMode prevCursorLock;

    [Header("Pause Menu Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Scene Vars")]
    [SerializeField] private string mainMenuScene = "Start Menu";
    [SerializeField] private string[] gameplayScenes = {"Game", "Game 2"};
    
    //Preps the pause menu system for general use, includes error checks before init.
    private void Awake() {
        if (instance != null && instance != this) {
            Debug.LogError("Multiple pauseMenu instances found. pauseMenu.cs");
            Destroy(gameObject);
            return;
        }
        if (!isGameplayScene(gameObject.scene)) {
            Debug.LogError("pauseMenu instance in non-gameplay scene destroyed. pauseMenu.cs");
            Destroy(gameObject);
            return;
        }
        if (pausePanel == null || settingsPanel == null) {
            Debug.LogError("Menu Panels not assigned. pauseMenu.cs");
            Destroy(gameObject);
            return;
        }

        instance = this;
        hidePanels();
        DontDestroyOnLoad(gameObject);
    }

    //Starts listening for scene changes to determine if the pause menu is needed.
    private void OnEnable() {
        if (instance == this) {
            SceneManager.sceneLoaded += onSceneLoaded;
        }
    } 

    //Stops listening for scene changes and restores gameplay if pauseed.
    private void OnDisable() {
        if (instance != this) {
            return;
        }
        
        SceneManager.sceneLoaded -= onSceneLoaded;
        restoreGameplay();
        hidePanels();
    }
    
    //Handles exiting the pause menu with key presses, ESC closes the menu.
    private void Update() {
        if (instance != this || returningToMenu) {
            return;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame) {
            return;
        }
        if (!paused) {
            Pause();
        }
        else if (settingsPanel.activeSelf) {
            closeSettings();
        }
        else {
            Resume();
        }
    }

    //Main method for pausing the game, shows the initial menu and freezes gameplay.
    public void Pause() {
        if (instance != this || paused || returningToMenu) {
            return;
        }

        prevTimeScale = Time.timeScale;
        prevAudioPause = AudioListener.pause;
        prevCursorVisibility = Cursor.visible;
        prevCursorLock = Cursor.lockState;
        paused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
        
    }

    //Main method for resuming the game, hides the pause menu and unfreezes gameplay.
    public void Resume() {
        if (instance != this || !paused || returningToMenu) {
            return;
        }

        hidePanels();
        restoreGameplay();

    }

    //Main method for opening the settings panel while the game is paused.
    public void openSettings() {
        if (instance != this || !paused || returningToMenu) {
            return;
        }
        
        pausePanel.SetActive(false); 
        settingsPanel.SetActive(true);
    }

    //Main method for closing the settings panel and returning to the pause menu.

    public void closeSettings() {
        if (instance != this || !paused || returningToMenu) {
            return;
        }
    
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true); 
    }

    //Method handles returning to the main menu scene "start menu" from whatever gameplay scene
    //the player is currently in, and cleans up gameplay objects to prevent duplicates.
    public void returnToMenu() {
        if (instance != this || !paused || returningToMenu) {
            return;
        }
        if (!Application.CanStreamedLevelBeLoaded(mainMenuScene)) {
            return;
        }

        returningToMenu = true;
        hidePanels();
        restoreGameplay();
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        removeGameplayObjects();
        SceneManager.LoadScene(mainMenuScene);
    }

    //Method reacts to the currently loaded scene, determines what scene is loaded and whether
    //to resume gameplay or destroy the pause menu instance.
    private void onSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode == LoadSceneMode.Additive) {
            return;
        }
        if (isGameplayScene(scene)) {
            Resume();
            return;
        }

        hidePanels();
        restoreGameplay();
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Destroy(gameObject);
    }

    //Restores all gameplay functions to their normal playing states.
    private void restoreGameplay() {
        if (!paused) {
            return;
        }
        Time.timeScale = prevTimeScale;
        AudioListener.pause = prevAudioPause;
        Cursor.lockState = prevCursorLock;
        Cursor.visible = prevCursorVisibility;
        paused = false;
    }

    //Hides all active menu panels when resuming current gameplay.
    private void hidePanels() {
        if (pausePanel != null) {
            pausePanel.SetActive(false);
        }
        if (settingsPanel != null) {
            settingsPanel.SetActive(false);
        }
    }

    //Function determines whether the loaded scene is 
    //a gameplay scene related to the serialized string array.
    public bool isGameplayScene(Scene scene) {
        foreach (string sceneName in gameplayScenes) {
            if (scene.name == sceneName) {
                return true;
            }
        }
        
        return false;
    }

    //Destroys all persistant gameplay objects that are currently in scene.
    private void removeGameplayObjects() {
        Player player = FindAnyObjectByType<Player>(FindObjectsInactive.Include);
        if (player != null) {
            GameObject playerRoot = player.playerObject != null ? player.playerObject : player.gameObject;
            playerRoot.SetActive(false);
            Destroy(playerRoot);
        }

        SkillTree skilltree = FindAnyObjectByType<SkillTree>(FindObjectsInactive.Include);
        if (skilltree != null) {
            skilltree.gameObject.SetActive(false);
            Destroy(skilltree.gameObject);
        }
    }

    //Method called to clean up the pause menu object when it is destroyed.
    private void OnDestroy() {
        if (instance != this) {
            return;
        }

        SceneManager.sceneLoaded -= onSceneLoaded;
        restoreGameplay();
        instance = null;
    }

}