using UnityEngine;
using System.Collections;

public class ThirdPersonPlayerAnimation : MonoBehaviour
{

    [SerializeField]
    float runSpeedScale = 1.0f;
    [SerializeField]
    float walkSpeedScale = 1.0f;
    [SerializeField]
    Animation anim;

    void Start()
    {

        anim.wrapMode = WrapMode.Loop;

        anim["run"].layer = -1;
        anim["walk"].layer = -1;
        anim["idle"].layer = -2;
        anim.SyncLayer(-1);

        anim["ledgefall"].layer = 9;
        anim["ledgefall"].wrapMode = WrapMode.Loop;

        anim["jump"].layer = 10;
        anim["jump"].wrapMode = WrapMode.ClampForever;

        anim["jumpfall"].layer = 10;
        anim["jumpfall"].wrapMode = WrapMode.ClampForever;

        anim["jetpackjump"].layer = 10;
        anim["jetpackjump"].wrapMode = WrapMode.ClampForever;

        anim["jumpland"].layer = 10;
        anim["jumpland"].wrapMode = WrapMode.Once;

        anim["walljump"].layer = 11;
        anim["walljump"].wrapMode = WrapMode.Once;

        anim["buttstomp"].speed = 0.15f;
        anim["buttstomp"].layer = 20;
        anim["buttstomp"].wrapMode = WrapMode.Once;
        AnimationState punch = anim["punch"];
        punch.wrapMode = WrapMode.Once;

        anim.Stop();
        anim.Play("idle");
    }

    void Update()
    {
        ThirdPersonController playerController = GetComponent<ThirdPersonController>();
        float currentSpeed = playerController.GetSpeed();

        if (currentSpeed > playerController.walkSpeed)
        {
            anim.CrossFade("run");
            anim.Blend("jumpland", 0);
        }
        else if (currentSpeed > 0.1f)
        {
            anim.CrossFade("walk");
            anim.Blend("jumpland", 0);
        }
        else
        {
            anim.Blend("walk", 0.0f, 0.3f);
            anim.Blend("run", 0.0f, 0.3f);
            anim.Blend("run", 0.0f, 0.3f);
        }

        anim["run"].normalizedSpeed = runSpeedScale;
        anim["walk"].normalizedSpeed = walkSpeedScale;

        if (playerController.IsJumping())
        {
            if (playerController.IsControlledDescent())
            {
                anim.CrossFade("jetpackjump", 0.2f);
            }
            else if (playerController.HasJumpReachedApex())
            {
                anim.CrossFade("jumpfall", 0.2f);
            }
            else
            {
                anim.CrossFade("jump", 0.2f);
            }
        }
        else if (!playerController.IsGroundedWithTimeout())
        {
            anim.CrossFade("ledgefall", 0.2f);
        }
        else
        {
            anim.Blend("ledgefall", 0.0f, 0.2f);
        }
    }

    void DidLand()
    {
        anim.Play("jumpland");
    }

    void DidButtStomp()
    {
        anim.CrossFade("buttstomp", 0.1f);
        anim.CrossFadeQueued("jumpland", 0.2f);
    }

    IEnumerator Slam()
    {
        anim.CrossFade("buttstomp", 0.2f);
        ThirdPersonController playerController = GetComponent<ThirdPersonController>();
        while (!playerController.IsGrounded())
        {
            yield return 0;
        }
        anim.Blend("buttstomp", 0, 0);
    }

    void DidWallJump()
    {
        anim.Play("walljump");
    }
}