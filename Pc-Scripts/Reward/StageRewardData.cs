using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public static class StageRewardData
{
    /// <summary>
    /// Key-Value = stageId-StageReward
    /// </summary>
    private static Dictionary<int, StageReward> stageRewardDictionary = new Dictionary<int, StageReward>();

    public static void SetStageRewardDictionary(List<StageReward> stageRewardList)
    {
        foreach (StageReward stageReward in stageRewardList)
        {
            if (!stageRewardDictionary.ContainsKey(stageReward.stageid))
            {
                stageRewardDictionary[stageReward.stageid] = stageReward;
            }
        }
    }

    public static StageReward GetStageReward(int stageId)
    {
        return stageRewardDictionary[stageId];
    }
}
