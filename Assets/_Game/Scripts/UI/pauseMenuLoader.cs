using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

//Handles loading the pauseMenu prefab into scenes.
public class pauseMenuLoader {
    //Pause Menu object reference.
    private static GameObject pauseMenu;

    //Finds the pauseMenu prefab, sets it up for use in 
    //scene changes, and checks the current starting scene.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] 
    private static void Initialize() {
        SceneManager.sceneLoaded -= onSceneLoaded;
        pauseMenu = findLoadedPrefab();
        if (pauseMenu == null) {
            Debug.LogError("pauseMenu prefab not found, add prefab into assets. pauseMenuLoader.cs");
            return;
        }

        SceneManager.sceneLoaded += onSceneLoaded;
        ensurePauseMenu(SceneManager.GetActiveScene());
    }

    //Finds the prefab asset by name "pauseMenu".
    private static GameObject findLoadedPrefab() {
        foreach (pauseMenu menu in Resources.FindObjectsOfTypeAll<pauseMenu>()) {
            GameObject menuPrefab = menu.gameObject;
            if (!menuPrefab.scene.IsValid() && menuPrefab.name == "pauseMenu") {
                return menuPrefab;
            }
        }

        return null;
    }

    //Checks new scene loads, additive loads are ignored 
    //and wont replace the current loaded scene.
    private static void onSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode == LoadSceneMode.Additive) {
            return;
        }

        ensurePauseMenu(scene);
    }

    //Creates the pause menu when inside supported 
    //gameplay scenes marked in the referenced array.
    private static void ensurePauseMenu(Scene scene) {
        pauseMenu scenes = pauseMenu.GetComponent<pauseMenu>();
        if (!scenes.isGameplayScene(scene)) {
            return;
        }

        ensureEventSystem();
        if (Object.FindAnyObjectByType<pauseMenu>() != null) {
            return;
        }

        Object.Instantiate(pauseMenu);
    }

    //Loads eventsystem if no event system is found.
    private static void ensureEventSystem() {
        if (EventSystem.current != null) {
            return;
        }
        
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }


}
