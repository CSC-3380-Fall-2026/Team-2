using UnityEngine;

[System.Serializable]
public class Skill
{

    /*
    branch 0  = tank path
    branch 1 = control path
    branch 2 = speed path
    */

    public string skillName;
    public int skillBranch;
    public int skillCost;
    public bool isUnlocked;
    public float maxSpeedEffect;
    public float shieldEffect;
    public float accelerationEffect;
    public float decelerationEffect;

}
