using UnityEngine;
using System.Collections;

public class FalloutDeath : MonoBehaviour
{

    void OnTriggerEnter(Collider col)
    {

        if (col.GetComponent<ThirdPersonStatus>())
        {
            col.GetComponent<ThirdPersonStatus>().FalloutDeath();
        }
        else if (col.attachedRigidbody)
        {
            Destroy(col.attachedRigidbody.gameObject);
        }
        else if (col.GetType() == typeof(CharacterController))
        {
            Destroy(col.gameObject);
        }
    }

}