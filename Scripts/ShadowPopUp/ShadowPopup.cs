using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Pool;
using DG.Tweening;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class ShadowPopup : MonoBehaviour
{
    public IObjectPool<GameObject> pool { get; set; }
    public RectTransform shadowPopupRectTransform;
    public CanvasGroup shadowPopupCanvasGroup;
    public Image popupBox;
    public TextMeshProUGUI popupText;

    [Header("UI Option")]
    public Vector2 boxSize;
    public float fontSize;
    
    [Header("Event Option")]
    public Vector3 spawnPosition;
    public Vector3 endPosition;
    public float delayTime;
    public float duration;
    public Ease ease;

    LocalizedString localizedString = new LocalizedString();

    private void Awake()
    {
        popupBox.rectTransform.sizeDelta = boxSize;
        popupText.fontSize = fontSize;
    }

    public void Get()
    {
        Initialize();
        this.gameObject.SetActive(true);

        ShadowPopupEvent();
    }

    public void Release()
    {
        if (this.gameObject.activeSelf)
            pool.Release(this.gameObject);
    }

    public void SetText(string text)
    {
        popupText.text = text;
    }

    public void SetLocalizedText(string stringKey)
    {
        localizedString.TableReference = "ui";
        localizedString.TableEntryReference = stringKey;

        popupText.text = localizedString.GetLocalizedString();
        localizedString.Clear();
    }

    // delayTime만큼 대기 후 endPosition으로 이동하면서 FadeOut
    private void ShadowPopupEvent()
    {
        shadowPopupRectTransform.DOAnchorPos3D(endPosition, duration)
            .SetDelay(delayTime)
            .SetEase(ease);
        shadowPopupCanvasGroup.DOFade(0f, duration)
            .SetDelay(delayTime)
            .OnComplete(Release);
    }

    private void Initialize()
    {
        this.transform.position = spawnPosition;
        shadowPopupCanvasGroup.alpha = 1f;
    }
}
