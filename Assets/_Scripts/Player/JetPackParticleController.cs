using UnityEngine;
using System.Collections;

public class JetPackParticleController : MonoBehaviour
{

    private float litAmount = 0.00f;

    IEnumerator Start()
    {

        ThirdPersonController playerController = GetComponent<ThirdPersonController>();
        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().Stop();

        Component[] particles = GetComponentsInChildren<ParticleEmitter>();
        Light childLight = GetComponentInChildren<Light>();

        foreach (ParticleEmitter p in particles)
        {
            p.emit = false;
        }

        childLight.enabled = false;

        while (true)
        {
            bool isFlying = playerController.IsJumping();

            if (isFlying)
            {
                if (!GetComponent<AudioSource>().isPlaying)
                {
                    GetComponent<AudioSource>().Play();
                }
            }
            else
            {
                GetComponent<AudioSource>().Stop();
            }


            foreach (ParticleEmitter p in particles)
            {
                p.emit = isFlying;
            }

            if (isFlying)
                litAmount = Mathf.Clamp01(litAmount + Time.deltaTime * 2);
            else
                litAmount = Mathf.Clamp01(litAmount - Time.deltaTime * 2);

            childLight.enabled = isFlying;
            childLight.intensity = litAmount * 2;

            yield return 0;
        }
    }

}