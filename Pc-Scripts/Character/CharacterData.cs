using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class CharacterData
{
    private static Dictionary<int, Character> characterDictionary = new Dictionary<int, Character>();

    public static void SetCharacterDictionary(List<Character> characterList)
    {
        foreach (Character character in characterList)
        {
            if (!characterDictionary.ContainsKey(character.id))
            {
                characterDictionary[character.id] = character;
            }
        }
    }

    public static void SetCharacterData(JSONObject jsonObject)
    {
        Character character = new Character();

        character.id = jsonObject["id"].intValue;
        character.key = jsonObject["key"].stringValue;
        character.characterType = (CharacterType)Enum.Parse(typeof(CharacterType), jsonObject["type"].stringValue);
        //character.characterColor = (CharacterColor)Enum.Parse(typeof(CharacterColor), jsonObject["color"].stringValue);
        character.characterBoardShape = jsonObject["boardShape"].intValue;
        //character.dropBonus = jsonObject["dropbonus"].intValue;
        //character.linkcount = jsonObject["linkcount"].intValue;

        // JSON 객체에서 "skills" 키에 해당하는 배열 가져오기
        JSONObject activeSkillListJsonObject = jsonObject.GetField("skills");
        // JSONObject를 int[]로 변환
        int[] activeSkillArray = new int[activeSkillListJsonObject.count];
        for (int i = 0; i < activeSkillListJsonObject.count; i++)
        {
            activeSkillArray[i] = activeSkillListJsonObject[i].intValue;
        }
        // 변환된 int[]를 할당
        character.activeSkillList = activeSkillArray;

        character.flowerSkill = jsonObject["flowerSkill"].intValue;
        //character.maximumlinkcount = jsonObject["maximumlinkcount"].intValue;
        character.maximumlevel = jsonObject["maximumlevel"].intValue;
        character.levelupcostperlevel = jsonObject["levelupcostperlevel"].intValue;

        if (!characterDictionary.ContainsKey(character.id))
        {
            characterDictionary.Add(character.id, character);
        }
    }
    public static int GetCharacterDataCount()
    {
        return characterDictionary.Count;
    }
    public static bool IsCharacterDataExist(int id)
    {
        if(characterDictionary.ContainsKey(id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static int IsCharacterFairyDataCount()
    {
        int caterpillarCount = characterDictionary.Values.Count(character => character.characterType == CharacterType.caterpillar);
        return caterpillarCount;
    }
    // 해당 Character의 Key를 id를 기준으로 반환
    public static string GetCharacterKey(int id)
    {
        if(characterDictionary.ContainsKey(id))
        {
            Character character = characterDictionary[id];
            return character.key;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 해당 Character의 boardShape를 id를 기준으로 반환
    public static int GetCharacterBoardShape(int id)
    {
        if(characterDictionary.ContainsKey(id))
        {
            Character character = characterDictionary[id];
            return character.characterBoardShape;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return 0;
        }
    }
    // 해당 Character를 id를 기준으로 반환
    public static Character GetCharacter(int id)
    {
        if(characterDictionary.ContainsKey(id))
        {
            Character character = characterDictionary[id];
            return character;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 해당 Character의 levelupcostperlevel을 id를 기준으로 반환
    public static int GetCharacterLevelupCostPerLevel(int id)
    {
        if(characterDictionary.ContainsKey(id))
        {
            Character character = characterDictionary[id];
            return character.levelupcostperlevel;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return 0;
        }
    }
    // 해당 Character의 maximumlevel id를 기준으로 반환
    public static int GetCharacterMaximumLevel(int id)
    {
        if(characterDictionary.ContainsKey(id))
        {
            Character character = characterDictionary[id];
            return character.maximumlevel;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return 0;
        }
    }
    // 해당 Character의 skillList를 id를 기준으로 반환
    public static int[] GetCharacterActiveSkillList(int id)
    {
        if(characterDictionary.ContainsKey(id))
        {
            Character character = characterDictionary[id];
            return character.activeSkillList;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 해당 Character의 FlowerSkill을 id를 기준으로 반환
    public static int GetCharacterFlowerSkill(int id)
    {
        if(characterDictionary.ContainsKey(id))
        {
            Character character = characterDictionary[id];
            return character.flowerSkill;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return 0;
        }
    }

    public static Dictionary<int, Character> GetCharacterDictionary()
    {
        return characterDictionary;
    }
}
