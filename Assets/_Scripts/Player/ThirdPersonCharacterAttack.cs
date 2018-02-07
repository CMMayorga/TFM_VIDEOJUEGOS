using UnityEngine;
using System.Collections;

public class ThirdPersonCharacterAttack : MonoBehaviour
{

    [SerializeField]
    float punchSpeed = 1;
    [SerializeField]
    float punchHitTime = 0.2f;
    [SerializeField]
    float punchTime = 0.4f;
    [SerializeField]
    Vector3 punchPosition = new Vector3(0, 0, 0.8f);
    [SerializeField]
    float punchRadius = 1.3f;
    [SerializeField]
    float punchHitPoints = 1;

    [SerializeField]
    AudioClip punchSound;

    private bool busy = false;

    [SerializeField]
    Animation anim;

    void Start()
    {
        anim["punch"].speed = punchSpeed;
    }

    void Update()
    {
        ThirdPersonController controller = GetComponent<ThirdPersonController>();
        if (!busy && Input.GetButtonDown("Fire1") && controller.IsGroundedWithTimeout() && !controller.IsMoving())
        {
            SendMessage("DidPunch");
            busy = true;
        }
    }

    IEnumerator DidPunch()
    {

        anim.CrossFadeQueued("punch", 0.1f, QueueMode.PlayNow);
        yield return new WaitForSeconds(punchHitTime);
        Vector3 pos = transform.TransformPoint(punchPosition);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject go in enemies)
        {
            EnemyDamage enemy = go.GetComponent<EnemyDamage>();
            if (enemy == null)
                continue;

            if (Vector3.Distance(enemy.transform.position, pos) < punchRadius)
            {
                enemy.SendMessage("ApplyDamage", punchHitPoints);

                if (punchSound) GetComponent<AudioSource>().PlayOneShot(punchSound);
            }
        }
        yield return new WaitForSeconds(punchTime - punchHitTime);
        busy = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.TransformPoint(punchPosition), punchRadius);
    }
}