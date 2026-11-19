using UnityEngine;
using UnityEngine.InputSystem;

public class SkillTreeOverlay : MonoBehaviour
{
    public GameObject skillTreePanel;

    private Player player;

    void Start()
    {
        player = Player.instance;

        skillTreePanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.mKey.wasPressedThisFrame)
        {
            ToggleSkillTree();
        }
    }

    public void ToggleSkillTree()
    {
        bool isOpen = !skillTreePanel.activeSelf;

        skillTreePanel.SetActive(isOpen);

        if (player != null)
        {
            player.isSkillTreeOpen = isOpen;
        }
    }

    public void CloseSkillTree()
    {
        skillTreePanel.SetActive(false);

        if (player != null)
        {
            player.isSkillTreeOpen = false;
        }
    }
}