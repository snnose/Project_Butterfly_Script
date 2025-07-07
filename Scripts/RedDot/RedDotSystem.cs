using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RedDot
{
    // 25.01.16) 일단 Collection으로 테스트 한 결과
    // 1. 결론적으로 우선 드롭을 새로 획득할 때 RedDot은 잘 출력 됨
    // 2. 문제는 획득 후 로비로 돌아왔을 때 컬렉션 버튼에 RedDot 출력이 바로 안됨
    // 3. 근데 컬렉션 버튼을 누르면 바로 RedDot이 출력됨
    // 4. 새 드롭을 얻기 전 컬렉션을 한 번이라도 진입하면 정상적으로 RedDot 출력됨

    public static class RedDotSystem
    {
        public static Action<string> OnRedDotChange;

        static Dictionary<string, int> eRedDotCounter = new Dictionary<string, int>();

        public static void Initialize(string eRedDotItem)
        {
            if (eRedDotCounter.ContainsKey(eRedDotItem))
            {
                eRedDotCounter.Remove(eRedDotItem);
            }

            OnRedDotChange?.Invoke(eRedDotItem);
        }

        public static void AddRedDot(string eRedDotItem, int count)
        {
            if (eRedDotCounter.ContainsKey(eRedDotItem))
            {
                eRedDotCounter[eRedDotItem] += count;
            }
            else
            {
                eRedDotCounter[eRedDotItem] = count;
            }

            // PlayerPrefs에 레드닷 정보 저장
            // FIXME : key값으로 dropId를 바로 집어 넣는 게 좋은 방법은 아닌 거 같다. 해결 방안 생각해야함
            PlayerPrefs.SetInt(eRedDotItem, count);

            OnRedDotChange?.Invoke(eRedDotItem);
        }

        public static void SetRedDot(string eRedDotItem, int count)
        {            
            eRedDotCounter[eRedDotItem] = count;
            
            // PlayerPrefs에 레드닷 정보 저장
            // FIXME : key값으로 dropId를 바로 집어 넣는 게 좋은 방법은 아닌 거 같다. 해결 방안 생각해야함
            PlayerPrefs.SetInt(eRedDotItem, count);

            OnRedDotChange?.Invoke(eRedDotItem);
        }

        public static void ClearRedDot(string eRedDotItem)
        {            
            eRedDotCounter[eRedDotItem] = 0;

            // PlayerPrefs에 저장된 레드닷 정보 초기화
            PlayerPrefs.SetInt(eRedDotItem, 0);

            OnRedDotChange?.Invoke(eRedDotItem);
        }

        public static int GetRedDotCount(string eRedDotItem)
        {
            if (eRedDotCounter.ContainsKey(eRedDotItem))
            {
                return eRedDotCounter[eRedDotItem];
            }

            return 0;
        }
    }
}