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
    LastTileSequence,
    LimiterPop,
    CorruptedTilePop,
    TileEnterLevel,
    FoxRelease,
    StagRelease,
    OwlRelease,
    RiveRelease,
    RiveRootRelease,
    RingAppear,
    AlbumAnimalAppear,
    ChestAppear,
    ChestOpen,
    ChestClose,
    GiveChestItem,
    BoarRelease,
    DialogueAppear,
    DialogueStages,
    SuccessTextAppear,
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
    public AudioClip[] soundForChestBar;

    public Dictionary<Sounds, AudioClip[]> enumToSound;

    public bool muteMusic, muteSFX;

    public Toggle musicToggle, SFXToggle;

    public float timeFadeInAmbienceLevel;
    public float timeFadeOutAmbienceLevel;

    public float timeFadeInBGMusic;
    public float timeFadeOutBGMusic;

    public float fadeOutIntroSound;

    public AudioSource forestSounds, normalAmbienceLevel, hudBGMuisc;

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
    public void PlaySound(AudioClip soundClip)
    {
        audioSourceSFX.PlayOneShot(soundClip);
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

        audioSourceSFX.volume = 0;

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));

        if (!muteMusic)
        {
            audioSourceSFX.PlayOneShot(enumToSound[soundEnum][ran]);
        }

        float vol = 0;

        LeanTween.value(audioSourceSFX.gameObject, 0, 1, 1.5f).setOnUpdate((float val) =>
        {
            vol = val;
            audioSourceSFX.volume = vol;
        });

        yield return null;
    }
    public IEnumerator PlaySoundAmbienceFadeOut(float time)
    {
        Debug.Log("Fading out");
        //if (muteMusic)
        //{
        //    yield break;
        //}

        audioSourceSFX.volume = 1;

        float vol = 1;

        LeanTween.value(audioSourceSFX.gameObject, 1, 0, time).setOnUpdate((float val) =>
        {
            vol = val;
            audioSourceSFX.volume = vol;
        });

        yield return new WaitForSeconds(time);
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
    public void StopSFXOneshots()
    {
        audioSourceSFX.Stop();
    }
    
    public void CallFadeOutAmbientMusicLevel(float time, bool isStop)
    {
        StartCoroutine(FadeOutAmbientMusicLevel(time, isStop));
    }

    public IEnumerator FadeOutAmbientMusicLevel(float time, bool isStop)
    {
        //if (muteMusic)
        //{
        //    yield break;
        //}

        float vol = normalAmbienceLevel.volume;

        //LeanTween.value(forestSounds.gameObject, 1, 0, timeFadeInAmbienceLevel).setOnUpdate((float val) =>
        //{
        //    vol = val;
        //    forestSounds.volume = vol;
        //});

        //vol = 1;

        LeanTween.value(normalAmbienceLevel.gameObject, normalAmbienceLevel.volume, 0, time).setOnUpdate((float val) =>
        {
            vol = val;
            normalAmbienceLevel.volume = vol;
        });

        if (isStop) /// temporary here
        {
            vol = forestSounds.volume;

            LeanTween.value(forestSounds.gameObject, forestSounds.volume, 0, time).setOnUpdate((float val) =>
            {
                vol = val;
                forestSounds.volume = vol;
            });
        }

        yield return new WaitForSeconds(time);

        if (isStop)
        {
            audioSourceAmbience.Stop();
            forestSounds.Stop();
            normalAmbienceLevel.Stop();
        }
    }
    public IEnumerator FadeInAmbientMusicLevel(Sounds soundEnum)
    {

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        normalAmbienceLevel.clip = enumToSound[soundEnum][ran];

        forestSounds.volume = 0;
        normalAmbienceLevel.volume = 0;


        forestSounds.Play();
        normalAmbienceLevel.Play();

        if (!muteMusic)
        {
            float vol = 0;

            LeanTween.value(forestSounds.gameObject, 0, 1, timeFadeInAmbienceLevel).setOnUpdate((float val) =>
            {
                vol = val;
                forestSounds.volume = vol;
            });

            vol = 0;

            LeanTween.value(normalAmbienceLevel.gameObject, 0, 1, timeFadeInAmbienceLevel).setOnUpdate((float val) =>
            {
                vol = val;
                normalAmbienceLevel.volume = vol;
            });
        }


        yield return new WaitForSeconds(timeFadeInAmbienceLevel);
    }

    public IEnumerator FadeInMapBGMusic()
    {
        hudBGMuisc.Play();

        if (!muteMusic)
        {
            float vol = 0;

            LeanTween.value(hudBGMuisc.gameObject, 0, 1, timeFadeInBGMusic).setOnUpdate((float val) =>
            {
                vol = val;
                hudBGMuisc.volume = vol;
            });
        }


        yield return new WaitForSeconds(timeFadeInAmbienceLevel);
    }
    public void FadeOutMapBGMusic(float time, bool isStop)
    {

        float vol = hudBGMuisc.volume;

        LeanTween.value(hudBGMuisc.gameObject, hudBGMuisc.volume, 0, time).setOnComplete(() => hudBGMuisc.Stop()).setOnUpdate((float val) =>
        {
            vol = val;
            hudBGMuisc.volume = vol;
        });
    }

    public IEnumerator FadeInOnlyLevelVolume(Sounds soundEnum)
    {
        if (muteMusic)
        {
            yield break;
        }

        int ran = Random.Range(0, (enumToSound[soundEnum].Length));
        normalAmbienceLevel.clip = enumToSound[soundEnum][ran];

        //forestSounds.volume = 0;
        //normalAmbienceLevel.volume = 0;

        float vol = 0;

        LeanTween.value(forestSounds.gameObject, forestSounds.volume, 1, timeFadeInAmbienceLevel).setOnUpdate((float val) =>
        {
            vol = val;
            forestSounds.volume = vol;
        });

        vol = 0;

        LeanTween.value(normalAmbienceLevel.gameObject, normalAmbienceLevel.volume, 1, timeFadeInAmbienceLevel).setOnUpdate((float val) =>
        {
            vol = val;
            normalAmbienceLevel.volume = vol;
        });

        yield return new WaitForSeconds(timeFadeInAmbienceLevel);
    }

    //public IEnumerator FadeInOnlyVolume(Sounds soundEnum)
    //{
    //    if (muteMusic)
    //    {
    //        yield break;
    //    }

    //    audioSourceAmbience.volume = 0;

    //    float vol = 0;

    //    LeanTween.value(audioSourceAmbience.gameObject, 0, 1, 2).setOnUpdate((float val) =>
    //    {
    //        vol = val;
    //        audioSourceAmbience.volume = vol;
    //    });

    //    yield return new WaitForSeconds(2);
    //}

    public void MuteUnmuteMusic()
    {
        PlaySound(Sounds.ButtonPressUI);

        LeanTween.cancel(audioSourceAmbience.gameObject);
        LeanTween.cancel(forestSounds.gameObject);
        LeanTween.cancel(normalAmbienceLevel.gameObject);
        LeanTween.cancel(hudBGMuisc.gameObject);

        muteMusic = !muteMusic;

        if (muteMusic)
        {
            UIManager.Instance.SetMusicOffIcons();
            //musicToggle.isOn = false;
            audioSourceAmbience.volume = 0;
            forestSounds.volume = 0;
            normalAmbienceLevel.volume = 0;
            hudBGMuisc.volume = 0;
        }
        else
        {
            //musicToggle.isOn = true;
            UIManager.Instance.SetMusicOnIcons();

            audioSourceAmbience.volume = 1;
            hudBGMuisc.volume = 1;

            if (GameManager.Instance.levelStarted)
            {
                StartCoroutine(FadeInOnlyLevelVolume(ZoneManagerHelpData.Instance.musicPerZone[ZoneManagerHelpData.Instance.currentZoneCheck.id].levelAmbience));
            }
            else
            {
                forestSounds.volume = 0;
                normalAmbienceLevel.volume = 0;
            }
        }
    }

    public void MuteUnmuteSFX()
    {
        PlaySound(Sounds.ButtonPressUI);

        muteSFX = !muteSFX;

        if (muteSFX)
        {
            UIManager.Instance.SetSFXOffIcons();

            //SFXToggle.isOn = false;

            audioSourceSFX.volume = 0;
        }
        else
        {
            //SFXToggle.isOn = true;
            UIManager.Instance.SetSFXOnIcons();

            audioSourceSFX.volume = 1;
        }
    }

    public void CancelLeantweensSound()
    {
        LeanTween.cancel(normalAmbienceLevel.gameObject);
        LeanTween.cancel(forestSounds.gameObject);
    }
    public void CancelCoRoutinesSound()
    {
        StopAllCoroutines();
    }
}
