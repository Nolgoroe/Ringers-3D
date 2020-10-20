using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PowerUp
{
    Joker,
    Switch,
    PieceBomb,
    SliceBomb,
    ExtraDeal,
    FourColorTransform,
    FourShapeTransform,
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
        theButton.onClick.AddListener(() => UsingPowerup(theButton));

        PowerupProperties prop = theButton.gameObject.GetComponent<PowerupProperties>();
        switch (ThePower)
        {
            case PowerUp.Joker:
                theButton.onClick.AddListener(() => CallJokerCoroutine(prop));
                break;
            case PowerUp.Switch:
                theButton.onClick.AddListener(() => CallSwitchPowerCoroutine(prop));
                break;
            case PowerUp.PieceBomb:
                theButton.onClick.AddListener(() => CallPieceBombPowerCoroutine(prop));
                break;
            case PowerUp.SliceBomb:
                theButton.onClick.AddListener(() => CallSliceBombPowerCoroutine(prop));
                break;
            case PowerUp.ExtraDeal:
                theButton.onClick.AddListener(() => ExtraDealPower(prop));
                break;
            case PowerUp.FourColorTransform:
                theButton.onClick.AddListener(() => CallFourColorPowerCoroutine(prop));
                break;
            case PowerUp.FourShapeTransform:
                theButton.onClick.AddListener(() => CallFourSymbolPowerCoroutine(prop));
                break;
            default:
                break;
        }
    }
    public void InstantiatePowerUps()
    {
        for (int i = 0; i < GameManager.Instance.playerManager.activePowerups.Count; i++)
        {
            GameObject go = Instantiate(powerupButtonPreab, instnatiateZones[i]);

            PowerupProperties prop = go.GetComponent<PowerupProperties>();

            PowerUp current = GameManager.Instance.playerManager.activePowerups[i];

            go.name = current.ToString();

            prop.SetProperties(current);

            AssignPowerUp(current, go.GetComponent<Button>());

            powerupButtons.Add(go.GetComponent<Button>());
        }
    }

    public void Deal()
    {
        GameManager.Instance.clipManager.clipCount--;
        if(GameManager.Instance.clipManager.clipCount == 0)
        {
            Debug.Log("Lose game");
        }

        GameManager.Instance.clipManager.RefreshSlots();
    }
    public void ExtraDealPower(PowerupProperties prop)
    {
        GameManager.Instance.clipManager.ExtraDealSlots();

        FinishedUsingPowerup(true, prop);

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

        if (toWorkOn.partOfBoard && !toWorkOn.isLocked)
        {
            toWorkOn.transform.parent.GetComponent<Cell>().RemovePiece();

            toWorkOn.rightChild.symbolOfPiece = PieceSymbol.Joker;
            toWorkOn.rightChild.colorOfPiece = PieceColor.Joker;

            toWorkOn.leftChild.symbolOfPiece = PieceSymbol.Joker;
            toWorkOn.leftChild.colorOfPiece = PieceColor.Joker;

            toWorkOn.rightChild.RefreshPiece();
            toWorkOn.leftChild.RefreshPiece();

            toWorkOn.transform.parent.GetComponent<Cell>().AddPiece(toWorkOn.transform, false);
        }

        FinishedUsingPowerup(toWorkOn.partOfBoard && !toWorkOn.isLocked, prop);

        Debug.Log("Joker");
    }
    public IEnumerator SwitchPower(PowerupProperties prop)
    {
        layerToHit = LayerMask.GetMask("Piece Parent");
        yield return new WaitUntil(() => HasUsedPowerUp == true);
        Piece toWorkOn = ObjectToUsePowerUpOn.GetComponent<Piece>();

        if (toWorkOn.partOfBoard && !toWorkOn.isLocked)
        {
            toWorkOn.transform.parent.GetComponent<Cell>().RemovePiece();

            PieceColor tempColor = toWorkOn.rightChild.colorOfPiece;
            PieceSymbol tempSymbol = toWorkOn.rightChild.symbolOfPiece;

            toWorkOn.rightChild.colorOfPiece = toWorkOn.leftChild.colorOfPiece;
            toWorkOn.rightChild.symbolOfPiece = toWorkOn.leftChild.symbolOfPiece;

            toWorkOn.leftChild.colorOfPiece = tempColor;
            toWorkOn.leftChild.symbolOfPiece = tempSymbol;

            toWorkOn.rightChild.RefreshPiece();
            toWorkOn.leftChild.RefreshPiece();

            toWorkOn.transform.parent.GetComponent<Cell>().AddPiece(toWorkOn.transform, false);
        }

        FinishedUsingPowerup(toWorkOn.partOfBoard && !toWorkOn.isLocked, prop);
        Debug.Log("Switch");

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
                toWorkOn.transform.parent.GetComponent<Cell>().lockSprite.SetActive(false);
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
            b = GameManager.Instance.connectionManager.cells.Length - 1;
        }

        if (GameManager.Instance.connectionManager.cells[a].isFull)
        {
            GameManager.Instance.connectionManager.cells[a].pieceHeld.isLocked = false;

            GameManager.Instance.connectionManager.cells[a].lockSprite.SetActive(false);

        }

        if(GameManager.Instance.connectionManager.cells[b].isFull)
        {
            GameManager.Instance.connectionManager.cells[b].pieceHeld.isLocked = false;

            GameManager.Instance.connectionManager.cells[b].lockSprite.SetActive(false);
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

        if (par.partOfBoard && !par.isLocked)
        {
            par.transform.parent.GetComponent<Cell>().RemovePiece();

            toWorkOn.colorOfPiece = prop.transformColor;

            toWorkOn.RefreshPiece();

            par.transform.parent.GetComponent<Cell>().AddPiece(par.transform, false);

        }
        FinishedUsingPowerup(par.partOfBoard, prop);

        Debug.Log("Four Color");

    }
    public IEnumerator FourSymbolPower(PowerupProperties prop)
    {
        layerToHit = LayerMask.GetMask("Sub Piece");
        yield return new WaitUntil(() => HasUsedPowerUp == true);
        SubPiece toWorkOn = ObjectToUsePowerUpOn.GetComponent<SubPiece>();

        Piece par = toWorkOn.transform.parent.GetComponent<Piece>();

        if (par.partOfBoard && !par.isLocked)
        {
            par.transform.parent.GetComponent<Cell>().RemovePiece();

            toWorkOn.symbolOfPiece = prop.transformSymbol;

            toWorkOn.RefreshPiece();

            par.transform.parent.GetComponent<Cell>().AddPiece(par.transform, false);

        }
        FinishedUsingPowerup(par.partOfBoard, prop);

        Debug.Log("Four Symbol");

    }

    public void UsingPowerup(Button butt)
    {
        currentlyInUse = butt.gameObject.GetComponent<PowerupProperties>();

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
    }
}
