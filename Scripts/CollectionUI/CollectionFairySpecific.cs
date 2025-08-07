using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class CollectionFairySpecific : MonoBehaviour
{
    [Header("FairySpecific")]
    [SerializeField] private LocalizeStringEvent fairy_name;
    [SerializeField] private Image fairy_color; // 임시 테스트를 위해 Image로 정의
    [SerializeField] private Image fairy_type; // 임시 테스트를 위해 Image로 정의
    [SerializeField] private LocalizeStringEvent fairy_skill;
    [SerializeField] private LocalizeStringEvent fairy_explanation;
    [SerializeField] private Button exitButton;

    public void SetTableReferences(string fairyKey)
    {
        fairy_name.StringReference.TableEntryReference = $"string_collectioninfo_fairy_name_{fairyKey}";
        fairy_explanation.StringReference.TableEntryReference = $"string_collectioninfo_fairy_info_{fairyKey}";
    }

    public void OnClickExitButton()
    {
        gameObject.SetActive(false);

        UIManager.Instance.DeactiveSetGlobalVoume();
    }
}
