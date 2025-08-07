using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CollectionTabButton : MonoBehaviour
{
    [SerializeField] RectTransform buttonRectTransform;

    [SerializeField] private Vector2 selectedSizeDelta = new Vector2();

    public Action<CollectionTabButton> onClickTabButton;

    public void OnClickTabButton()
    {
        onClickTabButton.Invoke(this);
    }
}
