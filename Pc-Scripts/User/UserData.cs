using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class UserData
{
    private static User user;
    private static Dictionary<string, User> userDictionary = new Dictionary<string, User>();
    private static Dictionary<int, bool> userCharacterOwnedDictionary = new Dictionary<int, bool>();
    private static Dictionary<int, bool> userCharacterFairyOwnedDictionary = new Dictionary<int, bool>();
    private static Dictionary<int, int> userCharacterLevelDictionary = new Dictionary<int, int>();
    private static Dictionary<int, int> userCharacterFairyLevelDictionary = new Dictionary<int, int>();
    private static Dictionary<int, bool> userFairyOwnedDictionary = new Dictionary<int, bool>();
    private static Dictionary<int, bool> userDropOwnedDictionary = new Dictionary<int, bool>();

    public static void SetUserDataDictionary(List<User> userDataList)
    {
        // 유저 데이터가 단 하나만 존재한다 (내부 Owned 리스트가 주요).
        user = userDataList[0];

        foreach (UserCharacterOwned characterOwned in user.userCharacterOwned)
        {
            // UserCharacterOwnedDictionary 작성
            if (!userCharacterOwnedDictionary.ContainsKey(characterOwned.id))
            {
                // 0~100 : character id
                if (characterOwned.id < 100)
                {
                    userCharacterOwnedDictionary[characterOwned.id] = characterOwned.isOwned;
                }
                // 101~ : character fairy id
                else
                {
                    userCharacterFairyOwnedDictionary[characterOwned.id] = characterOwned.isOwned;
                }
            }

            // UserCharacterFairyLevelDictionary 작성
            if (!userCharacterFairyLevelDictionary.ContainsKey(characterOwned.id))
            {
                userCharacterFairyLevelDictionary[characterOwned.id] = characterOwned.level;
            }
        }

        // UserFairyOwnedDictionary 작성
        foreach (UserFairyOwned fairyOwned in user.userFairyOwned)
        {    
            if (!userFairyOwnedDictionary.ContainsKey(fairyOwned.id))
            {
                userFairyOwnedDictionary[fairyOwned.id] = fairyOwned.isOwned;
            }
        }

        // UserDropOwnedDictionary 작성
        foreach (UserDropOwned dropOwned in user.userDropOwned)
        {
            if (!userFairyOwnedDictionary.ContainsKey(dropOwned.id))
            {
                userDropOwnedDictionary[dropOwned.id] = dropOwned.isOwned;
            }
        }
    }

    /// <summary>
    /// 세이브 파일 갱신 시, 현재 User 데이터를 저장하도록 갱신합니다.
    /// </summary>
    public static List<User> UpdateUserData()
    {
        List<User> userList = new List<User>();

        UserCharacterOwned[] userCharacterOwnedArray = user.userCharacterOwned;
        UserFairyOwned[] userFairyOwnedArray = user.userFairyOwned;
        UserDropOwned[] userDropOwnedArray = user.userDropOwned;

        // 캐릭터 보유 정보 갱신
        foreach (UserCharacterOwned userCharacterOwned in userCharacterOwnedArray)
        {
            if (userCharacterOwned.id < 100)
            {
                if (userCharacterOwnedDictionary.ContainsKey(userCharacterOwned.id))
                {
                    userCharacterOwned.isOwned = userCharacterOwnedDictionary[userCharacterOwned.id];
                }
            }
            else
            {
                if (userCharacterFairyOwnedDictionary.ContainsKey(userCharacterOwned.id))
                {
                    userCharacterOwned.isOwned = userCharacterFairyOwnedDictionary[userCharacterOwned.id];
                }
            }

            if (userCharacterFairyLevelDictionary.ContainsKey(userCharacterOwned.id))
            {
                userCharacterOwned.level = userCharacterFairyLevelDictionary[userCharacterOwned.id];
            }
        }

        // Fairy 보유 정보 갱신
        foreach (UserFairyOwned userFairyOwned in userFairyOwnedArray)
        {
            if (userFairyOwnedDictionary.ContainsKey(userFairyOwned.id))
            {
                userFairyOwned.isOwned = userFairyOwnedDictionary[userFairyOwned.id];
            }
        }

        // Drop 보유 정보 갱신
        foreach (UserDropOwned userDropOwned in userDropOwnedArray)
        {
            if (userDropOwnedDictionary.ContainsKey(userDropOwned.id))
            {
                userDropOwned.isOwned = userDropOwnedDictionary[userDropOwned.id];
            }
        }

        userList.Add(user);
        return userList;
    }

    /*
    public static void SetUserData(JSONObject jsonObject)
    {
        User user = new User();
        user.id = PlayFabUserData.GetPlayFabId();
        user.characters = jsonObject["characters"];
        user.fairies = jsonObject["fairies"];
        user.drops = jsonObject["drops"];

        if (!userDictionary.ContainsKey(user.id))
        { 
            userDictionary.Add(user.id, user);
        }

        // user.characters를 다시 dictionary화 : Owned Dictionary
        foreach (JSONObject item in user.characters)
        {
            // 0~100 : character id
            if(item["id"].intValue < 100)
            {
                if (!userCharacterOwnedDictionary.ContainsKey(item["id"].intValue))
                {
                    userCharacterOwnedDictionary.Add(item["id"].intValue, item["isOwned"].boolValue);
                }
            }
            // 101~ : character fairy id
            else
            {
                if (!userCharacterFairyOwnedDictionary.ContainsKey(item["id"].intValue))
                {
                    userCharacterFairyOwnedDictionary.Add(item["id"].intValue, item["isOwned"].boolValue);
                }
            }
        }
        // user.characters를 다시 dictionary화 : Level Dictionary
        foreach (JSONObject item in user.characters)
        {
            // 0~100 : character id
            if(item["id"].intValue < 100)
            {
                if (!userCharacterLevelDictionary.ContainsKey(item["id"].intValue))
                {
                    userCharacterLevelDictionary.Add(item["id"].intValue, item["level"].intValue);
                }
            }
            // 101~ : character fairy id
            else
            {
                if (!userCharacterFairyLevelDictionary.ContainsKey(item["id"].intValue))
                {
                    userCharacterFairyLevelDictionary.Add(item["id"].intValue, item["level"].intValue);
                }
            }
        }
        // user.fairies 다시 dictionary화
        foreach (JSONObject item in user.fairies)
        {
            if (!userFairyOwnedDictionary.ContainsKey(item["id"].intValue))
            {
                userFairyOwnedDictionary.Add(item["id"].intValue, item["isOwned"].boolValue);
            }
        }
        // user.drops 다시 dictionary화
        foreach (JSONObject item in user.drops)
        {
            if (!userDropOwnedDictionary.ContainsKey(item["id"].intValue))
            {
                userDropOwnedDictionary.Add(item["id"].intValue, item["isOwned"].boolValue);
            }
        }
    }
    */

    public static User GetUser()
    {
        return user;
    }

    // 해당 User정보를 id를 기준으로 반환
    public static User GetUser(string id)
    {
        if(userDictionary.ContainsKey(id))
        {
            User user = userDictionary[id];
            return user;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // character 를 보유중인지 id로 체크
    public static bool GetUserCharacterOwnedData(int id)
    {
        if(userCharacterOwnedDictionary.ContainsKey(id))
        {
            return userCharacterOwnedDictionary[id];
        }
        else
        {
            Debug.LogWarning("Id:"+ id +" NotOwned!");
            return false;
        }
    }
    public static Dictionary<int, bool> GetUserCharacterFairy()
    {
        return userCharacterFairyOwnedDictionary;
    }
    // character fairy를 보유중인지 id로 체크
    public static bool GetUserCharacterFairyOwnedData(int id)
    {
        if(userCharacterFairyOwnedDictionary.ContainsKey(id))
        {
            return userCharacterFairyOwnedDictionary[id];
        }
        else
        {
            Debug.LogWarning("Id:"+ id +" NotOwned!");
            return false;
        }
    }
    // character fairy 보유 상태 변경
    public static void SetUserCharacterFairyOwnedData(int id, bool value)
    {
        if(userCharacterFairyOwnedDictionary.ContainsKey(id))
        {
            userCharacterFairyOwnedDictionary[id] = value;
        }
        else
        {
            Debug.LogWarning("Id:"+ id +" Not Existed!");
        }
    }
    // fairy의 보유 상태 변경
    public static void SetUserFairyOwnedData(int id, bool value)
    {
        if(userFairyOwnedDictionary.ContainsKey(id))
        {
            userFairyOwnedDictionary[id] = value;
        }
        else
        {
            Debug.LogWarning("Id:"+ id +" Not Existed!");
        }
    }
    // fairy 를 보유중인지 id로 체크
    public static bool GetUserFairyOwnedData(int id)
    {
        return userFairyOwnedDictionary[id];
    }
    // drop의 보유 상태 변경
    public static void SetUserdropOwnedData(int id, bool value)
    {
        if(userDropOwnedDictionary.ContainsKey(id))
        {
            userDropOwnedDictionary[id] = value;
        }
        else
        {
            Debug.LogWarning("Id:"+ id +" Not Existed!");
        }
    }
    // drop를 보유중인지 id로 체크
    public static bool GetUserDropOwnedData(int id)
    {
        // id에 해당하는 키가 없으면 false 반환
        if (!userDropOwnedDictionary.ContainsKey(id))
        {
            Debug.LogWarning($"UserData - UserDropOwnedDictionary에 Drop Id : {id}에 해당하는 값이 존재하지 않습니다!");
            return false;
        }

        return userDropOwnedDictionary[id];
    }
    // 해당 id의 fairy 의 레벨을 반환
    public static int GetUserFairyLevelData(int id)
    {
        return userCharacterFairyLevelDictionary[id];
    }
    // 해당 id의 fairy 의 레벨을 갱신(+1)
    public static void UpdateUserFairyLevelData(int fairyId, int fairyLevel)
    {
        userCharacterFairyLevelDictionary[fairyId] = fairyLevel;
    }
    
}
