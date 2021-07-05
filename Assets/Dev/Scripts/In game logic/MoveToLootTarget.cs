using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToLootTarget : MonoBehaviour
{
    public Transform target;

    public Sprite look;
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = look;

    }

    // Start is called before the first frame update

    public void LeanMove()
    {
        LTBezierPath ltPath = new LTBezierPath(new Vector3[] { transform.position, new Vector3(transform.position.x + 3, transform.position.y + 5f, -1f), new Vector3(transform.position.x + 7, transform.position.y + 10f, -1f), target.transform.position });

        LeanTween.move(gameObject, ltPath, 4.0f).setEase(LeanTweenType.easeInOutQuad).setDestroyOnComplete(true); // animate

        //LeanTween.move(gameObject, target, 2).setDestroyOnComplete(true);
    }
}
