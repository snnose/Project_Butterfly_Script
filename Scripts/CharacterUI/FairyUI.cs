using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine;
using Spine.Unity;
using DG.Tweening;
using Utils.SpineUtil;

public class FairyUI : MonoBehaviour
{
    [Header("CharacterUIManager")]
    [SerializeField] private CharacterUIManager characterUIManager;

    [Header("FairyUIs")]
    [SerializeField] private FairyChangeUI fairyChangeUI;
    [SerializeField] private FairyDetailUI fairyDetailUI;
    [SerializeField] private FairySkinChangeUI fairySkinChangeUI;

    [Header("Selected Fairy")]
    [SerializeField] private FairySlot selectedFairySlot = null;

    [Header("Using Fairy")]
    [SerializeField] private FairySlot usingFairySlot = null;

    private void Awake()
    {
        InitializeFairyUI();
        AddActionEvents();
    }

    private void OnEnable()
    {
      
    }

    /// <summary>
    /// 하위 UI에 Action 이벤트를 더해줍니다.
    /// </summary>
    private void AddActionEvents()
    {
        fairyChangeUI.updatePlayFabCharacter += GetCharacterUIManager().playfabUpdateCharacter.UpdateUserCharacterOrFairy;
        fairySkinChangeUI.updatePlayFabCharacter += GetCharacterUIManager().playfabUpdateCharacter.UpdateUserCharacterOrFairy;
    }

    public void InitializeFairyUI()
    {
        fairyChangeUI.InitializeFairyChangeUI();
        fairyDetailUI.InitializeFairyDetailUI();
    }

    public void UpdateFairyUI()
    {
        fairyChangeUI.UpdateFairyList();
    }

    public void SetActiveCurrentFairySkinList()
    {

    }

    /// <summary>
    /// 정령을 터치하면 자세히 보기 씬으로 전환됩니다.
    /// </summary>
    public void OnClickFairyBackground()
    {
        
    }

    public void SetFairyUIActive(bool isActive)
    {
        this.gameObject.SetActive(isActive);
        fairyChangeUI.gameObject.SetActive(isActive);
    }

    public void ClearSelectedFairySlot()
    {
        selectedFairySlot = null;
    }

    public void ClearUsingFairySlot()
    {
        usingFairySlot.SetActiveTextBox(false);
        usingFairySlot.EnableChangeButton();
    }

    public void SetUsingFairySlot(FairySlot fairySlot)
    {
        usingFairySlot = fairySlot;
    }

    public void SetSelectedFairySlot(FairySlot fairySlot)
    {
        selectedFairySlot = fairySlot;
    }

    public FairySlot GetUsingFairySlot()
    {
        return usingFairySlot;
    }

    public FairySlot GetSelectedFairySlot()
    {
        return selectedFairySlot;
    }

    public int GetCurrentFairyId()
    {
        int fairyId = selectedFairySlot != null ? selectedFairySlot.GetFairySlotId() : usingFairySlot.GetFairySlotId();

        return fairyId;
    }

    public int GetCurrentFairyLevel()
    {
        int fairyLevel = selectedFairySlot != null ? selectedFairySlot.GetFairySlotLevel() : usingFairySlot.GetFairySlotLevel();

        return fairyLevel;
    }

    public CharacterUIManager GetCharacterUIManager()
    {
        return characterUIManager;
    }

    public void HideAllFairyUI()
    {
        fairyChangeUI.gameObject.SetActive(false);
        fairyDetailUI.gameObject.SetActive(false);
        fairySkinChangeUI.gameObject.SetActive(false);
    }
}
