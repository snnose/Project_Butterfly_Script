using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CollectionCharacterCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Button cardButton;
    [SerializeField] private int cardNumber;
    [SerializeField] private bool isSelected = false;
    [SerializeField] private bool isFront = true;

    [Header("Background")]
    [SerializeField] private RectTransform backgroundRectTransform;
    [Header("Front")]
    [SerializeField] private RectTransform frontRectTransform;
    [Header("Back")]
    [SerializeField] private RectTransform backRectTransform;

    [Header("Tween Option")]
    [SerializeField] private float originYPos;
    [SerializeField] private float moveYPos;
    [SerializeField] private Vector3 rotationValue;
    [SerializeField] private float rotationDuration;

    public void SetCardNumber(int num)
    {
        cardNumber = num;
    }

    public void SetIsSelected(bool isSelected)
    {
        this.isSelected = isSelected;
    }

    public void SetAsLastSibling()
    {
        if (isSelected)
            rectTransform.SetAsLastSibling();
    }

    public void DOMoveCard(float duration)
    {
        if (isSelected)
            rectTransform.DOAnchorPos(new Vector2(rectTransform.anchoredPosition.x, moveYPos), duration);
        else
            rectTransform.DOAnchorPos(new Vector2(rectTransform.anchoredPosition.x, originYPos), duration);     
    }

    /// <summary>
    /// 배경이 Y축 기준 90도 회전 후, 다시 원 상태로 돌아옵니다.
    /// </summary>
    public void RotateBackground()
    {
        if (!isSelected)
            return;

        backgroundRectTransform.DORotate(rotationValue, rotationDuration)
            .OnComplete(() =>
            {
                backgroundRectTransform.DORotate(Vector3.zero, rotationDuration);
            });
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        scrollRect.OnBeginDrag(pointerEventData);

        cardButton.enabled = false;
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        scrollRect.OnDrag(pointerEventData);
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        scrollRect.OnEndDrag(pointerEventData);

        cardButton.enabled = true;
    }

    public void OnClickCharacterCard()
    {
        if (isFront)
        {
            OnClickFront();
            isFront = false;
        }
        else
        {
            OnClickBack();
            isFront = true;
        }
    }

    public void OnClickFront()
    {
        if (!isSelected)
            return;

        cardButton.enabled = false;

        frontRectTransform.DORotate(rotationValue, rotationDuration)
            .OnComplete(() => 
            {
                frontRectTransform.gameObject.SetActive(false);

                backRectTransform.gameObject.SetActive(true);
                backRectTransform.DORotate(Vector3.zero, rotationDuration);

                cardButton.enabled = true;
            });
    }

    public void OnClickBack()
    {
        if (!isSelected)
            return;

        cardButton.enabled = false;

        backRectTransform.DORotate(rotationValue, rotationDuration)
            .OnComplete(() =>
            {
                backRectTransform.gameObject.SetActive(false);

                frontRectTransform.gameObject.SetActive(true);
                frontRectTransform.DORotate(Vector3.zero, rotationDuration);

                cardButton.enabled = true;
            });
    }

    public void SetScrollRect(ScrollRect scrollRect)
    {
        this.scrollRect = scrollRect;
    }
}