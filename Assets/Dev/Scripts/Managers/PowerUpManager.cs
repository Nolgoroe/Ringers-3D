using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public enum PowerUp
{
    
    //FourColorTransform,
    Switch,
    Joker,
    PieceBomb,
    SliceBomb,
    //ExtraDeal,
    //FourShapeTransform,
    None
}
public enum SpecialPowerUp
{
    DragonflyCross,
    BadgerExtraDeal,
    GoatWhatever,
    TurtleWhatever,
    None
}

[Serializable]
public class SpecialPowerData
{
    public PieceSymbol symbol;
    public int amount;
}

public class PowerUpManager : MonoBehaviour
{
    public getChildrenHelpData[] instnatiateZones;
    public GameObject powerupButtonPreab;
    public GameObject specialPowerPrefabLeft/*, specialPowerPrefabRight*/;
    public GameObject selectedPowerupVFX;
    public GameObject[] potionAnimationObjects;
    public Transform specialPowerPrefabParent;
    public Dictionary<PowerUp, string> spriteByType;
    public Dictionary<PowerUp, string> nameTextByType;
    public Dictionary<SpecialPowerUp, Sprite> specialPowerUpSpriteByType;

    public string[] powerupSpritesPath;
    public string[] powerupNames;
    public Sprite[] specialPowerupSprites;


    public List<PowerupProperties> powerupButtons;

    public static bool IsUsingPowerUp;
    public static bool HasUsedPowerUp;
    public static GameObject ObjectToUsePowerUpOn;

    public LayerMask layerToHit;

    public PowerupProperties currentlyInUse;

    public int instnatiatedZonesCounter = 0;

    public List<InGameSpecialPowerUp> specialPowerupsInGame;

    //public float offsetYSpecialPowers = 100;

    public Material jokerMat;

    Vector3 originalPotionPos = Vector3.zero;

    public GameObject specialPowerVFXPrefab;

    private void Start()
    {
        GameManager.Instance.powerupManager = this;

        spriteByType = new Dictionary<PowerUp, string>();
        nameTextByType = new Dictionary<PowerUp, string>();
        specialPowerUpSpriteByType = new Dictionary<SpecialPowerUp, Sprite>();

        for (int i = 0; i < System.Enum.GetValues(typeof(PowerUp)).Length - 1; i++)
        {
            spriteByType.Add((PowerUp)i, powerupSpritesPath[i]);
            nameTextByType.Add((PowerUp)i, powerupNames[i]);
        }

        for (int i = 0; i < System.Enum.GetValues(typeof(SpecialPowerUp)).Length - 1; i++)
        {
            specialPowerUpSpriteByType.Add((SpecialPowerUp)i, specialPowerupSprites[i]);
        }

    }
    public void AssignPowerUp(PowerUp ThePower, PowerupProperties theButton)
    {
        //if (ThePower != PowerUp.ExtraDeal)
        //{
            theButton.interactEvent.AddListener(() => UsingPowerup(theButton));
        //}

        PowerupProperties prop = theButton.gameObject.GetComponent<PowerupProperties>();
        switch (ThePower)
        {
            case PowerUp.Joker:
                theButton.interactEvent.AddListener(() => CallJokerCoroutine(prop));
                break;
            case PowerUp.Switch:
                theButton.interactEvent.AddListener(() => CallSwitchPowerCoroutine(prop));
                break;
            case PowerUp.PieceBomb:
                theButton.interactEvent.AddListener(() => CallPieceBombPowerCoroutine(prop));
                break;
            case PowerUp.SliceBomb:
                theButton.interactEvent.AddListener(() => CallSliceBombPowerCoroutine(prop));
                break;
            //case PowerUp.ExtraDeal:
            //    theButton.onClick.AddListener(() => ExtraDealPower(prop));
            //    break;
            //case PowerUp.FourColorTransform:
            //    theButton.onClick.AddListener(() => CallFourColorPowerCoroutine(prop));
            //    break;
            //case PowerUp.FourShapeTransform:
            //    theButton.onClick.AddListener(() => CallFourSymbolPowerCoroutine(prop));
            //    break;
            default:
                break;
        }
    }
    public void InstantiatePowerUps(EquipmentData data)
    {

        GameObject go = Instantiate(powerupButtonPreab, instnatiateZones[instnatiatedZonesCounter].transform);

        instnatiatedZonesCounter++;

        PowerupProperties prop = go.GetComponent<PowerupProperties>();

        prop.connectedEquipment = data;

        PowerUp current = data.power;

        go.name = current.ToString();

        prop.SetProperties(current);

        prop.numOfUses = data.numOfUses;

        prop.FindNumOfUsesTextObject();

        prop.UpdateNumOfUsesText();
        //if (current == PowerUp.FourColorTransform)
        //{
        //    prop.transformColor = data.specificColor;
        //}
        //else if(current == PowerUp.FourShapeTransform)
        //{
        //    prop.transformSymbol = data.specificSymbol;
        //}

        if (prop.connectedEquipment.scopeOfUses == 0) /// 0 = daily 1 = per patch
        {
            go.GetComponent<PowerupProperties>().canBeSelected = prop.connectedEquipment.nextTimeAvailable == null || prop.connectedEquipment.nextTimeAvailable == "";
        }
        else
        {
            go.GetComponent<PowerupProperties>().canBeSelected = true;
            Debug.Log("Can be selected");
        }
        AssignPowerUp(current, go.GetComponent<PowerupProperties>());

        powerupButtons.Add(go.GetComponent<PowerupProperties>());
    }
    public void Deal()
    {
        //CameraShake.ShakeOnce();

        if (!UIManager.isUsingUI)
        {
            SoundManager.Instance.PlaySound(Sounds.DealButton);
            UIManager.Instance.turnOnDealVFX();

            if (!GameManager.Instance.isDisableTutorials && GameManager.Instance.currentLevel.isTutorial)
            {
                if (TutorialSequence.Instacne.currentPhaseInSequenceLevels < TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase.Length)
                {
                    if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList].phase[TutorialSequence.Instacne.currentPhaseInSequenceLevels].dealPhase)
                    {
                       StartCoroutine(TutorialSequence.Instacne.IncrementCurrentPhaseInSequence());
                    }
                }
            }

