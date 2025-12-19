using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;

    public float minSpawnTime = 2f;
    public float maxSpawnTime = 5f;

    public float minDespawnTime = 5f;
    public float maxDespawnTime = 10f;

    public float spawnRadius = 10f;

    [Range(0f, 1f)]
    public float spawnArc = 0.5f;

    public float movementThreshold = 0.1f;

    private GameObject currentEnemy;
    private Vector3 lastPlayerPosition;
    private Vector3 lastMoveDirection;

    private void Start()
    {
        lastPlayerPosition = player.position;
        lastMoveDirection = player.forward;
        StartCoroutine(SpawnLoop());
    }

    private void Update()
    {
        Vector3 delta = player.position - lastPlayerPosition;

        if (delta.magnitude > movementThreshold)
        {
            lastMoveDirection = delta.normalized;
        }

        lastPlayerPosition = player.position;
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            while (currentEnemy != null)
                yield return null;

            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            Vector3 spawnPos;
            if (RandomPointInMovementArc(out spawnPos))
            {
                currentEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                EnemyMovement move = currentEnemy.GetComponent<EnemyMovement>();
                if (move != null)
                    move.SetTarget(player);

                StartCoroutine(DespawnEnemy(currentEnemy));
            }
        }
    }

    private IEnumerator DespawnEnemy(GameObject enemy)
    {
        yield return new WaitForSeconds(Random.Range(minDespawnTime, maxDespawnTime));

        if (enemy != null)
        {
            Destroy(enemy);
            currentEnemy = null;
        }
    }

    private bool RandomPointInMovementArc(out Vector3 result)
    {
        Vector3 baseDirection = lastMoveDirection.sqrMagnitude > 0.01f
            ? lastMoveDirection
            : player.forward;

        baseDirection.y = 0;
        baseDirection.Normalize();

        for (int i = 0; i < 30; i++)
        {
            float halfAngle = Mathf.Lerp(90f, 0f, spawnArc);
            float angle = Random.Range(-halfAngle, halfAngle);

            Vector3 direction = Quaternion.Euler(0, angle, 0) * baseDirection;
            Vector3 randomPos = player.position + direction * spawnRadius;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 3f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}
