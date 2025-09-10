using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class OptionData
{
    private static Dictionary<int, Option> optionDictionary = new Dictionary<int, Option>();

    public static void SetOptionDictionary(List<Option> optionList)
    {
        foreach (Option option in optionList)
        {
            if (!optionDictionary.ContainsKey(option.id))
            {
                optionDictionary[option.id] = option;
            }
        }
    }

    public static void SetOptionData(JSONObject jsonObject)
    {
        Option option = new Option();

        option.id = jsonObject["id"].intValue;
        option.key = jsonObject["key"].stringValue;
        option.basevalue = jsonObject["basevalue"].floatValue;
        option.addvalue = jsonObject["addvalue"].floatValue;

        if (!optionDictionary.ContainsKey(option.id))
        {
            optionDictionary.Add(option.id, option);
        }
    }
    public static int GetOptionDataCount()
    {
        return optionDictionary.Count;
    }
    public static bool IsOptionDataExist(int id)
    {
        if(optionDictionary.ContainsKey(id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // 해당 Option의 Key를 id를 기준으로 반환
    public static string GetOptionKey(int id)
    {
        if(optionDictionary.ContainsKey(id))
        {
            Option option = optionDictionary[id];
            return option.key;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 해당 Option를 id를 기준으로 반환
    public static Option GetOption(int id)
    {
        if(optionDictionary.ContainsKey(id))
        {
            Option option = optionDictionary[id];
            return option;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
}
