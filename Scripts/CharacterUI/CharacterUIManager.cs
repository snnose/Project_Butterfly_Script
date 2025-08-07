using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUIManager : MonoBehaviour
{
    [Header("PlayFab")]
    public PlayFabUpdateCharacter playfabUpdateCharacter;

    [Header("TopBar")]
    [SerializeField] private GameObject topBar;
    [SerializeField] private TextMeshProUGUI profile1;  // 0715) TopBar가 정확히 무슨 역할인지 기억이 안난ㄷ,..
    [SerializeField] private TextMeshProUGUI profile2;

    [Header("BottomBar")]
    [SerializeField] private Button returnToLobbyButton;

    [Header("UIChange")]
    [SerializeField] private Button UIChangeButton;
    [SerializeField] private Image currentUIImage;
    [SerializeField] private Image anotherUIImage;

    [Header("CharacterUI")]
    [SerializeField] private CharacterUI characterUI;
    [SerializeField] private Sprite characterUISprite;

    [Header("FairyUI")]
    [SerializeField] private FairyUI fairyUI;
    [SerializeField] private GameObject userCurrency;
    [SerializeField] private Sprite fairyUISprite;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        characterUI.gameObject.SetActive(true);
        currentUIImage.sprite = characterUISprite;

        fairyUI.gameObject.SetActive(false);
        anotherUIImage.sprite = fairyUISprite;
    }

    public void InitializeTopBar()
    {

    }

    public void OnClickUIChangeButton() 
    {
        bool isCharacterUIActive = characterUI.gameObject.activeInHierarchy;

        if (isCharacterUIActive)
        {
            // FairyChangeUI를 띄운다.
            fairyUI.HideAllFairyUI();
            fairyUI.SetFairyUIActive(true);
            userCurrency.SetActive(true);
            characterUI.gameObject.SetActive(false);
        }
        else
        {
            fairyUI.SetFairyUIActive(false);
            userCurrency.SetActive(false);
            characterUI.gameObject.SetActive(true);
        }

        ChangeImagesSprite();
    }

    /// <summary>
    /// current, another 이미지의 sprite를 서로 교체합니다.
    /// </summary>
    private void ChangeImagesSprite()
    {
        Sprite currentImageSprite;

        currentImageSprite = currentUIImage.sprite;

        currentUIImage.sprite = anotherUIImage.sprite;
        anotherUIImage.sprite = currentImageSprite;
    }

    public void OnClickReturnToLobbyButton()
    {
        UIManager.Instance.ShowLobbyUI();
        UIManager.Instance.ShowNavigatorUI();
        this.gameObject.SetActive(false);
    }

    public void SetUIChangeButtonActive(bool isActive)
    {
        UIChangeButton.gameObject.SetActive(isActive);
    }

    public void SetTopBarActive(bool isActive)
    {
        topBar.SetActive(isActive);
    }
}
