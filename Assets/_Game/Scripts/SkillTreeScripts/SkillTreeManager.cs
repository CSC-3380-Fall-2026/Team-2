using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


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

        skills.Add(new Skill
        {
            skillName = "TestSKill1",
            skillDesc = "test skill",
            skillCost = 1,
            maxSpeedEffect = 100f,
            shieldEffect = 1

        });

        skills.Add(new Skill
        {
            skillName = "TestSkill2",
            skillDesc = "ts2",
            skillCost = 2,
            deccelerationEffect = 20f,
            accelerationEffect = 15f

        });

    }

    public void UnlockSkill(int index)
    {


        //index checl
        if (index < 0 || index >= skills.Count)
        {
            Debug.Log("Invalid Inxex");
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
    }



    //PerkDebugging
    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            UnlockSkill(0);
        }

        if (Keyboard.current != null &&
        Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            UnlockSkill(1);
        }

        if (Keyboard.current != null &&
        Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            player.skillPoints += 1;
            Debug.Log("SkillPoints: " + player.skillPoints);
        }
    }

    public void ApplySkills(Skill skill)
    {
        player.numOfShield += skill.shieldEffect;
        player.acceleration += skill.accelerationEffect;
        player.deceleration += skill.deccelerationEffect;
        player.skillMaxSpeedBonus += skill.maxSpeedEffect;
    }
}