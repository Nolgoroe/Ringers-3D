using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<PowerUp> activePowerups;

    private void Start()
    {
        GameManager.Instance.playerManager = this;
    }
}
