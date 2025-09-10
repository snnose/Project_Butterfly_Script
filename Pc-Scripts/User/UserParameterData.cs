using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;

public static class UserParameterData
{
    private static UserParameter userParameter { get; set; }

    public static void SetUserParameterData(List<UserParameter> userParameterList)
    {
        // UserParameter는 리스트 내 단 하나만 존재한다.
        userParameter = userParameterList[0];
    }

    public static List<UserParameter> UpdateUserParameterData()
    {
        List<UserParameter> userParameterList = new List<UserParameter>();
        userParameterList.Add(userParameter);

        return userParameterList;
    }

    public static int GetUserExperience()
    {
        return userParameter.experience;
    }

    public static int GetUserItemExperience()
    {
        return userParameter.itemexperience;
    }

    public static int GetUserMainProgress()
    {
        return userParameter.mainprogress;
    }

    public static int GetUserSubProgress()
    {
        return userParameter.subprogress;
    }

    public static (int, int) GetUserProgress()
    {
        (int, int) progress = (userParameter.mainprogress, userParameter.subprogress);

        return progress;
    }

    public static void UpdateUserExperience(int experience)
    {
        userParameter.experience = experience;
    }

    public static void UpdateUserItemExperience(int itemExperience)
    {
        userParameter.itemexperience = itemExperience;
    }

    public static void UpdateUserMainProgress(int mainProgress)
    {
        userParameter.mainprogress = mainProgress;
    }

    public static void UpdateUserSubProgress(int subProgress)
    {
        userParameter.subprogress = subProgress;
    }

    public static void UpdateUserProgress(int mainProgress, int subProgress)
    {
        userParameter.mainprogress = mainProgress;
        userParameter.subprogress = subProgress;
    }

    /// <summary>
    /// 매개 변수값에 따라 각 progress를 1씩 증가시킵니다.
    /// </summary>
    /// <param name="isIncreaseMain">true일 시 메인 progress 증가</param>
    /// <param name="isIncreaseSub">true일 시 서브 progress 증가</param>
    public static void IncreaseUserProgressData(bool isIncreaseMain, bool isIncreaseSub = false)
    {
        (int main, int sub) = (userParameter.mainprogress, userParameter.subprogress);

        if (isIncreaseMain)
        {
            // 메인 progress가 상승하면 서브 progress는 0으로 초기화된다.
            main += 1; sub = 0;
        }

        if (isIncreaseSub)
        {
            sub += 1;
        }

        // 클라이언트 데이터 갱신
        userParameter.mainprogress = main;
        userParameter.subprogress = sub;
    }

    [Serializable]
    public class UserProgressData
    {
        public int mainprogress;
        public int subprogress;

        public (int main, int sub) ToTuple() => (mainprogress, subprogress); 
    }
}
