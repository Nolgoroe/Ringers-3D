using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public enum DialogueSide
{
    right,
    left,
    None
}
public enum DialogueType
{
    Dialogue,
    Image,
    None
}

[Serializable]
public class EntryData
{
    public DialogueSide dialogueSide = DialogueSide.None;
    public DialogueType dialogueType = DialogueType.None;
    public Sprite imageEntrySprite;
    public Sprite dialogueEntryPortraitSprite;
    public Sprite dialogueEntryNameBGSprite;
    public Sprite dialogueTextBGSprite;

    public string displayName;
    [TextArea(5, 5)]
    public string conversationBlock;

    public UnityEvent entryStartEvents;
    public UnityEvent entryEndEvents;
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Create Dialogue")]
public class DialogueScriptableObject : ScriptableObject
{
    public EntryData[] allEntries;

    public UnityEvent startEventsGENERAL;
    public UnityEvent endEventsGENERAL;
    public UnityEvent onClickOnScreen;

    public GameObject dialogueEntryLeftPrefab;
    public GameObject dialogueEntryRightPrefab;
    public GameObject imageEntryPrefab;
    public void LaunchStartingEventsEntry(int index)
    {
        allEntries[index].entryStartEvents?.Invoke();
    }
    public void LaunchEndEventsEntry(int index)
    {
        allEntries[index].entryEndEvents?.Invoke();
    }

    public void LaunchStartingEventsGeneral()
    {
        startEventsGENERAL?.Invoke();
    }
    public void LaunchEndEventGeneral()
    {
        endEventsGENERAL?.Invoke();
    }

    public void MoveToNextEntry()
    {
        LaunchStartingEventsEntry(GameManager.Instance.currentIndexInDialogue);
    }

    public void InitDialogue()
    {
        GameManager.Instance.hasFinishedShowingDialogue = false;
        UIManager.Instance.isUsingUI = true;
        UIManager.Instance.dialogueMainGameobject.SetActive(true);

        LaunchStartingEventsEntry(GameManager.Instance.currentIndexInDialogue);
    }

    public void EndAllDialogue()
    {
        LaunchEndEventGeneral();

        GameManager.Instance.hasFinishedShowingDialogue = true;
        UIManager.Instance.dialogueMainGameobject.SetActive(false);
    }

    public void ClearDialogueParent()
    {
        for (int i = 0; i < UIManager.Instance.DialogueParent.childCount; i++)
        {
            Destroy(UIManager.Instance.DialogueParent.GetChild(i).gameObject);
        }

        UIManager.Instance.isUsingUI = false;

        GameManager.Instance.currentDialogue = null;
        GameManager.Instance.currentIndexInDialogue = 0;

        UIManager.Instance.continueDialogueButton.gameObject.SetActive(false);
        UIManager.Instance.endDialogueButton.gameObject.SetActive(false);
        UIManager.Instance.skipButton.gameObject.SetActive(false);

    }

    public void InstantiateDialoguePrefab(int index)
    {
        UIManager.Instance.continueDialogueButton.gameObject.SetActive(false);
        UIManager.Instance.endDialogueButton.gameObject.SetActive(false);
        UIManager.Instance.skipButton.gameObject.SetActive(true);

        RectTransform rect = null;
        switch (allEntries[index].dialogueType)
        {
            case DialogueType.Dialogue:
                switch (allEntries[index].dialogueSide)
                {
                    case DialogueSide.right:
                        rect = Instantiate(dialogueEntryRightPrefab, UIManager.Instance.DialogueParent).GetComponent<RectTransform>();
                        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - index * 300);
                        break;
                    case DialogueSide.left:
                        rect = Instantiate(dialogueEntryLeftPrefab, UIManager.Instance.DialogueParent).GetComponent<RectTransform>();
                        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - index * 300);
                        break;
                    default:
                        break;
                }

                if (rect)
                {
                    SetDialogueEntryData(rect.gameObject, index);
                }
                break;
            case DialogueType.Image:
                rect = Instantiate(imageEntryPrefab, UIManager.Instance.DialogueParent).GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - index * 300);

                if (rect)
                {
                    SetImageEntryData(rect.gameObject, index);
                }

                LaunchEndEventsEntry(index);

                break;
            default:
                break;
        }

    }

    private void SetDialogueEntryData(GameObject dialogueRef, int index)
    {
        DialogueObjectRefrences refs = dialogueRef.GetComponent<DialogueObjectRefrences>();
        refs.portraitRenderer.sprite = allEntries[index].dialogueEntryPortraitSprite;
        refs.textBGRender.sprite = allEntries[index].dialogueTextBGSprite;
        refs.nameBGRenderer.sprite = allEntries[index].dialogueEntryNameBGSprite;
        refs.textObject.text = "";
        refs.nameText.text = allEntries[index].displayName;

        UIManager.Instance.CallTypewriterText(this, index, refs.textObject);

    }
    private void SetImageEntryData(GameObject dialogueRef, int index)
    {
        DialogueObjectRefrences refs = dialogueRef.GetComponent<DialogueObjectRefrences>();
        refs.spriteImageRenderer.sprite = allEntries[index].imageEntrySprite;
    }

    public void ActiavteContinueDialogueButton()
    {
        UIManager.Instance.continueDialogueButton.gameObject.SetActive(true);
    }

    public void ActiavteEndDialogueButton()
    {
        UIManager.Instance.endDialogueButton.gameObject.SetActive(true);
    }

    public void IncrementPhaseInDialogue()
    {
        GameManager.Instance.currentIndexInDialogue++;

        if (GameManager.Instance.currentIndexInDialogue == GameManager.Instance.currentDialogue.allEntries.Length)
        {
            ActiavteEndDialogueButton();
        }
        else
        {
            ActiavteContinueDialogueButton();
        }
    }
}
