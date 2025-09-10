using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Board : IDataFromDto<BoardDTO>
{
    [SerializeField] private int _id;
    [SerializeField] private int _row;
    [SerializeField] private int _column;
    [SerializeField] private int[] _boardShape;

    public Board() {}

    public void FromDto(BoardDTO dto)
    {
        _id = dto.id;
        _row = dto.row;
        _column = dto.column;
        _boardShape = dto.boardShape;
    }

    public int id { get { return _id; } set { _id = value; } } // 해당 Board의 id
    public int row { get { return _row; } set { _row = value; } } // 해당 Board의 Row
    public int column { get { return _column; } set { _column = value; } } // 해당 Board의 Column
    public int[] boardShape { get { return _boardShape; } set { _boardShape = value; } } // 해당 board의 모양
}

[System.Serializable]
public class BoardDTO : IDto
{
    public int id;
    public int row;
    public int column;
    public int[] boardShape;
}
