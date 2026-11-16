using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;


public class SkillTreeUI : MonoBehaviour
{
private SkillTree skillTree;

void Start()
{
    skillTree = FindAnyObjectByType<SkillTree>();

    skillTree.UnlockSkill;
}
}


//PUT BUTTON CODE IN HERE