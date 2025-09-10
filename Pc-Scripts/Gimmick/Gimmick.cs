using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gimmick : IDataFromDto<GimmickDTO>
{
    [SerializeField] private int _id;
    [SerializeField] private GimmickType _type;
    [SerializeField] private int _level;
    [SerializeField] private bool _isOccupied;
    [SerializeField] private int _activeTurn;
    [SerializeField] private int _durationTurn;
    [SerializeField] private int _amount;
    [SerializeField] private int _drop;
    [SerializeField] private bool _isRandomPosition;
    [SerializeField] private int[] _positionList;
    [SerializeField] private LinePlacement _linePlacement;
    [SerializeField] private LinePlacementDirection _linePlacementDirection;
    [SerializeField] private bool _isInteractable;

    public Gimmick() { }

    public void FromDto(GimmickDTO dto)
    {
        _id = dto.id;
        _type = (GimmickType)System.Enum.Parse(typeof(GimmickType), dto.type);
        _level = dto.level;
        _isOccupied = dto.isOccupied;
        _activeTurn = dto.ActiveTurn;
        _durationTurn = dto.DurationTurn;
        _amount = dto.amount;
        _drop = dto.drop;
        _isRandomPosition = dto.isRandomPosition;
        _positionList = dto.position;
        _linePlacement = (LinePlacement)System.Enum.Parse(typeof(LinePlacement), dto.linePlacement);
        _linePlacementDirection = (LinePlacementDirection)System.Enum.Parse(typeof(LinePlacementDirection), dto.linePlacementDirection);
        _isInteractable = dto.isInteractable;
    }

    /// <summary>
    /// 개별 Gimmick의 id
    /// </summary>
    public int id { get { return _id; } set { _id = value; } }
    /// <summary>
    /// 개별 Gimmick의 type
    /// </summary>
    public GimmickType type { get { return _type; } set { _type = value; } }
    /// <summary>
    /// 해당 기믹의 레벨. 개별 카운터 용으로 사용
    /// </summary>
    public int level { get { return _level; } set { _level = value; } }
    /// <summary>
    /// TRUE : 라인 지워질 때 영향 있음, FALSE : 라인 지워질 때 영향 없음
    /// </summary>
    public bool isOccupied { get { return _isOccupied; } set { _isOccupied = value; } }
    /// <summary>
    /// 해당 기믹이 동작하는 턴 수. 매 N턴마다 동작
    /// </summary>
    public int activeTurn { get { return _activeTurn; } set { _activeTurn = value; } }
    /// <summary>
    /// 해당 기믹이 동작 후, 유지되는 턴 수
    /// </summary>
    public int durationTurn { get { return _durationTurn; } set { _durationTurn = value; } }
    /// 해당 기믹 동작 시, 생성되는 수량
    /// </summary>
    public int amount { get { return _amount; } set { _amount = value; } }
    /// <summary>
    /// 해당 기믹이 생성할 drop의 id (gimmick drop)
    /// </summary>
    public int drop { get { return _drop; } set { _drop = value; } }
    /// <summary>
    /// TRUE : 무작위 위치, FALSE : 지정된 위치
    /// </summary>
    public bool isRandomPosition { get { return _isRandomPosition; } set { _isRandomPosition = value; } }
    /// <summary>
    /// isRandomPosition = TRUE인 경우 : 사용하지 않음. 0으로 표시, FALSE : 해당 좌표에 지정된 위치.
    /// 좌표는 좌 상단부터 순서대로 0, 1, 2, ... 로 지정
    /// </summary>
    public int[] positionList { get { return _positionList; } set { _positionList = value; } }
    /// <summary>
    /// 기믹의 배치 위치
    /// </summary>
    public LinePlacement linePlacement { get { return _linePlacement; } set { _linePlacement = value; } }
    /// <summary>
    /// 기믹의 배치 방향
    /// </summary>
    public LinePlacementDirection linePlacementDirection { get { return _linePlacementDirection; } set { _linePlacementDirection = value; } }
    /// <summary>
    /// TRUE : 터치 가능한 반응형 기믹, FALSE : 터치 불가능한 기믹
    /// </summary>
    public bool isInteractable { get { return _isInteractable; } set { _isInteractable = value; } }
}

[System.Serializable]
public class GimmickDTO : IDto
{
    public int id;
    public string type;
    public int level;
    public bool isOccupied;
    public int ActiveTurn;
    public int DurationTurn;
    public int amount;
    public int drop;
    public bool isRandomPosition;
    public int[] position;
    public string linePlacement;
    public string linePlacementDirection;
    public bool isInteractable;
}