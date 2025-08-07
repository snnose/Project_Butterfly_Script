using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using RedDot;

public class CollectionCharacterView : MonoBehaviour
{ 
    [Header("CharacterCollections")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform characterViewContainer;
    [SerializeField] private ContentSizeFitter contentSizeFitter;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private List<CollectionCharacterCard> collectionCharacterCardList;

    [SerializeField] private float contentSpace;
    private float distance;

    [SerializeField] private float stopVelocityThreshold;
    [SerializeField] private float tweenDuration;
    [SerializeField] private int currentCardIndex;

    private void OnEnable()
    {
        /*
        // 첫번째 카드 선택
        currentCardIndex = 0;
        collectionCharacterCardList[currentCardIndex].SetIsSelected(true);
        collectionCharacterCardList[currentCardIndex].DOMoveCard(tweenDuration);
        */
    }

    public void InitializeCharacterView()
    {
        foreach(CollectionCharacterCard card in collectionCharacterCardList)
        {
            card.SetScrollRect(scrollRect);
        }

        int count = collectionCharacterCardList.Count;

        scrollbar.numberOfSteps = count;
        distance = 1f / (count - 1);
    }

    public void OnSliderValueChange()
    {
        float value = Mathf.Clamp(scrollbar.value, 0, 1);
        int count = collectionCharacterCardList.Count;

        for (int i = 0; i < count; i++)
        {
            // distance * index <= value < distance * (index + 1)
            if (distance * i <= value && value < distance * (i + 1))
            {
                currentCardIndex = i;
                collectionCharacterCardList[i].SetIsSelected(true);
            }
            else
            {
                collectionCharacterCardList[i].SetIsSelected(false);
            }
        }

        DOMoveCharacterCards(tweenDuration);
    }

    private void DOMoveCharacterCards(float tweenDuration)
    {
        foreach (CollectionCharacterCard card in collectionCharacterCardList)
        {
            card.DOMoveCard(tweenDuration);
        }
    }

    public void OnClickPrevButton()
    {
        if (currentCardIndex <= 0)
        {
            return;
        }

        currentCardIndex--;
        scrollbar.value = distance * currentCardIndex;
    }

    public void OnClickNextButton()
    {
        if (currentCardIndex >= collectionCharacterCardList.Count - 1)
            return;

        currentCardIndex++;
        scrollbar.value = distance * currentCardIndex;
    }
}
