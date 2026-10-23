using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Player playerScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerScript = Object.FindAnyObjectByType<Player>(); 
        playerScript.getCameraBounds();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
