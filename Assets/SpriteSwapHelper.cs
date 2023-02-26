using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSwapHelper : MonoBehaviour
{
    public SpriteRenderer renderer;
    public GameObject childObject;

    public Sprite spriteA, spriteB;

    public AnimationClip noMatchIndicatorAnim; // used to get duration of animation
    private void OnValidate()
    {
        TryGetComponent<SpriteRenderer>(out renderer);

        if(renderer)
        {
            renderer.sprite = null;
        }

        if(transform.childCount > 0)
        {
            childObject = transform.GetChild(0).gameObject;
        }
    }

    public void SwitchSprites(bool versionA)
    {
        renderer.sprite = versionA == true ? spriteA : spriteB;

        childObject.SetActive(!versionA);

        renderer.color = Color.white;
    }

}
