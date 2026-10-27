using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SkillTreeManager : MonoBehaviour
{
    public int skillPoints = 0;
    public List<Skill> skills = new List<Skill>();

    public bool UnlockSkill(string skillName)
    {
        Skill skill = skills.Find(
            s => s.skillName == skillName
        );

        if (skill == null)
        {
            return false;
        }

        if (skill.isUnlocked)
        {
            return false;
        }
        
        foreach (string prerequisite in skill.prerequisites)
        {
            Skill requiredSkill = skills.Find(
                s => s.skillName == prerequisite
            );
        

        if(requiredSkill == null || !requiredSkill.isUnlocked)
            {
                return false;
            }

       }

       skillPoints -= skill.cost;
       skill.isUnlocked = true;

       return true;
    }
}