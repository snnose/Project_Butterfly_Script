using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static ObjectPoolManager instance;
    public static ObjectPoolManager Instance
    {
        get
        {
            if (null == instance)
                return null;

            return instance;
        }
    }

    public EffectPool effectPool;
    public ShadowPopupPool shadowPopupPool { get; set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public void InitializeObjectPool(string objectPoolType, string objectType)
    {
        switch(objectPoolType)
        {
            case "Effect":
                effectPool.InitializeEffectPool(objectType);
                break;
            default:
                Debug.Log(objectPoolType + "에 해당하는 오브젝트풀이 존재하지 않습니다!");
                break;
        }
    }

    // 특정 오브젝트의 활성화를 요청하는 메서드
    // 요청한 오브젝트가 모두 활성화 상태라면 추가적으로 생성해 활성화한다
    public GameObject RequestObject(string objectPoolType, string objectKey)
    {
        switch(objectPoolType)
        {
            case "Effect":
                return effectPool.RequestEffectObject(objectKey);
            case "ShadowPopup":
                return shadowPopupPool.RequestObject(objectKey);
            default:
                Debug.Log(objectPoolType + "에 해당하는 오브젝트풀이 존재하지 않습니다!");
                break;
        }

        return null;
    }

    public GameObject RequestObject(string objectPoolType, string objectKey, string stringKey)
    {
        switch (objectPoolType)
        {
            case "Effect":
                return effectPool.RequestEffectObject(objectKey);
            case "ShadowPopup":
                return shadowPopupPool.RequestObject(objectKey, stringKey);
            default:
                Debug.Log(objectPoolType + "에 해당하는 오브젝트풀이 존재하지 않습니다!");
                break;
        }

        return null;
    }

    public void ResetObjectPool(string objectPoolType, string objectType)
    {
        switch (objectPoolType)
        {
            case "Effect":
                effectPool.ResetEffectPool(objectType);
                break;
            default:
                Debug.Log(objectPoolType + "에 해당하는 오브젝트풀이 존재하지 않습니다!");
                break;
        }
    }
}
