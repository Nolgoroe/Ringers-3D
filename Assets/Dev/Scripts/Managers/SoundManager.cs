﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sounds
{
    DealButton,
    UnlockZone,
    RuneLimiterUnMatch,
    RuneLimiterMatch,
    SliceLimiterMatch,
    SolvedRing,
    TileMatch,
    TileUnmatch,
    ButtonPressUI,
    AddTileBoard
    //// Add all sounds here
}

[System.Serializable]
public class EnumAndClip
{
    public Sounds enumSound;

    public AudioClip theSound;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource audioSource;

    public EnumAndClip[] soundsForGame;

    public Dictionary<Sounds, AudioClip> enumToSound;

    private void Start()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        enumToSound = new Dictionary<Sounds, AudioClip>();


        foreach (EnumAndClip EAC in soundsForGame)
        {
            enumToSound.Add(EAC.enumSound, EAC.theSound);
        }
    }


    public void PlaySound(Sounds soundEnum)
    {
        audioSource.PlayOneShot(enumToSound[soundEnum]);
    }
}
