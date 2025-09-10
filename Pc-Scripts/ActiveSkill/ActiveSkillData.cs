using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class ActiveSkillData
{
    private static Dictionary<int, ActiveSkill> activeSkillDictionary = new Dictionary<int, ActiveSkill>();

    public static void SetActiveSkillDictionary(List<ActiveSkill> activeSkillList)
    {
        foreach (ActiveSkill activeSkill in activeSkillList)
        {
            if (!activeSkillDictionary.ContainsKey(activeSkill.id))
            {
                activeSkillDictionary[activeSkill.id] = activeSkill;
            }
        }
    }

    public static void SetActiveSkillData(JSONObject jsonObject)
    {
        ActiveSkill activeSkill = new ActiveSkill();

        activeSkill.id = jsonObject["id"].intValue;
        activeSkill.activeSkillType = (ActiveSkillType)Enum.Parse(typeof(ActiveSkillType), jsonObject["skillType"].stringValue);
        activeSkill.key = jsonObject["key"].stringValue;
        // JSON 객체에서 "skillParameter" 키에 해당하는 배열 가져오기
        JSONObject skillParameterJsonObject = jsonObject.GetField("skillParameter");
        // JSONObject를 int[]로 변환
        int[] skillParameterArray = new int[skillParameterJsonObject.count];
        for (int i = 0; i < skillParameterJsonObject.count; i++)
        {
            skillParameterArray[i] = skillParameterJsonObject[i].intValue;
        }
        // 변환된 int[]를 할당
        activeSkill.skillParameter = skillParameterArray;
        activeSkill.skillCoolTime = jsonObject["coolTime"].intValue;
        activeSkill.skillLimitCount = jsonObject["limitCount"].intValue;
        activeSkill.isActiveSkill = jsonObject["isActiveSkill"].boolValue;
        activeSkill.gimmickId = jsonObject["gimmickId"].intValue;
        activeSkill.maxSkillCounter = jsonObject["maxSkillCounter"].intValue;

        if (!activeSkillDictionary.ContainsKey(activeSkill.id))
        {
            activeSkillDictionary.Add(activeSkill.id, activeSkill);
        }
    }

    public static ActiveSkill GetActiveSkillById(int id)
    {
        if(activeSkillDictionary.ContainsKey(id))
        {
            ActiveSkill activeSkill = activeSkillDictionary[id];
            return activeSkill;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }   
    public static int GetActiveSkillGimmickId(int id)
    {
        if(activeSkillDictionary.ContainsKey(id))
        {
            return activeSkillDictionary[id].gimmickId;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return -1;
        }
    }   
}
