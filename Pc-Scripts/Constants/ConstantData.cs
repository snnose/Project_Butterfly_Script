using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class ConstantData
{
    private static Dictionary<string, int> constantDictionary = new Dictionary<string, int>();

    public static void SetConstantDictionary(List<Constant> constantList)
    {
        foreach (Constant constant in constantList)
        {
            if (!constantDictionary.ContainsKey("prologueprogress"))
            {
                constantDictionary.Add("prologueprogress", constant.prologueProgress);
            }
            if (!constantDictionary.ContainsKey("epilogueprogress"))
            {
                constantDictionary.Add("epilogueprogress", constant.epilogueProgress);
            }
        }
    }

    public static void SetConstantData(JSONObject jsonObject)
    {
        if (!constantDictionary.ContainsKey("prologueprogress"))
        {
            constantDictionary.Add("prologueprogress", jsonObject["prologueprogress"].intValue);
        }
        if (!constantDictionary.ContainsKey("epilogueprogress"))
        {
            constantDictionary.Add("epilogueprogress", jsonObject["epilogueprogress"].intValue);
        }
    }
    // 해당 Constant의 value를 key를 기준으로 반환
    public static int GetConstant(string key)
    {
        if(constantDictionary.ContainsKey(key))
        {
            int value = constantDictionary[key];
            return value;
        }
        else
        {
            Debug.LogError("Id:"+ key +" Cannot Found!");
            return 0;
        }
    }
}