            if (GameManager.Instance.clipManager.clipCount - 1 == 0)
            {
                UIManager.Instance.DisplayClipsAboutToEndMessage();
            }
            else
            {
                StartCoroutine(GameManager.Instance.clipManager.DealAnimation());
                //GameManager.Instance.clipManager.clipCount--;
                //GameManager.Instance.clipManager.RefreshSlots();
            }
        }
    }
    public void ExtraDealPower(PowerupProperties prop)
    {
        if(GameManager.Instance.clipManager.clipCount != 4)
        {
            GameManager.Instance.clipManager.ExtraDealSlots();
            FinishedUsingPowerup(true, prop);
        }
        else
        {
            FinishedUsingPowerup(false, prop);
        }

        Debug.Log("Extra Deal");

    }
    public void CallJokerCoroutine(PowerupProperties prop)
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (TutorialSequence.Instacne.currentSpecificTutorial == SpecificTutorialsEnum.JokerTutorial && TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].isPowerupPhase)
            {
                TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial();
                StartCoroutine(JokerPower(prop));
            }
        }
        else
        {
            StartCoroutine(JokerPower(prop));
        }
    }
    public void CallSwitchPowerCoroutine(PowerupProperties prop)
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (TutorialSequence.Instacne.currentSpecificTutorial == SpecificTutorialsEnum.SwapSidesTutorial && TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].isPowerupPhase)
            {
                TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial();
                StartCoroutine(SwitchPower(prop));
            }
        }
        else
        {
            StartCoroutine(SwitchPower(prop));
        }
    }
    public void CallPieceBombPowerCoroutine(PowerupProperties prop)
    {
        //Debug.LogError("TIMES CALLED HERE NOW");
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (TutorialSequence.Instacne.currentSpecificTutorial == SpecificTutorialsEnum.TileBombTutorial && TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].isPowerupPhase)
            {
                TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial();
                StartCoroutine(PieceBombPower(prop));
            }
        }
        else
        {
            StartCoroutine(PieceBombPower(prop));
        }
    }
    public void CallSliceBombPowerCoroutine(PowerupProperties prop)
    {
        if (TutorialSequence.Instacne.duringSequence)
        {
            if (TutorialSequence.Instacne.currentSpecificTutorial == SpecificTutorialsEnum.SliceBombTutorial && TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].isPowerupPhase)
            {
                TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial();
                StartCoroutine(SliceBombPower(prop));
            }
        }
        else
        {
            StartCoroutine(SliceBombPower(prop));
        }
    }
    public void CallFourColorPowerCoroutine(PowerupProperties prop)
    {
        StartCoroutine(FourColorPower(prop));
    }
    public void CallFourSymbolPowerCoroutine(PowerupProperties prop)
    {
        StartCoroutine(FourSymbolPower(prop));
    }
    public IEnumerator JokerPower(PowerupProperties prop)
    {
        layerToHit = LayerMask.GetMask("Piece Parent");
        yield return new WaitUntil(() => HasUsedPowerUp == true);

        bool successfulUse = false;

        Piece toWorkOn = ObjectToUsePowerUpOn.GetComponent<Piece>();

        if(toWorkOn.leftChild.symbolOfPiece != PieceSymbol.Joker) ///// If 1 of the sub pieces is a joker - so is the other. If the symbol is a joker then the color is awell
        {

            if (toWorkOn.partOfBoard /*&& !toWorkOn.isLocked*/)
            {
                InstantiatePotionAnimObject((int)prop.powerupType);
                yield return new WaitForSeconds(2.5f);

                toWorkOn.transform.parent.GetComponent<Cell>().RemovePiece(false);

                toWorkOn.leftChild.symbolOfPiece = PieceSymbol.Joker;
                toWorkOn.leftChild.colorOfPiece = PieceColor.Joker;

                toWorkOn.rightChild.symbolOfPiece = PieceSymbol.Joker;
                toWorkOn.rightChild.colorOfPiece = PieceColor.Joker;

                toWorkOn.leftChild.SetPieceAsJoker();
                toWorkOn.rightChild.SetPieceAsJoker();

                toWorkOn.transform.parent.GetComponent<Cell>().AddPiece(toWorkOn.transform, false);

                successfulUse = true;

            }
            else
            {
                InstantiatePotionAnimObject((int)prop.powerupType);
                yield return new WaitForSeconds(2.5f);

                toWorkOn.leftChild.symbolOfPiece = PieceSymbol.Joker;
                toWorkOn.leftChild.colorOfPiece = PieceColor.Joker;

                toWorkOn.rightChild.symbolOfPiece = PieceSymbol.Joker;
                toWorkOn.rightChild.colorOfPiece = PieceColor.Joker;

                toWorkOn.leftChild.SetPieceAsJoker();
                toWorkOn.rightChild.SetPieceAsJoker();

                successfulUse = true;
            }


            ShakePiecePowerUp(toWorkOn.gameObject);

            FinishedUsingPowerup(successfulUse, prop);


            if (TutorialSequence.Instacne.currentSpecificTutorial == SpecificTutorialsEnum.JokerTutorial)
            {
                TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial();
            }
            Debug.Log("Joker");
        }
        else
        {
            FinishedUsingPowerup(false, prop);
        }
    }
    public IEnumerator SwitchPower(PowerupProperties prop)
    {
        layerToHit = LayerMask.GetMask("Piece Parent");
        yield return new WaitUntil(() => HasUsedPowerUp == true);
        Piece toWorkOn = ObjectToUsePowerUpOn.GetComponent<Piece>();

        if ((toWorkOn.leftChild.symbolOfPiece != toWorkOn.rightChild.symbolOfPiece || toWorkOn.leftChild.colorOfPiece != toWorkOn.rightChild.colorOfPiece) && !toWorkOn.isStone)
        {
            if (toWorkOn.partOfBoard)
            {
                if (!toWorkOn.isLocked)
                {
                    InstantiatePotionAnimObject((int)prop.powerupType);

                    yield return new WaitForSeconds(2.5f);
                    toWorkOn.transform.parent.GetComponent<Cell>().RemovePiece(false);

                    PieceColor tempColor = toWorkOn.leftChild.colorOfPiece;
                    PieceSymbol tempSymbol = toWorkOn.leftChild.symbolOfPiece;

                    toWorkOn.leftChild.colorOfPiece = toWorkOn.rightChild.colorOfPiece;
                    toWorkOn.leftChild.symbolOfPiece = toWorkOn.rightChild.symbolOfPiece;

                    toWorkOn.rightChild.colorOfPiece = tempColor;
                    toWorkOn.rightChild.symbolOfPiece = tempSymbol;

                    toWorkOn.leftChild.RefreshPiece();
                    toWorkOn.rightChild.RefreshPiece();

                    toWorkOn.transform.parent.GetComponent<Cell>().AddPiece(toWorkOn.transform, false);

                    FinishedUsingPowerup(true, prop);
                    ShakePiecePowerUp(toWorkOn.gameObject);
                }
                else
                {
                    FinishedUsingPowerup(false, prop);
                }
            }
            else
            {
                InstantiatePotionAnimObject((int)prop.powerupType);
                yield return new WaitForSeconds(2.5f);

                PieceColor tempColor = toWorkOn.leftChild.colorOfPiece;
                PieceSymbol tempSymbol = toWorkOn.leftChild.symbolOfPiece;

                toWorkOn.leftChild.colorOfPiece = toWorkOn.rightChild.colorOfPiece;
                toWorkOn.leftChild.symbolOfPiece = toWorkOn.rightChild.symbolOfPiece;

                toWorkOn.rightChild.colorOfPiece = tempColor;
                toWorkOn.rightChild.symbolOfPiece = tempSymbol;

                toWorkOn.leftChild.RefreshPiece();
                toWorkOn.rightChild.RefreshPiece();
                FinishedUsingPowerup(true, prop);
                ShakePiecePowerUp(toWorkOn.gameObject);
            }

            if (TutorialSequence.Instacne.currentSpecificTutorial == SpecificTutorialsEnum.SwapSidesTutorial)
            {
                TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial();
            }

            Debug.Log("Switch");
        }
        else
        {
            FinishedUsingPowerup(false, prop);
        }
    }
    public IEnumerator PieceBombPower(PowerupProperties prop)
    {
        layerToHit = LayerMask.GetMask("Piece Parent");
        yield return new WaitUntil(() => HasUsedPowerUp == true);

        Piece toWorkOn = ObjectToUsePowerUpOn.GetComponent<Piece>();

        if (toWorkOn.partOfBoard)
        {
            InstantiatePotionAnimObject((int)prop.powerupType);
            yield return new WaitForSeconds(2.5f);

            //Debug.LogError("Times called");
            toWorkOn.transform.parent.GetComponent<Cell>().RemovePiece(false);

            if (toWorkOn.isLocked)
            {
                //toWorkOn.transform.parent.GetComponent<Cell>().lockSprite.SetActive(false);
                //Destroy(toWorkOn.transform.GetChild(0).gameObject); ///WIP
            }
            yield return new WaitForEndOfFrame();
            GameManager.Instance.currentFilledCellCount--;

            ShakePiecePowerUp(toWorkOn.gameObject);

            Destroy(toWorkOn.gameObject, 0.5f);

            FinishedUsingPowerup(true, prop);

            if (TutorialSequence.Instacne.currentSpecificTutorial == SpecificTutorialsEnum.TileBombTutorial)
            {
                TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial();
            }
        }
        else
        {
            FinishedUsingPowerup(false, prop);
        }

        Debug.Log("Piece Bomb");

    }
    public IEnumerator SliceBombPower(PowerupProperties prop)
    {
        layerToHit = LayerMask.GetMask("Slice");
        yield return new WaitUntil(() => HasUsedPowerUp == true);

        InstantiatePotionAnimObject((int)prop.powerupType);
        yield return new WaitForSeconds(2.5f);

        Slice toWorkOn = ObjectToUsePowerUpOn.transform.parent.GetComponent<Slice>();

        int a, b;
        a = toWorkOn.sliceIndex;
        b = toWorkOn.sliceIndex - 1;

        bool rightFull = false;
        bool leftFull = false;

        if (a == 0)
        {
            b = ConnectionManager.Instance.cells.Count - 1;
        }

        if (ConnectionManager.Instance.cells[a].isFull)
        {
            SubPiece subPieceL = ConnectionManager.Instance.cells[a].pieceHeld.leftChild;

            rightFull = true;

            if (ConnectionManager.Instance.cells[a].pieceHeld.isLocked)
            {
                ConnectionManager.Instance.cells[a].pieceHeld.isLocked = false;

                //Destroy(subPieceL.transform.GetChild(0).gameObject);
                //ConnectionManager.Instance.cells[a].lockSpriteCellLeft.SetActive(false);
            }
        }

        if (ConnectionManager.Instance.cells[b].isFull)
        {
            SubPiece subPieceR = ConnectionManager.Instance.cells[b].pieceHeld.rightChild;

            leftFull = true;

            if (ConnectionManager.Instance.cells[b].pieceHeld.isLocked)
            {
                ConnectionManager.Instance.cells[b].pieceHeld.isLocked = false;

                //Destroy(subPieceR.transform.GetChild(0).gameObject);
                //ConnectionManager.Instance.cells[b].lockSpriteCellRight.SetActive(false);
            }
        }

        if (toWorkOn.isLock)
        {
            //toWorkOn.connectedCells[0].lockSpriteCellLeft.SetActive(false); // old lock system
            //toWorkOn.connectedCells[1].lockSpriteCellRight.SetActive(false); // old lock system

            toWorkOn.lockSpriteAnim.gameObject.SetActive(false);
            toWorkOn.lockSpriteHeighlightAnim.gameObject.SetActive(false);
        }

        toWorkOn.isLock = false;

        if (toWorkOn.isLimiter)
        {
            if (rightFull && leftFull)
            {
                Debug.Log("Right and Left");
                Transform pa = ConnectionManager.Instance.cells[a].pieceHeld.transform;
                Transform pb = ConnectionManager.Instance.cells[b].pieceHeld.transform;

                ConnectionManager.Instance.cells[a].RemovePiece(true);
                ConnectionManager.Instance.cells[b].RemovePiece(true);
                toWorkOn.hasSlice = false;

                yield return new WaitForEndOfFrame();
                ConnectionManager.Instance.cells[a].AddPiece(pa, false);
                ConnectionManager.Instance.cells[b].AddPiece(pb, false);
                //ConnectionManager.Instance.CallConnection(ConnectionManager.Instance.cells[a].cellIndex, ConnectionManager.Instance.cells[a].isOuter, false);
            }
            else if (leftFull)
            {
                Debug.Log("Left Only");

                Transform p = ConnectionManager.Instance.cells[b].pieceHeld.transform;
                ConnectionManager.Instance.cells[b].RemovePiece(true);
                toWorkOn.hasSlice = false;

                yield return new WaitForEndOfFrame();
                ConnectionManager.Instance.cells[b].AddPiece(p, false);
            }
            else if (rightFull)
            {
                Debug.Log("Right Only");

                Transform p = ConnectionManager.Instance.cells[a].pieceHeld.transform;
                ConnectionManager.Instance.cells[a].RemovePiece(true);
                toWorkOn.hasSlice = false;

                yield return new WaitForEndOfFrame();
                ConnectionManager.Instance.cells[a].AddPiece(p, false);
            }
            else
            {
                Debug.LogError("No Pieces to check connections");
                toWorkOn.hasSlice = false;
            }
        }

        toWorkOn.ResetDate();

        ObjectToUsePowerUpOn.GetComponent<CameraShake>().ShakeOnce();

        yield return new WaitForEndOfFrame();
        Destroy(ObjectToUsePowerUpOn.gameObject, 0.6f);

        FinishedUsingPowerup(true, prop);

        if (TutorialSequence.Instacne.currentSpecificTutorial == SpecificTutorialsEnum.SliceBombTutorial)
        {
            TutorialSequence.Instacne.IncrementPhaseInSpecificTutorial();
        }
        Debug.Log("Slice Bomb");

    }
    public IEnumerator FourColorPower(PowerupProperties prop)
    {
        layerToHit = LayerMask.GetMask("Sub Piece");
        yield return new WaitUntil(() => HasUsedPowerUp == true);
        SubPiece toWorkOn = ObjectToUsePowerUpOn.GetComponent<SubPiece>();
        
        Piece par = toWorkOn.transform.parent.GetComponent<Piece>();

        if(toWorkOn.colorOfPiece != prop.transformColor /*&& !par.isLocked*/ && par.partOfBoard)
        {
            par.transform.parent.GetComponent<Cell>().RemovePiece(false);

            toWorkOn.colorOfPiece = prop.transformColor;

            toWorkOn.RefreshPiece();

            par.transform.parent.GetComponent<Cell>().AddPiece(par.transform, false);

            ShakePiecePowerUp(par.gameObject);

            FinishedUsingPowerup(true, prop);

            Debug.Log("Four Color");
        }
        else
        {
            FinishedUsingPowerup(false, prop);
        }
    }
    public IEnumerator FourSymbolPower(PowerupProperties prop)
    {
        layerToHit = LayerMask.GetMask("Sub Piece");
        yield return new WaitUntil(() => HasUsedPowerUp == true);
        SubPiece toWorkOn = ObjectToUsePowerUpOn.GetComponent<SubPiece>();

        Piece par = toWorkOn.transform.parent.GetComponent<Piece>();

        if(toWorkOn.symbolOfPiece != prop.transformSymbol && par.partOfBoard /*&& !par.isLocked*/)
        {
            par.transform.parent.GetComponent<Cell>().RemovePiece(false);

            toWorkOn.symbolOfPiece = prop.transformSymbol;

            toWorkOn.RefreshPiece();

            par.transform.parent.GetComponent<Cell>().AddPiece(par.transform, false);

            FinishedUsingPowerup(true, prop);

            Debug.Log("Four Symbol");
        }
        else
        {
            FinishedUsingPowerup(false, prop);
        }
    }
    public void UsingPowerup(PowerupProperties butt)
    {
        if (TutorialSequence.Instacne.duringSequence) /// this if statemene does exactly the same as the else??????
        {
            if (TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].isPowerupPhase)
            {
                currentlyInUse = butt.gameObject.GetComponent<PowerupProperties>();
                UIManager.Instance.ActivateUsingPowerupMessage(true);

                foreach (PowerupProperties but in powerupButtons)
                {
                    if (but.interactEvent != butt.interactEvent)
                    {
                        but.canBeSelected = false;
                    }
                    else
                    {
                        but.canBeSelected = false;
                        //originalPotionPos = butt.gameObject.transform.position;
                        //Vector3 pos = butt.gameObject.transform.position;
                        //pos.y += 0.1f;

                        //LeanTween.move(butt.gameObject, pos, 0.5f).setEase(LeanTweenType.easeInOutQuad); // animate

                        LeanTween.scale(but.gameObject, new Vector3(but.transform.localScale.x - 8, but.transform.localScale.y - 8, 1), 0.1f).setOnComplete(() => ScalePotionBack(but.gameObject));
                        Instantiate(selectedPowerupVFX, but.transform);
                    }
                }

                StartCoroutine(WaitForEndFrame());
            }
        }
        else
        {
            currentlyInUse = butt.gameObject.GetComponent<PowerupProperties>();
            UIManager.Instance.ActivateUsingPowerupMessage(true);

            foreach (PowerupProperties but in powerupButtons)
            {
                if (but != butt)
                {
                    but.canBeSelected = false;
                }
                else
                {
                    but.canBeSelected = false;
                    //originalPotionPos = butt.gameObject.transform.position;
                    //Vector3 pos = butt.gameObject.transform.position;
                    //pos.y += 0.1f;

                    //LeanTween.move(butt.gameObject, pos, 0.5f).setEase(LeanTweenType.easeInOutQuad); // animate
                    LeanTween.scale(but.gameObject, new Vector3(but.transform.localScale.x - 8, but.transform.localScale.y - 8, 1), 0.1f).setOnComplete(() => ScalePotionBack(but.gameObject));
                    Instantiate(selectedPowerupVFX, but.transform);
                }
            }

            StartCoroutine(WaitForEndFrame());
        }
    }

    public void ScalePotionBack(GameObject toScale)
    {
        LeanTween.scale(toScale, new Vector3(toScale.transform.localScale.x + 8, toScale.transform.localScale.y + 8, 1), 0.1f);
    }
    public IEnumerator WaitForEndFrame()
    {
        yield return new WaitForEndOfFrame();
        IsUsingPowerUp = true;
    }

    public void CancelPowerup(PowerupProperties prop)
    {
        //StopAllCoroutines();
        UIManager.Instance.ActivateUsingPowerupMessage(false);

        IsUsingPowerUp = false;
        currentlyInUse = null;
        HasUsedPowerUp = false;
        layerToHit = new LayerMask();
        EquipmentData ED = null;

        for (int i = 0; i < prop.transform.childCount; i++)
        {
            if (prop.transform.GetChild(i).CompareTag("DestroyVFX"))
            {
                Destroy(prop.transform.GetChild(i).gameObject);
            }
        }

        ReactivatePowerButtons();
        //LeanTween.move(prop.gameObject, originalPotionPos, 0.4f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => ReactivatePowerButtons()); // animate
    }

    public void FinishedUsingPowerup(bool successfull, PowerupProperties prop)
    {
        StopAllCoroutines();
        UIManager.Instance.ActivateUsingPowerupMessage(false);

        IsUsingPowerUp = false;
        currentlyInUse = null;
        HasUsedPowerUp = false;
        layerToHit = new LayerMask();


        EquipmentData ED = null;

        for (int i = 0; i < prop.transform.childCount; i++)
        {
            if (prop.transform.GetChild(i).CompareTag("DestroyVFX"))
            {
                Destroy(prop.transform.GetChild(i).gameObject);
            }
        }

        if (successfull)
        {

            if (TutorialSequence.Instacne.duringSequence)
            {
                bool singleCellPhase = TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].isSingleCellPhase;
                bool singleSlicePhase = TutorialSequence.Instacne.specificTutorials[(int)GameManager.Instance.currentLevel.specificTutorialEnum - 1].phase[TutorialSequence.Instacne.currentPhaseInSequenceSpecific].isSingleSlice;

                if (singleCellPhase || singleSlicePhase)
                {
                    ED = PlayerManager.Instance.ownedPowerups.Where(p => (p.power == prop.connectedEquipment.power && p.isTutorialPower == prop.connectedEquipment.isTutorialPower)).First();

                }
            }
            else
            {
                ED = PlayerManager.Instance.ownedPowerups.Where(p => p.power == prop.connectedEquipment.power).First();
            }

            if (ED != null)
            {
                ED.numOfUses--;

                prop.numOfUses--;

                prop.UpdateNumOfUsesText();
            }
            else
            {
                Debug.LogError("Something very wrong happened here - stop the finished using power coroutine");
                return;
            }
        }
        else
        {
            SoundManager.Instance.PlaySound(Sounds.NegativeSound);
        }
        //if (prop.connectedEquipment.isTutorialPower)
        //{
        //    if (prop.numOfUses == 0)
        //    {
        //        ReactivatePowerButtons();
        //        powerupButtons.Remove(prop.GetComponent<Button>());
        //        Destroy(prop.gameObject, 0.55f);
        //    }
        //}
        //else
        //{
        ReactivatePowerButtons();
        //LeanTween.move(prop.gameObject, originalPotionPos, 0.4f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => ReactivatePowerButtons()); // animate

        if (prop.numOfUses == 0 && prop.connectedEquipment.scopeOfUses == 0) //// if the num of uses is 0 and the scope is cooldown and not per match
        {
            //EquipmentData ED = PlayerManager.Instance.ownedPowerups.Where(p => p.name == prop.connectedEquipment.name).First();

            ED.nextTimeAvailable = System.DateTime.Now.AddSeconds(ED.timeForCooldown).ToString(); ///// change the datetime for equipment on player

            PlayerManager.Instance.equipmentInCooldown.Add(ED);
            //PlayerManager.Instance.SavePlayerData();
            //PlayfabManager.instance.SaveAllGameData();
        }

        if (prop.numOfUses == 0 && prop.connectedEquipment.scopeOfUses == 1) //// if the num of uses is 0 and the scope is per match
        {
            //EquipmentData ED = PlayerManager.Instance.ownedPowerups.Where(p => p.name == prop.connectedEquipment.name).First();

            PlayerManager.Instance.activePowerups.Remove(ED.power);
            PlayerManager.Instance.ownedPowerups.Remove(ED);
            //PlayerManager.Instance.SavePlayerData();
            //PlayfabManager.instance.SaveAllGameData();

            powerupButtons.Remove(prop.GetComponent<PowerupProperties>());
            //Destroy(prop.gameObject, 0.45f);            
            Destroy(prop.gameObject);

            ReactivatePowerButtons();
        }

        if (successfull)
        {
            PlayfabManager.instance.SaveGameData(new SystemsToSave[] { SystemsToSave.Player });
        }

        //}

        //Vector3 pos = prop.gameObject.transform.position;
        //pos.y -= 0.1f;


    }

    public void ReactivatePowerButtons()
    {
        Debug.LogError("reactivating");
        foreach (PowerupProperties but in powerupButtons)
        {
            if (but.gameObject.GetComponent<PowerupProperties>().numOfUses > 0)
            {
                but.canBeSelected = true;
            }
            else
            {
                but.canBeSelected = false;
            }
        }

        //originalPotionPos = Vector3.zero;
    }

    public void CallSpecialPowerUp(InGameSpecialPowerUp IGSP)
    {
        switch (IGSP.type)
        {
            case SpecialPowerUp.BadgerExtraDeal:
                GameManager.Instance.clipManager.ExtraDealSlotsBadgerSpecial(IGSP);
                break;
            case SpecialPowerUp.DragonflyCross:
                StartCoroutine(GameManager.Instance.clipManager.DragonflyCrossSpecial(IGSP));
                break;
            default:
                break;
        }
    }

    public void UpdateSpecialPowerupsCount(int amount, PieceSymbol symbol)
    {
        foreach (InGameSpecialPowerUp IGSP in specialPowerupsInGame)
        {
            if(IGSP.SymbolNeeded == symbol)
            {
                Debug.Log(IGSP.SymbolNeeded);

                IGSP.UpdateSlider(amount);
            }
        }
    }

    public void InstantiateSpecialPowers()
    {
        if(GameManager.Instance.currentLevel.symbolsNeededForSpecialPowers.Length > 0)
        {
            //bool leftSide = true;
            //bool up = false;
            GameObject go = null;

            foreach (SpecialPowerData SPD in GameManager.Instance.currentLevel.symbolsNeededForSpecialPowers)
            {
                //if (up)
                //{
                    //if (!leftSide)
                   // {
                        //go = Instantiate(specialPowerPrefabRight, specialPowerPrefabParent);

                        //Transform pos = go.transform;

                        //go.transform.localPosition = new Vector3(-pos.localPosition.x, pos.localPosition.y + offsetYSpecialPowers, pos.localPosition.z);
                    //}
                    //else
                    //{
                        go = Instantiate(specialPowerPrefabLeft, specialPowerPrefabParent);

                        Transform pos = go.transform;

                        go.transform.localPosition = new Vector3(pos.localPosition.x, pos.localPosition.y/* + offsetYSpecialPowers*/, pos.localPosition.z);
                    //}
                //}
                //else
                //{
                //    if (!leftSide)
                //    {
                //        go = Instantiate(specialPowerPrefabRight, specialPowerPrefabParent);

                //        Transform pos = go.transform;

                //        go.transform.position = new Vector3(-pos.position.x, pos.position.y, pos.position.z);
                //        up = true;
                //    }
                //    else
                //    {
                //        go = Instantiate(specialPowerPrefabLeft, specialPowerPrefabParent);
                //    }
                //}

                InGameSpecialPowerUp IGSP = null;

                if (go)
                {
                    IGSP = go.GetComponent<InGameSpecialPowerUp>();

                    IGSP.button.onClick.AddListener(() => CallSpecialPowerUp(IGSP));
                    specialPowerupsInGame.Add(IGSP);
                }


                switch (SPD.symbol)
                {
                    case PieceSymbol.FireFly:
                        IGSP.type = SpecialPowerUp.DragonflyCross;
                        IGSP.SymbolNeeded = PieceSymbol.FireFly;
                        IGSP.amountNeededToActivate = SPD.amount;
                        IGSP.icon.sprite = specialPowerUpSpriteByType[IGSP.type];
                        break;
                    case PieceSymbol.Badger:
                        IGSP.type = SpecialPowerUp.BadgerExtraDeal;
                        IGSP.SymbolNeeded = PieceSymbol.Badger;
                        IGSP.amountNeededToActivate = SPD.amount;
                        IGSP.icon.sprite = specialPowerUpSpriteByType[IGSP.type];
                        break;
                    case PieceSymbol.Goat:
                        IGSP.type = SpecialPowerUp.GoatWhatever;
                        IGSP.SymbolNeeded = PieceSymbol.Goat;
                        IGSP.amountNeededToActivate = SPD.amount;
                        IGSP.icon.sprite = specialPowerUpSpriteByType[IGSP.type];
                        break;
                    case PieceSymbol.Turtle:
                        IGSP.type = SpecialPowerUp.TurtleWhatever;
                        IGSP.SymbolNeeded = PieceSymbol.Turtle;
                        IGSP.amountNeededToActivate = SPD.amount;
                        IGSP.icon.sprite = specialPowerUpSpriteByType[IGSP.type];
                        break;
                    case PieceSymbol.Joker:
                        break;
                    case PieceSymbol.None:
                        break;
                    default:
                        break;
                }

                //leftSide = !leftSide;
            }
        }
    }

    public void DestroySpecialPowersObjects()
    {
        for (int i = 0; i < specialPowerupsInGame.Count; i++)
        {
            Destroy(specialPowerupsInGame[i].gameObject);
        }

        specialPowerupsInGame.Clear();
    }

    public void PowerupButtonsActivation(bool activate)
    {
        foreach (PowerupProperties b in powerupButtons)
        {
            b.canBeSelected = activate;
        }
    }

    public void ClearTutorialPowerups()
    {
        List<EquipmentData> tempList = new List<EquipmentData>();
        tempList.AddRange(PlayerManager.Instance.ownedPowerups);

        foreach (EquipmentData ED in tempList)
        {
            if (ED.isTutorialPower)
            {
                PlayerManager.Instance.ownedPowerups.Remove(ED);
            }
        }

        //PlayerManager.Instance.SavePlayerData();
        //PlayfabManager.instance.SaveAllGameData();
    }

    public void CheckTurnTempPowerToPermaPower()
    {
        foreach (EquipmentData ED in PlayerManager.Instance.ownedPowerups)
        {
            if (ED.isTutorialPower)
            {
                ED.isTutorialPower = false;
            }
        }
    }

    void ShakePiecePowerUp(GameObject toShake)
    {
        toShake.GetComponent<CameraShake>().ShakeOnce();
    }


    public void ResetData()
    {
        IsUsingPowerUp = false;
        HasUsedPowerUp = false;
        instnatiatedZonesCounter = 0;
        layerToHit = LayerMask.GetMask("Nothing");

        foreach (getChildrenHelpData GCHD in instnatiateZones)
        {
            GCHD.referenceNumUsesText.SetActive(false);
        }
    }

    public void InstantiatePotionAnimObject(int index)
    {
        GameObject go = Instantiate(potionAnimationObjects[index], GameManager.Instance.destroyOutOfLevel);
    }

    public void MoveSpecialPowerVFXToTarget(GameObject toMove, bool isDouble, PieceSymbol symbolNeeded)
    {
        Transform target = specialPowerupsInGame[0].icon.transform;

        Vector3 midPoint = new Vector3(toMove.transform.position.x -0.1f, toMove.transform.position.y - 1.5f, toMove.transform.position.z);

        Vector3 closePoint = new Vector3(toMove.transform.position.x - 1, toMove.transform.position.y, toMove.transform.position.z);

        LTBezierPath path = new LTBezierPath(new Vector3[] { toMove.transform.position, midPoint, closePoint, target.position});

        LeanTween.move(toMove, path, 2).setOnComplete(() => OnCompleteMovePowerVFX(toMove, isDouble, symbolNeeded));
    }

    public void OnCompleteMovePowerVFX(GameObject toMove, bool isDouble, PieceSymbol symbolNeeded)
    {
        if (isDouble)
        {
            GameManager.Instance.powerupManager.UpdateSpecialPowerupsCount(2, symbolNeeded);
        }
        else
        {
            GameManager.Instance.powerupManager.UpdateSpecialPowerupsCount(1, symbolNeeded);
        }

        Destroy(toMove);
    }

    public void ForceStopAllPowerupCoroutines()
    {
        StopAllCoroutines();
    }
}
