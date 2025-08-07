using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Events;
using UnityEngine.Localization.Components;

public class CollectionCharacterSpecific : MonoBehaviour
{
    [Header("CharacterItemSpecific")]
    public Button characterItemButton;
    public LocalizeStringEvent character_name;
    public Image character_color; // 임시 테스트를 위해 Image로 정의
    public Image character_type; // 임시 테스트를 위해 Image로 정의
    public LocalizeStringEvent character_skill;
    public LocalizeStringEvent character_explanation;
}
