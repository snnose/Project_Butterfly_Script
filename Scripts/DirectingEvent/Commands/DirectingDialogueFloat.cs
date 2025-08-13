using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DirectingEventSystem
{
    public class DirectingDialogueFloat : DirectingEvent
    {
        public DialogueOption dialogueOption;

        public override IEnumerator Execute()
        {
            yield return FloatDialogue(dialogueOption);
        }

        private IEnumerator FloatDialogue(DialogueOption dialogueOption)
        {
            DialogueManager dialogueManager = CheckDialogueExist();

            if (dialogueManager == null)
            {
                Debug.LogError($"DialogueFloatCommandSO : DialogueUI가 존재하지 않습니다!");
                yield break;
            }

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
                dialogueManager = existingDialogueUI.GetComponent<DialogueManager>();
            }
            else
            {
                // 존재하지 않는 경우, instantiate
                GameObject loadedPrefab = Resources.Load<GameObject>("DialogueUI");
                dialogueManager = loadedPrefab.GetComponent<DialogueManager>();
                existingDialogueUI = dialogueManager.InstantiateDialogueUI(Vector3.zero, Quaternion.identity);
                existingDialogueUI.transform.SetParent(UIManager.Instance.gameObject.transform);
                existingDialogueUI.GetComponent<Canvas>().worldCamera = UIManager.Instance.GetUICamera(); // UI 카메라를 연결
            }

            return dialogueManager;
        }
    }
}