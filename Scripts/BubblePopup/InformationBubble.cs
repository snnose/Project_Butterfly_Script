using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Events;
using UnityEngine.Localization.Components;

public class InformationBubble : MonoBehaviour
{
    private static InformationBubble instance;
    public static InformationBubble Instance
    {
        get
        {
            if (null == instance)
                return null;

            return instance;
        }
    }

    public Image image;
    public TextMeshProUGUI optionName;
    public LocalizeStringEvent optionNameLocalize;

    public TextMeshProUGUI description;
    public LocalizeStringEvent descriptionLocalize;
    public Canvas canvas;   // UI 마우스 포인터 위치로 이동시키는데 필요 (좌표 변환)

    [Header("Box Size")]
    public Vector2 boxPadding;
    public Vector2 boxSize;
    public Vector2 maxBoxSize;

    [Header("Text Size")]
    public Vector2 textPadding;
    public Vector2 optionNameSize;
    public Vector2 descriptionSize;
    public Vector2 maxTextSize;

    [Header("Text Alignment")]
    public TextAlignmentOptions optionNameAlignment;
    public TextAlignmentOptions descriptionAlignment;

    [Header("Fixed Option")]
    public bool isFixed;
    public Vector2 fixedPosition; 

    private RectTransform rectTransform;
    private LocalizedString localizedString = new LocalizedString();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        rectTransform = this.GetComponent<RectTransform>();
        boxSize = image.GetComponent<RectTransform>().sizeDelta;

        if (optionName != null)
            optionName.alignment = optionNameAlignment;
        if (description != null)
            description.alignment = description.alignment;

