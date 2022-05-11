using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //public static CameraShake Instance;

    public Vector3 Amount = new Vector3(1f, 1f, 0);

    public float Duration = 1;

    public float Speed = 10;

    public bool ResetPosition = true;

    //Camera Camera;
    float time = 0;
    Vector3 lastPos;
    Vector3 nextPos;
    float lastFoV;
    float nextFoV;
    bool destroyAfterPlay;

    //private void Awake()
    //{
    //    //Instance = this;
    //    //Camera = GetComponent<Camera>();
    //}


    public void ShakeOnce()
    {
        //Debug.Log("Shake");
        destroyAfterPlay = false;
        Shake();
    }

    public void Shake()
    {
        //ResetCam();
        time = Duration;
    }

    private void LateUpdate()
    {
        if (time > 0)
        {
            //do something
            time -= Time.deltaTime;
            if (time > 0)
            {
                //next position based on perlin noise
                nextPos = (Mathf.PerlinNoise(time * Speed, time * Speed * 2) - 0.5f) * Amount.x * transform.right +
                          (Mathf.PerlinNoise(time * Speed * 2, time * Speed) - 0.5f) * Amount.y * transform.up;
                nextFoV = (Mathf.PerlinNoise(time * Speed * 2, time * Speed * 2) - 0.5f) * Amount.z;

                //Camera.fieldOfView += (nextFoV - lastFoV);
                gameObject.transform.Translate(ResetPosition ? (nextPos - lastPos) : nextPos);

                lastPos = nextPos;
                lastFoV = nextFoV;
            }
            else
            {
                //last frame
                ResetCam();
                if (destroyAfterPlay)
                    Destroy(this);
            }
        }
    }

    private void ResetCam()
    {
        //reset the last delta
        gameObject.transform.Translate(ResetPosition ? -lastPos : Vector3.zero);
        //Camera.fieldOfView -= lastFoV;

        //clear values
        lastPos = nextPos = Vector3.zero;
        lastFoV = nextFoV = 0f;
    }
}
