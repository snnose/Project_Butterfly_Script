using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DirectingEventSystem;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "DirectingEvent/Commands/FadeOutIn")]
public class FadeOutInCommandSO : DirectingEvent
{
    public FadeOutInOption fadeOutInOption;

    public override IEnumerator Execute()
    {
        yield return FadeOutIn(fadeOutInOption);
    }

    private IEnumerator FadeOutIn(FadeOutInOption fadeOutInOption)
    {
        FadeUIManager fadeUIManager = GetFadeUIManager();

        yield return fadeUIManager.FadeOut(fadeOutInOption.fadeType, fadeOutInOption.fadeOutColor, fadeOutInOption.fadeOutDuration);

        // gapEvent
        if (fadeOutInOption.gapEvents.Count > 0)
        {
            foreach (DirectingEvent directingEvent in fadeOutInOption.gapEvents)
            {
                yield return directingEvent.Execute();
            }
        }

        if (fadeOutInOption.stringKeys.Count > 0)
        {
            int count = fadeOutInOption.stringKeys.Count;
            string text;

            for (int i = 0; i < count; i++)
            {
                // FIXME: stringKey 대응 해둬야 함
                text = GetLocalizedString(fadeOutInOption.stringKeys[i]);
                fadeUIManager.SetText(text);
                fadeUIManager.ChangeTextOption(fadeOutInOption.textOption[i]);
                yield return fadeUIManager.FloatText(fadeOutInOption.stringDuration);
            }
        }

        yield return fadeUIManager.FadeIn(fadeOutInOption.fadeType, fadeOutInOption.fadeOutColor, fadeOutInOption.fadeOutDuration);

        yield break;
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

    private FadeUIManager GetFadeUIManager()
    {
        return UIManager.Instance.fadeUIManager;
    }
}
