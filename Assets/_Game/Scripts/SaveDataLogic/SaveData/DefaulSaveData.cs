using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSaveData", menuName = "Save System/Default Save")]
public class DefaultSaveData : ScriptableObject
{
    public SceneData sceneData = new SceneData();
}