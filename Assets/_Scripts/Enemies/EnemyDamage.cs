using UnityEngine;
using System.Collections;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField]
    float hitPoints = 3;
    [SerializeField]
    Transform explosionPrefab;
    [SerializeField]
    Transform deadModelPrefab;
    [SerializeField]
    DroppableMover healthPrefab;
    [SerializeField]
    DroppableMover fuelPrefab;
    [SerializeField]
    float dropMin = 0;
    [SerializeField]
    float dropMax = 0;
    [SerializeField]
    AudioClip struckSound;
    private bool dead = false;

    void ApplyDamage(int damage)
    {

        if (GetComponent<AudioSource>() && struckSound)
        {
            GetComponent<AudioSource>().PlayOneShot(struckSound);
        }

        if (hitPoints <= 0) return;

        hitPoints -= damage;
        if (!dead && hitPoints <= 0)
        {
            Die();
            dead = true;
        }
    }

    void Die()
    {

        Destroy(gameObject);
        Transform deadModel = Instantiate(deadModelPrefab, transform.position, transform.rotation);
        CopyTransformsRecurse(transform, deadModel);

        Transform effect = Instantiate(explosionPrefab, transform.position, transform.rotation);
        effect.parent = deadModel;

        Rigidbody deadModelRigidbody = deadModel.GetComponent<Rigidbody>();
        Vector3 relativePlayerPosition = transform.InverseTransformPoint(Camera.main.transform.position);
        deadModelRigidbody.AddTorque(Vector3.up * 7);
        if (relativePlayerPosition.z > 0)
            deadModelRigidbody.AddForceAtPosition(-transform.forward * 2, transform.position + (transform.up * 5), ForceMode.Impulse);
        else
            deadModelRigidbody.AddForceAtPosition(transform.forward * 2, transform.position + (transform.up * 2), ForceMode.Impulse);

        float toDrop = Random.Range(dropMin, dropMax + 1);
        for (float i = 0; i < toDrop; i++)
        {
            Vector3 direction = Random.onUnitSphere;
            if (direction.y < 0)
            {
                direction.y = -direction.y;
            }

            Vector3 dropPosition = transform.TransformPoint(Vector3.up * 1.5f) + (direction / 2);
            DroppableMover dropped;

            if (Random.value > 0.5f)
                dropped = Instantiate(healthPrefab, dropPosition, Quaternion.identity);
            else
                dropped = Instantiate(fuelPrefab, dropPosition, Quaternion.identity);

            dropped.Bounce(direction * 4 * (Random.value + 0.2f));
        }
    }

    static void CopyTransformsRecurse(Transform src, Transform dst)
    {
        dst.position = src.position;
        dst.rotation = src.rotation;

        foreach (Transform child in dst)
        {
            Transform curSrc = src.Find(child.name);
            if (curSrc) CopyTransformsRecurse(curSrc, child);
        }
    }
}