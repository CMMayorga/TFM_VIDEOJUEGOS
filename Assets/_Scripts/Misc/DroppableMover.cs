using UnityEngine;
using System.Collections;

public class DroppableMover : MonoBehaviour
{

    [SerializeField]
    float gravity = 10.00f;
    [SerializeField]
    LayerMask collisionMask;

    private Vector3 velocity = Vector3.zero;
    private Vector3 position;

    public void Bounce(Vector3 force)
    {
        position = transform.position;
        velocity = force;
    }

    void Update()
    {
        velocity.y -= gravity * Time.deltaTime;
        Vector3 moveThisFrame = velocity * Time.deltaTime;
        float distanceThisFrame = moveThisFrame.magnitude;

        if (Physics.Raycast(position, moveThisFrame, distanceThisFrame, collisionMask))
        {
            enabled = false;
        }
        else
        {
            position += moveThisFrame;
            transform.position = new Vector3(position.x, position.y + 0.75f, position.z);
        }
    }
}