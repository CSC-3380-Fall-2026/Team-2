using UnityEngine;
using System.Collections.Generic;


public class SkillTreeManager : MonoBehaviour
{
    public int skillPoints = 0;
    public List<Skill> skills = new List<Skill>();

    void Awake()
    {
         skills = new List<Skill>
         {
            new Skill
            {
                skillName = "Shield", 
                cost = 3
            },

            new Skill
            {
                skillName = "Max Speed 1",
                cost = 1
            }

         };
    }

  }