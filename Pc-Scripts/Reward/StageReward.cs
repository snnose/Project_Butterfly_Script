using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageReward : IDataFromDto<StageRewardDTO>
{
    [SerializeField] private int _id;
    [SerializeField] private int _stageid;
    [SerializeField] private Reward[] _firstClearRewards;
    [SerializeField] private Reward[] _repeatClearRewards;

    public StageReward() { }
    public void FromDto(StageRewardDTO dto)
    {
        _id = dto.id;
        _stageid = dto.stageid;

        int len = dto.FirstClearRewards.Length;
        _firstClearRewards = new Reward[len];
        for (int i = 0; i < len; i++)
        {
            _firstClearRewards[i] = new Reward(dto.FirstClearRewards[i]);
        }

        len = dto.RepeatClearRewards.Length;
        _repeatClearRewards = new Reward[len];
        for (int i = 0; i < len; i++)
        {
            _repeatClearRewards[i] = new Reward(dto.RepeatClearRewards[i]);
        }
    }

    public int id { get { return _id; } set { _id = value; } }
    public int stageid { get { return _stageid; } set { _stageid = value; } }
    public Reward[] firstClearRewards { get { return _firstClearRewards; } set { _firstClearRewards = value; } }
    public Reward[] repeatClearRewards { get { return _repeatClearRewards; } set { _repeatClearRewards = value; } }
}

[System.Serializable]
public class StageRewardDTO : IDto
{
    public int id;
    public int stageid;
    public RewardDTO[] FirstClearRewards;
    public RewardDTO[] RepeatClearRewards;
}