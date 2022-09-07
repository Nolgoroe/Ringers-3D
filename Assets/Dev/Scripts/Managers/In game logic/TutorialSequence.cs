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
    public bool isClipPhase, isBoardPhase, isPowerupPhase, isSingleCellPhase, isSingleSlice, isHubButtonPhase;
    public bool isOpenInventoryPhase, isPotionTabPhase, isEmptyTouchPhase, isBrewPhase, isBrewDisplayMaterials;
    public bool isAnimalSymbolCollectionPhase, hasDelay, isAllLocked, isClearScreen, isBoardGone, isGameUIGone;
    public bool isOpenDenPhase, isOpenHollowCraftTabPhase, isOpenInventoryInDenPhase, isCraftPhase, isCloseInventoryPhase, isDragHollowItemPhase;
    public bool dealPhase, isStatuePhase, isAnimalAlbumPhase, isAnimalAlbumStagTabPhase, isAnimalAlbumAllTabsPhase;
    public bool enterAnimationPhase;

    public int[] unlockedPowerups;
    public int[] unlockedClips;
    public int unlockedBoardCells = -1;

    public int[] targetCells;

    public int[] targetSlices;

    public GameObject[] targetTutorialHoles;

    public float delayAmount;
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
    DenScreen,
    PotionCraft,
    SliceBombTutorial,
    JokerTutorial,
    TileBombTutorial,
    AnimalAlbum,
    ColorMatch,
    ShapeMatch,

}
public class TutorialSequence : MonoBehaviour
{
    public static TutorialSequence Instacne;

    public GameObject tutorialHandPrefabMove;
    public GameObject tutorialHandPrefabTap;
    public Vector3 tutorialHandPosClip, tutorialHandPosCellOffset, tutorialHandPosDealButton, tutorialHandRotationDealButton, tutorialHandPosPowerupOffset;
    public Vector3 tutorialHandRotationPotions;
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

    public Transform handPosToHub, handPosOpenInventory, handPosOpenInventoryInDen, handPosOpenDen, handPosOpenAnimalAlbum, handPosClickAnimalTab, handPosChangePotionTab, handPosChangeHollowCraftTab, handPosBrewButton, handPosCraftItemButton, handPosCloseInventory;

