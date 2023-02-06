using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.Events;
using TMPro;
using GameAnalyticsSDK;
using Spine.Unity;
using Spine.Unity.Editor;
using UnityEngine.Playables;

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
    BabyBunnyBag,
    BabyBunnyNoBag,
    None
}

[Serializable]
public class NpcNametagCombo
{
    public NPCs npcType;
    public GameObject nameTagObject;
    public Sprite potrtaitSprite;
    public Sprite nameTagSprite;
    public float LeftSideXpos, rightSideXpos;
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

    public SkeletonDataAsset skeletonDataAsset;
    public PlayableAsset playableAsset;
    public AnimationReferenceAsset blinkAnimation;

    [SpineAnimation(dataField: "skeletonDataAsset")]
    public string relaventPos;

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

    public int dialogueID;
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

        GameAnalytics.NewDesignEvent("Dialogues:Continued Dialogue Normally:" + dialogueID);

        LaunchStartingEventsEntry(GameManager.Instance.currentIndexInDialogue);
    }

    public void InitDialogue()
    {
        GameManager.Instance.hasFinishedShowingDialogue = false;
        UIManager.Instance.isUsingUI = true;
        UIManager.Instance.dialogueMainGameobject.SetActive(true);

        LaunchStartingEventsGeneral();

        LaunchStartingEventsEntry(GameManager.Instance.currentIndexInDialogue);
    }

    public IEnumerator EndAllDialogue()
    {
        GameManager.Instance.hasFinishedShowingDialogue = true;
        UIManager.Instance.dialogueMainGameobject.SetActive(false);

        GameAnalytics.NewDesignEvent("Dialogues:Skipped dialogue:" + dialogueID);

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

        DialogueSide side = DialogueSide.None;


        UIManager.Instance.continueDialogueButton.gameObject.SetActive(false);
        UIManager.Instance.endDialogueButton.gameObject.SetActive(false);
        UIManager.Instance.skipButton.gameObject.SetActive(true);

        RectTransform rect = null;

        if (GameManager.Instance.currentIndexInDialogue == 0)
        {
            SoundManager.Instance.PlaySound(Sounds.DialogueAppear);
        }
        else
        {
            SoundManager.Instance.PlaySound(Sounds.DialogueStages);
        }
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

                        side = DialogueSide.right;

                        rect = Instantiate(dialogueEntryRightPrefab, UIManager.Instance.DialogueParent).GetComponent<RectTransform>();
                        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, GameManager.Instance.currentDialogueHeightValue - /*GameManager.Instance.currentDialogueMultiplier **/ (offsetYAfterImage + UIManager.Instance.dialogueEntryOffsetAddRight));

                        UIManager.Instance.heightScrollToAdd = offsetYAfterImage + UIManager.Instance.dialogueEntryOffsetAddRight;
                        break;
                    case DialogueSide.left:

                        side = DialogueSide.left;

                        rect = Instantiate(dialogueEntryLeftPrefab, UIManager.Instance.DialogueParent).GetComponent<RectTransform>();
                        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, GameManager.Instance.currentDialogueHeightValue - /*GameManager.Instance.currentDialogueMultiplier **/ (offsetYAfterImage + UIManager.Instance.dialogueEntryOffsetAddLeft));

                        UIManager.Instance.heightScrollToAdd = offsetYAfterImage + UIManager.Instance.dialogueEntryOffsetAddLeft;
                        break;
                    default:
                        break;
                }

                if (rect)
                {
                    SetDialogueEntryData(rect.gameObject, index, side);

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

        if(index == allEntries.Length - 1)
        {
            DialogueObjectRefrences refs;
            rect.TryGetComponent(out refs);

            if(refs)
            {
                refs.arrowObject.SetActive(false);
            }

        }
    }

    private void SetDialogueEntryData(GameObject dialogueRef, int index, DialogueSide side)
    {

        DialogueObjectRefrences refs = dialogueRef.GetComponent<DialogueObjectRefrences>();
        //refs.textBGRender.sprite = allEntries[index].dialogueTextBGSprite;

        refs.skeletonAnimation.skeletonDataAsset = allEntries[index].skeletonDataAsset;
        SpineEditorUtilities.ReloadSkeletonDataAssetAndComponent(refs.skeletonRenderer);

        refs.director.playableAsset = allEntries[index].playableAsset;
        refs.blinkAnimationRef.blinkAnimation = allEntries[index].blinkAnimation;
        refs.CallStartBlinkingSpine();

        NpcNametagCombo combo = UIManager.Instance.npcNametagsCombos.Where(p => p.npcType == allEntries[index].npcType).SingleOrDefault();
        
        if(combo == null)
        {
            Debug.LogError("Error loading dialogue");
            return;
        }

        CreateNameTagByNPC(refs, combo, side);

        refs.portraitRenderer.sprite = combo.potrtaitSprite;
        refs.nameBGRenderer.sprite = combo.nameTagSprite;
        refs.textObject.text = "";
        refs.nameText.text = allEntries[index].displayName;
        refs.textObject.text = allEntries[index].conversationBlock;

        //refs.nameText.fontSizeMin = 25;
        //refs.nameText.fontSizeMax = 25;

        GameManager.Instance.latestEntry = refs;


        refs.CallPlanDirectorAnim();
        //UIManager.Instance.CallTypewriterText(this, index, refs.textObject);
    }
    private void SetImageEntryData(GameObject dialogueRef, int index)
    {
        DialogueObjectRefrences refs = dialogueRef.GetComponent<DialogueObjectRefrences>();
        refs.spriteImageRenderer.sprite = allEntries[index].imageEntrySprite;

        //image entry has no display we need to change
        GameManager.Instance.latestEntry = null;
    }

    private void CreateNameTagByNPC(DialogueObjectRefrences dialogueRef, NpcNametagCombo combo, DialogueSide side)
    {
        GameObject go = Instantiate(combo.nameTagObject, dialogueRef.textBoxParent);

        if(go)
        {
            dialogueRef.nameBGRenderer = go.GetComponent<SpriteRenderer>();

            if(go.transform.childCount > 0)
            {
                dialogueRef.nameText = go.transform.GetChild(0).GetComponent<TMP_Text>();
            }

            if(side == DialogueSide.left)
            {
                go.transform.localPosition = new Vector3(combo.LeftSideXpos, go.transform.localPosition.y, go.transform.localPosition.z);
            }
            else
            {
                go.transform.localPosition = new Vector3(combo.rightSideXpos, go.transform.localPosition.y, go.transform.localPosition.z);
            }
        }
    }

    public void ActiavteContinueDialogueButton()
    {
        UIManager.Instance.continueDialogueButton.gameObject.SetActive(true);
    }

    public void ActiavteEndDialogueButton()
    {
        UIManager.Instance.endDialogueButton.gameObject.SetActive(true);
    }

    public void ResetSpineAnimationData()
    {
        //GameManager.Instance.latestEntry.blinkAnimationRef.StopBlinking();

        //GameManager.Instance.latestEntry.RefreshAnimationData();

        ///// we do -1 here since the current index in dialogue always increments by 1  
        ///// directly when an entry is spawned
        ///// but we still want a ref to the precious entry

        //GameManager.Instance.latestEntry.spineAnimationState.SetAnimation(
        //    0,
        //    allEntries[GameManager.Instance.currentIndexInDialogue - 1].relaventPos,
        //    false
        //    );

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

    [ContextMenu("Reset spline data")]
    public void ResetAllSplineData()
    {
        foreach (EntryData entry in allEntries)
        {
            entry.skeletonDataAsset = null;
            entry.playableAsset = null;
            entry.blinkAnimation = null;
        }
        

    }
}
