using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleTween : MonoBehaviour
{
    public Vector3 scaleTo;
    private void Start()
    {
        LeanTween.scale(gameObject, scaleTo, 1);
    }

    public IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
