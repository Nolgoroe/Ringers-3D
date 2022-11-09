using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
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

public enum NPCs
{
    Fox,
    Crow,
    Buttercup,
    Bramble,
    GreatFirSpirit,
    Chameleon,
    None
}

[Serializable]
public class NpcNametagCombo
{
    public NPCs npcType;
    public Sprite potrtaitSprite;
    public Sprite nameTagSprite;
}

[Serializable]
public class EntryData
{
    public NPCs npcType;
    public DialogueSide dialogueSide = DialogueSide.None;
    public DialogueType dialogueType = DialogueType.None;
    public Sprite imageEntrySprite;
    //public Sprite dialogueEntryPortraitSprite;
    //public Sprite dialogueEntryNameBGSprite;
    //public Sprite dialogueTextBGSprite;

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
        if(GameManager.Instance.latestEntry)
        {
            GameManager.Instance.latestEntry.arrowObject.SetActive(false);
        }

        LaunchStartingEventsEntry(GameManager.Instance.currentIndexInDialogue);
    }

    public void InitDialogue()
    {
        GameManager.Instance.hasFinishedShowingDialogue = false;
        UIManager.Instance.isUsingUI = true;
        UIManager.Instance.dialogueMainGameobject.SetActive(true);

        LaunchStartingEventsEntry(GameManager.Instance.currentIndexInDialogue);
    }

    public IEnumerator EndAllDialogue()
    {
        GameManager.Instance.hasFinishedShowingDialogue = true;
        UIManager.Instance.dialogueMainGameobject.SetActive(false);

        yield return new WaitForEndOfFrame();

        LaunchEndEventGeneral();

    }

    public void ClearDialogueParent()
    {
        for (int i = 0; i < UIManager.Instance.DialogueParent.childCount; i++)
        {
            Destroy(UIManager.Instance.DialogueParent.GetChild(i).gameObject);
        }

        UIManager.Instance.isUsingUI = false;

        GameManager.Instance.hasFinishedShowingDialogue = true;
        GameManager.Instance.currentDialogue = null;
        GameManager.Instance.currentIndexInDialogue = -1;
        GameManager.Instance.currentDialogueMultiplier = -1;
        GameManager.Instance.currentDialogueHeightValue = -1;
        GameManager.Instance.currentHeightAdded = -1;
        GameManager.Instance.latestEntry = null;

        UIManager.Instance.continueDialogueButton.gameObject.SetActive(false);
        UIManager.Instance.endDialogueButton.gameObject.SetActive(false);
        UIManager.Instance.skipButton.gameObject.SetActive(false);

        UIManager.Instance.dialogueScroller.content.localPosition = Vector3.zero;
    }

    public void InstantiateDialoguePrefab(int index)
    {
        UIManager.Instance.continueDialogueButton.gameObject.SetActive(false);
        UIManager.Instance.endDialogueButton.gameObject.SetActive(false);
        UIManager.Instance.skipButton.gameObject.SetActive(true);

        RectTransform rect = null;

        SoundManager.Instance.PlaySound(Sounds.DialogueAppear);

        switch (allEntries[index].dialogueType)
        {
            case DialogueType.Dialogue:

                GameManager.Instance.currentDialogueMultiplier++;

                float offsetYAfterImage = 0;

                if (GameManager.Instance.previousWasImage)
                {
                    offsetYAfterImage = UIManager.Instance.dialogueEntryOffsetAddAfterImage;
                }

                switch (allEntries[index].dialogueSide)
                {
                    case DialogueSide.right:
                        rect = Instantiate(dialogueEntryRightPrefab, UIManager.Instance.DialogueParent).GetComponent<RectTransform>();
                        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, GameManager.Instance.currentDialogueHeightValue - /*GameManager.Instance.currentDialogueMultiplier **/ (offsetYAfterImage + UIManager.Instance.dialogueEntryOffsetAddRight));

                        UIManager.Instance.heightScrollToAdd = offsetYAfterImage + UIManager.Instance.dialogueEntryOffsetAddRight;
                        break;
                    case DialogueSide.left:
                        rect = Instantiate(dialogueEntryLeftPrefab, UIManager.Instance.DialogueParent).GetComponent<RectTransform>();
                        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, GameManager.Instance.currentDialogueHeightValue - /*GameManager.Instance.currentDialogueMultiplier **/ (offsetYAfterImage + UIManager.Instance.dialogueEntryOffsetAddLeft));

                        UIManager.Instance.heightScrollToAdd = offsetYAfterImage + UIManager.Instance.dialogueEntryOffsetAddLeft;
                        break;
                    default:
                        break;
                }

                if (rect)
                {
                    SetDialogueEntryData(rect.gameObject, index);

                    CheckAutoScrollDialogue();

                    GameManager.Instance.currentDialogueHeightValue = rect.anchoredPosition.y;
                }

                LaunchEndEventsEntry(index);

                GameManager.Instance.previousWasImage = false;
                break;
            case DialogueType.Image:

                GameManager.Instance.currentDialogueMultiplier ++;

                rect = Instantiate(imageEntryPrefab, UIManager.Instance.DialogueParent).GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, GameManager.Instance.currentDialogueHeightValue - /*GameManager.Instance.currentDialogueMultiplier **/ UIManager.Instance.imageEntryOffsetAdd);

                UIManager.Instance.heightScrollToAdd = UIManager.Instance.imageEntryOffsetAdd;

                if (rect)
                {
                    SetImageEntryData(rect.gameObject, index);

                    CheckAutoScrollDialogue();

                    GameManager.Instance.currentDialogueHeightValue = rect.anchoredPosition.y;
                }

                LaunchEndEventsEntry(index); //launches the end events of the entry


                GameManager.Instance.previousWasImage = true;
                break;
            default:
                break;
        }

    }

    private void SetDialogueEntryData(GameObject dialogueRef, int index)
    {
        DialogueObjectRefrences refs = dialogueRef.GetComponent<DialogueObjectRefrences>();
        //refs.textBGRender.sprite = allEntries[index].dialogueTextBGSprite;

        NpcNametagCombo combo = UIManager.Instance.npcNametagsCombos.Where(p => p.npcType == allEntries[index].npcType).SingleOrDefault();
        
        if(combo == null)
        {
            Debug.LogError("Error loading dialogue");
            return;
        }
        
        refs.portraitRenderer.sprite = combo.potrtaitSprite;
        refs.nameBGRenderer.sprite = combo.nameTagSprite;
        refs.textObject.text = "";
        refs.nameText.text = allEntries[index].displayName;
        refs.textObject.text = allEntries[index].conversationBlock;

        GameManager.Instance.latestEntry = refs;

        //UIManager.Instance.CallTypewriterText(this, index, refs.textObject);
    }
    private void SetImageEntryData(GameObject dialogueRef, int index)
    {
        DialogueObjectRefrences refs = dialogueRef.GetComponent<DialogueObjectRefrences>();
        refs.spriteImageRenderer.sprite = allEntries[index].imageEntrySprite;

        //image entry has no display we need to change
        GameManager.Instance.latestEntry = null;
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

    public void CheckAutoScrollDialogue()
    {

        if (GameManager.Instance.currentDialogueHeightValue <= UIManager.Instance.maxDownLimit)
        {
            GameManager.Instance.currentHeightAdded += UIManager.Instance.heightScrollToAdd;

            Vector3 currentPos = UIManager.Instance.dialogueScroller.content.anchoredPosition;
            Vector3 target = new Vector3(currentPos.x, GameManager.Instance.currentHeightAdded + UIManager.Instance.startingHeight, currentPos.z);

            LeanTween.moveLocal(UIManager.Instance.dialogueScroller.content.gameObject, target, UIManager.Instance.timeToScroll);
        }
    }
}
