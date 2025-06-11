using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using EffectSystem;
// Utility Tools
using Utils;

public class EffectPool : MonoBehaviour
{
    private string onCreateEffectKey;   // OnCreateEffect에서 생성할 이펙트의 키
    private GameObject basket;

    private Dictionary<string, IObjectPool<EffectBehaviour>> effectPoolDictionary = new Dictionary<string, IObjectPool<EffectBehaviour>>();
    private Dictionary<string, int> effectActiveCountDictionary = new Dictionary<string, int>();
    private Dictionary<string, int> effectMaxSizeDictionary = new Dictionary<string, int>();
    private Dictionary<string, GameObject> effectObjectDictionary = new Dictionary<string, GameObject>();

    /* /////////////////////////////////
     * // 24.12.04) 손봐야할 것들 정리 //
     * /////////////////////////////////
     * 
     * 1. 현재 모든 프리팹에 EffectBehaviour를 붙이는데 매우 비효율적
     * 
     * 2. 루프 이펙트는 외부에서 꺼줘야하는데, 현재 방법으론 루프 이펙트를 따로 기억해둔 다음 필요할 때 접근해 Release() 해줘야함
     * => 비효율적. 루프 이펙트를 사용할 때 마다 리스트를 작성해서 따로 처리해줘야함
     * 
     * ///////////////
     * // 해결 방법 //
     * //////////////
     * 
     * 1-1) AddComponent를 사용하는 방법
     * => 하지만 AddComponent는 비용이 굉장히 큼 (GetComponent의 1000배)
     * 
     * 1-2) 단일 프리팹을 사용하면서 ParticleSystem만 변경해주는 방법
     * => 풀에 넣어줄 이펙트 프리팹 하나만 사용 
     * => 초기 Init 단계에서 각 이펙트의 ParticleSystem을 위에 언급한 프리팹에 적용하고 풀에 넣어주면?
     * 
     * 1-3) EffectBehaviour만 갖는 프리팹의 자식으로 ParticleSystem을 배치
     * 
     * 2-1) ObjectPool을 쓰는게 아니라 List를 써서 직접 구현하는 방법
     * 
     */

    public void InitializeEffectPool(string objectType)
    {
        initializeEffectPool(objectType);
    }

    public void ResetEffectPool(string objectType)
    {
        resetEffectPool(objectType);
    }

    private void initializeEffectPool(string objectType)
    {
        GameObject[] effectPoolArray;

        // 경로 설정
        string path = Utility.BuildPath("Effects/", objectType);
        effectPoolArray = Resources.LoadAll<GameObject>(path);
        Utility.ClearStringBuilder();
        
        // 해당 유형의 각 이펙트 오브젝트 풀을 생성한다
        foreach(GameObject effect in effectPoolArray)
        {                              
            string effectKey;
            int effectPoolingCount = 1;
            
            if (effect.TryGetComponent(out EffectBehaviour effectBehaviour))
            {
                effectKey = effectBehaviour.effectInformation.key;
                effectPoolingCount = effectBehaviour.effectInformation.poolingCount;
                onCreateEffectKey = effectKey;
            }
            else
            {
                Debug.Log(effect.name + " 이펙트에 EffectBehaivour 스크립트가 부착되어있지 않습니다!");
                continue;
            }

            IObjectPool<EffectBehaviour> effectPool = new ObjectPool<EffectBehaviour>(
                                                                            OnCreateEffect,
                                                                            OnGetEffect,
                                                                            OnReleaseEffect,
                                                                            OnDestroyEffect,
                                                                            maxSize: effectPoolingCount);

            // 24.12.01) 현재 기준 effectBehaivour의 effectInfo.key를 키로 사용한다
            //           => effectBehaviour에 반드시 key값을 입력해줘야한다
            if (!effectPoolDictionary.ContainsKey(effectKey))
            {
                effectPoolDictionary.Add(effectKey, effectPool);
                effectActiveCountDictionary.Add(effectKey, effectPoolingCount);
                effectMaxSizeDictionary.Add(effectKey, effectPoolingCount);
                effectObjectDictionary.Add(effectKey, effect);
            }

            // 이펙트 바구니 생성 및 EffectManager에 등록
            basket = new GameObject();
            basket.name = effectKey;
            EffectManager.Instance.AddBasket(effectKey, basket);

            // EffectBehaviour에서 지정한 개수 만큼 오브젝트 생성
            for (int i = 0; i < effectPoolingCount; i++)
            { 
                if (effectPoolDictionary[effectKey].CountInactive < effectPoolingCount)
                {
                    PoolingEffect(effectKey, basket.transform);
                }
            }
        }

        return;
    }

