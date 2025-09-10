using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class CollectionData
{
    private static Dictionary<int, Collection> collectionDictionary = new Dictionary<int, Collection>();

    public static void SetCollectionDictionary(List<Collection> collectionList)
    {
        foreach (Collection collection in collectionList)
        {
            if (!collectionDictionary.ContainsKey(collection.id))
            {
                collectionDictionary[collection.id] = collection;
            }
        }
    }

    public static void SetCollectionData(JSONObject jsonObject)
    {
        Collection collection = new Collection();

        collection.id = jsonObject["id"].intValue;
        collection.key = jsonObject["key"].stringValue;
        collection.optionid = jsonObject["option"].intValue;

        if (!collectionDictionary.ContainsKey(collection.id))
        {
            collectionDictionary.Add(collection.id, collection);
        }
    }
    public static int GetCollectionDataCount()
    {
        return collectionDictionary.Count;
    }
    public static bool IsCollectionDataExist(int id)
    {
        if(collectionDictionary.ContainsKey(id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // 해당 Collection의 Key를 id를 기준으로 반환
    public static string GetCollectionKey(int id)
    {
        if(collectionDictionary.ContainsKey(id))
        {
            Collection collection = collectionDictionary[id];
            return collection.key;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 해당 Collection를 id를 기준으로 반환
    public static Collection GetCollection(int id)
    {
        if(collectionDictionary.ContainsKey(id))
        {
            Collection collection = collectionDictionary[id];
            return collection;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }

    // optionId로 해당하는 Collection의 id를 반환
    public static int GetCollectionIdByOptionId(int optionId)
    {
        // 조건에 맞는 Fairy 찾기
        foreach (var collection in collectionDictionary.Values)
        {
            if (collection.optionid == optionId)
            {
                return collection.id; // 조건에 맞는 Collection Id 반환
            }
        }

        // 조건에 맞는 Collection이 없으면 0 반환
        return 0;
    }
}
