using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class SkillTree : MonoBehaviour
{
    public Player player;
    public List<Skill> skills = new List<Skill>();

    private static SkillTree instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        player = FindAnyObjectByType<Player>();
        AddSkills();


    }

    public void UnlockSkill(int index)
    {


        //index checl
        if (index < 0 || index >= skills.Count)
        {
            Debug.Log("Invalid Index");
            return;
        }

        Skill skill = skills[index];

        if (skill.isUnlocked) { return; }

        if (player == null)
        {
            Debug.LogError("Player refrence not found");
            return;
        }

        if (player.skillPoints >= skill.skillCost)
        {

            player.skillPoints -= skill.skillCost;
            skill.isUnlocked = true;
            ApplySkills(skill);
            Debug.Log("Unlocked Skill: " + skill.skillName);
            Debug.Log("Remainging SKill Points: " + player.skillPoints);
            return;
        }
        else
        {
            Debug.Log("Not enough skill points");
            return;
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = FindAnyObjectByType<Player>();

        if (player == null)
        {
            return;
        }

    }


    public void ApplySkills(Skill skill)
    {
        if (player == null)
        {
            Debug.LogError("player not found");
            return;
        }

        player.numOfShield += skill.shieldEffect;
        player.acceleration += skill.accelerationEffect;
        player.deceleration += skill.decelerationEffect;
        player.skillMaxSpeedBonus += skill.maxSpeedEffect;
    }


    private void AddSkills()
    {
        skills.Add(new Skill
        {
            skillName = "Shield 1",
            skillBranch = 1,
            skillCost = 1,
            shieldEffect = 1,
            maxSpeedEffect = -player.maxSpeed * 0.5f,
            accelerationEffect = -player.acceleration * 0.35f
        });

        skills.Add(new Skill
        {
            skillName = "Shield 2",
            skillBranch = 1,
            skillCost = 1,
            shieldEffect = 1,
            accelerationEffect = -player.acceleration * 0.325f,
            decelerationEffect = player.deceleration * 0.45f
        });

        skills.Add(new Skill
        {
            skillName = "Shield 3",
            skillBranch = 1,
            skillCost = 1,
            shieldEffect = 1,
            accelerationEffect = -player.acceleration * 0.10f
        });

        skills.Add(new Skill
        {
            skillName = "Speed 1",
            skillBranch = 2,
            skillCost = 1,
            accelerationEffect = player.acceleration * 0.10f
        });

        skills.Add(new Skill
        {
            skillName = "Speed 2",
            skillBranch = 2,
            skillCost = 1,
            maxSpeedEffect = player.maxSpeed * 0.5f,
            decelerationEffect = -player.deceleration * 0.25f
        });

        skills.Add(new Skill
        {
            skillName = "Speed 3",
            skillBranch = 2,
            skillCost = 1,
            maxSpeedEffect = player.maxSpeed * 0.5f,
            accelerationEffect = player.acceleration * 0.5f,
            decelerationEffect = -player.deceleration * 0.25f
        });

        skills.Add(new Skill
        {
            skillName = "Control 1",
            skillBranch = 3,
            skillCost = 1,
            decelerationEffect = player.deceleration * 0.35f
        });

        skills.Add(new Skill
        {
            skillName = "Control 2",
            skillBranch = 3,
            skillCost = 1,
            decelerationEffect = player.deceleration * 0.35f
        });

        skills.Add(new Skill
        {
            skillName = "Control 3",
            skillBranch = 3,
            skillCost = 1,
            decelerationEffect = player.deceleration * 0.35f
        });
    }

}