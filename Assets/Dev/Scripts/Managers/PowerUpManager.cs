using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum PowerUp
{
    //Joker,
    FourColorTransform,
    Switch,
    //PieceBomb,
    //SliceBomb,
    //ExtraDeal,
    //FourShapeTransform,
    None
}
public class PowerUpManager : MonoBehaviour
{
    public Transform[] instnatiateZones;
    public GameObject powerupButtonPreab;
    public Dictionary<PowerUp, string> spriteByType;
    public Dictionary<PowerUp, string> nameTextByType;

    public string[] powerupSpritesPath;
    public string[] powerupNames;


    public List<Button> powerupButtons;

    public static bool IsUsingPowerUp;
    public static bool HasUsedPowerUp;
    public static GameObject ObjectToUsePowerUpOn;

    public LayerMask layerToHit;

    public PowerupProperties currentlyInUse;

    public int instnatiatedZonesCounter = 0;
    private void Start()
    {
        GameManager.Instance.powerupManager = this;

        spriteByType = new Dictionary<PowerUp, string>();
        nameTextByType = new Dictionary<PowerUp, string>();

        for (int i = 0; i < System.Enum.GetValues(typeof(PowerUp)).Length - 1; i++)
        {
            spriteByType.Add((PowerUp)i, powerupSpritesPath[i]);
            nameTextByType.Add((PowerUp)i, powerupNames[i]);
        }

    }
    public void AssignPowerUp(PowerUp ThePower, Button theButton)
    {
        //if (ThePower != PowerUp.ExtraDeal)
        //{
            theButton.onClick.AddListener(() => UsingPowerup(theButton));
        //}

        PowerupProperties prop = theButton.gameObject.GetComponent<PowerupProperties>();
        switch (ThePower)
        {
            //case PowerUp.Joker:
            //    theButton.onClick.AddListener(() => CallJokerCoroutine(prop));
            //    break;
            case PowerUp.Switch:
                theButton.onClick.AddListener(() => CallSwitchPowerCoroutine(prop));
                break;
            //case PowerUp.PieceBomb:
            //    theButton.onClick.AddListener(() => CallPieceBombPowerCoroutine(prop));
            //    break;
            //case PowerUp.SliceBomb:
            //    theButton.onClick.AddListener(() => CallSliceBombPowerCoroutine(prop));
            //    break;
            //case PowerUp.ExtraDeal:
            //    theButton.onClick.AddListener(() => ExtraDealPower(prop));
            //    break;
            case PowerUp.FourColorTransform:
                theButton.onClick.AddListener(() => CallFourColorPowerCoroutine(prop));
                break;
            //case PowerUp.FourShapeTransform:
            //    theButton.onClick.AddListener(() => CallFourSymbolPowerCoroutine(prop));
            //    break;
            default:
                break;
        }
    }
    public void InstantiatePowerUps(EquipmentData data)
    {
        GameObject go = Instantiate(powerupButtonPreab, instnatiateZones[instnatiatedZonesCounter]);

        instnatiatedZonesCounter++;

        PowerupProperties prop = go.GetComponent<PowerupProperties>();

        prop.connectedEquipment = data;

        PowerUp current = data.power;

        go.name = current.ToString();

        prop.SetProperties(current);

        prop.numOfUses = data.numOfUses;

        if (current == PowerUp.FourColorTransform)
        {
            prop.transformColor = data.specificColor;
        }
        //else if(current == PowerUp.FourShapeTransform)
        //{
        //    prop.transformSymbol = data.specificSymbol;
        //}

        if(prop.connectedEquipment.scopeOfUses == 0) /// 0 = daily 1 = per patch
        {
            go.GetComponent<Button>().interactable = prop.connectedEquipment.nextTimeAvailable == null || prop.connectedEquipment.nextTimeAvailable == "";
        }
        else
        {
            go.GetComponent<Button>().interactable = true;
        }
        AssignPowerUp(current, go.GetComponent<Button>());

        powerupButtons.Add(go.GetComponent<Button>());
    }
    public void Deal()
    {
        if (GameManager.Instance.currentLevel.isTutorial)
        {
            if (TutorialSequence.Instacne.levelSequences[GameManager.Instance.currentLevel.tutorialIndexForList - 1].phase[TutorialSequence.Instacne.currentPhaseInSequence].dealPhase)
            {
                TutorialSequence.Instacne.IncrementCurrentPhaseInSequence();
            }
        }

        if (GameManager.Instance.clipManager.clipCount - 1 == 0)
        {
            UIManager.Instance.DisplayClipsAboutToEndMessage();
        }
        else
        {
            GameManager.Instance.clipManager.clipCount--;
            GameManager.Instance.clipManager.RefreshSlots();
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
        StartCoroutine(JokerPower(prop));
    }
    public void CallSwitchPowerCoroutine(PowerupProperties prop)
    {
        StartCoroutine(SwitchPower(prop));
    }
    public void CallPieceBombPowerCoroutine(PowerupProperties prop)
    {
        StartCoroutine(PieceBombPower(prop));
    }
    public void CallSliceBombPowerCoroutine(PowerupProperties prop)
    {
        StartCoroutine(SliceBombPower(prop));
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

        Piece toWorkOn = ObjectToUsePowerUpOn.GetComponent<Piece>();

        if(toWorkOn.leftChild.symbolOfPiece != PieceSymbol.Joker) ///// If 1 of the sub pieces is a joker - so is the other. If the symbol is a joker then the color is awell
        {
            if (toWorkOn.partOfBoard && !toWorkOn.isLocked)
            {
                toWorkOn.transform.parent.GetComponent<Cell>().RemovePiece();

                toWorkOn.leftChild.symbolOfPiece = PieceSymbol.Joker;
                toWorkOn.leftChild.colorOfPiece = PieceColor.Joker;

                toWorkOn.rightChild.symbolOfPiece = PieceSymbol.Joker;
                toWorkOn.rightChild.colorOfPiece = PieceColor.Joker;

                toWorkOn.leftChild.RefreshPiece();
                toWorkOn.rightChild.RefreshPiece();

                toWorkOn.transform.parent.GetComponent<Cell>().AddPiece(toWorkOn.transform, false);
            }

            FinishedUsingPowerup(toWorkOn.partOfBoard && !toWorkOn.isLocked, prop);

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

        if(toWorkOn.leftChild.symbolOfPiece != toWorkOn.rightChild.symbolOfPiece || toWorkOn.leftChild.colorOfPiece != toWorkOn.rightChild.colorOfPiece)
        {
            if (toWorkOn.partOfBoard && !toWorkOn.isLocked)
            {
                toWorkOn.transform.parent.GetComponent<Cell>().RemovePiece();

                PieceColor tempColor = toWorkOn.leftChild.colorOfPiece;
                PieceSymbol tempSymbol = toWorkOn.leftChild.symbolOfPiece;

                toWorkOn.leftChild.colorOfPiece = toWorkOn.rightChild.colorOfPiece;
                toWorkOn.leftChild.symbolOfPiece = toWorkOn.rightChild.symbolOfPiece;

                toWorkOn.rightChild.colorOfPiece = tempColor;
                toWorkOn.rightChild.symbolOfPiece = tempSymbol;

                toWorkOn.leftChild.RefreshPiece();
                toWorkOn.rightChild.RefreshPiece();

                toWorkOn.transform.parent.GetComponent<Cell>().AddPiece(toWorkOn.transform, false);
            }

            FinishedUsingPowerup(toWorkOn.partOfBoard && !toWorkOn.isLocked, prop);
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
            toWorkOn.transform.parent.GetComponent<Cell>().RemovePiece();

            if (toWorkOn.isLocked)
            {
                //toWorkOn.transform.parent.GetComponent<Cell>().lockSprite.SetActive(false);
                Destroy(toWorkOn.transform.GetChild(0).gameObject); ///WIP
            }
            yield return new WaitForEndOfFrame();
            GameManager.Instance.currentFilledCellCount--;
            Destroy(toWorkOn.gameObject);

        }

        FinishedUsingPowerup(toWorkOn.partOfBoard, prop);

        Debug.Log("Piece Bomb");

    }
    public IEnumerator SliceBombPower(PowerupProperties prop)
    {
        layerToHit = LayerMask.GetMask("Slice");
        yield return new WaitUntil(() => HasUsedPowerUp == true);

        Slice toWorkOn = ObjectToUsePowerUpOn.transform.parent.GetComponent<Slice>();
        int a, b;
        a = toWorkOn.sliceIndex;
        b = toWorkOn.sliceIndex - 1;
        if (a == 0)
        {
            b = ConnectionManager.Instance.cells.Count - 1;
        }

        if (ConnectionManager.Instance.cells[a].isFull)
        {
            ConnectionManager.Instance.cells[a].pieceHeld.isLocked = false;

            //ConnectionManager.Instance.cells[a].lockSprite.SetActive(false);
            Destroy(ConnectionManager.Instance.cells[a].pieceHeld.leftChild.transform.GetChild(0).gameObject);
        }

        if(ConnectionManager.Instance.cells[b].isFull)
        {
            //ConnectionManager.Instance.cells[b].pieceHeld.isLocked = false;

            Destroy(ConnectionManager.Instance.cells[b].pieceHeld.leftChild.transform.GetChild(0).gameObject);
        }

        toWorkOn.ResetDate();

        yield return new WaitForEndOfFrame();
        Destroy(toWorkOn.gameObject);

        FinishedUsingPowerup(true, prop);

        Debug.Log("Slice Bomb");

    }
    public IEnumerator FourColorPower(PowerupProperties prop)
    {
        layerToHit = LayerMask.GetMask("Sub Piece");
        yield return new WaitUntil(() => HasUsedPowerUp == true);
        SubPiece toWorkOn = ObjectToUsePowerUpOn.GetComponent<SubPiece>();

        Piece par = toWorkOn.transform.parent.GetComponent<Piece>();

        if(toWorkOn.colorOfPiece != prop.transformColor && !par.isLocked && par.partOfBoard)
        {
            par.transform.parent.GetComponent<Cell>().RemovePiece();

            toWorkOn.colorOfPiece = prop.transformColor;

            toWorkOn.RefreshPiece();

            par.transform.parent.GetComponent<Cell>().AddPiece(par.transform, false);

            FinishedUsingPowerup(par.partOfBoard, prop);

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

        if(toWorkOn.symbolOfPiece != prop.transformSymbol && par.partOfBoard && !par.isLocked)
        {
            par.transform.parent.GetComponent<Cell>().RemovePiece();

            toWorkOn.symbolOfPiece = prop.transformSymbol;

            toWorkOn.RefreshPiece();

            par.transform.parent.GetComponent<Cell>().AddPiece(par.transform, false);

            FinishedUsingPowerup(par.partOfBoard, prop);

            Debug.Log("Four Symbol");
        }
        else
        {
            FinishedUsingPowerup(false, prop);
        }
    }
    public void UsingPowerup(Button butt)
    {
        currentlyInUse = butt.gameObject.GetComponent<PowerupProperties>();
        UIManager.Instance.ActivateUsingPowerupMessage(true);

        foreach (Button but in powerupButtons)
        {
            if (but != butt)
            {
                but.interactable = false;
            }
        }

        StartCoroutine(WaitForEndFrame());
    }
    public IEnumerator WaitForEndFrame()
    {
        yield return new WaitForEndOfFrame();
        IsUsingPowerUp = true;
    }

    public void FinishedUsingPowerup(bool successfull, PowerupProperties prop)
    {
        UIManager.Instance.ActivateUsingPowerupMessage(false);

        IsUsingPowerUp = false;
        currentlyInUse = null;
        HasUsedPowerUp = false;
        layerToHit = new LayerMask();

        if (successfull)
        {
            prop.numOfUses--;
        }
       
        foreach (Button but in powerupButtons)
        {
            if (but.gameObject.GetComponent<PowerupProperties>().numOfUses > 0)
            {
                but.interactable = true;
            }
            else
            {
                but.interactable = false;
            }
        }

        if(prop.numOfUses == 0 && prop.connectedEquipment.scopeOfUses == 0) //// if the num of uses is 0 and the scope is cooldown and not per match
        {
            EquipmentData ED = PlayerManager.Instance.ownedPowerups.Where(p => p.name == prop.connectedEquipment.name).Single();

            ED.nextTimeAvailable = System.DateTime.Now.AddSeconds(ED.timeForCooldown).ToString(); ///// change the datetime for equipment on player

            PlayerManager.Instance.equipmentInCooldown.Add(ED);
            PlayerManager.Instance.SavePlayerData();
        }
    }
}
