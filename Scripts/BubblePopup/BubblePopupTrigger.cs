using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BubblePopupTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] 
    private string bubbleTitleKey;
    [SerializeField] 
    private string bubbleDescriptionKey;

    public void OnPointerDown(PointerEventData eventData)
    {
        InformationBubble bubble = InformationBubble.Instance;

        if (bubble != null)
        {
            bubble.SetOptionNameText("ui", bubbleTitleKey);
            bubble.SetDescriptionText("ui", bubbleDescriptionKey);
            bubble.SetActive(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InformationBubble.Instance.SetActive(false);
    }

    public void SetTitleKey(string titleKey)
    {
        bubbleTitleKey = titleKey;
    }

    public void SetDescriptionKey(string descriptionKey)
    {
        bubbleDescriptionKey = descriptionKey;
    }
}
