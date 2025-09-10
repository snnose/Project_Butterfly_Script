using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;


public static class BoardData
{
    private static Dictionary<int, Board> boardDictionary = new Dictionary<int, Board>();

    public static void SetBoardDictionary(List<Board> boardList)
    {
        foreach (Board board in boardList)
        {
            if (!boardDictionary.ContainsKey(board.id))
            {
                boardDictionary[board.id] = board;
            }
        }
    }

    public static void SetBoardData(JSONObject jsonObject)
    {
        Board board = new Board();

        board.id = jsonObject["id"].intValue;
        board.row = jsonObject["row"].intValue;
        board.column = jsonObject["column"].intValue;

        // JSON 객체에서 "boardShape" 키에 해당하는 배열 가져오기
        JSONObject boardShapeObject = jsonObject.GetField("boardShape");
        // JSONObject를 int[]로 변환
        int[] boardShapeArray = new int[boardShapeObject.count];
        for (int i = 0; i < boardShapeObject.count; i++)
        {
            boardShapeArray[i] = boardShapeObject[i].intValue;
        }
        // 변환된 int[]를 board.boardShape에 할당
        board.boardShape = boardShapeArray;

        if (!boardDictionary.ContainsKey(board.id))
        {
            boardDictionary.Add(board.id, board);
        }
    }
    // 해당 Board을 id를 기준으로 반환
    public static Board GetCell(int id)
    {
        if(boardDictionary.ContainsKey(id))
        {
            Board board = boardDictionary[id];
            return board;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // Board의 BoardShape를 id를 기준으로 반환
    public static int[] GetBoardShape(int id)
    {
        if(boardDictionary.ContainsKey(id))
        {
            int[] boardShape = boardDictionary[id].boardShape;
            return boardShape;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return null;
        }
    }
    // Board의 Row를 id를 기준으로 반환
    public static int GetBoardRow(int id)
    {
        if(boardDictionary.ContainsKey(id))
        {
            int boardRow = boardDictionary[id].row;
            return boardRow;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return 0;
        }
    }
    // Board의 Column을 id를 기준으로 반환
    public static int GetBoardColumn(int id)
    {
        if(boardDictionary.ContainsKey(id))
        {
            int boardColumn = boardDictionary[id].column;
            return boardColumn;
        }
        else
        {
            Debug.LogError("Id:"+ id +" Cannot Found!");
            return 0;
        }
    }
}
