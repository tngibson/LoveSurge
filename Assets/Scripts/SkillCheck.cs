using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillCheck : MonoBehaviour
{
    // get the skills
    // compare the skills to a certain set num
    // change the text

    // Number that the player has to meet or beat
    [SerializeField] int SkillCheckLevel;
    [SerializeField] TextMeshProUGUI textbox;
    [SerializeField] int id;
    [SerializeField] TextMeshProUGUI passText;
    [SerializeField] TextMeshProUGUI failText;
    List<int> stats;

    private void Start()
    {
        passText.enabled= false;
        failText.enabled= false;
        stats = Player.instance.GetStats();
        Debug.Log(SkillCheckLevel + " " + id + " " + stats.Count);
    }

    // id refers to which skill it is
    // order of stats is Charisma, Cleverness, Creativity, Courage
    public bool CompareStats()
    {
        if (id == 0)
        {
            if (stats[0] >= SkillCheckLevel)
            {
                return true;
            }
            else { return false; }
        }
        if (id == 1)
        {
            if (stats[1] >= SkillCheckLevel)
            {
                return true;
            }
            else { return false; }
        }
        if (id == 2)
        {
            if (stats[2] >= SkillCheckLevel)
            {
                return true;
            }
            else { return false; }
        }
        if (id == 3)
        {
            if (stats[3] >= SkillCheckLevel)
            {
                return true;
            }
            else { return false; }
        }
        else {return false; }
    }

    public void changeText()
    {
        
        bool CheckResult = CompareStats();
        textbox.enabled = false;
        if (CheckResult) { passText.enabled = true; }
        else { failText.enabled = true; }
        Debug.Log(CheckResult);
    }
}
