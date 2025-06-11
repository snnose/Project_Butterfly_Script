using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ShadowPopupPool : MonoBehaviour
{
    private Dictionary<string, IObjectPool<GameObject>> shadowPopupPoolDictionary = new Dictionary<string, IObjectPool<GameObject>>();
    private Dictionary<string, int> shadowPopupActiveCountDictionary = new Dictionary<string, int>();
    private Dictionary<string, GameObject> shadowPopupObjectDictionary = new Dictionary<string, GameObject>();

    public string shadowPopupKey = "Shadow_Popup";
    public int poolingCount;
    public int maxSize;

    private string stringKey;

    private void Awake()
    {
        ObjectPoolManager.Instance.shadowPopupPool = this;
        InitializeShadowPopupPool();
    }

    public void InitializeShadowPopupPool()
    {
        initializeShadowPopupPool();
    }

    public void ResetShadowPopupPool()
    {
        resetShadowPopupPool();
    }

    private void initializeShadowPopupPool()
    {
        GameObject shadowPopup;

        // 경로 설정
        string path = Utils.Utility.BuildPath("ui/Prefabs/ShadowPopup");
        shadowPopup = Resources.Load<GameObject>(path);

        IObjectPool<GameObject> shadowPopupPool = new ObjectPool<GameObject>(OnCreate,
                                                                             OnGet,
                                                                             OnRelease,
                                                                             OnDestroyPopup,
                                                                             maxSize: 10);

        shadowPopupPoolDictionary[shadowPopupKey] = shadowPopupPool;
        shadowPopupActiveCountDictionary[shadowPopupKey] = poolingCount;
        shadowPopupObjectDictionary[shadowPopupKey] = shadowPopup;

        for (int i = 0; i < poolingCount; i++)
        {
            if (shadowPopupPoolDictionary[shadowPopupKey].CountInactive < poolingCount)
            {
                shadowPopup = OnCreate();
                if (shadowPopup == null)
                    continue;
                shadowPopup.GetComponent<ShadowPopup>().Release();
            }
        }
        
        return;
    }

    private void resetShadowPopupPool()
    {

    }

    private GameObject OnCreate()
    {
        if (shadowPopupActiveCountDictionary[shadowPopupKey] + shadowPopupPoolDictionary[shadowPopupKey].CountInactive >= maxSize)
        {
            return null;
        }
        GameObject shadowPopup = Instantiate(shadowPopupObjectDictionary[shadowPopupKey], this.transform);
        shadowPopup.GetComponent<ShadowPopup>().pool = shadowPopupPoolDictionary[shadowPopupKey];

        return shadowPopup;
    }

    private void OnGet(GameObject shadowPopup)
    {
        if (shadowPopupActiveCountDictionary[shadowPopupKey] >= maxSize)
        {
            return;
        }

        shadowPopupActiveCountDictionary[shadowPopupKey] += 1;

        //Debug.Log("shadowPopupActiveCountDictionary[shadowPopupKey] = " + shadowPopupActiveCountDictionary[shadowPopupKey]);
        //Debug.Log("shadowPopupPoolDictionary[shadowPopupKey].CountInactive = " + shadowPopupPoolDictionary[shadowPopupKey].CountInactive);

        shadowPopup.GetComponent<ShadowPopup>().Get();
    }

    private void OnRelease(GameObject shadowPopup)
    {
        shadowPopup.SetActive(false);
        shadowPopupActiveCountDictionary[shadowPopupKey] -= 1;
    }

    private void OnDestroyPopup(GameObject shadowPopup)
    {
        Destroy(shadowPopup);
    }

    public GameObject RequestObject(string objectKey)
    {
        shadowPopupKey = objectKey;

        if (!shadowPopupPoolDictionary.ContainsKey(shadowPopupKey))
        {
            Utils.Utility.DebugLog("Shadow Popup이 풀에 존재하지 않습니다!");
            return null;
        }

        return shadowPopupPoolDictionary[shadowPopupKey].Get();
    }

    public GameObject RequestObject(string objectKey, string stringKey = null)
    {
        GameObject shadowPopup;

        shadowPopupKey = objectKey;

        if (!shadowPopupPoolDictionary.ContainsKey(shadowPopupKey))
        {
            Utils.Utility.DebugLog("Shadow Popup이 풀에 존재하지 않습니다!");
            return null;
        }

        shadowPopup = shadowPopupPoolDictionary[shadowPopupKey].Get();

        if (shadowPopup == null)
            return null;

        if (stringKey != null)
        {
            shadowPopup.GetComponent<ShadowPopup>().SetText(stringKey);
            //shadowPopup.GetComponent<ShadowPopup>().SetLocalizedText(stringKey);
        }

        return shadowPopup;
    }
}
