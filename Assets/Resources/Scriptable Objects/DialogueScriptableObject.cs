using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    void LaunchStartingEventsEntry(int index)
    {
        allEntries[index].entryStartEvents?.Invoke();
    }
    void LaunchEndEventsEntry(int index)
    {
        allEntries[index].entryEndEvents?.Invoke();
    }

    void LaunchStartingEventsGeneral()
    {
        startEventsGENERAL?.Invoke();
    }
    void LaunchEndEventGeneral()
    {
        endEventsGENERAL?.Invoke();
    }

    public void MoveToNextEntry(int index)
    {

    }
}
