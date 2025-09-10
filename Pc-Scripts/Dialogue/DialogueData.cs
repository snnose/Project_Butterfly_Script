using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class DialogueData
{
    private static Dictionary<int, Dialogue> dialogueDictionary = new Dictionary<int, Dialogue>();

    public static void SetDialogueDictionary(List<Dialogue> dialogueList)
    {
        foreach (Dialogue dialogue in dialogueList)
        {
            if (!dialogueDictionary.ContainsKey(dialogue.id))
            {
                dialogueDictionary[dialogue.id] = dialogue;
            }
        }
    }

    public static void SetDialogueData(JSONObject jsonObject)
    {
        Dialogue dialogue = new Dialogue();

        dialogue.id = jsonObject["id"].intValue;
        dialogue.progress = jsonObject["progress"].intValue;
        dialogue.scriptkey = jsonObject["scriptkey"].stringValue;
        dialogue.character = jsonObject["character"].stringValue;
        dialogue.anim = jsonObject["anim"].stringValue;
        dialogue.direction = jsonObject["direction"].stringValue;

        if (!dialogueDictionary.ContainsKey(dialogue.id))
        {
            dialogueDictionary.Add(dialogue.id, dialogue);
        }
    }
    public static int GetDialogueDataCount()
    {
        return dialogueDictionary.Count;
    }
    public static bool IsDialogueDataExist(int id)
    {
        if(dialogueDictionary.ContainsKey(id))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // 해당 Dialogue를 id를 기준으로 반환
    public static Dialogue GetDialogue(int id)
    {
        if(dialogueDictionary.ContainsKey(id))
        {
            Dialogue dialogue = dialogueDictionary[id];
            return dialogue;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // 특정 progress의 Dialogue만을 모두 추출해서 반환
    public static List<Dialogue> GetDialogueProgress(int targetProgress)
    {
        List<Dialogue> dialogues = new List<Dialogue>();

        // Dictionary의 모든 키-값 쌍을 순회하여 progress 값이 일치하는 Dialogue를 리스트에 추가
        foreach (var entry in dialogueDictionary.Values)
        {
            if (entry.progress == targetProgress)
            {
                dialogues.Add(entry);
            }
        }

        // id를 기준으로 정렬
        dialogues.Sort((a, b) => a.id.CompareTo(b.id));

        return dialogues;
    }
}
