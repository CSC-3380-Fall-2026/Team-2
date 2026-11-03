using UnityEngine;

[System.Serializable]
public class Skill
{

    public string skillName;
    public string skillDesc;

    /*
    branch 0 = general upgrades
    branch 1  = tank path
    branch 2 = control path
    branch 3 = speed path
    */
    public int branch;
    public int skillCost;
    public bool isUnlocked;
    public float maxSpeedEffect;
    public float shieldEffect;
    public float accelerationEffect;
    public float deccelerationEffect;


}