        // Localization에 누락된 키를 어떻게 처리할지 설정
        LocalizationSettings.StringDatabase.MissingTranslationState =
            MissingTranslationBehavior.ShowMissingTranslationMessage;
        // 누락된 키는 일단 key 그대로 보여준다
        LocalizationSettings.StringDatabase.NoTranslationFoundMessage = "{key}";

        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        // 활성화 중일 때만 UI가 마우스 포인터를 따라간다
        if (this.gameObject.activeSelf)
        {
            // 고정형이 아닐 때만 마우스 포인터를 따라간다
            if (!isFixed)
                FollowMousePointer();
        }
    }

    private void FollowMousePointer()
    {
        Vector2 mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera, // Render Mode에 따라 설정
            out mousePosition
        );

        // 버블을 마우스 위치로 이동
        Vector2 bubblePosition = mousePosition + boxSize.y * Vector2.up;
        // 화면 밖으로 나가지 않도록 조정한다
        bubblePosition = AdjustBubblePosition(bubblePosition);
        rectTransform.anchoredPosition = bubblePosition;
    }

    private Vector2 AdjustBubblePosition(Vector2 bubblePosition)
    {
        // 기준 해상도 (1080 x 2340)
        float originWidth = 1080f; float originHeight = 2340f;

        Vector2 adjustPosition = bubblePosition;

        // Bubble이 화면을 넘지 않기 위한 position 값 범위
        // -540 + imageWidth / 2 < x < 540 - imageWidth / 2
        // -1170 + imageHeight / 2 < y < 1170 - imageHeight / 2

        // 화면 한계치
        float leftLimit = 0.5f * (-originWidth + boxSize.x);  // 왼쪽
        float rightLimit = 0.5f * (originWidth - boxSize.x);  // 오른쪽
        float upLimit = 0.5f * (originHeight - boxSize.y);   // 위
        float downLimit = 0.5f * (-originHeight + boxSize.y);  // 아래

        // position.x 조정
        if (leftLimit > bubblePosition.x)
            adjustPosition.x = leftLimit;
        if (rightLimit < bubblePosition.x)
            adjustPosition.x = rightLimit;

        // position.y 조정
        if (upLimit < bubblePosition.y)
            adjustPosition.y = upLimit;
        if (downLimit > bubblePosition.y)
            adjustPosition.y = downLimit;

        return adjustPosition;
    }

    private void ResizeBubble()
    {
        ResizeBox();
        ResizeText();
        RepositionText();
    }

    private void ResizeBox()
    {
        boxSize = GetPreferredBoxSize();
        SetBoxSize(boxSize);
    }

    private void ResizeText()
    {
        optionNameSize = optionName.GetPreferredValues(maxTextSize.x, maxTextSize.y) + textPadding;
        SetTextSize(optionName, optionNameSize);

        descriptionSize = description.GetPreferredValues(maxTextSize.x, maxTextSize.y) + textPadding;
        SetTextSize(description, descriptionSize);
    }

    private void RepositionText()
    {
        RepositionOptionNameText();
        RepositionDescriptionText();
    }

    private void RepositionOptionNameText()
    {
        float optionNamePositionX = 0f;
        // x 좌표 결정
        switch (optionNameAlignment)
        {
            case TextAlignmentOptions.Left:
                optionNamePositionX = 0.5f * (optionNameSize.x - boxSize.x) + boxPadding.x;
                break;
            case TextAlignmentOptions.Center:
                break;
            case TextAlignmentOptions.Right:
                optionNamePositionX = 0.5f * (-optionNameSize.x + boxSize.x) - boxPadding.x;
                break;
            default:
                break;
        }

        // y 좌표 결정
        float optionNamePositionY = 0.5f * (boxSize.y - optionNameSize.y) - boxPadding.y;

        optionName.rectTransform.anchoredPosition = new Vector2(optionNamePositionX, optionNamePositionY);
    }

    private void RepositionDescriptionText()
    {
        float descriptionPositionX = 0f;
        // x 좌표 결정
        switch (descriptionAlignment)
        {
            case TextAlignmentOptions.Left:
                break;
            case TextAlignmentOptions.Center:
                break;
            case TextAlignmentOptions.Right:
                break;
            default:
                break;
        }

        // y 좌표 결정
        float descriptionPositionY = optionName.transform.position.y - 0.5f * optionNameSize.y;

        description.rectTransform.anchoredPosition = new Vector2(descriptionPositionX, descriptionPositionY);
    }

    private Vector2 GetPreferredBoxSize()
    {
        Vector2 preferredBoxSize = Vector2.zero;
        float preferredX;
        float preferredY;

        if (optionName.GetPreferredValues().x >= description.GetPreferredValues().x)
        {
            preferredX = optionName.GetPreferredValues(maxTextSize.x, maxTextSize.y).x + 2f * boxPadding.x;
        }
        else
        {
            preferredX = description.GetPreferredValues(maxTextSize.x, maxTextSize.y).x + 2f * boxPadding.x;
        }

        if (preferredX > maxBoxSize.x)
            preferredX = maxBoxSize.x;

        preferredY = optionName.GetPreferredValues(maxTextSize.x, maxTextSize.y).y + 
                     description.GetPreferredValues(maxTextSize.x, maxTextSize.y).y + 
                     2f * boxPadding.y;

        preferredBoxSize = new Vector2(preferredX, preferredY);
        Utils.Utility.DebugLog("BubbleBoxSize : " + preferredBoxSize);

        return preferredBoxSize;
    }

    public void SetIsFixed(bool flag)
    {
        this.isFixed = flag;
    }
    
    public void SetActive(bool ret)
    {
        if (!isFixed)
        {
            // 활성화 후 마우스 포인터를 따라가면 원래 있던 위치 -> 마우스 포인터로 이동하는 게 짧은 시간 보인다
            // 위 현상을 방지하기 위해 활성화 직전에 마우스 포인터를 따라가도록 하였음
            FollowMousePointer();
        }
        if (optionName != null || description != null)
        {
            ResizeBubble();
        }

        this.gameObject.SetActive(ret);
    }

    public void SetBoxSize(Vector2 size)
    {
        this.image.GetComponent<RectTransform>().sizeDelta = size;
    }

    public void SetTextSize(TextMeshProUGUI tmpro, Vector2 size)
    {
        tmpro.GetComponent<RectTransform>().sizeDelta = size;
    }

    public void SetOptionNameTableReference(string tableReference)
    {
        optionNameLocalize.StringReference.TableReference = tableReference;
    }

    public void SetOptionNameStringReference(string tableEntryReference)
    {
        optionNameLocalize.StringReference.TableEntryReference = tableEntryReference;
    }

    public void SetOptionNameText(string tableReference, string tableEntryReference)
    {
        localizedString.TableReference = tableReference;
        localizedString.TableEntryReference = tableEntryReference;

        optionName.text = localizedString.GetLocalizedString();
        localizedString.Clear();
    }

    public void SetOptionNameText(string text)
    {
        optionName.text = text;
    }

    // optionId로 옵션 이름 텍스트와 설명 텍스트를 설정
    public void SetOptionNameText(int optionId)
    {
        // 현재 옵션 이름을 옵션 키로 출력하기 때문에 따로 이름으로 변경하면 좋을 것
        optionName.text = OptionData.GetOptionKey(optionId);

        // 옵션 설명은 switch 문을 사용해서 직접 작성한다거나?
        //description.text = SetDescriptionText(optionId);
    }

    public void SetDescriptionTableReference(string tableReference)
    {
        descriptionLocalize.StringReference.TableReference = tableReference;
    }

    public void SetDescriptionStringReference(string tableEntryReference)
    {
        descriptionLocalize.StringReference.TableEntryReference = tableEntryReference;
        description.ForceMeshUpdate();
    }

    public void SetDescriptionText(string tableReference, string tableEntryReference)
    {
        localizedString.TableReference = tableReference;
        localizedString.TableEntryReference = tableEntryReference;

        description.text = localizedString.GetLocalizedString();
        localizedString.Clear();
    }

    public void SetDescriptionText(string text)
    {
        description.text = text;
    }

    private void SetDescriptionText(int optionId)
    {
        switch (optionId)
        {
            default:
                Debug.Log(optionId + "에 해당하는 옵션이 존재하지 않음!");
                break;
        }

        return;
    }
}
