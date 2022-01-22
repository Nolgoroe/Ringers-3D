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
    public bool doFadeInEnd;
    public int levelTutorialIndex;
    public int EndPhaseID;
    //public OutLineData[] cellOutlines; // old lock system
    public Phase[] phase;
    public float waitTimeEndPhase;
    public GameObject[] screens;

    //public Sprite[] sprites;
}


[System.Serializable]
public class Phase
{
    public int phaseID;
    public bool isClipPhase, isBoardPhase, isPowerupPhase, isSingleCellPhase, isSingleSlice, isHubButtonPhase, isOpenInventoryPhase, isPotionTabPhase ,isEmptyTouchPhase, isBrewPhase, isBrewDisplayMaterials;
    public bool dealPhase;

    public int[] unlockedPowerups;
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

public enum SpecificTutorialsEnum
{
    None,
    lootTutorial,
    SwapSidesTutorial,
    TileBombTutorial,
    SliceBombTutorial,
    JokerTutorial,
    PotionCraft,


}
public class TutorialSequence : MonoBehaviour
{
    public static TutorialSequence Instacne;

    public GameObject tutorialHandPrefabMove;
    public GameObject tutorialHandPrefabTap;
    public Vector3 tutorialHandPosClip,tutorialHandPosDealButton, tutorialHandRotationDealButton, tutorialHandPosPowerupOffset;
    public float tutorialHandMoveSpeed, tutorialHandTapSpeed;

    public GameObject currentlyActiveTutorialHand;

    public int currentPhaseInSequenceLevels;
    public int currentPhaseInSequenceSpecific;

    public Sequence[] levelSequences;
    public Sequence[] specificTutorials;

    public bool duringSequence;

    public SpriteMask maskImage;

    //public RenderTexture tempToWorkOn;

    public Texture currentTex;

    public List<GameObject> activatedHeighlights;

    
    public List<ParticleSystem> activatedBoardParticles;

    public Camera secondCam;

    public SpecificTutorialsEnum currentSpecificTutorial;

    public List<GameObject> screensDeactivateOnTouch;

    public Transform handPosToHub, handPosOpenInventory, handPosChangeTab, handPosBrewButton;
    private void Start()
    {
        Instacne = this;
        //maskImage.gameObject.SetActive(false);
        activatedHeighlights = new List<GameObject>();
        activatedBoardParticles = new List<ParticleSystem>();
    }

