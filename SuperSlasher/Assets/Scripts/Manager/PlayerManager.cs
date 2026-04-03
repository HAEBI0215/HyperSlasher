using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("체력")]
    public int maxPlayerHp = 100;
    public int currentPlayerHp;

    [Header("스킬")]
    public float skillGauge = 0.0f;
    public float maxSkillGauge = 100.0f;
    public SkillControll skillControll;

    void Start()
    {
        currentPlayerHp = maxPlayerHp;
    }

    void Update()
    {
        ExcuteSkill();
    }

    private void ExcuteSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            skillControll.RushSlash(0);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            skillControll.ThrowsScythe(1);
        }
    }
    private void LoadSkillData(int index)
    {
        
    }
}
