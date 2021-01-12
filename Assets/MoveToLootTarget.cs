using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToLootTarget : MonoBehaviour
{
    public Transform target;

    public Sprite look;
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = look;
    }

    public void LeanMove()
    {
        LeanTween.move(gameObject, target, 2).setDestroyOnComplete(true);
    }
}
