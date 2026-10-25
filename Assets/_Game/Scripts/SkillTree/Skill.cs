using System.Collections.Generic;

[System.Serializable]
public class Skill
{
    public string skillName;
    public int cost;
    public List<string> prerequisites = new List<string>();
    public bool isUnlocked = false;
}