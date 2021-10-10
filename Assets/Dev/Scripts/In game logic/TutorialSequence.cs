using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

[System.Serializable]
public class pieceDataStruct
{
    public PieceColor colorOfPieceRight;
    public PieceSymbol symbolOfPieceRight;
    public PieceColor colorOfPieceLeft;
    public PieceSymbol symbolOfPieceLeft;
}

[System.Serializable]
public class Sequence
{
    public int levelTutorialIndex;
    public int EndPhaseID;
    public OutLineData[] cellOutlines;
    public Phase[] phase;
    public GameObject[] screens;

    //public Sprite[] sprites;
}

[System.Serializable]
public class Phase
{
    public int phaseID;
    public bool isClipPhase, isBoardPhase;
    public bool dealPhase;

    public int[] unlockedClips;
    public int unlockedBoardCells = -1;

    public int[] targetCells;

    public int[] targetSlices;

}
[System.Serializable]
public class OutLineData
{
    public int cellIndex;
    public bool right;
}

public class TutorialSequence : MonoBehaviour
{
    public static TutorialSequence Instacne;
    public int currentPhaseInSequence;

    public Sequence[] levelSequences;

    public bool duringSequence;

    public RawImage maskImage;

    //public RenderTexture tempToWorkOn;

    public Texture currentTex;

    public List<GameObject> activatedHeighlights;

    
    public List<ParticleSystem> activatedBoardParticles;

    public Camera secondCam;

    public bool level3TurorialFlag;

    private void Start()
    {
        Instacne = this;
        //maskImage.gameObject.SetActive(false);
        activatedHeighlights = new List<GameObject>();
        activatedBoardParticles = new List<ParticleSystem>();
    }

    public void StartSequence(int sequenceNum)
    {
        if (!GameManager.Instance.isDisableTutorials)
        {
            UIManager.Instance.tutorialCanvas.SetActive(true);

            DisplayTutorialScreens();
            OutlineInstantiate();
            UIManager.Instance.dealButton.interactable = false;

            currentPhaseInSequence = 0;
            duringSequence = true;

            for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
            {
                Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

                //for (int k = 0; k < levelSequences[levelNum - 1].phase[currentPhaseInSequence].unlockedClips.Length; k++)
                //{
                if (levelSequences[sequenceNum - 1].phase[currentPhaseInSequence].unlockedClips.Contains(i)/*[k]*/)
                {
                    p.isTutorialLocked = false;
                }
                else
                {
                    p.isTutorialLocked = true;
                }
                //}
            }

        }
    }

    public void DisableTutorialSequence()
    {
        UIManager.Instance.tutorialCanvas.SetActive(false);

        UIManager.Instance.dealButton.interactable = true;

        currentPhaseInSequence = 0;
        duringSequence = false;

        //for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        //{
        //    Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

        //    //for (int k = 0; k < levelSequences[levelNum - 1].phase[currentPhaseInSequence].unlockedClips.Length; k++)
        //    //{
        //    if (levelSequences[levelNum - 1].phase[currentPhaseInSequence].unlockedClips.Contains(i)/*[k]*/)
        //    {
        //        p.isTutorialLocked = false;
        //    }
        //    else
        //    {
        //        p.isTutorialLocked = true;
        //    }
        //    //}
        //}
    }

    private void DisplayTutorialScreens()
    {
        foreach (GameObject go in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens) /// THESE ARE ALL THE TEXT POPUPS
        {
            go.SetActive(false);
        }

        StartCoroutine(SelectReleventHeighlights(0));

        //maskImage.sprite = levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].sprites[0]; /// NEW
    }

    [ContextMenu("Render Now")]
    public void toTexture()
    {
        if(secondCam.targetTexture.width != Display.main.systemWidth || secondCam.targetTexture.height != Display.main.systemHeight)
        {
            StartCoroutine(RecreateRenderTexture());
        }
        else
        {
            Texture2D texture = new Texture2D(Display.main.systemWidth, Display.main.systemHeight, TextureFormat.ARGB32, false);

            Graphics.CopyTexture(secondCam.targetTexture, texture);
            
            maskImage.texture = texture;

            maskImage.gameObject.SetActive(true);
        }
    }

