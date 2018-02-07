using UnityEngine;
using System.Collections;

public class EnemyRespawn : MonoBehaviour
{

    [SerializeField]
    float spawnRange = 0.0f;
    [SerializeField]
    string gizmoName;
    [SerializeField]
    GameObject enemyPrefab;

    private Transform player;
    private GameObject currentEnemy;
    private bool wasOutside = true;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < spawnRange)
        {
            if (!currentEnemy && wasOutside)
                currentEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation);

            wasOutside = false;
        }
        else
        {
            if (currentEnemy && !wasOutside)
                Destroy(currentEnemy);

            wasOutside = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 1);
        Gizmos.DrawIcon(transform.position, gizmoName + ".psd");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1);
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}