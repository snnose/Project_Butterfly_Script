using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class UserCharacterData
{
    // slot id 에 적용된 User Character
    private static Dictionary<int, UserCharacter> usercharacterDictionary = new Dictionary<int, UserCharacter>();

    public static void SetUserCharacterDataDictionary(List<UserCharacter> userCharacterList)
    {
        foreach (UserCharacter userCharacter in userCharacterList)
        {
            // userCharacterDictionary 설정
            if (!usercharacterDictionary.ContainsKey(userCharacter.slotid))
            {
                usercharacterDictionary[userCharacter.slotid] = userCharacter;
            }
        }
    }

    /// <summary>
    /// 세이브 파일 갱신 시, 현재 User Character 데이터를 저장하도록 갱신합니다.
    /// </summary>
    /// <returns></returns>
    public static List<UserCharacter> UpdateUserCharacterData()
    {
        List<UserCharacter> userCharacterList = new List<UserCharacter>();

        foreach (KeyValuePair<int, UserCharacter> pair in usercharacterDictionary)
        {         
            userCharacterList.Add(pair.Value);
        }

        return userCharacterList;
    }

    /*
    public static void SetUserCharacterData(JSONObject jsonObject)
    {
        UserCharacter usercharacter = new UserCharacter();

        usercharacter.slotid = jsonObject["slotid"].intValue;
        usercharacter.id = jsonObject["id"].intValue;
        usercharacter.level = jsonObject["level"].intValue;

        if (!usercharacterDictionary.ContainsKey(usercharacter.slotid))
        {
            usercharacterDictionary.Add(usercharacter.slotid, usercharacter.id);
        }
        if (!usercharacterlevelDictionary.ContainsKey(usercharacter.id))
        {
            usercharacterlevelDictionary.Add(usercharacter.id, usercharacter.level);
        }
    }
    */

    public static UserCharacter GetUserCharacter(int slotId)
    {
        return usercharacterDictionary[slotId];
    }

    /// <summary>
    /// Slot Id에 해당하는 User Character의 Id를 받습니다.
    /// </summary>
    /// <param name="slotId"></param>
    /// <returns></returns>
    public static int GetUserCharacterId(int slotId)
    {
        if(usercharacterDictionary.ContainsKey(slotId))
        {
            return usercharacterDictionary[slotId].id;
        }
        else
        {
            Debug.LogError("Id:"+ slotId +" Cannot Found!");
            return 0;
        }
    }
    // 해당 character의 level 정보를 character id를 기준으로 반환
    public static int GetUserCharacterLevel(int slotId)
    {
        if (usercharacterDictionary.ContainsKey(slotId))
        {
            return usercharacterDictionary[slotId].level;
        }
        else
        {
            Debug.LogError("Id:" + slotId + " Cannot Found!");
            return 0;
        }
    }
   
    /// <summary>
    /// 현재 적용 중인 캐릭터를 교체합니다.
    /// </summary>
    /// <param name="slotId">교체할 슬롯의 번호</param>
    /// <param name="characterId">교체할 캐릭터의 id</param>
    /// <param name="characterLevel">교체할 캐릭터의 레벨</param>
    public static void ChangeCharacter(int slotId, int characterId, int characterLevel)
    {
        usercharacterDictionary[slotId].id = characterId;
        usercharacterDictionary[slotId].level = characterLevel;
    }
}
