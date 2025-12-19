using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NpcSpawner : MonoBehaviour
{
    [Header("NPC Settings")]
    public GameObject npcPrefab;

    [Header("Limits")]
    public int maxNPCsAlive = 20;
    public int minNPCsPerRow = 1;
    public int maxNPCsPerRow = 3;

    [Header("Row Settings")]
    public float npcSpacing = 1.5f;
    public float minRowDistance = 3f;

    [Header("Movement")]
    public float minRowSpeed = 1.5f;
    public float maxRowSpeed = 4f;
    public float maxAngleOffset = 20f;

    [Header("Spawn Timing")]
    public float minTimeBetweenRows = 1f;
    public float maxTimeBetweenRows = 3f;

    [Header("Spawn Height")]
    public float spawnHeightOffset = 0.1f;

    [Header("Spawn Distance & Player")]
    public Transform player;
    public float spawnDistanceFromPlayer = 10f;

    [Header("Despawn")]
    public float despawnDistanceFromPlayer = 25f;

    
    private class NpcRow
    {
        public List<GameObject> npcs = new List<GameObject>();
        public List<Transform> slots = new List<Transform>();
        public Vector3 direction;
        public float speed;
        public Vector3 center;
    }

    private List<NpcRow> activeRows = new List<NpcRow>();

    void Start()
    {
        StartCoroutine(RowSpawnRoutine());
    }

    void Update()
    {
        MoveRows();
        HandleDespawning();
    }

    IEnumerator RowSpawnRoutine()
    {
        while (true)
        {
            if (GetTotalNpcCount() < maxNPCsAlive)
                SpawnSingleRow();

            float wait = Random.Range(minTimeBetweenRows, maxTimeBetweenRows);
            yield return new WaitForSeconds(wait);
        }
    }

    void SpawnSingleRow()
    {
        int npcCount = Random.Range(minNPCsPerRow, maxNPCsPerRow + 1);
        npcCount = Mathf.Min(npcCount, maxNPCsAlive - GetTotalNpcCount());
        if (npcCount <= 0) return;

        float rowSpeed = Random.Range(minRowSpeed, maxRowSpeed);
        Vector3 baseDir = Random.value > 0.5f ? Vector3.forward : Vector3.back;
        float angle = Random.Range(-maxAngleOffset, maxAngleOffset);
        Vector3 rowDirection = Quaternion.Euler(0, angle, 0) * baseDir;
        Vector3 rowRight = Vector3.Cross(Vector3.up, rowDirection).normalized;

        Vector3 rowCenter = player.position + rowDirection.normalized * spawnDistanceFromPlayer;

        
        foreach (var row in activeRows)
            if (Vector3.Distance(rowCenter, row.center) < minRowDistance)
                return;

        NpcRow newRow = new NpcRow();
        newRow.direction = rowDirection;
        newRow.speed = rowSpeed;
        newRow.center = rowCenter;

        float halfWidth = (npcCount - 1) * npcSpacing * 0.5f;

        for (int i = 0; i < npcCount; i++)
        {
            Vector3 offset = rowRight * (i * npcSpacing - halfWidth);
            Vector3 intendedPos = rowCenter + offset;

            if (NavMesh.SamplePosition(intendedPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                
                Vector3 elevatedTarget = hit.position + Vector3.up * 1.0f;

                
                GameObject npc = Instantiate(npcPrefab, elevatedTarget, Quaternion.LookRotation(rowDirection));

                
                NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
                if (agent != null) agent.enabled = false;

                
                Collider npcCollider = npc.GetComponent<Collider>();
                if (npcCollider != null)
                {
                    float bottomY = npcCollider.bounds.min.y - npc.transform.position.y;

                    
                    npc.transform.position = elevatedTarget - new Vector3(0, bottomY, 0) + Vector3.up * (spawnHeightOffset + 1.0f);
                }

                
                if (agent != null) agent.enabled = true;

                
                GameObject slot = new GameObject("RowSlot");
                slot.transform.position = npc.transform.position + offset * 0.5f;
                slot.transform.parent = null;

                
                NpcMovement move = npc.GetComponent<NpcMovement>();
                if (move != null)
                    move.SetRowSlot(slot.transform, rowSpeed);

                newRow.npcs.Add(npc);
                newRow.slots.Add(slot.transform);
            }
        }

        activeRows.Add(newRow);
    }


    void MoveRows()
    {
        foreach (var row in activeRows)
        {
            row.center += row.direction.normalized * row.speed * Time.deltaTime;

            
            if (row.slots.Count == 0) continue;

            float halfWidth = (row.slots.Count - 1) * npcSpacing * 0.5f;
            Vector3 rowRight = Vector3.Cross(Vector3.up, row.direction).normalized;

            for (int i = 0; i < row.slots.Count; i++)
            {
                Vector3 offset = rowRight * (i * npcSpacing - halfWidth);
                row.slots[i].position = row.center + offset;
            }
        }
    }

    void HandleDespawning()
    {
        for (int i = activeRows.Count - 1; i >= 0; i--)
        {
            var row = activeRows[i];

            bool rowOffNavMesh = !NavMesh.SamplePosition(row.center, out _, 1f, NavMesh.AllAreas);
            bool rowTooFar = row.npcs.Count > 0 && Vector3.Distance(player.position, row.center) >= despawnDistanceFromPlayer;

            if (rowOffNavMesh || rowTooFar)
            {
                
                foreach (var npc in row.npcs)
                {
                    if (npc != null) Destroy(npc);
                }

                foreach (var slot in row.slots)
                {
                    if (slot != null) Destroy(slot.gameObject);
                }

                activeRows.RemoveAt(i);
                continue;
            }

            
            for (int j = row.npcs.Count - 1; j >= 0; j--)
            {
                GameObject npc = row.npcs[j];
                if (npc == null || !npc.GetComponent<NavMeshAgent>().isOnNavMesh)
                {
                    if (npc != null) Destroy(npc);
                    if (row.slots[j] != null) Destroy(row.slots[j].gameObject);
                    row.npcs.RemoveAt(j);
                    row.slots.RemoveAt(j);
                }
            }
        }
    }

    int GetTotalNpcCount()
    {
        int count = 0;
        foreach (var row in activeRows)
            count += row.npcs.Count;
        return count;
    }
}
