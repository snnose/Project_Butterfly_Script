using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class CollectionFlowerSpecific : MonoBehaviour
{
    [Header("FlowerSpecific")]
    [SerializeField] private LocalizeStringEvent flower_name;
    [SerializeField] private Image flower_color; // �ӽ� �׽�Ʈ�� ���� Image�� ����
    [SerializeField] private Image flower_type; // �ӽ� �׽�Ʈ�� ���� Image�� ����
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
