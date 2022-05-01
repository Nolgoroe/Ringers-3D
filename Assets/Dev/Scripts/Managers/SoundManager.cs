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
    TileLock,
    ItemPop,
    LevelWin,
    LevelAmbience
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

    public AudioSource audioSourceSFX;
    public AudioSource audioSourceAmbience;

    public EnumAndClip[] soundsForGame;

    public Dictionary<Sounds, AudioClip[]> enumToSound;

    private void Start()
    {
        Instance = this;
        audioSourceSFX = GetComponent<AudioSource>();
        enumToSound = new Dictionary<Sounds, AudioClip[]>();


        foreach (EnumAndClip EAC in soundsForGame)
        {
            enumToSound.Add(EAC.enumSound, EAC.theSound);
        }
    }


    public void PlaySound(Sounds soundEnum)
    {
        audioSourceSFX.volume = 0.5f;

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSourceSFX.PlayOneShot(enumToSound[soundEnum][ran]);
    }

    public void PlaySoundChangeVolume(Sounds soundEnum, float Volume)
    {
        audioSourceSFX.volume = Volume;

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSourceSFX.PlayOneShot(enumToSound[soundEnum][ran]);

    }

    public IEnumerator PlaySoundDelay(Sounds soundEnum, float Delay)
    {
        yield return new WaitForSeconds(Delay);
        audioSourceSFX.volume = 0.5f;

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSourceSFX.PlayOneShot(enumToSound[soundEnum][ran]);

    }

    public IEnumerator PlaySoundChangeVolumeAndDelay(Sounds soundEnum, float Volume, float Delay)
    {
        yield return new WaitForSeconds(Delay);

        audioSourceSFX.volume = Volume;

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSourceSFX.PlayOneShot(enumToSound[soundEnum][ran]);
    }

    public void PlayAmbience(Sounds soundEnum)
    {
        //audioSourceSFX.volume = 0.5f;
        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSourceAmbience.clip = enumToSound[soundEnum][ran];

        audioSourceAmbience.Play();
    }

    public void StopAmbienceMusic()
    {
        audioSourceAmbience.Stop();
    }
    public IEnumerator FadeOutAmbientMusic()
    {
        float vol = 1;

        LeanTween.value(audioSourceAmbience.gameObject, 1, 0, 2).setOnUpdate((float val) =>
        {
            vol = val;
            audioSourceAmbience.volume = vol;
        });

        yield return new WaitForSeconds(2);
        audioSourceAmbience.Stop();
    }
    public IEnumerator FadeInAmbientMusic(Sounds soundEnum)
    {
        audioSourceAmbience.volume = 0;

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSourceAmbience.clip = enumToSound[soundEnum][ran];

        audioSourceAmbience.Play();

        float vol = 0;

        LeanTween.value(audioSourceAmbience.gameObject, 0, 1, 2).setOnUpdate((float val) =>
        {
            vol = val;
            audioSourceAmbience.volume = vol;
        });

        yield return new WaitForSeconds(2);
    }
}
