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
public class SpecificTutorialsData
{
    public int EndPhaseID;
    public SpecificTutorialsPhaseData[] phase;
    public GameObject[] screens;

    //public Sprite[] sprites;
}

[System.Serializable]
public class SpecificTutorialsPhaseData
{
    public int phaseID;
    public float timeDelay;
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

    public GameObject tutorialHandPrefabMove;
    public GameObject tutorialHandPrefabTap;
    public Vector3 tutorialHandPosDealButton, tutorialHandRotationDealButton;
    public float tutorialHandMoveSpeed, tutorialHandTapSpeed;

    public GameObject currentlyActiveTutorialHand;

    public int currentPhaseInSequence;

    public Sequence[] levelSequences;
    public SpecificTutorialsData[] specificTutorials;

    public bool duringSequence;

    public RawImage maskImage;

    //public RenderTexture tempToWorkOn;

    public Texture currentTex;

    public List<GameObject> activatedHeighlights;

    
    public List<ParticleSystem> activatedBoardParticles;

    public Camera secondCam;

    private void Start()
    {
        Instacne = this;
        //maskImage.gameObject.SetActive(false);
        activatedHeighlights = new List<GameObject>();
        activatedBoardParticles = new List<ParticleSystem>();
    }

    public void StartTutorialLevelSequence(int sequenceNum) /// ONLY for level tutorials
    {
        if (!GameManager.Instance.isDisableTutorials)
        {
            foreach (GameObject go in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens) /// THESE ARE ALL THE TEXT POPUPS
            {
                go.SetActive(false);
            }

            GameManager.Instance.powerupManager.PowerupButtonsActivation(false);

            UIManager.Instance.tutorialCanvas.SetActive(true);

            //DisplayTutorialScreens();

            OutlineInstantiate();
            UIManager.Instance.dealButton.interactable = false;

            currentPhaseInSequence = -1; /// it goes up by one in function so it actually starts at 0
            duringSequence = true;
            IncrementCurrentPhaseInSequence();

            //for (int i = 0; i < GameManager.Instance.clipManager.slots.Length; i++)
            //{
            //    Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();

            //    //for (int k = 0; k < levelSequences[levelNum - 1].phase[currentPhaseInSequence].unlockedClips.Length; k++)
            //    //{
            //    if (levelSequences[sequenceNum].phase[currentPhaseInSequence].unlockedClips.Contains(i)/*[k]*/)
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
    }

    public void DisplaySpecificTutorialSequence(int tutorialIndex) /// for anything not levels - like loot tutorial or crafting tutorial
    {
        if (!GameManager.Instance.isDisableTutorials)
        {
            GameManager.Instance.powerupManager.PowerupButtonsActivation(false);

            UIManager.Instance.tutorialCanvas.SetActive(true);

            UIManager.Instance.dealButton.interactable = false;

            specificTutorials[tutorialIndex].screens[0].SetActive(true);

            //currentPhaseInSequence = 0;
            //duringSequence = true;
            // Might need this later for other types of tutorials
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

    //private void DisplayTutorialScreens()
    //{
    //    foreach (GameObject go in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens) /// THESE ARE ALL THE TEXT POPUPS
    //    {
    //        go.SetActive(false);
    //    }

    //    StartCoroutine(SelectReleventHeighlights(0));

    //    //maskImage.sprite = levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].sprites[0]; /// NEW
    //}

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
        levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[index].SetActive(true);

        if(levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[index].dealPhase)
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

        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[index].unlockedClips.Length > 0)
        {
            foreach (int i in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[index].unlockedClips)
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

        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[index].targetCells.Length > 0)
        {
            for (int i = 0; i < levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[index].targetCells.Length; i++)
            {
                int num = levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[index].targetCells[i];
                Debug.Log(num + "yasbfyabfyasb");


                for (int k = 0; k < ConnectionManager.Instance.cells[num].transform.childCount; k++)
                {
                    if (ConnectionManager.Instance.cells[num].transform.GetChild(k).CompareTag("Tile Hole"))
                    {
                        ConnectionManager.Instance.cells[num].transform.GetChild(k).gameObject.SetActive(true);

                        if (num != levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[index].unlockedBoardCells)
                        {
                            ConnectionManager.Instance.cells[num].highlightParticle.gameObject.SetActive(true);
                            activatedBoardParticles.Add(ConnectionManager.Instance.cells[num].highlightParticle);
                        }

                        activatedHeighlights.Add(ConnectionManager.Instance.cells[num].transform.GetChild(k).gameObject);
                    }
                }
            }
        }

        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[index].targetSlices.Length > 0)
        {
            foreach (int i in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[index].targetSlices)
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
        if (currentlyActiveTutorialHand)
        {
            Destroy(currentlyActiveTutorialHand.gameObject);
        }

        currentPhaseInSequence++;

        if(levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequence])
        {
            if(currentPhaseInSequence > 0)
            {
                levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequence - 1].SetActive(false);
            }

            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequence].SetActive(true);
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequence].transform.GetChild(0).gameObject.SetActive(true);
        }


        if (currentPhaseInSequence >= levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].EndPhaseID)
        {
            TutorialSaveData.Instance.completedTutorialLevelId.Add(GameManager.Instance.currentLevel.levelNum);
            TutorialSaveData.Instance.SaveTutorialSaveData();

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
        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequence].isClipPhase)
        {
            ClipPhaseLogic();
        }

        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequence].isBoardPhase)
        {
            BoardPhaseLogic();
        }

        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequence].dealPhase)
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
                if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequence].unlockedClips.Contains(i)/*[k]*/)
                {
                    p.isTutorialLocked = false;
                }
                else
                {
                    p.isTutorialLocked = true;
                }
                //}
            }

        int clipID = levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequence].unlockedClips[0];
        int cellID = levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequence].targetCells[0];

        DisplayTutorialHandClipToBoardCell(clipID, cellID);
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
                if (c.cellIndex == levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequence].unlockedBoardCells/*[i]*/)
                {
                    c.pieceHeld.isTutorialLocked = false;
                }
                //}
            }
        }

        int cellID1 = levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequence].unlockedBoardCells;
        int cellID2 = -1;

        for (int i = 0; i < levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequence].targetCells.Length; i++)
        {
            if(cellID1 != levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequence].targetCells[i])
            {
                cellID2 = levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequence].targetCells[i];
                break;
            }
        }

        if(cellID2 == -1)
        {
            Debug.LogError("Something bad happened here!");
            return;
        }

        DisplayTutorialHandBoardCellToBoardCell(cellID1, cellID2);
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

        DisplayTutorialHandTap(tutorialHandPosDealButton, tutorialHandRotationDealButton);
    }
    public void OutlineInstantiate()
    {
        foreach (OutLineData old in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].cellOutlines)
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

        GameManager.Instance.powerupManager.PowerupButtonsActivation(true);
    }

    public void DeactivateTutorialScreens()
    {
        foreach (GameObject go in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens)
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
    public void TurnOnTutorialScreensAfterRestart()
    {
        if(currentPhaseInSequence < levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].EndPhaseID)
        {
            if(currentPhaseInSequence > 0)
            {
                levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequence].SetActive(false);
            }
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequence].SetActive(true);
        }
    }

    public void DisplayTutorialHandClipToBoardCell(int clipIndex, int CellIndex)
    {
        Vector3 pos = GameManager.Instance.clipManager.slots[clipIndex].transform.position;
        pos.z = -0.3f;
        
        Vector3 targetPos = ConnectionManager.Instance.cells[CellIndex].transform.position;
        targetPos.z = -0.3f;

        GameObject go = Instantiate(tutorialHandPrefabMove, pos, Quaternion.identity);
        currentlyActiveTutorialHand = go;

        LeanTween.move(go, targetPos, tutorialHandMoveSpeed).setEase(LeanTweenType.easeInOutQuad).setLoopClamp(); // animate
    }

    public void DisplayTutorialHandBoardCellToBoardCell(int CellIndexFrom, int CellIndexTo)
    {
        Vector3 pos = ConnectionManager.Instance.cells[CellIndexFrom].transform.position;
        pos.z = -0.3f;

        GameObject go = Instantiate(tutorialHandPrefabMove, pos, Quaternion.identity);
        currentlyActiveTutorialHand = go;

        Vector3 targetPos = ConnectionManager.Instance.cells[CellIndexTo].transform.position;
        targetPos.z = -0.3f;

        LeanTween.move(go, targetPos, tutorialHandMoveSpeed).setEase(LeanTweenType.easeInOutQuad).setLoopClamp(); // animate
    }

    public void DisplayTutorialHandTap(Vector3 position, Vector3 rotation)
    {
        Vector3 pos = position;
        pos.z = -0.3f;

        GameObject go = Instantiate(tutorialHandPrefabTap, pos, Quaternion.Euler(rotation));
        currentlyActiveTutorialHand = go;

        LeanTween.rotate(go, new Vector3(rotation.x, rotation.y, rotation.z + 5), tutorialHandTapSpeed).setEase(LeanTweenType.linear).setLoopPingPong();


    }

}

