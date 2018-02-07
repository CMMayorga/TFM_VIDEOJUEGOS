using UnityEngine;
using System.Collections;

public class FenceTextureOffset : MonoBehaviour
{

    [SerializeField]
    float scrollSpeed = 0.25f;

    void FixedUpdate()
    {
        float offset = Time.time * scrollSpeed;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, offset);
    }
}