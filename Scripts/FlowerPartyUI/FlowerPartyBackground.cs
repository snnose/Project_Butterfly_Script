using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlowerPartyBackground : MonoBehaviour
{
    [SerializeField] private int presetNumber;
    private float DOScaleOffset = -0.05f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }

    public void CompleteTween()
    {
        if (DOTween.IsTweening(rectTransform))
        {
            rectTransform.DOComplete();
        }
    }

    public void DOMoveBackground(Vector3 distance, float duration)
    {
        Vector3 endPosition = rectTransform.anchoredPosition3D + distance;
        rectTransform.DOAnchorPos3D(endPosition, duration);
    }

    public void DOScaleBackground(int selectedNumber, float duration)
    {
        int distance = presetNumber - selectedNumber;
        if (distance < 0)
            distance *= -1;

        rectTransform.DOScale(1 + distance * DOScaleOffset, duration);
    }

    public void SetPosition(int selectedNumber, float backgroundYSpacing)
    {
        rectTransform.anchoredPosition = new Vector3(0, (presetNumber - selectedNumber) * backgroundYSpacing, 0);
    }

    public void SetPresetNumber(int num)
    {
        presetNumber = num;
    }
}
