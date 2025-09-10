using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

[System.Serializable]
public class UserStage : IDataFromDto<UserStageDTO>
{
    [SerializeField] private int _id;
    [SerializeField] private bool _isCleared;
    [SerializeField] private int _highscore;
    [SerializeField] private int _highrank;

    public void FromDto(UserStageDTO dto)
    {
        _id = dto.id;
        _isCleared = dto.isCleared;
        _highscore = dto.highscore;
        _highrank = dto.highrank;
    }

    public int id { get { return _id; } set { _id = value; } } // User id
    public bool isCleared { get { return _isCleared; } set { _isCleared = value; } }
    public int highscore { get { return _highscore; } set { _highscore = value; } }
    public int highrank { get { return _highrank; } set { _highrank = value; } }
}

[System.Serializable]
public class UserStageDTO : IDto
{
    public int id;
    public bool isCleared;
    public int highscore;
    public int highrank;
}