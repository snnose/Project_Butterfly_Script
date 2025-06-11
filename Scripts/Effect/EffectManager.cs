using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EffectSystem;

public class EffectManager : MonoBehaviour
{
    private static EffectManager instance;
    public static EffectManager Instance
    {
        get
        {
            if (null == instance)
                return null;

            return instance;
        }
    }

    public Dictionary<string, GameObject> effectBasket = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public void ActivateEffect(string objectType, string objectKey, Vector3 initPosition, bool uiFlag = false, Action onEffectCompleted = null, MaterialEffectOptions materialOptions = null)
    {
        if (!effectBasket[objectKey].activeSelf)
        {
            return;
        }

        EffectBehaviour effectBehaviour;
        GameObject effect = ObjectPoolManager.Instance.RequestObject("Effect", objectKey);
        if (effect == null)
        {
            return;
        }

        effect.TryGetComponent<EffectBehaviour>(out effectBehaviour);
        if (effectBehaviour == null)
        {
            return;
        }

        effectBehaviour.SetInitPosition(initPosition);
        effectBehaviour.SetUIFlag(uiFlag);

        if (materialOptions != null)
        {
            effectBehaviour.SetMaterialOptions(materialOptions);
        }
        else
        {
            effectBehaviour.SetMaterialOptionsNull();
        }

        effectBehaviour.ActivateEffect(() =>
        {
        // onEffectCompleted가 null이 아닐 경우 호출
        onEffectCompleted?.Invoke();
        });
    }

    public void ActivateEffect(string objectType, string objectKey, RectTransform initRectTransform, bool uiFlag = false, Action onEffectCompleted = null, MaterialEffectOptions materialOptions = null)
    {
        EffectBehaviour effectBehaviour;
        GameObject effect = ObjectPoolManager.Instance.RequestObject("Effect", objectKey);
        if (effect == null)
        {
            return;
        }

        effect.TryGetComponent<EffectBehaviour>(out effectBehaviour);
        if (effectBehaviour == null)
        {
            return;
        }

        effectBehaviour.SetInitPosition(ConverseCoordinates(initRectTransform));
        effectBehaviour.SetUIFlag(uiFlag);

        if (materialOptions != null)
        {
            effectBehaviour.SetMaterialOptions(materialOptions);
        }
        else
        {
            effectBehaviour.SetMaterialOptionsNull();
        }

        effectBehaviour.ActivateEffect(() =>
        {
        // onEffectCompleted가 null이 아닐 경우 호출
        onEffectCompleted?.Invoke();
        });
    }

    public void ActivateDOMoveEffect(string objectType, string objectKey, Vector3 initPosition, RectTransform endRectTransform, bool uiFlag = false, Action onEffectCompleted = null, MaterialEffectOptions materialOptions = null)
    {
        EffectBehaviour effectBehaviour;
        GameObject effect = ObjectPoolManager.Instance.RequestObject("Effect", objectKey);
        if (effect == null)
        {
            return;
        }

        effect.TryGetComponent<EffectBehaviour>(out effectBehaviour);
        if (effectBehaviour == null)
        {
            return;
        }

        effectBehaviour.SetDOMove(initPosition, ConverseCoordinates(endRectTransform));
        effectBehaviour.SetUIFlag(uiFlag);

        if (materialOptions != null)
        {
            effectBehaviour.SetMaterialOptions(materialOptions);
        }
        else
        {
            effectBehaviour.SetMaterialOptionsNull();
        }

        effectBehaviour.ActivateEffect(() =>
        {
        // onEffectCompleted가 null이 아닐 경우 호출
        onEffectCompleted?.Invoke();
        });
    }

    public void ActivateDOMoveEffect(string objectType, string objectKey, Vector3 initPosition, Vector3 endPosition, bool uiFlag = false, Action onEffectCompleted = null, MaterialEffectOptions materialOptions = null)
    {
        EffectBehaviour effectBehaviour;
        GameObject effect = ObjectPoolManager.Instance.RequestObject("Effect", objectKey);
        if (effect == null)
        {
            return;
        }

        effect.TryGetComponent<EffectBehaviour>(out effectBehaviour);
        if (effectBehaviour == null)
        {
            return;
        }

        effectBehaviour.SetDOMove(initPosition, endPosition);
        effectBehaviour.SetUIFlag(uiFlag);

        if (materialOptions != null)
        {
            effectBehaviour.SetMaterialOptions(materialOptions);
        }
        else
        {
            effectBehaviour.SetMaterialOptionsNull();
        }

        effectBehaviour.ActivateEffect(() =>
        {
        // onEffectCompleted가 null이 아닐 경우 호출
        onEffectCompleted?.Invoke();
        });
    }

