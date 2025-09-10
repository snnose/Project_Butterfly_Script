using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class UserItemData
{
    private static Dictionary<int, UserItem> useritemDictionary = new Dictionary<int, UserItem>();

    public static void SetUserItemData(JSONObject jsonObject)
    {
        UserItem useritem = new UserItem();

        useritem.id = jsonObject["id"].intValue;
        useritem.level = jsonObject["level"].intValue;
        useritem.exp = jsonObject["exp"].intValue;
        useritem.mainoptionlevel = jsonObject["mainoptionlevel"].intValue;
        useritem.suboption1 = jsonObject["suboption1"].intValue;
        useritem.suboption1level = jsonObject["suboption1level"].intValue;
        useritem.suboption2 = jsonObject["suboption2"].intValue;
        useritem.suboption2level = jsonObject["suboption2level"].intValue;

        if (!useritemDictionary.ContainsKey(useritem.id))
        {
            useritemDictionary.Add(useritem.id, useritem);
        }
    }

    // 해당 User정보를 id를 기준으로 반환
    public static UserItem GetUserItem(int id)
    {
        if(useritemDictionary.ContainsKey(id))
        {
            UserItem useritem = useritemDictionary[id];
            return useritem;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    public static void UserItemExpUp(int id, int exp)
    {
        if(useritemDictionary.ContainsKey(id))
        {
            useritemDictionary[id].exp = exp;
        }
    }
    public static void UserItemLevelUp(int id, int level)
    {
        if(useritemDictionary.ContainsKey(id))
        {
            useritemDictionary[id].level = level;
        }
    }
    public static void UserItemReset(int id)
    {
        if(useritemDictionary.ContainsKey(id))
        {
            useritemDictionary[id].level = 1;
            useritemDictionary[id].exp = 0;
            useritemDictionary[id].mainoptionlevel = 1;
            useritemDictionary[id].suboption1 = 0;
            useritemDictionary[id].suboption1level = 0;
            useritemDictionary[id].suboption2 = 0;
            useritemDictionary[id].suboption2level = 0;
        }
    }
    /* FIXME :: UserItem데이터 변경 시 클라도 변경될 수 있도록 수정
    public static void SetUserItemDataCleared(int id, int score)
    {
        if (useritemDictionary.ContainsKey(id))
        {
            useritemDictionary[id].isCleared = true;
            if(useritemDictionary[id].highscore < score)
            {
                useritemDictionary[id].highscore = score;
            }
        }
    }
    */
}
