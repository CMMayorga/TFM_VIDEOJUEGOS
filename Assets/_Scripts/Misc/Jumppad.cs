using UnityEngine;
using System.Collections;

public class Jumppad : MonoBehaviour
{

    [SerializeField]
    float jumpHeight = 5.0f;

    void OnTriggerEnter(Collider col)
    {
        ThirdPersonController controller = col.GetComponent<ThirdPersonController>();
        if (controller != null)
        {
            if (GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().Play();
            }

            controller.SuperJump(jumpHeight);
        }
    }

    void Reset()
    {
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
        GetComponent<Collider>().isTrigger = true;
    }

}