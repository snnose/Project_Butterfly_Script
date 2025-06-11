using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DirectingEventSystem
{
    [CreateAssetMenu(menuName = "DirectingEvent/Commands/DialogueFloat")]
    public class DialogueFloatCommandSO : DirectingEvent
    {
        public override IEnumerator Execute()
        {
            yield return FloatDialogue(dialogueOptions);
        }

        private IEnumerator FloatDialogue(List<DialogueOption> dialogueOptions)
        {
            DialogueManager dialogueManager = CheckDialogueExist();

            if (dialogueManager == null)
            {
                Debug.LogError($"DialogueFloatCommandSO : DialogueUI가 존재하지 않습니다!");
                yield break;
            }

            int index = DirectingEventManager.Instance.GetOptionIndex()[(int)OptionType.DialogueFloat];
            DialogueOption dialogueOption = dialogueOptions[index];
            DirectingEventManager.Instance.SetOptionIndexValue((int)OptionType.DialogueFloat, ++index);          
            
            List<Dialogue> dialogues = DialogueData.GetDialogueProgress(dialogueOption.progress);
            List<Dialogue> selectedDialogues = new List<Dialogue>();

            for (int i = dialogueOption.startPage - 1; i < dialogueOption.endPage; i++)
            {
                selectedDialogues.Add(dialogues[i]);
            }

            dialogueManager.SetDialogues(dialogueOption.progress, selectedDialogues);
            dialogueManager.NextDialogue();

            DirectingEventManager.Instance.SetIsFloatingDialogue(true);

            yield break;
        }

        private DialogueManager CheckDialogueExist()
        {
            DialogueManager dialogueManager = null;

            // instantiate된 DialogueUI가 이미 있는지 확인
            GameObject existingDialogueUI = GameObject.Find("DialogueUI(Clone)");
            if (existingDialogueUI != null)
            {
                existingDialogueUI.SetActive(true);
            }
            else
            {
                // 존재하지 않는 경우, instantiate
                GameObject loadedPrefab = Resources.Load<GameObject>("DialogueUI");
                existingDialogueUI = Instantiate(loadedPrefab, Vector3.zero, Quaternion.identity);
                existingDialogueUI.transform.SetParent(UIManager.Instance.gameObject.transform);
                existingDialogueUI.GetComponent<Canvas>().worldCamera = UIManager.Instance.GetUICamera(); // UI 카메라를 연결
            }

            dialogueManager = existingDialogueUI.GetComponent<DialogueManager>();

            return dialogueManager;
        }
    }
}