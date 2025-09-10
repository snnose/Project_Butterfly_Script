using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Reward
{
    [SerializeField] private string _type;
    [SerializeField] private int _value;
    [SerializeField] private int _amount;
    [SerializeField] private int _chance;

    public Reward() { }
    public Reward(RewardDTO dto)
    {
        _type = dto.type;
        _value = dto.value;
        _amount = dto.amount;
        _chance = dto.chance;
    }

    public string type { get { return _type; } set { _type = value; } }
    public int value { get { return _value; } set { _value = value; } }
    public int amount { get { return _amount; } set { _amount = value; } }
    public int chance { get { return _chance; } set { _chance = value; } }
}

[System.Serializable]
public class RewardDTO
{
    public string type;
    public int value;
    public int amount;
    public int chance;
}