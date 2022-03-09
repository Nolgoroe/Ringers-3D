using System.Collections;
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
    AddTileBoard,
    PieceMoveDeal,
    ElementCrafted,
    NegativeSound,
    PageFlip,
    TileLock
    //// Add all sounds here
}

[System.Serializable]
public class EnumAndClip
{
    public Sounds enumSound;

    public AudioClip[] theSound;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource audioSource;

    public EnumAndClip[] soundsForGame;

    public Dictionary<Sounds, AudioClip[]> enumToSound;

    private void Start()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        enumToSound = new Dictionary<Sounds, AudioClip[]>();


        foreach (EnumAndClip EAC in soundsForGame)
        {
            enumToSound.Add(EAC.enumSound, EAC.theSound);
        }
    }


    public void PlaySound(Sounds soundEnum)
    {
        audioSource.volume = 0.5f;

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSource.PlayOneShot(enumToSound[soundEnum][ran]);
    }

    public void PlaySoundChangeVolume(Sounds soundEnum, float Volume)
    {
        audioSource.volume = Volume;

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSource.PlayOneShot(enumToSound[soundEnum][ran]);

    }

    public IEnumerator PlaySoundDelay(Sounds soundEnum, float Delay)
    {
        yield return new WaitForSeconds(Delay);
        audioSource.volume = 0.5f;

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSource.PlayOneShot(enumToSound[soundEnum][ran]);

    }

    public IEnumerator PlaySoundChangeVolumeAndDelay(Sounds soundEnum, float Volume, float Delay)
    {
        yield return new WaitForSeconds(Delay);

        audioSource.volume = Volume;

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSource.PlayOneShot(enumToSound[soundEnum][ran]);
    }

}
