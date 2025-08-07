using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class CollectionFlowerSpecific : MonoBehaviour
{
    [Header("FlowerSpecific")]
    [SerializeField] private LocalizeStringEvent flower_name;
    [SerializeField] private Image flower_color; // 임시 테스트를 위해 Image로 정의
    [SerializeField] private Image flower_type; // 임시 테스트를 위해 Image로 정의
    [SerializeField] private LocalizeStringEvent flower_skill;
    [SerializeField] private LocalizeStringEvent flower_explanation;
    [SerializeField] private Button exitButton;

    public void SetTableReferences(int flowerId, string flowerKey)
    {
        string optionKey = OptionData.GetOptionKey(FairyData.FindFairyMainOptionByDropId(flowerId));

        flower_name.StringReference.TableEntryReference = $"string_collectioninfo_fairy_name_{flowerKey}";
        flower_skill.StringReference.TableEntryReference = $"string_collectioninfo_item_option_{optionKey}";
        flower_explanation.StringReference.TableEntryReference = $"string_collectioninfo_fairy_info_{flowerKey}";
    }

    public void OnClickExitButton()
    {
        gameObject.SetActive(false);

        UIManager.Instance.DeactiveSetGlobalVoume();
    }
}
