using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerLevelManager : MonoBehaviour
{
    public static TimerLevelManager instance;

    public GameObject timerObject;
    public Transform timerParent;

    //public float timeToPingPong;
    public float extraTimeOnLose;

    public SpriteRenderer timeManagerRenderer;

    [Header("Resetable Data")]
    //public GameObject summonedTimerObject;
    public TimerObjectRefrencer summonedTimerObject;
    public bool isActive;
    public int currentIndex;
    public float currentTime;
    public float timeBetweenIntervals;
    public List<SpriteRenderer> tileRenderes;

    //public List<SpriteRenderer> ToPingPongColor;

    IEnumerator loseLevenIenum;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        isActive = false;

        timeManagerRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isActive)
        {
            if(currentIndex < 8)
            {
                currentTime += Time.deltaTime;

                if (currentTime > timeBetweenIntervals)
                {
                    currentTime = 0;

                    TweenTile(currentIndex);

                    currentIndex++;

                    if (currentIndex == 6)
                    {
                        BeginColorTween();
                    }

                    if (currentIndex == 8)
                    {
                        loseLevenIenum = LostTimedLevel();
                        StartCoroutine(loseLevenIenum);
                    }
                }
            }


            //if (currentIndex >= 6)
            //{
            //    //PingPongColor();
            //}

            //if (currentIndex > 6)
            //{
            //    PingPongColorSpecificIndex(currentIndex);
            //}

        }

    }

    private IEnumerator LostTimedLevel()
    {
        yield return new WaitForSeconds(timeBetweenIntervals + extraTimeOnLose);
        GameManager.Instance.LoseTimedLevel();

    }
    public void InitTimer()
    {
        timeBetweenIntervals = GameManager.Instance.currentLevel.timeForLevel / 8;

        summonedTimerObject = Instantiate(timerObject, timerParent).GetComponent<TimerObjectRefrencer>();
        //TimerObjectRefrencer summonedTimer = summonedTimerObject.GetComponent<TimerObjectRefrencer>();

        //ToPingPongColor.Add(summonedTimer.sandClockRenderer);
        //ToPingPongColor.AddRange(summonedTimer.tileRenderes);

    }

    public void StartTimer()
    {
        tileRenderes.AddRange(summonedTimerObject.tileRenderes);

        isActive = true;

        currentTime = timeBetweenIntervals;
    }

    private void TweenTile(int currentIndex)
    {
        //ToPingPongColor.Add(tileRenderes[currentIndex]);

        LeanTween.value(tileRenderes[currentIndex].gameObject, 0f, 1, 0.7f).setOnUpdate((float val) =>
        {
            SpriteRenderer renderer = tileRenderes[currentIndex];
            Color newColor = renderer.color;
            newColor.a = val;
            renderer.color = newColor;
        });

    }
    //private void PingPongColor()
    //{
    //    for (int i = 0; i < ToPingPongColor.Count; i++)
    //    {
    //        ColorPingPongEffect effect = ToPingPongColor[i].GetComponent<ColorPingPongEffect>();
    //        effect.PingPongColorNormal(timeManagerRenderer.color);
    //    }
    //}
    //private void PingPongColorSpecificIndex(int index)
    //{
    //    ColorPingPongEffect effect = ToPingPongColor[index].GetComponent<ColorPingPongEffect>();
    //    effect.TweenPingPongColor(targetColor);
    //}

    public void DeactivateAll()
    {

        for (int i = 0; i < tileRenderes.Count; i++)
        {
            LeanTween.cancel(tileRenderes[i].gameObject);
        }

        Destroy(summonedTimerObject.gameObject);

        tileRenderes.Clear();

        isActive = false;

        currentIndex = 0;
        currentTime = 0;
        timeBetweenIntervals = 0;

        //ToPingPongColor.Clear();

        LeanTween.cancel(gameObject);
    }

    public void DeactivateTimer()
    {
        isActive = false;

        if(loseLevenIenum != null)
        {
            StopCoroutine(loseLevenIenum);
            loseLevenIenum = null;
        }
    }
    void BeginColorTween()
    {
        summonedTimerObject.GetComponent<Animator>().SetTrigger("ColorTween");

        //LeanTween.value(gameObject, Color.white, targetColor, timeToPingPong).setLoopPingPong();
    }
}
