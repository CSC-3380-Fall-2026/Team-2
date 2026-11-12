using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.VisualScripting;


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



    //PerkDebugging
    private void Update()
    {
        if (player == null || Keyboard.current == null)
        {
            return;
        }

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
        Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            UnlockSkill(2);
        }
        if (Keyboard.current != null &&
        Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            UnlockSkill(3);
        }

        if (Keyboard.current != null &&
        Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            player.skillPoints += 1;
            Debug.Log("SkillPoints: " + player.skillPoints);
        }

        if (Keyboard.current != null &&
        Keyboard.current.digit9Key.wasPressedThisFrame)
        {
            Debug.Log("Max Speed: " + player.maxSpeed);
            Debug.Log( "Acceleration: "+ player.acceleration );
            Debug.Log( "Decceleration: " + player.deceleration );
            Debug.Log( "Num Shield: " + player.numOfShield);
            
        }
    }

    public void ApplySkills(Skill skill)
    {
        if (player == null)
        {
            Debug.LogError("player not found");
            return;
        }

        //player.numOfShield += skill.shieldEffect;
        player.acceleration += skill.accelerationEffect;
        //player.deceleration += skill.deccelerationEffect;
        //player.skillMaxSpeedBonus += skill.maxSpeedEffect;
    }

/*
    [SerializeField] public float acceleration = 0.1f;
    [SerializeField] public float deceleration = 0.2f;
    [SerializeField] public float baseMaxSpeed = 20f;

*/

    private void AddSkills()
    {
        skills.Add(new Skill
        {
         skillName = "Shield 1",
         skillBranch = 0,
         skillCost = 1,
         shieldEffect = 1,
         maxSpeedEffect = -player.maxSpeed * 0.5f,
         accelerationEffect = -player.acceleration * 0.35f
        });

        skills.Add(new Skill
        {
         skillName = "Shield 2",
         skillBranch = 0,
         skillCost = 1,
         shieldEffect = 1,
         accelerationEffect = -player.acceleration * 0.325f,
         deccelerationEffect = player.deceleration * 0.45f
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

        });
        Debug.Log("Total Skills Added: " + skills.Count);
    }

}