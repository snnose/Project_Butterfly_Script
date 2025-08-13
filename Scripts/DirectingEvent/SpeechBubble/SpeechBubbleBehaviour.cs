using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using DG.Tweening;

public class SpeechBubbleBehaviour : MonoBehaviour
{
    public Canvas canvas;
    public Image bubble;
    public List<Sprite> bubbleSpriteList;
    public TextMeshProUGUI bubbleText;
    public LocalizeStringEvent bubbleLocalizeStringEvent;
    public Button nextSpeechButton;

    public bool isClickNextButton = false;
    public bool isPrinting = false;

    List<DirectingEventSystem.TextOption> textOptions;
    private DirectingEventSystem.SpeechBubbleShake speechBubbleShake;
    private DirectingEventSystem.StringShake stringShake;
    private IEnumerator printOneByOne;
    private float printDelay = 0.25f;

    private void OnEnable()
    {
        bubble.DOFade(1, 0);
        bubbleText.DOFade(1, 0);
    }

    public IEnumerator FloatSpeechBubble(List<string> stringKeyList, DirectingEventSystem.StringPrintType stringPrintType,
                                         float remainingDuration, float printSpeed, bool isBubbleShake, bool isStringShake)
    {
        int count = stringKeyList.Count;

        for (int i = 0; i < count; i++)
        {
            string stringKey = stringKeyList[i];
            string text = GetLocalizedString(stringKey);
            ChangeTextOption(textOptions[i]);

            if (isBubbleShake)
            {
                BubbleShake();
            }

            if (isStringShake)
            {
                StringShake();
            }

            if (stringPrintType == DirectingEventSystem.StringPrintType.OneByOne)
            {
                if (printOneByOne != null)
                {
                    StopCoroutine(printOneByOne);
                    printOneByOne = null;
                }

                printOneByOne = PrintOneByOne(text, printSpeed);
                StartCoroutine(printOneByOne);
            }

            yield return new WaitUntil(() => isClickNextButton);
            isClickNextButton = false;
        }

        bubble.DOFade(0, remainingDuration);
        bubbleText.DOFade(0, remainingDuration);
        yield return new WaitForSeconds(remainingDuration);

        this.gameObject.SetActive(false);
    }

    private IEnumerator PrintOneByOne(string text, float printSpeed)
    {
        isPrinting = true;

        if (printSpeed <= 0)
        {
            printSpeed = 1;
        }
        WaitForSeconds printDelay = new WaitForSeconds(this.printDelay / printSpeed);

        bubbleText.maxVisibleCharacters = 0;
        SetText(text);
        SetPrefferedWidth(bubbleText.GetPreferredValues());

        yield return null;

        int count = text.Length;
        
        for (int i = 1; i <= count; i++)
        {
            bubbleText.maxVisibleCharacters = i;
            yield return printDelay;
        }
        isPrinting = false;

        yield break;
    }

    private void BubbleShake()
    {
        bubble.transform.DOShakePosition(speechBubbleShake.duration, speechBubbleShake.strength);
        StringShake();
    }

    private void StringShake()
    {
        bubbleText.transform.DOShakePosition(stringShake.duration, stringShake.strength);
    }

    private void SetPrefferedWidth(Vector2 scale)
    {
        bubble.rectTransform.sizeDelta = new Vector2(1.05f * scale.x, bubble.rectTransform.sizeDelta.y);
        bubbleText.rectTransform.sizeDelta = new Vector2(scale.x, bubbleText.rectTransform.sizeDelta.y);
    }

    private Vector2 ConverseCoordinates(Vector3 worldPosition)
    {
        Vector2 UIPosition;
        RectTransform canvasRect = canvas.transform as RectTransform;
        Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;

        Vector2 screenPosition = DirectingEventSystem.DirectingEventManager.Instance.stageCamera.WorldToScreenPoint(worldPosition);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, camera, out UIPosition))
        {
            
        }

        return UIPosition;
    }

    private string GetLocalizedString(string stringkey)
    {
        string text;
        LocalizedString localizedString = new LocalizedString("ui", stringkey);
        text = localizedString.GetLocalizedString();

        if (string.IsNullOrEmpty(text))
        {
            return stringkey;
        }

        return text;
    }

    public void NextSpeech()
    {
        if (isPrinting)
        {
            if (printOneByOne != null)
            {
                StopCoroutine(printOneByOne);
                printOneByOne = null;
            }

            bubbleText.maxVisibleCharacters = 222;
            isPrinting = false;
        }
        else
        {
            isClickNextButton = true;
        }
    }

    public void FollowSpeecher(Vector3 endPosition, float duration)
    {
        bubble.rectTransform.DOAnchorPos3D(endPosition, duration);
        bubbleText.rectTransform.DOAnchorPos3D(endPosition, duration);
    }

    public void SetBubblePosition(Vector3 position)
    {
        bubble.rectTransform.anchoredPosition = ConverseCoordinates(position);
        bubbleText.rectTransform.anchoredPosition = ConverseCoordinates(position);
    }

    public void SetRenderer(DirectingEventSystem.SpeechBubbleType speechBubbleType)
    {
        switch (speechBubbleType)
        {
            case DirectingEventSystem.SpeechBubbleType.Talk:
                break;
            case DirectingEventSystem.SpeechBubbleType.Think:
                break;
            case DirectingEventSystem.SpeechBubbleType.Shout:
                break;
            default:
                break;
        }
    }

    public void SetTextOptionList(List<DirectingEventSystem.TextOption> textOptionList)
    {
        textOptions = textOptionList;
    }

    public void ChangeTextOption(DirectingEventSystem.TextOption textOption)
    {
        bubbleText.rectTransform.DOScale(textOption.textScale, 0);
        bubbleText.font = textOption.fontAsset;
        bubbleText.fontStyle = textOption.fontStyle;
        bubbleText.color = textOption.textColor;
    }

    public void SetBubbleShake(DirectingEventSystem.SpeechBubbleShake speechBubbleShake)
    {
        this.speechBubbleShake = speechBubbleShake;
    }

    public void SetStringShake(DirectingEventSystem.StringShake stringShake)
    {
        this.stringShake = stringShake;
    }

    public void SetText(string text)
    {
        bubbleText.text = text;
    }

    public void SetStringTableKey()
    {
        //bubbleLocalizeStringEvent.SetTable()
    }

    public GameObject InstantiateBubble()
    {
        return Instantiate(this.gameObject);
    }
}
