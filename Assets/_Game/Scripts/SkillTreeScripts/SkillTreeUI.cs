
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillTreeUI : MonoBehaviour
{
    public SkillTree skillTree;
    public TMP_Text skillPointsText;
    public Button[] skillButtons;
    public TMP_Text[] skillButtonText;

    public Player player;
    public TMP_Text[] skillDescText;
    private void Start()
    {
        skillTree = FindAnyObjectByType<SkillTree>();

        if (skillTree == null)
        {
            Debug.LogError("Skill Tree OBJ not found");
            return;
        }
        skillTree.Initialize();
        player = skillTree.player;
        UpdateSkillPointsUI();
        


        for (int i = 0; i < skillTree.skills.Count; i++)
        {
            ShowSkillDesc(i);
            if (skillTree.skills[i].isUnlocked)
            {
                skillButtonText[i].text = "Skill Unlocked";
            }

        }
    }

    public void UpdateSkillPointsUI()
    {
        if (player != null && skillPointsText != null)
        {
            skillPointsText.text = "Skill Points: " + player.skillPoints;
        }
    }

    public void UnlockSkill(int index)
    {
        if (skillTree == null)
            return;

        if (index < 0 || index >= skillTree.skills.Count)
            return;

        skillTree.UnlockSkill(index);

        UpdateSkillPointsUI();

        if (skillTree.skills[index].isUnlocked)
        {
            skillButtonText[index].text = "Skill Unlocked";
        }
    }


    public void ShowSkillDesc(int index)
    {
        if (index < 0 || index >= skillTree.skills.Count)
            return;

        skillDescText[index].text = skillTree.skills[index].skillName;
    }

    public void CloseSkillTree()
    {
        skillTree.CloseSkillTree();
    }
}
