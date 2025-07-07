using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class FlowerPartySwipe : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private FlowerPartyUIManager flowerPartyUIManager;
    [SerializeField] private GameObject partyBackgroundPrefab;       // 파티 UI 배경 프리팹
    [SerializeField] private ScrollRect scrollRect;
    // partyBackgroundContent : 파티 UI 배경들이 들어갈 부모 컨테이너
    // ContentSizeFitter, HorizontalLayoutGroup => partyBackgroundContent에 붙은 컴포넌트
    [SerializeField] private RectTransform partyBackgroundContent;
    [SerializeField] private ContentSizeFitter contentSizeFitter;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;

    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] List<FlowerPartyBackground> flowerPartyBackgrounds;

    [SerializeField] private float contentSpace = 320f;
    private float distance = 0f;
    [SerializeField] private float stopVelocityThreshold;
    [SerializeField] private float tweenDuration;
    [SerializeField] private bool isWaitingScrollStopped = false;
    [SerializeField] private bool wasMoving = false;
    [SerializeField] private bool isDragging = false;
    private IEnumerator waitUntilScrollStopped;

    [Header("Party Preset")]
    public int maxPresetNumber;
    [SerializeField] private int currentPresetNumber;

    public void InitializePartySwipe(int selectedNumber)
    {
        int count = maxPresetNumber;

        for (int i = 0; i < count; i++)
        {
            GameObject pbg = Instantiate(partyBackgroundPrefab, partyBackgroundContent);
            pbg.TryGetComponent(out FlowerPartyBackground flowerPartyBackground);
            flowerPartyBackground.SetPresetNumber(i + 1);
            flowerPartyBackgrounds.Add(flowerPartyBackground);
        }

        distance = 1f / maxPresetNumber;

        currentPresetNumber = selectedNumber;

        SetSelectedBackground(currentPresetNumber);
        DOScaleBackgrounds(currentPresetNumber, tweenDuration);
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        isDragging = true;
        isWaitingScrollStopped = false;

        if (waitUntilScrollStopped != null)
        {
            StopCoroutine(waitUntilScrollStopped);
            waitUntilScrollStopped = null;
            partyBackgroundContent.DOKill(true);
        }

        contentSizeFitter.enabled = false;
        verticalLayoutGroup.enabled = false;
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        isDragging = false;
        isWaitingScrollStopped = true;
    }

    private void LateUpdate()
    {
        bool isMovingNow = scrollRect.velocity.sqrMagnitude > 0.01f;

        // 스크롤이 끝나기 기다리는 중이고
        if (isWaitingScrollStopped)
        {
            // 이전에 움직였는데, 지금 완전히 멈췄다면 잠금 해제
            if (wasMoving && !isMovingNow && !isDragging)
            {
                waitUntilScrollStopped = WaitUntilScrollStopped(tweenDuration);

                // 드래그가 끝나면 결정된 currentPresetNumber가 중앙으로 고정되도록 한다
                StartCoroutine(waitUntilScrollStopped);
                isWaitingScrollStopped = false;
            }
        }

        wasMoving = isMovingNow;
    }


    public void OnSliderValueChange()
    {
        float value = Mathf.Clamp(scrollbar.value, 0, 1);

        for (int i = 0; i < maxPresetNumber; i++)
        {
            // distance * index <= value < distance * (index + 1)
            if (distance * i <= value && value < distance * (i + 1))
            {
                currentPresetNumber = maxPresetNumber - i;
                flowerPartyUIManager.SetCurrentPresetNumber(currentPresetNumber);
                flowerPartyUIManager.SetFlowerParty(currentPresetNumber);
            }
        }

        DOScaleBackgrounds(currentPresetNumber, tweenDuration);
    }

    private IEnumerator WaitUntilScrollStopped(float duration)
    {
        //yield return new WaitForSeconds(0.1f);

        /*
        while (scrollRect.velocity.sqrMagnitude > stopVelocityThreshold)
        {
            yield return null;
        }
        */

        SetSelectedBackground(currentPresetNumber, duration);

        waitUntilScrollStopped = null;

        yield break;
    }

    private void SetSelectedBackground(int presetNumber, float duration = 0f)
    {
        partyBackgroundContent.DOAnchorPos(new Vector2(0, contentSpace * (presetNumber - 1)), duration)
            .OnComplete(() =>
            {
                contentSizeFitter.enabled = true;
                verticalLayoutGroup.enabled = true;
            });
    }

    private void DOScaleBackgrounds(int selectedNumber, float duration)
    {
        int count = flowerPartyBackgrounds.Count;
        for (int i = 0; i < count; i++)
        {
            flowerPartyBackgrounds[i].DOScaleBackground(selectedNumber, duration);
        }
    }
}