    //public float delayUnlockAll;

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
            StartCoroutine(IncrementCurrentPhaseInSequence());

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
            RecreateRenderTexture(false);
        }
        else
        {
            //secondCam.Render();

            Texture2D texture = new Texture2D(Display.main.systemWidth, Display.main.systemHeight, TextureFormat.ARGB32, false);
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            Graphics.CopyTexture(secondCam.targetTexture, texture);
            
            maskImage.sprite = sprite;

            //if (GameManager.Instance.currentLevel.isTutorial)
            //{
            //    if (!levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequenceLevels].isClearScreen)
            //    {
            //        maskImage.gameObject.SetActive(true);
            //    }
            //}
        }
    }
    public void toTextureDenScreen()
    {
        if(secondCam.targetTexture.width != Display.main.systemWidth || secondCam.targetTexture.height != Display.main.systemHeight)
        {
            //StartCoroutine(RecreateRenderTexture());
            RecreateRenderTexture(true);
        }
        else
        {
            //secondCam.Render();

            Texture2D texture = new Texture2D(Display.main.systemWidth, Display.main.systemHeight, TextureFormat.ARGB32, false);
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            Graphics.CopyTexture(secondCam.targetTexture, texture);

            UIManager.Instance.maskImageDenScreen.sprite = sprite;

            UIManager.Instance.maskImageDenScreen.gameObject.SetActive(true);
        }
    }

    public void RecreateRenderTexture(bool isDen)
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

        if (isDen)
        {
            toTextureDenScreen();
        }
        else
        {
            toTexture();
        }
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
                UIManager.Instance.requiredButtonForTutorialPhase = UIManager.Instance.openInventoryButttonMap;

                UIManager.Instance.openInventoryButtonHeighlight.SetActive(true);
                activatedHeighlights.Add(UIManager.Instance.openInventoryButtonHeighlight.gameObject);
                DisplayTutorialHandTapQuaternion(handPosOpenInventory.position, handPosOpenInventory.rotation, handPosOpenInventory.localScale);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isOpenInventoryInDenPhase)
            {
                UIManager.Instance.requiredButtonForTutorialPhase = UIManager.Instance.openInventoryButttonDen;

                UIManager.Instance.openInventoryButtonHeighlightDenScreen.SetActive(true);
                activatedHeighlights.Add(UIManager.Instance.openInventoryButtonHeighlightDenScreen.gameObject);
                DisplayTutorialHandTapQuaternion(handPosOpenInventoryInDen.position, handPosOpenInventoryInDen.rotation, handPosOpenInventoryInDen.localScale);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isOpenDenPhase)
            {
                UIManager.Instance.requiredButtonForTutorialPhase = UIManager.Instance.openDenButttonMap;

                UIManager.Instance.openDenButtonHeighlight.SetActive(true);
                activatedHeighlights.Add(UIManager.Instance.openDenButtonHeighlight.gameObject);
                DisplayTutorialHandTapQuaternion(handPosOpenDen.position, handPosOpenDen.rotation, handPosOpenDen.localScale);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isAnimalAlbumPhase)
            {
                UIManager.Instance.requiredButtonForTutorialPhase = UIManager.Instance.openAnimalAlbumButttonMap;

                UIManager.Instance.openAnimalAlbumButtonHeighlight.SetActive(true);
                activatedHeighlights.Add(UIManager.Instance.openAnimalAlbumButtonHeighlight.gameObject);
                DisplayTutorialHandTapQuaternion(handPosOpenAnimalAlbum.position, handPosOpenAnimalAlbum.rotation, handPosOpenAnimalAlbum.localScale);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isAnimalAlbumStagTabPhase)
            {
                UIManager.Instance.requiredButtonForTutorialPhase = UIManager.Instance.animalAlbumStagTabButton;

                UIManager.Instance.animalAlbumStagTabButtonHeighlight.SetActive(true);
                activatedHeighlights.Add(UIManager.Instance.animalAlbumStagTabButtonHeighlight.gameObject);
                DisplayTutorialHandTapQuaternion(handPosClickAnimalTab.position, handPosClickAnimalTab.rotation, handPosClickAnimalTab.localScale);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isAnimalAlbumAllTabsPhase)
            {
                foreach (var sortButton in UIManager.Instance.animalAlbumSortButtons)
                {
                    sortButton.highlightObject.SetActive(true);
                    activatedHeighlights.Add(sortButton.highlightObject.gameObject);
                }
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isPotionTabPhase)
            {
                UIManager.Instance.requiredButtonForTutorialPhase = UIManager.Instance.potionInventoryButton.GetComponent<Button>();

                UIManager.Instance.potionTabHeighlight.SetActive(true);
                activatedHeighlights.Add(UIManager.Instance.potionTabHeighlight.gameObject);
                DisplayTutorialHandTapQuaternion(handPosChangePotionTab.position, handPosChangePotionTab.rotation, handPosChangePotionTab.localScale);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isOpenHollowCraftTabPhase)
            {
                UIManager.Instance.requiredButtonForTutorialPhase = UIManager.Instance.forgeInventoryButton.GetComponent<Button>();

                UIManager.Instance.hollowCraftTabHeighlight.SetActive(true);
                activatedHeighlights.Add(UIManager.Instance.hollowCraftTabHeighlight.gameObject);
                DisplayTutorialHandTapQuaternion(handPosChangeHollowCraftTab.position, handPosChangeHollowCraftTab.rotation, handPosChangeHollowCraftTab.localScale);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isBrewPhase)
            {
                UIManager.Instance.requiredButtonForTutorialPhase = UIManager.Instance.brewPotionButton;

                foreach (EquipmentDisplayer equipment in MaterialsAndForgeManager.Instance.equipmentInBrewScreen)
                {
                    equipment.GetComponent<Button>().onClick.RemoveAllListeners();
                }

                UIManager.Instance.brewButtonHeighlight.SetActive(true);
                activatedHeighlights.Add(UIManager.Instance.brewButtonHeighlight.gameObject);
                DisplayTutorialHandTapQuaternion(handPosBrewButton.position, handPosBrewButton.rotation, handPosBrewButton.localScale);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isCraftPhase)
            {
                HollowObjectDisplayer HOD = HollowCraftAndOwnedManager.Instance.hollowObjectsCreated[0].GetComponent<HollowObjectDisplayer>();

                UIManager.Instance.requiredButtonForTutorialPhase = HOD.craftButton;
                UIManager.Instance.forgeScrollRectView.enabled = false;

                HOD.gameObject.SetActive(true);
                HOD.tutorialHole.gameObject.SetActive(true);
                activatedHeighlights.Add(HOD.tutorialHole.gameObject);
                DisplayTutorialHandTapQuaternion(handPosCraftItemButton.position, handPosCraftItemButton.rotation, handPosCraftItemButton.localScale);
            }
            
            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isCloseInventoryPhase)
            {
                UIManager.Instance.requiredButtonForTutorialPhase = UIManager.Instance.closeInventoryButton;
                UIManager.Instance.forgeScrollRectView.enabled = true;

                UIManager.Instance.closeInventoryHeighlight.SetActive(true);
                activatedHeighlights.Add(UIManager.Instance.closeInventoryHeighlight.gameObject);
                DisplayTutorialHandTapQuaternion(handPosCloseInventory.position, handPosCloseInventory.rotation, handPosCloseInventory.localScale);
            }

            if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[index].isDragHollowItemPhase)
            {
                foreach (GameObject go in specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[currentPhaseInSequenceSpecific].targetTutorialHoles)
                {
                    go.SetActive(true);
                    activatedHeighlights.Add(go);
                }

                DisplayTutorialHandHoleToHole(specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[currentPhaseInSequenceSpecific].targetTutorialHoles[0], specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[currentPhaseInSequenceSpecific].targetTutorialHoles[1], 2);
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
            if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[index].dealPhase)
            {
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
                    //Debug.Log(num + "yasbfyabfyasb");


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
            
            if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[index].isAnimalSymbolCollectionPhase)
            {
                Transform t = GameManager.Instance.powerupManager.specialPowerupsInGame[0].transform;

                for (int i = 0; i < t.childCount; i++)
                {
                    if (t.GetChild(i).CompareTag("Tile Hole"))
                    {
                        t.GetChild(i).gameObject.SetActive(true);

                        activatedHeighlights.Add(t.GetChild(i).gameObject);
                    }

                    if (t.GetChild(i).transform.childCount > 0)
                    {
                        for (int k = 0; k < t.GetChild(i).childCount; k++)
                        {
                            if (t.GetChild(i).GetChild(k).CompareTag("Tile Hole"))
                            {
                                t.GetChild(i).GetChild(k).gameObject.SetActive(true);

                                activatedHeighlights.Add(t.GetChild(i).GetChild(k).gameObject);
                            }
                        }
                    }
                }
            }

            if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[index].isStatuePhase)
            {
                Transform t = AnimalsManager.Instance.statueToSwap.transform;

                for (int i = 0; i < t.childCount; i++)
                {
                    if (t.GetChild(i).CompareTag("Tile Hole"))
                    {
                        t.GetChild(i).gameObject.SetActive(true);

                        activatedHeighlights.Add(t.GetChild(i).gameObject);
                    }
                }
            }
        }


        //yield return new WaitForSeconds(0.8f);
        yield return new WaitForEndOfFrame();
        toTexture();
    }


    public IEnumerator IncrementCurrentPhaseInSequence()
    {

        if (currentlyActiveTutorialHand)
        {
            Destroy(currentlyActiveTutorialHand.gameObject);
        }


        currentPhaseInSequenceLevels++;


        if (currentPhaseInSequenceLevels == levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].EndPhaseID)
        {
            //TutorialSaveData.Instance.completedTutorialLevelId.Add(GameManager.Instance.currentLevel.numIndexForLeaderBoard);
            //TutorialSaveData.Instance.SaveTutorialSaveData();

            maskImage.gameObject.SetActive(false);
            duringSequence = false;
            Debug.Log("Phases are done!");
            //Invoke("UnlockAll", 2);

            if (!GameManager.LevelEnded)
            {
                StartCoroutine(UnlockAll());
            }


            //Invoke("DeactivateTutorialScreens", 0.1f);

            DeactivateAllTutorialScreens();
            currentPhaseInSequenceLevels = 0;

            //if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].doFadeInEnd)
            //{
            //    StartCoroutine(DeactivateTutorialScreens(levelSequences, GameManager.Instance.currentLevel.tutorialIndexForList, levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].waitTimeEndPhase));
            //}
            //else
            //{
            //    screensDeactivateOnTouch.Add(levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequenceLevels - 1]);
            //}

            //PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TutorialSaveData });
            yield break;
        }
        //else
        //{
        //    StartCoroutine(SelectReleventHeighlights(currentPhaseInSequenceLevels, false));
        //    ChangePhase(levelSequences, GameManager.Instance.currentLevel.tutorialIndexForList, currentPhaseInSequenceLevels);
        //}

        

        //clear screen to show it better
        if (currentPhaseInSequenceLevels < levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase.Count())
        {
            if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequenceLevels].isClearScreen)
            {
                maskImage.gameObject.SetActive(false);
                UIManager.Instance.tutorialCanvasParent.SetActive(false);

                if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequenceLevels].isAllLocked)
                {
                    AllLockedLogic(levelSequences, GameManager.Instance.currentLevel.tutorialIndexForList, currentPhaseInSequenceLevels);
                }


            }
            else
            {
                //maskImage.gameObject.SetActive(true);
                UIManager.Instance.tutorialCanvasParent.SetActive(true);
            }
        }

        if (currentPhaseInSequenceLevels < levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase.Count())
        {
            if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequenceLevels].hasDelay)
            {
                yield return new WaitForSeconds(levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[currentPhaseInSequenceLevels].delayAmount);

                maskImage.gameObject.SetActive(true);
                UIManager.Instance.tutorialCanvasParent.SetActive(true);
            }
        }

        if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequenceLevels])
        {
            if(currentPhaseInSequenceLevels > 0)
            {
                levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequenceLevels - 1].SetActive(false);
            }

            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequenceLevels].SetActive(true);
            levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequenceLevels].transform.GetChild(0).gameObject.SetActive(true);
        }



        if (currentPhaseInSequenceLevels + 1 <= levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].EndPhaseID)
        {
            Debug.Log("ARRIVED HERE");
            StartCoroutine(SelectReleventHeighlights(currentPhaseInSequenceLevels, false));
            ChangePhase(levelSequences, GameManager.Instance.currentLevel.tutorialIndexForList, currentPhaseInSequenceLevels);
        }


        yield return null;
        ///maskImage.sprite = levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].sprites[currentPhaseInSequence]; /// NEW

    }

    public void IncrementPhaseInSpecificTutorial()
    {
        currentPhaseInSequenceSpecific++;

        if (currentlyActiveTutorialHand)
        {
            Destroy(currentlyActiveTutorialHand.gameObject);
        }

        if (currentPhaseInSequenceSpecific == specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].EndPhaseID)
        {
            if (currentSpecificTutorial == SpecificTutorialsEnum.PotionCraft)
            {
                TutorialSaveData.Instance.hasFinishedPotion = true;

                Debug.LogError("Potions should work now");
                MaterialsAndForgeManager.Instance.ResetPotionDataAfterTutorial();
            }

            if (currentSpecificTutorial == SpecificTutorialsEnum.DenScreen)
            {
                TutorialSaveData.Instance.hasFinishedDen = true;

                Debug.LogError("Den Tutorial Done");
            }

            if (currentSpecificTutorial == SpecificTutorialsEnum.AnimalAlbum)
            {
                TutorialSaveData.Instance.hasFinishedAnimalAlbum = true;

                Debug.LogError("Animal Album Tutorial Done");
            }

            if (!TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains(GameManager.Instance.currentLevel.numIndexForLeaderBoard))
            {
                TutorialSaveData.Instance.completedSpecificTutorialLevelId.Add(GameManager.Instance.currentLevel.numIndexForLeaderBoard);
                PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TutorialSaveData });
            }

            UIManager.Instance.requiredButtonForTutorialPhase = null;

            //TutorialSaveData.Instance.completedSpecificTutorialLevelId.Add(GameManager.Instance.currentLevel.numIndexForLeaderBoard);
            currentSpecificTutorial = SpecificTutorialsEnum.None;
            maskImage.gameObject.SetActive(false);
            duringSequence = false;
            activatedHeighlights.Clear();
            activatedBoardParticles.Clear();

            if (!GameManager.LevelEnded)
            {
                StartCoroutine(UnlockAll());
            }

            PlayerManager.Instance.checkDoAddPotionsToInventory();
            currentPhaseInSequenceSpecific = 0;

            DeactivateAllTutorialScreens();

            return;
        }

        //maskImage.gameObject.SetActive(true);

        if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens[currentPhaseInSequenceSpecific])
        {
            if (currentPhaseInSequenceSpecific > 0)
            {
                specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens[currentPhaseInSequenceSpecific - 1].SetActive(false);
            }

            specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens[currentPhaseInSequenceSpecific].SetActive(true);
            specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens[currentPhaseInSequenceSpecific].transform.GetChild(0).gameObject.SetActive(true);
        }


        if (currentPhaseInSequenceLevels + 1 <= specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].EndPhaseID)
        {
            Debug.Log("ARRIVED HERE");
            StartCoroutine(SelectReleventHeighlights(currentPhaseInSequenceSpecific, true));
            ChangePhase(specificTutorials, (int)GameManager.Instance.currentLevel.specificTutorialEnum - 1, currentPhaseInSequenceSpecific);
        }
    }

    public void ChangePhase(Sequence[] tutorialArray, int TutorialIndex, int phaseIndex)
    {
        CursorController.canMovePieces = true;

        if (tutorialArray[TutorialIndex].phase[phaseIndex].isBoardGone)
        {
            GameManager.Instance.gameBoard.SetActive(false);
            GameManager.Instance.gameClip.SetActive(false);
            GameManager.Instance.selectedLevelBG.transform.GetChild(0).gameObject.SetActive(false);
            //UIManager.Instance.gameplayCanvasBotom.SetActive(false);
            UIManager.Instance.DecideBottmUIShow(GameManager.Instance.currentLevel.bottomUIToShow);
            UIManager.Instance.gameplayCanvasTop.SetActive(false);
            maskImage.gameObject.SetActive(false);
        }
        else
        {
            if (GameManager.Instance.gameBoard && GameManager.Instance.gameClip)
            {
                //GameManager.Instance.gameBoard.SetActive(true);
                //GameManager.Instance.gameClip.SetActive(true);
                //GameManager.Instance.selectedLevelBG.transform.GetChild(0).gameObject.SetActive(true);
                //UIManager.Instance.gameplayCanvasBotom.SetActive(true);
                UIManager.Instance.DecideBottmUIShow(GameManager.Instance.currentLevel.bottomUIToShow);
                UIManager.Instance.gameplayCanvasTop.SetActive(true);
                maskImage.gameObject.SetActive(true);
            }
        }

        if (tutorialArray[TutorialIndex].phase[phaseIndex].isGameUIGone)
        {
            UIManager.Instance.DecideBottmUIShow(BottomUIToShow.None);
        }
        else
        {
            if (GameManager.Instance.gameBoard && GameManager.Instance.gameClip)
            {
                UIManager.Instance.DecideBottmUIShow(GameManager.Instance.currentLevel.bottomUIToShow);
            }
        }

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

        if (tutorialArray[TutorialIndex].phase[phaseIndex].isAnimalSymbolCollectionPhase)
        {
            AnimalCollectPhaseLogic(tutorialArray, TutorialIndex, phaseIndex);
        }

        if (tutorialArray[TutorialIndex].phase[phaseIndex].isAllLocked)
        {
            AllLockedLogic(tutorialArray, TutorialIndex, phaseIndex);
        }

        if (tutorialArray[TutorialIndex].phase[phaseIndex].enterAnimationPhase)
        {
            EnterLevelAnimationPhaseLogic();
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

        Vector3 handPosDeal = tutorialHandPosDealButton;

        handPosDeal.z = -0.3f;

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

        //Debug.Log(handPosPowerup);

        handPosPowerup.z = 0;

        DisplayTutorialHandTap(handPosPowerup + tutorialHandPosPowerupOffset, tutorialHandRotationPotions, Vector3.one);
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
        pos.z = -0.3f;

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
        pos.z = -0.3f;

        DisplayTutorialHandTap(pos,tutorialHandRotationDealButton, Vector3.one);
    }
    public void AnimalCollectPhaseLogic(Sequence[] tutorialArray, int TutorialIndex, int phaseIndex)
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
            p.isTutorialLocked = true;
        }
    }
    public void AllLockedLogic(Sequence[] tutorialArray, int TutorialIndex, int phaseIndex)
    {
        UIManager.Instance.dealButton.interactable = false;
        CursorController.canMovePieces = false;

        foreach (Cell c in ConnectionManager.Instance.cells)
        {
            if (c.isFull)
            {
                c.pieceHeld.isTutorialLocked = true;
            }
        }

        for (int i = 0; i < GameManager.Instance.clipManager.clipCount; i++)
        {
            Piece p = GameManager.Instance.clipManager.slots[i].GetComponentInChildren<Piece>();
            p.isTutorialLocked = true;
        }
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

    public IEnumerator UnlockAll()
    {
        UIManager.Instance.dealButton.interactable = false;

        CursorController.canMovePieces = true;

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

            if (p)
            {
                p.isTutorialLocked = false;
            }
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


        GameManager.Instance.gameBoard.SetActive(true);
        GameManager.Instance.gameClip.SetActive(true);
        GameManager.Instance.selectedLevelBG.transform.GetChild(0).gameObject.SetActive(true);
        UIManager.Instance.DecideBottmUIShow(GameManager.Instance.currentLevel.bottomUIToShow);
        UIManager.Instance.gameplayCanvasTop.SetActive(true);

        UIManager.Instance.restartButton.interactable = true;
        UIManager.Instance.optionsButtonIngame.interactable = true;
        UIManager.Instance.cheatOptionsButtonIngame.interactable = true;

        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.dealButton.interactable = true;
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
        
        Vector3 targetPos = ConnectionManager.Instance.cells[CellIndex].transform.position + tutorialHandPosCellOffset;
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

    public void DisplayTutorialHandHoleToHole(GameObject origin, GameObject target, float scale)
    {
        Vector3 pos = origin.transform.position;
        //pos.z = -0.3f;

        GameObject go = Instantiate(tutorialHandPrefabMove, pos, Quaternion.identity);
        currentlyActiveTutorialHand = go;

        go.transform.localScale = new Vector3(scale, scale, scale);

        Vector3 targetPos = target.transform.position;
        //targetPos.z = -0.3f;

        LeanTween.move(go, targetPos, tutorialHandMoveSpeed).setEase(LeanTweenType.easeInOutQuad).setLoopClamp(); // animate
    }

    public void DisplayTutorialHandTap(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        Vector3 pos = position;
        //pos.z = -0.3f;

        GameObject go = Instantiate(tutorialHandPrefabTap, pos, Quaternion.Euler(rotation));
        currentlyActiveTutorialHand = go;

        go.transform.localScale = scale;

        //LeanTween.rotate(go, new Vector3(rotation.x, rotation.y, rotation.z + 5), tutorialHandTapSpeed).setEase(LeanTweenType.linear).setLoopPingPong();
        LeanTween.rotateAround(go, Vector3.forward, 5, tutorialHandTapSpeed).setEase(LeanTweenType.linear).setLoopPingPong();
    }

    public void DisplayTutorialHandTapQuaternion(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Vector3 pos = position;
        //pos.z = -0.3f;

        GameObject go = Instantiate(tutorialHandPrefabTap, pos, Quaternion.identity);
        currentlyActiveTutorialHand = go;

        go.transform.localScale = scale;

        Transform child = go.transform.GetChild(0);

        child.transform.rotation = rotation;

        LeanTween.rotate(go, new Vector3(0, 0, 5), tutorialHandTapSpeed).setEase(LeanTweenType.linear).setLoopPingPong();
    }

    public void DeactivateAllTutorialScreens()
    {
        //if(GameManager.Instance.currentLevel.specificTutorialEnum != SpecificTutorialsEnum.None)
        //{
        if(GameManager.Instance.currentLevel.isSpecificTutorial || GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.lootTutorial)
        {
            foreach (GameObject go in specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens) /// THESE ARE ALL THE TEXT POPUPS
            {
                go.SetActive(false);
            }
        }
        //}
        if (GameManager.Instance.currentLevel.isTutorial)
        {
            foreach (GameObject go in levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens) /// THESE ARE ALL THE TEXT POPUPS
            {
                go.SetActive(false);
            }
        }

        if (currentlyActiveTutorialHand)
        {
            Destroy(currentlyActiveTutorialHand.gameObject);
        }

        //maskImage.gameObject.SetActive(false);
    }

    public void CheckDoPotionTutorial()
    {
        if (!TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains(GameManager.Instance.currentLevel.numIndexForLeaderBoard))
        {
            if (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.PotionCraft)
            {
                //UIManager.Instance.gameplayCanvasScreensUIHEIGHLIGHTS.SetActive(true);
                maskImage.gameObject.SetActive(true);
                UIManager.Instance.nextLevelFromWinScreen.gameObject.SetActive(false);
                //TutorialSequence.Instacne.DisplaySpecificTutorialSequence();
                currentSpecificTutorial = GameManager.Instance.currentLevel.specificTutorialEnum;
                StartCoroutine(DisplaySpecificTutorialSequence());
            }
        }
    }
    public void CheckDoAnimalAlbumTutorial()
    {
        if (!TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains(GameManager.Instance.currentLevel.numIndexForLeaderBoard))
        {
            if (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.AnimalAlbum)
            {
                maskImage.gameObject.SetActive(true);
                UIManager.Instance.nextLevelFromWinScreen.gameObject.SetActive(false);
                currentSpecificTutorial = GameManager.Instance.currentLevel.specificTutorialEnum;
                StartCoroutine(DisplaySpecificTutorialSequence());
            }
        }
    }
    public void CheckDoDenTutorial()
    {
        if (!TutorialSaveData.Instance.completedSpecificTutorialLevelId.Contains(GameManager.Instance.currentLevel.numIndexForLeaderBoard))
        {
            if (GameManager.Instance.currentLevel.specificTutorialEnum == SpecificTutorialsEnum.DenScreen)
            {
                //UIManager.Instance.gameplayCanvasScreensUIHEIGHLIGHTS.SetActive(true);
                UIManager.Instance.nextLevelFromWinScreen.gameObject.SetActive(false);
                maskImage.gameObject.SetActive(true);
                //TutorialSequence.Instacne.DisplaySpecificTutorialSequence();
                currentSpecificTutorial = GameManager.Instance.currentLevel.specificTutorialEnum;
                StartCoroutine(DisplaySpecificTutorialSequence());
            }
        }
    }

    public void AddToPlayerMatsForPotion(List<CraftingMatsNeeded> CMN)
    {
        foreach (CraftingMatsNeeded mat in CMN)
        {
            if(mat.mat == CraftingMats.DewDrops)
            {
                PlayerManager.Instance.collectedDewDrops += mat.amount;
            }
            else
            {
                CraftingMatEntry CME = PlayerManager.Instance.craftingMatsInInventory.Where(p => p.mat == mat.mat).Single();

                CME.amount += mat.amount;
            }
        }

        Debug.Log("Added missing ingredients!");
    }

    public void AddToPlayerMatsForHollowCraft(List<CraftingMatsNeeded> CMN)
    {
        foreach (CraftingMatsNeeded mat in CMN)
        {
            if (mat.mat == CraftingMats.DewDrops)
            {
                PlayerManager.Instance.collectedDewDrops += mat.amount;
            }
            else
            {
                CraftingMatEntry CME = PlayerManager.Instance.craftingMatsInInventory.Where(p => p.mat == mat.mat).Single();

                CME.amount += mat.amount;
            }
        }

        HollowCraftAndOwnedManager.Instance.RefreshHollowObjects();
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
            StartCoroutine(IncrementCurrentPhaseInSequence());
        }

    }

    public void CheatTutorialClearNormal()
    {
        if (currentlyActiveTutorialHand)
        {
            Destroy(currentlyActiveTutorialHand.gameObject);
        }

        if(!TutorialSaveData.Instance.completedTutorialLevelId.Contains(GameManager.Instance.currentLevel.numIndexForLeaderBoard))
        {
            TutorialSaveData.Instance.completedTutorialLevelId.Add(GameManager.Instance.currentLevel.numIndexForLeaderBoard);
        }
        //TutorialSaveData.Instance.SaveTutorialSaveData();

        maskImage.gameObject.SetActive(false);
        duringSequence = false;
        Debug.Log("Phases are done!");
        //Invoke("UnlockAll", 2);

        if (!GameManager.LevelEnded)
        {
            StartCoroutine(UnlockAll());
        }


        //Invoke("DeactivateTutorialScreens", 0.1f);

        //if (levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].doFadeInEnd)
        //{
        //    StartCoroutine(DeactivateTutorialScreens(levelSequences, GameManager.Instance.currentLevel.tutorialIndexForList, levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].waitTimeEndPhase));
        //}
        //else
        //{
        //    screensDeactivateOnTouch.Add(levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].screens[currentPhaseInSequenceLevels]);
        //}

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TutorialSaveData });
    }

    public void CheatTutorialClearSpecific()
    {
        if (currentlyActiveTutorialHand)
        {
            Destroy(currentlyActiveTutorialHand.gameObject);
        }

        //TutorialSaveData.Instance.completedSpecificTutorialLevelId.Add((int)GameManager.Instance.currentLevel.specificTutorialEnum);
        currentSpecificTutorial = SpecificTutorialsEnum.None;
        maskImage.gameObject.SetActive(false);
        duringSequence = false;
        activatedHeighlights.Clear();
        activatedBoardParticles.Clear();

        if (!GameManager.LevelEnded)
        {
            StartCoroutine(UnlockAll());
        }

        PlayerManager.Instance.checkDoAddPotionsToInventory();

        DeactivateAllTutorialScreens();

        PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.TutorialSaveData });

        //if (specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].doFadeInEnd)
        //{
        //    StartCoroutine(DeactivateTutorialScreens(specificTutorials, GameManager.Instance.currentLevel.tutorialIndexForList, specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].waitTimeEndPhase));
        //}
        //else
        //{
        //    screensDeactivateOnTouch.Add(specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].screens[currentPhaseInSequenceSpecific - 1]);
        //}
    }

    public void EnterLevelAnimationPhaseLogic()
    {
        AnimationManager.instance.PopulateRefrencesEnterLevelAnim();
    }
}

