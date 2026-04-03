using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillControll : MonoBehaviour
{
    public SkillData[] equippedSkills = new SkillData[3];

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RushSlash(int index)
    {
        Debug.Log("돌진베기");
        Debug.Log(equippedSkills[index].skillName + " 사용/데미지: " + equippedSkills[index].damage);
    }
    public void ThrowsScythe(int index)
    {
        Debug.Log("낫 투척");
        Debug.Log(equippedSkills[index].skillName + " 사용/데미지: " + equippedSkills[index].damage);
    }
}