    public IEnumerator RecreateRenderTexture()
    {
        secondCam.targetTexture.Release();
        yield return null;

        //tempToWorkOn.width = Screen.width;
        //tempToWorkOn.height = Screen.height;

        //float deltaW = Screen.width;
        //float deltaH = Screen.height;
        secondCam.targetTexture = new RenderTexture(Display.main.systemWidth, Display.main.systemHeight, 24);
        secondCam.Render();

        yield return null;

        //ScalableBufferManager.ResizeBuffers(deltaW, deltaH);
        //tempToWorkOn.Create();
        yield return null;
        yield return null;
        toTexture();
    }
    public IEnumerator SelectReleventHeighlights(int index)
    {
        UIManager.Instance.dealButtonHeighlight.SetActive(false);

        foreach (GameObject go in activatedHeighlights)
        {
            go.SetActive(false);
        }

        foreach (ParticleSystem go in activatedBoardParticles)
        {
            go.gameObject.SetActive(false);
        }

        activatedHeighlights.Clear();
        activatedBoardParticles.Clear();

        ///// Maybe do this part below better
        levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[index].SetActive(true);

        if(levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[index].dealPhase)
        {
            //for (int i = 0; i < UIManager.Instance.dealButton.transform.childCount; i++)
            //{
            //    if (UIManager.Instance.dealButton.transform.GetChild(i).CompareTag("Tile Hole"))
            //    {
            //        UIManager.Instance.dealButton.transform.GetChild(i).gameObject.SetActive(true);

            //        activatedHeighlights.Add(UIManager.Instance.dealButton.transform.GetChild(i).gameObject);
            //    }

            //}

            UIManager.Instance.dealButtonHeighlight.SetActive(true);
            activatedHeighlights.Add(UIManager.Instance.dealButtonHeighlight.gameObject);
            
        }

        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[index].unlockedClips.Length > 0)
        {
            foreach (int i in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[index].unlockedClips)
            {
                for (int k = 0; k < GameManager.Instance.clipManager.slots[i].childCount; k++)
                {
                    if (GameManager.Instance.clipManager.slots[i].GetChild(k).CompareTag("Tile Hole"))
                    {
                        GameManager.Instance.clipManager.slots[i].GetChild(k).gameObject.SetActive(true);

                        activatedHeighlights.Add(GameManager.Instance.clipManager.slots[i].GetChild(k).gameObject);
                    }
                }
            }
        }

        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[index].targetCells.Length > 0)
        {
            for (int i = 0; i < levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[index].targetCells.Length; i++)
            {
                int num = levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[index].targetCells[i];
                Debug.Log(num + "yasbfyabfyasb");


                for (int k = 0; k < ConnectionManager.Instance.cells[num].transform.childCount; k++)
                {
                    if (ConnectionManager.Instance.cells[num].transform.GetChild(k).CompareTag("Tile Hole"))
                    {
                        ConnectionManager.Instance.cells[num].transform.GetChild(k).gameObject.SetActive(true);

                        if (num != levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[index].unlockedBoardCells)
                        {
                            ConnectionManager.Instance.cells[num].highlightParticle.gameObject.SetActive(true);
                            activatedBoardParticles.Add(ConnectionManager.Instance.cells[num].highlightParticle);
                        }

                        activatedHeighlights.Add(ConnectionManager.Instance.cells[num].transform.GetChild(k).gameObject);
                    }
                }
            }
        }

        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[index].targetSlices.Length > 0)
        {
            foreach (int i in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[index].targetSlices)
            {
                for (int k = 0; k < GameManager.Instance.sliceManager.sliceSlots[i].transform.childCount; k++)
                {
                    if (GameManager.Instance.sliceManager.sliceSlots[i].transform.GetChild(k).CompareTag("Tile Hole"))
                    {
                        GameManager.Instance.sliceManager.sliceSlots[i].transform.GetChild(k).gameObject.SetActive(true);

                        activatedHeighlights.Add(GameManager.Instance.sliceManager.sliceSlots[i].transform.GetChild(k).gameObject);
                    }
                }
            }
        }


        yield return new WaitForEndOfFrame();
        toTexture();
    }
    public void IncrementCurrentPhaseInSequence()
    {
        currentPhaseInSequence++;

        if(levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[currentPhaseInSequence])
        {
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[currentPhaseInSequence - 1].SetActive(false);
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[currentPhaseInSequence].SetActive(true);
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[currentPhaseInSequence].transform.GetChild(0).gameObject.SetActive(true);
        }


        if (currentPhaseInSequence >= levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].EndPhaseID)
        {

            maskImage.gameObject.SetActive(false);
            duringSequence = false;
            Debug.Log("Phases are done!");
            //Invoke("UnlockAll", 2);

            UnlockAll();


            Invoke("DeactivateTutorialScreens", 0.1f);

            return;
        }
        else
        {
            StartCoroutine(SelectReleventHeighlights(currentPhaseInSequence));
            ChangePhase();
        }

        ///maskImage.sprite = levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].sprites[currentPhaseInSequence]; /// NEW

    }

    //public void CheckContinuedTutorials()
    //{
    //    //// VERY TEMPORARY! CREATE FLAG SYSTEM!
    //    if(level3TurorialFlag)
    //    {
    //        StartSequence(7);
    //    }
    //}

    public void ChangePhase()
    {
        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[currentPhaseInSequence].isClipPhase)
        {
            ClipPhaseLogic();
        }

        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[currentPhaseInSequence].isBoardPhase)
        {
            BoardPhaseLogic();
        }

        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[currentPhaseInSequence].dealPhase)
        {
            DealPhaseLogic();
        }
    }

    public void ClipPhaseLogic()
    {
            UIManager.Instance.dealButton.interactable = false;

            foreach (Cell c in ConnectionManager.Instance.cells)
            {
                if (c.isFull)
                {
                    c.pieceHeld.isTutorialLocked = true;
                }
            }

            for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
            {
                Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

                //for (int k = 0; k < levelSequences[GameManager.Instance.currentLevel.levelNum - 1].phase[currentPhaseInSequence].unlockedClips.Length; k++)
                //{
                if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[currentPhaseInSequence].unlockedClips.Contains(i)/*[k]*/)
                {
                    p.isTutorialLocked = false;
                }
                else
                {
                    p.isTutorialLocked = true;
                }
                //}
            }
    }
    public void BoardPhaseLogic()
    {
        UIManager.Instance.dealButton.interactable = false;

        for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

            p.isTutorialLocked = true;
        }


        foreach (Cell c in ConnectionManager.Instance.cells)
        {
            if (c.isFull)
            {
                c.pieceHeld.isTutorialLocked = true;
                //int length = levelSequences[GameManager.Instance.currentLevel.levelNum - 1].phase[currentPhaseInSequence].unlockedBoardCells.Length;
                //for (int i = 0; i < length; i++)
                //{
                if (c.cellIndex == levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[currentPhaseInSequence].unlockedBoardCells/*[i]*/)
                {
                    c.pieceHeld.isTutorialLocked = false;
                }
                //}
            }
        }
    }
    public void DealPhaseLogic()
    {
        UIManager.Instance.dealButton.interactable = true;

        for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

            p.isTutorialLocked = true;
        }

        foreach (Cell c in ConnectionManager.Instance.cells)
        {
            if (c.isFull)
            {
                c.pieceHeld.isTutorialLocked = true;
            }
        }
    }
    public void OutlineInstantiate()
    {
        foreach (OutLineData old in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].cellOutlines)
        {
            if (old.right)
            {
                Instantiate(ConnectionManager.Instance.cells[old.cellIndex].outlinedSpriteRight, ConnectionManager.Instance.cells[old.cellIndex].transform);
            }
            else
            {
                Instantiate(ConnectionManager.Instance.cells[old.cellIndex].outlinedSpriteLeft, ConnectionManager.Instance.cells[old.cellIndex].transform);
            }
        }
    }

    public void UnlockAll()
    {
        //foreach (GameObject go in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens)
        //{
        //    go.SetActive(false);
        //}

        UIManager.Instance.dealButton.interactable = true;

        foreach (Cell c in ConnectionManager.Instance.cells)
        {
            if (c.isFull)
            {
                c.pieceHeld.isTutorialLocked = false;
            }
        }

        for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

            p.isTutorialLocked = false;
        }

        UIManager.Instance.dealButtonHeighlight.SetActive(false);

        foreach (GameObject go in activatedHeighlights)
        {
            go.SetActive(false);
        }

        foreach (ParticleSystem go in activatedBoardParticles)
        {
            go.gameObject.SetActive(false);
        }

        activatedHeighlights.Clear();
        activatedBoardParticles.Clear();
    }

    public void DeactivateTutorialScreens()
    {
        foreach (GameObject go in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens)
        {
            if (go.activeInHierarchy)
            {
                //StartCoroutine(FadeImage(go, 2f));
                FadeImage(go, 2f,false);

                foreach (TMP_Text child in go.GetComponentsInChildren<TMP_Text>())
                {
                    FadeImage(child.gameObject, 2f, true);
                }
            }
        }
    }

    private void FadeImage(GameObject toFade, float speed, bool isText)
    {
        if (!isText)
        {
            LeanTween.value(toFade, 1, 0, speed).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
            {
                Image sr = toFade.GetComponent<Image>();
                Color newColor = sr.color;
                newColor.a = val;
                sr.color = newColor;
            });

        }
        else
        {
            LeanTween.value(toFade, 1, 0, speed).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float val) =>
            {
                TMP_Text sr = toFade.GetComponent<TMP_Text>();
                Color newColor = sr.color;
                newColor.a = val;
                sr.color = newColor;
            });
        }

        StartCoroutine(DisableFadeImage(toFade, speed, isText));
    }

    IEnumerator DisableFadeImage(GameObject go, float time, bool isText)
    {
        if (isText)
        {
            yield return new WaitForSeconds(time);
            //go.SetActive(false);
            TMP_Text sr = go.GetComponent<TMP_Text>();
            sr.color = new Color(0.2f, 0.2f, 0.2f, 1); ////////////VERY TEMPORARY
        }
        else
        {
            yield return new WaitForSeconds(time);
            go.SetActive(false);
            Image sr = go.GetComponent<Image>();

            sr.color = Color.white;
        }

    }
    public void TurnOnTutorialScreensAfterOptions()
    {
        if(currentPhaseInSequence < levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].EndPhaseID)
        {
            if(currentPhaseInSequence > 0)
            {
                levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[currentPhaseInSequence - 1].SetActive(false);
            }
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].screens[currentPhaseInSequence].SetActive(true);
        }
    }
}

