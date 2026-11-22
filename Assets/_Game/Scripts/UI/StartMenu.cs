using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//Start Menu: Continue resumes the save, New Game erases it (after a second click) and starts over.
public class StartMenu : MonoBehaviour
{
    [SerializeField] private string firstScene = "Game";
    [Tooltip("Must match the Save Name on the SaveHandler in the gameplay scenes.")]
    [SerializeField] private string saveName = "devSave1";
    [SerializeField] private GameObject continueButton;
    [SerializeField] private TMP_Text newGameLabel;
    [SerializeField] private string newGameText = "New Game";
    [SerializeField] private string confirmText = "Erase save?";
    [SerializeField, Min(0.5f)] private float confirmSeconds = 3f;

    private bool confirming;
    private Coroutine confirmTimeout;

    private string SaveName => saveName;

    void Start()
    {
        if (continueButton != null)
        {
            continueButton.SetActive(saveDataHandler.HasSave(SaveName));
        }
        ResetNewGameLabel();
    }

    public void Continue()
    {
        if (!saveDataHandler.HasSave(SaveName))
        {
            StartNewGame();
            return;
        }

        saveDataHandler.continueRequested = true;
        SceneManager.LoadScene(firstScene);
    }

    //With a save on disk, the first click asks for confirmation and the second erases it.
    public void NewGame()
    {
        if (saveDataHandler.HasSave(SaveName) && !confirming)
        {
            confirming = true;
            if (newGameLabel != null)
            {
                newGameLabel.text = confirmText;
            }
            confirmTimeout = StartCoroutine(CancelConfirmAfterDelay());
            return;
        }

        StartNewGame();
    }

    //Kept so anything still wired to the old Play button starts a game.
    public void Play()
    {
        Continue();
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void StartNewGame()
    {
        saveDataHandler.continueRequested = false;
        saveDataHandler.DeleteSave(SaveName);
        SceneManager.LoadScene(firstScene);
    }

    private IEnumerator CancelConfirmAfterDelay()
    {
        yield return new WaitForSecondsRealtime(confirmSeconds);
        ResetNewGameLabel();
    }

    private void ResetNewGameLabel()
    {
        confirming = false;
        if (confirmTimeout != null)
        {
            StopCoroutine(confirmTimeout);
            confirmTimeout = null;
        }
        if (newGameLabel != null)
        {
            newGameLabel.text = newGameText;
        }
    }
}