    public void ActivateCustomPathEffect(string objectKey, Vector3 initPosition, RectTransform endRectTransform, bool uiFlag = false, Action onEffectCompleted = null, MaterialEffectOptions materialOptions = null)
    {
        EffectBehaviour effectBehaviour;
        GameObject effect = ObjectPoolManager.Instance.RequestObject("Effect", objectKey);
        if (effect == null)
        {
            return;
        }

        effect.TryGetComponent<EffectBehaviour>(out effectBehaviour);
        if (effectBehaviour == null)
        {
            return;
        }

        effectBehaviour.SetCustomDOPath(initPosition, ConverseCoordinates(endRectTransform));
        effectBehaviour.SetUIFlag(uiFlag);

        if (materialOptions != null)
        {
            effectBehaviour.SetMaterialOptions(materialOptions);
        }
        else
        {
            effectBehaviour.SetMaterialOptionsNull();
        }

        effectBehaviour.ActivateEffect(() =>
        {
            // onEffectCompleted가 null이 아닐 경우 호출
            onEffectCompleted?.Invoke();
        });
    }

    public void ActivateDOPathEffect(string objectKey, List<Vector3> path, bool uiFlag = false, Action onEffectCompleted = null, MaterialEffectOptions materialOptions = null)
    {
        EffectBehaviour effectBehaviour;
        GameObject effect = ObjectPoolManager.Instance.RequestObject("Effect", objectKey);
        if (effect == null)
        {
            return;
        }

        effect.TryGetComponent<EffectBehaviour>(out effectBehaviour);
        if (effectBehaviour == null)
        {
            return;
        }

        effectBehaviour.SetDOPath(path);
        effectBehaviour.SetUIFlag(uiFlag);

        if (materialOptions != null)
        {
            effectBehaviour.SetMaterialOptions(materialOptions);
        }
        else
        {
            effectBehaviour.SetMaterialOptionsNull();
        }

        effectBehaviour.ActivateEffect(() =>
        {
        // onEffectCompleted가 null이 아닐 경우 호출
        onEffectCompleted?.Invoke();
        });
    }
    public void ActivateSpawnedEffect(Drop drop)
    {
        switch (drop.key)
        {
            case "thorn_flower":
                ActivateEffect("InGame", "Thron_Flower_Spawned", drop.dropBehaviour.transform.position);
                break;
            case "purple_poison_flower":
                ActivateEffect("InGame", "Purple_Poison_Flower_Spawned", drop.dropBehaviour.transform.position);
                break;
            case "glue_flower":
                ActivateEffect("InGame", "Glue_Flower_Spawned", drop.dropBehaviour.transform.position);
                break;
            // Normal 드롭의 경우
            default:
                ActivateEffect("InGame", "Drop_Spawned", drop.dropBehaviour.transform.position);
                break;
        }
    }

    public EffectBehaviour ActivateDropLinkEffect(Vector3 spawnPosition)
    {
        EffectBehaviour dropLinkingEffect = ObjectPoolManager.Instance.RequestObject("Effect", "Drop_Linking").GetComponent<EffectBehaviour>();
        dropLinkingEffect.SetInitPosition(spawnPosition);

        dropLinkingEffect.ActivateEffect();

        return dropLinkingEffect;
    }

    public void ReleaseEffect(EffectBehaviour effectBehaviour)
    {
        if (effectBehaviour != null)
        {
            effectBehaviour.Release();
        }
    }
    public void AddBasket(string key, GameObject basket)
    {
        effectBasket[key] = basket;
    }

    public GameObject GetBasket(string key)
    {
        if (effectBasket.ContainsKey(key))
        {
            return effectBasket[key];
        }

        return null;
    }

    public void SetBasketActive(string key, bool isActive)
    {
        effectBasket[key].SetActive(isActive);
    }

    public void SetAllBaseketActive(bool isActive)
    {
        foreach (GameObject basket in effectBasket.Values)
        {
            basket.SetActive(isActive);
        }
    }

    public void RemoveBasket(string key)
    {
        Destroy(effectBasket[key]);
        effectBasket.Remove(key);
    }

    // RectTransform 좌표 -> WorldTransform 좌표로 변환
    private Vector3 ConverseCoordinates(RectTransform rectTransform)
    {
        Vector3 worldPosition = rectTransform.position;

        Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform,
                RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.position),
                Camera.main,
                out worldPosition
            );
        }

        return worldPosition;
    }
}