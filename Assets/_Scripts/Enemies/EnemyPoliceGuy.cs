using UnityEngine;
using System.Collections;

public class EnemyPoliceGuy : MonoBehaviour
{
    [SerializeField]
    float attackTurnTime = 0.7f;
    [SerializeField]
    float rotateSpeed = 120.0f;
    [SerializeField]
    float attackDistance = 17.0f;
    [SerializeField]
    float extraRunTime = 2.0f;
    [SerializeField]
    float damage = 1;
    [SerializeField]
    float attackSpeed = 5.0f;
    [SerializeField]
    float attackRotateSpeed = 20.0f;
    [SerializeField]
    float idleTime = 1.6f;
    [SerializeField]
    Vector3 punchPosition = new Vector3(0.4f, 0, 0.7f);
    [SerializeField]
    float punchRadius = 1.1f;

    [SerializeField]
    AudioClip idleSound;
    [SerializeField]
    AudioClip attackSound;

    float attackAngle = 10.0f;
    bool isAttacking = false;
    float lastPunchTime = 0.0f;

    [SerializeField]
    Transform target;

    CharacterController characterController;

    [SerializeField]
    LevelStatus levelStateMachine;
    [SerializeField]
    Animation anim;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    IEnumerator Start()
    {
        levelStateMachine = GameObject.Find("/Level").GetComponent<LevelStatus>();

        if (!target) target = GameObject.FindWithTag("Player").transform;

        anim.wrapMode = WrapMode.Loop;

        anim.Play("idle");
        anim["threaten"].wrapMode = WrapMode.Once;
        anim["turnjump"].wrapMode = WrapMode.Once;
        anim["gothit"].wrapMode = WrapMode.Once;
        anim["gothit"].layer = 1;

        GetComponent<AudioSource>().clip = idleSound;

        yield return new WaitForSeconds(Random.value);

        while (true)
        {
            yield return Idle();
            yield return Attack();
        }
    }

    IEnumerator Idle()
    {

        if (idleSound)
        {
            if (GetComponent< AudioSource > ().clip != idleSound)
            {
                GetComponent< AudioSource > ().Stop();
                GetComponent< AudioSource > ().clip = idleSound;
                GetComponent< AudioSource > ().loop = true;
                GetComponent< AudioSource > ().Play();
            }
        }

        yield return new WaitForSeconds(idleTime);

        while (true)
        {
            characterController.SimpleMove(Vector3.zero);
            yield return new WaitForSeconds(0.2f);

            Vector3 offset = transform.position - target.position;

            if (offset.magnitude < attackDistance) yield break;
        }
    }

    float RotateTowardsPosition(Vector3 targetPos, float rotateSpeed)
    {

        Vector3 relative = transform.InverseTransformPoint(targetPos);
        float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
        float maxRotation = rotateSpeed * Time.deltaTime;
        float clampedAngle = Mathf.Clamp(angle, -maxRotation, maxRotation);

        transform.Rotate(0, clampedAngle, 0);
        return angle;
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        if (attackSound)
        {
            if (GetComponent<AudioSource>().clip != attackSound)
            {
                GetComponent<AudioSource>().Stop();
                GetComponent<AudioSource>().clip = attackSound;
                GetComponent<AudioSource>().loop = true;
                GetComponent<AudioSource>().Play();
            }
        }

        anim.Play("attackrun");

        float angle;
        angle = 180.0f;
        float time;
        time = 0.0f;
        Vector3 direction;

        while (angle > 5 || time < attackTurnTime)
        {
            time += Time.deltaTime;
            angle = Mathf.Abs(RotateTowardsPosition(target.position, rotateSpeed));
            float move = Mathf.Clamp01((90 - angle) / 90);

            anim["attackrun"].weight = anim["attackrun"].speed = move;
            direction = transform.TransformDirection(Vector3.forward * attackSpeed * move);
            characterController.SimpleMove(direction);

            yield return 0;
        }

        float timer = 0.0f;
        bool lostSight = false;
        while (timer < extraRunTime)
        {
            angle = RotateTowardsPosition(target.position, attackRotateSpeed);

            if (Mathf.Abs(angle) > 40) lostSight = true;

            if (lostSight) timer += Time.deltaTime;

            direction = transform.TransformDirection(Vector3.forward * attackSpeed);
            characterController.SimpleMove(direction);

            Vector3 pos = transform.TransformPoint(punchPosition);
            if (Time.time > lastPunchTime + 0.3f && (pos - target.position).magnitude < punchRadius)
            {
                target.SendMessage("ApplyDamage", damage);
                Vector3 slamDirection = transform.InverseTransformDirection(target.position - transform.position);
                slamDirection.y = 0;
                slamDirection.z = 1;
                if (slamDirection.x >= 0)
                    slamDirection.x = 1;
                else
                    slamDirection.x = -1;
                target.SendMessage("Slam", transform.TransformDirection(slamDirection));
                lastPunchTime = Time.time;
            }

            if (characterController.velocity.magnitude < attackSpeed * 0.3f) break;

            yield return 0;
        }

        isAttacking = false;
        anim.CrossFade("idle");
    }

    void ApplyDamage()
    {
        anim.CrossFade("gothit");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.TransformPoint(punchPosition), punchRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}