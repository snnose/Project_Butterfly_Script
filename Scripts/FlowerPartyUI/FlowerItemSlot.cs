using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlowerItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("FlowerItemParameter")]
    [SerializeField] private Canvas canvas; // 드래그할 UI가 속한 Canvas
    [SerializeField] private GameObject previewObject; // 미리 Hierarchy에 있는 프리뷰 오브젝트
    [SerializeField] private RectTransform previewRect; // 프리뷰 오브젝트의 RectTransform
    [SerializeField] private Image previewImage; // 프리뷰 오브젝트의 이미지

    [Header("FlowerItemData")]
    [SerializeField] private Image myItemImage; // 내 아이템 이미지
    [SerializeField] private int itemId; // 실제 데이터 전달용

    public int SetItemId(int id) => itemId = id;
    public int GetItemId() => itemId;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (previewObject == null || previewRect == null || previewImage == null || myItemImage == null)
        {
            Debug.LogWarning("프리뷰 관련 참조가 빠져있습니다!");
            return;
        }

        previewImage.sprite = myItemImage.sprite;
        previewImage.enabled = true;

        previewObject.SetActive(true);
        previewRect.position = transform.position;

        eventData.pointerDrag = gameObject; // drop에서 인식 가능하게!
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (previewRect != null)
        {
            previewRect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (previewObject != null)
        {
            previewObject.SetActive(false);
        }
    }
}
