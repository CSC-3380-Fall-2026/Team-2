using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;


public class SkillTree: MonoBehaviour
{
    public Player player;
    public List<Skill> skills = new List<Skill>();

    private void Awake()
    {
        player = FindAnyObjectByType<Player>();

        skills.Add(new Skill
        {
            skillName = "TestSKill",
            skillDesc = "test skill",
            skillCost = 1,
            maxSpeedEffect = 100f,
            shieldEffect = 2,
            accelerationEffect = 5f,
            deccelerationEffect = 10f
        });

    }

    public void UnlockSkill(int index)
    {
        Skill skill = skills[index];

        //index checl
        if(index <0 || index >= skills.Count)
        {
            Debug.Log("Invalid Inxex");
            return;
        }

        if (skill.isUnlocked) {return;}

        if (player.skillPoints >= skill.skillCost)
        {
            player.skillPoints -= skill.skillCost;
            skill.isUnlocked = true;
            player.ApplySkills(skill);
            return;
        } else {return;}
    }

    private void Update()
 {
    if (Keyboard.current != null &&
        Keyboard.current.digit1Key.wasPressedThisFrame)
    {
        UnlockSkill(0);
    }
 }
}