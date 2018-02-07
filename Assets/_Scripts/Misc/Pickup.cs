using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{

    
    enum PickupType { Health = 0, FuelCell = 1 }
    [SerializeField]
    PickupType pickupType = PickupType.FuelCell;
    [SerializeField]
    int amount = 1;
    [SerializeField]
    AudioClip sound;
    private float soundVolume = 1.0f;
    private bool used = false;
    private DroppableMover mover;

    void Start()
    {
        mover = GetComponent<DroppableMover>();
    }

    bool ApplyPickup(ThirdPersonStatus playerStatus)
    {

        switch (pickupType)
        {
            case PickupType.Health:
                playerStatus.AddLife(amount);
                break;

            case PickupType.FuelCell:
                playerStatus.FoundItem(amount);
                break;
        }

        return true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (mover && mover.enabled) return;
        ThirdPersonStatus playerStatus = col.GetComponent<ThirdPersonStatus>();

        if (used || playerStatus == null) return;

        if (!ApplyPickup(playerStatus)) return;

        used = true;

        if (sound) PlayAudioClip(sound, transform.position, soundVolume);

        if (GetComponent<Animation>() && GetComponent<Animation>().clip)
        {
            GetComponent<Animation>().Play();
            Destroy(gameObject, GetComponent<Animation>().clip.length);
        }
        else
        {
            Destroy(gameObject);
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

    AudioSource PlayAudioClip(AudioClip clip, Vector3 position, float volume)
    {
        GameObject go = new GameObject("One shot audio");
        go.transform.position = position;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 1;
        source.volume = volume;
        source.Play();
        Destroy(go, clip.length);
        return source;
    }
}