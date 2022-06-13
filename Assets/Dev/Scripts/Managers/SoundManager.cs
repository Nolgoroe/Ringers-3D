using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    LevelAmbience,
    //LevelAmbienceSlow,
    PotionUse,
    PotionSelect,
    IntroMusic,
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

    public bool muteMusic, muteSFX;

    public Toggle musicToggle, SFXToggle;

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
        Debug.Log("Played " + soundEnum);
        //if (!muteSFX)
        //{
        //    audioSourceSFX.volume = 0.5f;
        //}

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSourceSFX.PlayOneShot(enumToSound[soundEnum][ran]);
    }

    public void PlaySoundChangeVolume(Sounds soundEnum, float Volume)
    {
        if (!muteSFX)
        {
            audioSourceSFX.volume = Volume;
        }

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSourceSFX.PlayOneShot(enumToSound[soundEnum][ran]);

    }

    public IEnumerator PlaySoundFadeIn(Sounds soundEnum)
    {
        if (muteMusic)
        {
            yield break;
        }

        audioSourceSFX.volume = 0;

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSourceSFX.PlayOneShot(enumToSound[soundEnum][ran]);

        float vol = 0;

        LeanTween.value(audioSourceSFX.gameObject, 0, 1, 1.5f).setOnUpdate((float val) =>
        {
            vol = val;
            audioSourceSFX.volume = vol;
        });
    }
    public IEnumerator PlaySoundFadeOut()
    {
        Debug.Log("Fading out");
        if (muteMusic)
        {
            yield break;
        }

        audioSourceSFX.volume = 1;

        float vol = 1;

        LeanTween.value(audioSourceSFX.gameObject, 1, 0, 0.5f).setOnUpdate((float val) =>
        {
            vol = val;
            audioSourceSFX.volume = vol;
        });

        yield return new WaitForSeconds(0.5f);
        audioSourceSFX.Stop();
        audioSourceSFX.volume = 1;

    }

    public IEnumerator PlaySoundDelay(Sounds soundEnum, float Delay)
    {
        yield return new WaitForSeconds(Delay);

        if (!muteSFX)
        {
            audioSourceSFX.volume = 0.5f;
        }

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSourceSFX.PlayOneShot(enumToSound[soundEnum][ran]);

    }

    public IEnumerator PlaySoundChangeVolumeAndDelay(Sounds soundEnum, float Volume, float Delay)
    {
        yield return new WaitForSeconds(Delay);

        if (!muteSFX)
        {
            audioSourceSFX.volume = Volume;
        }

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSourceSFX.PlayOneShot(enumToSound[soundEnum][ran]);
    }

    public void PlayAmbience(Sounds soundEnum)
    {
        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        audioSourceAmbience.clip = enumToSound[soundEnum][ran];

        audioSourceAmbience.Play();
    }

    public void StopAmbienceMusic()
    {
        audioSourceAmbience.Stop();
    }
    public IEnumerator FadeOutAmbientMusic(float time)
    {
        if (muteMusic)
        {
            yield break;
        }

        float vol = 1;

        LeanTween.value(audioSourceAmbience.gameObject, 1, 0, time).setOnUpdate((float val) =>
        {
            vol = val;
            audioSourceAmbience.volume = vol;
        });

        yield return new WaitForSeconds(time);
        audioSourceAmbience.Stop();
    }
    public IEnumerator FadeInAmbientMusic(Sounds soundEnum)
    {
        if (muteMusic)
        {
            yield break;
        }

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

    public IEnumerator FadeInOnlyVolume(Sounds soundEnum)
    {
        if (muteMusic)
        {
            yield break;
        }

        audioSourceAmbience.volume = 0;

        float vol = 0;

        LeanTween.value(audioSourceAmbience.gameObject, 0, 1, 2).setOnUpdate((float val) =>
        {
            vol = val;
            audioSourceAmbience.volume = vol;
        });

        yield return new WaitForSeconds(2);
    }

    public void MuteUnmuteMusic()
    {
        LeanTween.cancel(audioSourceAmbience.gameObject);

        muteMusic = !muteMusic;

        if (muteMusic)
        {
            musicToggle.isOn = false;
            audioSourceAmbience.volume = 0;
        }
        else
        {
            musicToggle.isOn = true;

            audioSourceAmbience.volume = 1;
        }
    }

    public void MuteUnmuteSFX()
    {
        muteSFX = !muteSFX;

        if (muteSFX)
        {
            SFXToggle.isOn = false;

            audioSourceSFX.volume = 0;
        }
        else
        {
            SFXToggle.isOn = true;

            audioSourceSFX.volume = 1;
        }
    }
}
