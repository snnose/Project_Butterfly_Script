using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/*
 * 스크립트 이름을 FlowerPartyToggleHandle로 작명했는데, 여기서 일반화해서 사용 가능할 듯?
 * 이 유튜브 댓글 창 비슷하게 작동하는 걸 뭐라고 해야할지 모르겠다....
 */

public class FlowerPartyToggleHandle : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform toggleHandle;
    [SerializeField] private RectTransform flowerItemSlots;
    [SerializeField] private RectTransform flowerItemViewPort;
    [SerializeField] private RectTransform itemContainer;

    [Header("ScrollRect Height Limit")]
    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;

    [Header("Scale Limit")]
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;

    [Header("Resizing UI")] // height 변경으로, 이에 영향받아 작아져야하는 UI 리스트
    [SerializeField] private List<RectTransform> resizingList;
    
    private float startHeight;
    private float pointerStartY;

    private void OnEnable()
    {
        flowerItemSlots.sizeDelta = new Vector2(flowerItemSlots.sizeDelta.x, minHeight);
        flowerItemViewPort.sizeDelta = new Vector2(flowerItemViewPort.sizeDelta.x, minHeight);

        ResizeUI(1f);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        // 드래그 시작 시 스크롤 잠금
        scrollRect.enabled = false;
        startHeight = flowerItemViewPort.sizeDelta.y;
        pointerStartY = pointerEventData.position.y;
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        float deltaY = pointerEventData.position.y - pointerStartY;
        float newHeight = Mathf.Clamp(startHeight + deltaY, minHeight, maxHeight);

        var size = flowerItemViewPort.sizeDelta;
        size.y = newHeight;

        flowerItemSlots.sizeDelta = size;
        flowerItemViewPort.sizeDelta = size;

        // Focus 슬롯 크기 조정
        float t = Mathf.InverseLerp(minHeight, maxHeight, newHeight);
        float scale = Mathf.Lerp(maxScale, minScale, t);

        ResizeUI(scale);
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        scrollRect.enabled = true;
    }

    /// <summary>
    /// resizingList에 속하는 UI의 Scale을 모두 scale로 변경합니다.
    /// </summary>
    /// <param name="scale"></param>
    private void ResizeUI(float scale)
    {
        foreach (RectTransform rectTransform in resizingList)
        {
            rectTransform.DOScale(scale, 0f);
        }
    }
}