    private void resetEffectPool(string objectType)
    {
        GameObject[] effectPoolArray;

        // 경로 설정
        string path = Utility.BuildPath("Effects/", objectType);
        effectPoolArray = Resources.LoadAll<GameObject>(path);
        Utility.ClearStringBuilder();

        foreach(GameObject effect in effectPoolArray)
        {
            string effectKey;

            if (effect.TryGetComponent<EffectBehaviour>(out EffectBehaviour effectBehaviour))
            {
                effectKey = effectBehaviour.effectInformation.key;
            }
            else
            {
                Debug.Log(effect.name + " 이펙트에 EffectBehaivour 스크립트가 부착되어있지 않습니다!");
                continue;
            }

            effectPoolDictionary[effectKey].Clear();
            EffectManager.Instance.RemoveBasket(effectKey);
        }
    }

    private void PoolingEffect(string effectKey, Transform basket)
    {
        GameObject effect = Instantiate(effectObjectDictionary[effectKey], basket);
        EffectBehaviour effectBehaviour;

        if (effect == null)
        {
            Utils.Utility.DebugLogError($"{effectKey}에 해당하는 이펙트가 없습니다!");
            return;
        }

        if (effect.TryGetComponent(out effectBehaviour))
        {
            effectBehaviour.pool = effectPoolDictionary[effectKey];
            effectBehaviour.Release();
        }
    }

    private EffectBehaviour OnCreateEffect()
    {
        if (effectActiveCountDictionary[onCreateEffectKey] + effectPoolDictionary[onCreateEffectKey].CountInactive >= effectMaxSizeDictionary[onCreateEffectKey])
        {
            return null;
        }

        GameObject effect = Instantiate(effectObjectDictionary[onCreateEffectKey], EffectManager.Instance.GetBasket(onCreateEffectKey).transform);
        effect.SetActive(false);
        EffectBehaviour effectBehaviour;

        if (effect.TryGetComponent(out effectBehaviour))
        {
            effectBehaviour.pool = effectPoolDictionary[onCreateEffectKey];
        }
        else
        {
            Debug.LogError($"이펙트에 EffectBehaviour가 존재하지 않습니다!: {effect.name}");
            return null;
        }

        return effectBehaviour;
    }

    private void OnGetEffect(EffectBehaviour effectBehaviour)
    {
        if (effectBehaviour == null)
        {
            return;
        }

        string effectKey = effectBehaviour.effectInformation.key;

        if (effectActiveCountDictionary[effectKey] >= effectMaxSizeDictionary[effectKey])
        {
            Debug.Log("풀에서 더 이상 가져올 수 없습니다!");
            return;
        }

        effectActiveCountDictionary[effectKey] += 1;

        effectBehaviour.gameObject.SetActive(true);
        effectBehaviour.Get();

        //Debug.Log(effectKey + ": effectActiveCountDictionary[effectKey] = " + effectActiveCountDictionary[effectKey]);
        //Debug.Log(effectKey + ": effectPoolDictionary[effectKey].CountInactive = " + effectPoolDictionary[effectKey].CountInactive);
    }

    private void OnReleaseEffect(EffectBehaviour effectBehaviour)
    {
        string effectKey = effectBehaviour.effectInformation.key;

        effectBehaviour.gameObject.SetActive(false);
        effectActiveCountDictionary[effectKey] -= 1;
    }

    private void OnDestroyEffect(EffectBehaviour effect)
    {
        Destroy(effect.gameObject);
    }

    public GameObject RequestEffectObject(string objectKey)
    {
        onCreateEffectKey = objectKey;

        if (!effectPoolDictionary.ContainsKey(objectKey))
        {
            Debug.Log($"{objectKey} 이펙트가 풀에 존재하지 않습니다!");
            return null;
        }

        EffectBehaviour eb = effectPoolDictionary[objectKey].Get();
        GameObject effect = null;

        //Debug.Log("RequestEffectObject / " + objectKey + ": effectActiveCountDictionary[effectKey] = " + effectActiveCountDictionary[objectKey]);
        //Debug.Log("RequestEffectObject / " + objectKey + ": effectPoolDictionary[effectKey].CountInactive = " + effectPoolDictionary[objectKey].CountInactive);
        if (eb != null)
        {
            effect = eb.gameObject;
        }

        return effect;
    }
}
