using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Events;
using UnityEngine.Localization.Components;

public class CollectionFlowerBundle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI flowerTitle;
    [SerializeField] private LocalizeStringEvent flowerTitleStringEvent;
    [SerializeField] private Transform flowersGrid;

    public void SetFlowerTitleText(string text)
    {
        flowerTitle.text = text;
    }

    /// <summary>
    /// FlowerTitle의 Localization stringKey를 설정합니다.
    /// </summary>
    /// <param name="stringKey"></param>
    public void SetFlowerTitleStringKey(string stringKey)
    {
        flowerTitleStringEvent.StringReference.TableEntryReference = $"name_{stringKey}";
    }

    public Transform GetFlowerGridTransform()
    {
        return flowersGrid;
    }
}