    public void StartTutorialLevelSequence() /// ONLY for level tutorials
    {
        if (!GameManager.Instance.isDisableTutorials)
        {
            DeactivateAllTutorialScreens();

            GameManager.Instance.powerupManager.PowerupButtonsActivation(false);

            UIManager.Instance.tutorialCanvasParent.SetActive(true);
            //UIManager.Instance.tutorialCanvasHolesParent.SetActive(true);
            UIManager.Instance.tutorialCanvasLevels.SetActive(true);

            //DisplayTutorialScreens();

            //OutlineInstantiate(); // old lock system
            UIManager.Instance.dealButton.interactable = false;

            currentPhaseInSequenceLevels = -1; /// it goes up by one in function so it actually starts at 0
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

    public IEnumerator DisplaySpecificTutorialSequence() /// for anything not levels - like loot tutorial, crafting tutorial or powerups
    {
        yield return new WaitForEndOfFrame();

        if (!GameManager.Instance.isDisableTutorials)
        {

            DeactivateAllTutorialScreens();

            //GameManager.Instance.powerupManager.PowerupButtonsActivation(false);

            UIManager.Instance.tutorialCanvasParent.SetActive(true);
            //UIManager.Instance.tutorialCanvasHolesParent.SetActive(true);
            UIManager.Instance.tutorialCanvasSpecific.SetActive(true);
            UIManager.Instance.dealButton.interactable = false;

            specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens[0].transform.parent.gameObject.SetActive(true);
            specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens[0].SetActive(true);

            currentPhaseInSequenceSpecific = -1; /// it goes up by one in function so it actually starts at 0
            duringSequence = true;
            IncrementPhaseInSpecificTutorial();

            //currentPhaseInSequence = 0;
            //duringSequence = true;
            // Might need this later for other types of tutorials
        }
    }

    public void DisableTutorialSequence()
    {
        UIManager.Instance.tutorialCanvasLevels.SetActive(false);

        UIManager.Instance.dealButton.interactable = true;

        currentPhaseInSequenceLevels = 0;
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
            //StartCoroutine(RecreateRenderTexture());
            RecreateRenderTexture();
        }
        else
        {
            //secondCam.Render();

            Texture2D texture = new Texture2D(Display.main.systemWidth, Display.main.systemHeight, TextureFormat.ARGB32, false);
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            Graphics.CopyTexture(secondCam.targetTexture, texture);
            
            maskImage.sprite = sprite;

            maskImage.gameObject.SetActive(true);
        }
    }

    public void RecreateRenderTexture()
    {
        Debug.LogError("IN HERE NOW");
        //secondCam.targetTexture.Release();
        //yield return new WaitForEndOfFrame();

        //tempToWorkOn.width = Screen.width;
        //tempToWorkOn.height = Screen.height;

        //float deltaW = Screen.width;
        //float deltaH = Screen.height;
        secondCam.targetTexture = new RenderTexture(Display.main.systemWidth, Display.main.systemHeight, 24);
        secondCam.Render();

        //ScalableBufferManager.ResizeBuffers(deltaW, deltaH);
        //tempToWorkOn.Create();
        //yield return new WaitForEndOfFrame();
        toTexture();
    }
    public IEnumerator SelectReleventHeighlights(int index, bool isSpecific) //// PASS LIST AND INDEX INTO FUNCTION TO MAKE THIS SHIT CODE BETTER
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

        if (isSpecific)
        {
            ///// Maybe do this part below better
            specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens[index].SetActive(true);

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].dealPhase)
            {
                UIManager.Instance.dealButtonHeighlight.SetActive(true);
                //UIManager.Instance.dealButtonHeighlight.GetComponent<testScreenResolutionFix>().DoMove();
                activatedHeighlights.Add(UIManager.Instance.dealButtonHeighlight.gameObject);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].unlockedPowerups.Length > 0)
            {
                foreach (int i in specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].unlockedPowerups)
                {
                    for (int k = 0; k < GameManager.Instance.powerupManager.instnatiateZones[i].transform.childCount; k++)
                    {
                        if (GameManager.Instance.powerupManager.instnatiateZones[i].transform.GetChild(k).CompareTag("Tile Hole"))
                        {
                            GameManager.Instance.powerupManager.instnatiateZones[i].transform.GetChild(k).gameObject.SetActive(true);

                            activatedHeighlights.Add(GameManager.Instance.powerupManager.instnatiateZones[i].transform.GetChild(k).gameObject);
                        }
                    }
                }
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].unlockedClips.Length > 0)
            {
                foreach (int i in specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].unlockedClips)
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

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].targetCells.Length > 0)
            {
                for (int i = 0; i < specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].targetCells.Length; i++)
                {
                    int num = specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].targetCells[i];

                    for (int k = 0; k < ConnectionManager.Instance.cells[num].transform.childCount; k++)
                    {
                        if (ConnectionManager.Instance.cells[num].transform.GetChild(k).CompareTag("Tile Hole"))
                        {
                            ConnectionManager.Instance.cells[num].transform.GetChild(k).gameObject.SetActive(true);

                            if (num != specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].unlockedBoardCells)
                            {
                                ConnectionManager.Instance.cells[num].highlightParticle.gameObject.SetActive(true);
                                activatedBoardParticles.Add(ConnectionManager.Instance.cells[num].highlightParticle);
                            }

                            activatedHeighlights.Add(ConnectionManager.Instance.cells[num].transform.GetChild(k).gameObject);
                        }
                    }
                }
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].targetSlices.Length > 0)
            {
                foreach (int i in specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].targetSlices)
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

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isHubButtonPhase)
            {
                UIManager.Instance.toHubButtonHeighlight.SetActive(true);
                activatedHeighlights.Add(UIManager.Instance.toHubButtonHeighlight.gameObject);
                DisplayTutorialHandTapQuaternion(handPosToHub.position, handPosToHub.rotation, handPosToHub.localScale);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isOpenInventoryPhase)
            {
                UIManager.Instance.openInventoryButtonHeighlight.SetActive(true);
                activatedHeighlights.Add(UIManager.Instance.openInventoryButtonHeighlight.gameObject);
                DisplayTutorialHandTapQuaternion(handPosOpenInventory.position, handPosOpenInventory.rotation, handPosOpenInventory.localScale);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isPotionTabPhase)
            {
                UIManager.Instance.potionTabHeighlight.SetActive(true);
                activatedHeighlights.Add(UIManager.Instance.potionTabHeighlight.gameObject);
                DisplayTutorialHandTapQuaternion(handPosChangeTab.position, handPosChangeTab.rotation, handPosChangeTab.localScale);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isBrewPhase)
            {
                UIManager.Instance.brewButtonHeighlight.SetActive(true);
                activatedHeighlights.Add(UIManager.Instance.brewButtonHeighlight.gameObject);
                DisplayTutorialHandTapQuaternion(handPosBrewButton.position, handPosBrewButton.rotation, handPosBrewButton.localScale);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isBrewDisplayMaterials)
            {
                foreach (GameObject g in UIManager.Instance.brewMaterialZonesHeighlights)
                {
                    g.SetActive(true);
                    activatedHeighlights.Add(g);
                }
            }
        }

        if (!isSpecific)
        {
            ///// Maybe do this part below better
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[index].SetActive(true);

            if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[index].dealPhase)
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
                //UIManager.Instance.dealButtonHeighlight.GetComponent<testScreenResolutionFix>().DoMove();
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

        currentPhaseInSequenceLevels++;

        if(levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequenceLevels])
        {
            if(currentPhaseInSequenceLevels > 0)
            {
                levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequenceLevels - 1].SetActive(false);
            }

            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequenceLevels].SetActive(true);
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequenceLevels].transform.GetChild(0).gameObject.SetActive(true);
        }


        if (currentPhaseInSequenceLevels + 1 > levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].EndPhaseID)
        {
            TutorialSaveData.Instance.completedTutorialLevelId.Add(GameManager.Instance.currentLevel.levelNum);
            //TutorialSaveData.Instance.SaveTutorialSaveData();

            maskImage.gameObject.SetActive(false);
            duringSequence = false;
            Debug.Log("Phases are done!");
            //Invoke("UnlockAll", 2);

            if (!GameManager.gameWon)
            {
                UnlockAll();
            }


            //Invoke("DeactivateTutorialScreens", 0.1f);

            if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].doFadeInEnd)
            {
                StartCoroutine(DeactivateTutorialScreens(levelSequences, GameManager.Instance.currentLevel.tutorialIndexForList, levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].waitTimeEndPhase));
            }
            else
            {
                screensDeactivateOnTouch.Add(levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequenceLevels]);
            }

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TutorialSaveData});
            return;
        }
        else
        {
            StartCoroutine(SelectReleventHeighlights(currentPhaseInSequenceLevels, false));
            ChangePhase(levelSequences, GameManager.Instance.currentLevel.tutorialIndexForList, currentPhaseInSequenceLevels);
        }

        ///maskImage.sprite = levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].sprites[currentPhaseInSequence]; /// NEW

    }

    public void IncrementPhaseInSpecificTutorial()
    {
        currentPhaseInSequenceSpecific++;

        if (currentlyActiveTutorialHand)
        {
            Destroy(currentlyActiveTutorialHand.gameObject);
        }

        if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens[currentPhaseInSequenceSpecific])
        {
            if (currentPhaseInSequenceSpecific > 0)
            {
                specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens[currentPhaseInSequenceSpecific - 1].SetActive(false);
            }

            specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens[currentPhaseInSequenceSpecific].SetActive(true);
            specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens[currentPhaseInSequenceSpecific].transform.GetChild(0).gameObject.SetActive(true);
        }

        if (currentPhaseInSequenceSpecific + 1 > specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].EndPhaseID)
        {
            //if(currentSpecificTutorial == SpecificTutorialsEnum.PotionCraft)
            //{
            //    UIManager.Instance.ItemAndForgeBagHEIGHLIGHTS.SetActive(false);
            //}

            TutorialSaveData.Instance.completedSpecificTutorialLevelId.Add((int)GameManager.Instance.currentLevel.specificTutorialEnum);
            //TutorialSaveData.Instance.completedTutorialLevelId.Add(GameManager.Instance.currentLevel.levelNum);
            //TutorialSaveData.Instance.SaveTutorialSaveData();
            currentSpecificTutorial = SpecificTutorialsEnum.None;
            maskImage.gameObject.SetActive(false);
            duringSequence = false;
            //Debug.Log("Phases are done!");
            //Invoke("UnlockAll", 2);
            activatedHeighlights.Clear();
            activatedBoardParticles.Clear();

            if (!GameManager.gameWon)
            {
                UnlockAll();
            }

            PlayerManager.Instance.checkDoAddPotionsToInventory();
            //Invoke("DeactivateTutorialScreens", 0.1f);

            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TutorialSaveData });

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].doFadeInEnd)
            {
                StartCoroutine(DeactivateTutorialScreens(specificTutorials, (int)GameManager.Instance.currentLevel.specificTutorialEnum - 1, specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].waitTimeEndPhase));
            }
            else
            {
                screensDeactivateOnTouch.Add(specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens[currentPhaseInSequenceSpecific]);
            }

            return;
        }
        else
        {
            StartCoroutine(SelectReleventHeighlights(currentPhaseInSequenceSpecific, true));
            ChangePhase(specificTutorials, (int)GameManager.Instance.currentLevel.specificTutorialEnum - 1, currentPhaseInSequenceSpecific);
        }
    }
    //public void CheckContinuedTutorials()
    //{
    //    //// VERY TEMPORARY! CREATE FLAG SYSTEM!
    //    if(level3TurorialFlag)
    //    {
    //        StartSequence(7);
    //    }
    //}

    public void ChangePhase(Sequence[] tutorialArray, int TutorialIndex, int phaseIndex)
    {
        if (tutorialArray[TutorialIndex].phase[phaseIndex].isClipPhase)
        {
            ClipPhaseLogic(tutorialArray, TutorialIndex, phaseIndex);
        }

        if (tutorialArray[TutorialIndex].phase[phaseIndex].isBoardPhase)
        {
            BoardPhaseLogic(tutorialArray, TutorialIndex, phaseIndex);
        }

        if (tutorialArray[TutorialIndex].phase[phaseIndex].dealPhase)
        {
            DealPhaseLogic();
        }

        if (tutorialArray[TutorialIndex].phase[phaseIndex].isPowerupPhase)
        {
            PowerUpPhase(tutorialArray, TutorialIndex, phaseIndex);
        }

        if (tutorialArray[TutorialIndex].phase[phaseIndex].isSingleCellPhase)
        {
            SingleCellChosenPhase(tutorialArray, TutorialIndex, phaseIndex);
        }

        if (tutorialArray[TutorialIndex].phase[phaseIndex].isSingleSlice)
        {
            SingleSliceChosenPhase(tutorialArray, TutorialIndex, phaseIndex);
        }
    }

    public void ClipPhaseLogic(Sequence[] tutorialArray, int TutorialIndex, int phaseIndex)
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
                if (tutorialArray[TutorialIndex].phase[phaseIndex].unlockedClips.Contains(i)/*[k]*/)
                {
                    p.isTutorialLocked = false;
                }
                else
                {
                    p.isTutorialLocked = true;
                }
                //}
            }

        int clipID = tutorialArray[TutorialIndex].phase[phaseIndex].unlockedClips[0];
        int cellID = tutorialArray[TutorialIndex].phase[phaseIndex].targetCells[0];

        DisplayTutorialHandClipToBoardCell(clipID, cellID);
    }
    public void BoardPhaseLogic(Sequence[] tutorialArray, int TutorialIndex, int phaseIndex)
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
                if (c.cellIndex == tutorialArray[TutorialIndex].phase[phaseIndex].unlockedBoardCells/*[i]*/)
                {
                    c.pieceHeld.isTutorialLocked = false;
                }
                //}
            }
        }

        int cellID1 = tutorialArray[TutorialIndex].phase[phaseIndex].unlockedBoardCells;
        int cellID2 = -1;

        for (int i = 0; i < tutorialArray[TutorialIndex].phase[phaseIndex].targetCells.Length; i++)
        {
            if(cellID1 != tutorialArray[TutorialIndex].phase[phaseIndex].targetCells[i])
            {
                cellID2 = tutorialArray[TutorialIndex].phase[phaseIndex].targetCells[i];
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

        DisplayTutorialHandTap(tutorialHandPosDealButton, tutorialHandRotationDealButton, Vector3.one);
    }

    public void PowerUpPhase(Sequence[] tutorialArray, int TutorialIndex, int phaseIndex)
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
            }
        }

        int powerupZoneID = tutorialArray[TutorialIndex].phase[phaseIndex].unlockedPowerups[0];

        Vector3 handPosPowerup = GameManager.Instance.powerupManager.instnatiateZones[powerupZoneID].transform.position;

        Debug.Log(handPosPowerup);

        DisplayTutorialHandTap(handPosPowerup + tutorialHandPosPowerupOffset, tutorialHandRotationDealButton, Vector3.one);
    }

    public void SingleCellChosenPhase(Sequence[] tutorialArray, int TutorialIndex, int phaseIndex)
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
            }
        }

        int cellID = tutorialArray[TutorialIndex].phase[phaseIndex].unlockedBoardCells;
        Vector3 pos = ConnectionManager.Instance.cells[cellID].transform.position;

        DisplayTutorialHandTap(pos,tutorialHandRotationDealButton, Vector3.one);
    }

    public void SingleSliceChosenPhase(Sequence[] tutorialArray, int TutorialIndex, int phaseIndex)
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
            }
        }

        int SliceID = tutorialArray[TutorialIndex].phase[phaseIndex].targetSlices[0];
        Vector3 pos = GameManager.Instance.sliceManager.sliceSlots[SliceID].transform.position;

        DisplayTutorialHandTap(pos,tutorialHandRotationDealButton, Vector3.one);
    }

    //public void OutlineInstantiate() // old lock system
    //{
    //    foreach (OutLineData old in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].cellOutlines)
    //    {
    //        if (old.right)
    //        {
    //            Instantiate(ConnectionManager.Instance.cells[old.cellIndex].outlinedSpriteRight, ConnectionManager.Instance.cells[old.cellIndex].transform);
    //        }
    //        else
    //        {
    //            Instantiate(ConnectionManager.Instance.cells[old.cellIndex].outlinedSpriteLeft, ConnectionManager.Instance.cells[old.cellIndex].transform);
    //        }
    //    }
    //}

    public void UnlockAll()
    {
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

    public IEnumerator DeactivateTutorialScreens(Sequence[] tutorialArray, int index, float timeToWaitTillFade)
    {
        yield return new WaitForSeconds(timeToWaitTillFade);
        foreach (GameObject go in tutorialArray[index].screens)
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
        if(currentPhaseInSequenceLevels < levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].EndPhaseID)
        {
            if(currentPhaseInSequenceLevels > 0)
            {
                levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequenceLevels].SetActive(false);
            }
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequenceLevels].SetActive(true);
        }
    }

    public void DisplayTutorialHandClipToBoardCell(int clipIndex, int CellIndex)
    {
        Vector3 pos = GameManager.Instance.clipManager.slots[clipIndex].transform.position + tutorialHandPosClip;
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

    public void DisplayTutorialHandTap(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        Vector3 pos = position;
        pos.z = -0.3f;

        GameObject go = Instantiate(tutorialHandPrefabTap, pos, Quaternion.Euler(rotation));
        currentlyActiveTutorialHand = go;

        go.transform.localScale = scale;

        LeanTween.rotate(go, new Vector3(rotation.x, rotation.y, rotation.z + 5), tutorialHandTapSpeed).setEase(LeanTweenType.linear).setLoopPingPong();
    }

    public void DisplayTutorialHandTapQuaternion(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Vector3 pos = position;
        pos.z = -0.3f;

        GameObject go = Instantiate(tutorialHandPrefabTap, pos, Quaternion.identity);
        currentlyActiveTutorialHand = go;

        go.transform.localScale = scale;

        Transform child = go.transform.GetChild(0);

        child.transform.rotation = rotation;

        LeanTween.rotate(go, new Vector3(0, 0, 5), tutorialHandTapSpeed).setEase(LeanTweenType.linear).setLoopPingPong();
    }

    public void DeactivateAllTutorialScreens()
    {
        if(GameManager.Instance.currentLevel.specificTutorialEnum != SpecificTutorialsEnum.None)
        {
            foreach (GameObject go in specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens) /// THESE ARE ALL THE TEXT POPUPS
            {
                go.SetActive(false);
            }
        }

        foreach (GameObject go in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens) /// THESE ARE ALL THE TEXT POPUPS
        {
            go.SetActive(false);
        }

        if (currentlyActiveTutorialHand)
        {
            Destroy(currentlyActiveTutorialHand.gameObject);
        }

        maskImage.gameObject.SetActive(false);
    }

    public void CheckDoPotionTutorial()
    {
        if (!TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains((int)GameManager.Instance.currentLevel.specificTutorialEnum))
        {
            if (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft)
            {
                //UIManager.Instance.gameplayCanvasScreensUIHEIGHLIGHTS.SetActive(true);
                UIManager.Instance.nextLevelFromWinScreen.gameObject.SetActive(false);
                //TutorialSequence.Instacne.DisplaySpecificTutorialSequence();
                currentSpecificTutorial = GameManager.Instance.currentLevel.specificTutorialEnum;
                StartCoroutine(DisplaySpecificTutorialSequence());
            }
        }
    }

    public void AddToPlayerMatsForPotion(List<CraftingMatsNeededToRubies> CMNTR)
    {
        foreach (CraftingMatsNeededToRubies mat in CMNTR)
        {
            if(mat.mat == CraftingMats.DewDrops)
            {
                PlayerManager.Instance.collectedDewDrops += mat.amountMissing;
            }
            else
            {
                CraftingMatEntry CME = PlayerManager.Instance.craftingMatsInInventory.Where(p => p.mat == mat.mat).Single();

                CME.amount += mat.amountMissing;
            }
        }

        Debug.Log("Added missing ingredients!");
    }

    public void CheckEmptyTouchIncrementPhase(bool isSpecificTutorial)
    {
        if (isSpecificTutorial)
        {
            IncrementPhaseInSpecificTutorial();
        }
        else
        {
            IncrementCurrentPhaseInSequence();
        }

    }
}

