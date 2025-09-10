using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Constant : IDataFromDto<ConstantDTO>
{
    [SerializeField] private int _prologueProgress;
    [SerializeField] private int _epilogueProgress;

    public Constant() {  }
    public void FromDto(ConstantDTO dto)
    {
        _prologueProgress = dto.prologueprogress;
        _epilogueProgress = dto.epilogueprogress;
    }

    public int prologueProgress{ get { return _prologueProgress; } set { _prologueProgress = value; } } // Constant : 프롤로그에 해당하는 Progress 숫자 지정
    public int epilogueProgress{ get { return _epilogueProgress; } set { _epilogueProgress = value; } } // Constant : 에필로그에 해당하는 Progress 숫자 지정
}

[System.Serializable]
public class ConstantDTO : IDto
{
    public int prologueprogress;
    public int epilogueprogress;
}