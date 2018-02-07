using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{

    [SerializeField]
    Respawn initialRespawn;
    [SerializeField]
    float RespawnState = 0;

    [SerializeField]
    AudioClip SFXPlayerRespawn;
    [SerializeField]
    AudioClip SFXRespawnActivate;
    [SerializeField]
    AudioClip SFXRespawnActiveLoop;

    [SerializeField]
    float SFXVolume;

    private ParticleEmitter emitterActive;
    private ParticleEmitter emitterInactive;
    private ParticleEmitter emitterRespawn1;
    private ParticleEmitter emitterRespawn2;
    private ParticleEmitter emitterRespawn3;

    private Light respawnLight;
    public static Respawn currentRespawn;


    void Start()
    {
        emitterActive = transform.Find("RSParticlesActive").GetComponent<ParticleEmitter>();
        emitterInactive = transform.Find("RSParticlesInactive").GetComponent<ParticleEmitter>();
        emitterRespawn1 = transform.Find("RSParticlesRespawn1").GetComponent<ParticleEmitter>();
        emitterRespawn2 = transform.Find("RSParticlesRespawn2").GetComponent<ParticleEmitter>();
        emitterRespawn3 = transform.Find("RSParticlesRespawn3").GetComponent<ParticleEmitter>();

        respawnLight = transform.Find("RSSpotlight").GetComponent<Light>();
        RespawnState = 0;

        if (SFXRespawnActiveLoop)
        {
            GetComponent<AudioSource>().clip = SFXRespawnActiveLoop;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().playOnAwake = false;
        }

        currentRespawn = initialRespawn;
        if (currentRespawn == this) SetActive();
    }

    void OnTriggerEnter()
    {
        if (currentRespawn != this)
        {
            currentRespawn.SetInactive();

            if (SFXRespawnActivate)
                AudioSource.PlayClipAtPoint(SFXRespawnActivate, transform.position, SFXVolume);

            currentRespawn = this;
            SetActive();
        }
    }

    void SetActive()
    {
        emitterActive.emit = true;
        emitterInactive.emit = false;
        respawnLight.intensity = 1.5f;

        GetComponent<AudioSource>().Play();
    }

    void SetInactive()
    {
        emitterActive.emit = false;
        emitterInactive.emit = true;
        respawnLight.intensity = 1.5f;

        GetComponent<AudioSource>().Stop();
    }

    public void FireEffect()
    {

        emitterRespawn1.Emit();
        emitterRespawn2.Emit();
        emitterRespawn3.Emit();
        respawnLight.intensity = 3.5f;

        if (SFXPlayerRespawn)
        {
            AudioSource.PlayClipAtPoint(SFXPlayerRespawn, transform.position, SFXVolume);
        }

        //yield WaitForSeconds (2);
        respawnLight.intensity = 2.0f;
    }
}