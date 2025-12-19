using UnityEngine;
using UnityEngine.AI;

public class NpcMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform rowSlot;

    [Header("Row Following")]
    public float maxDistanceFromRow = 2f;      
    public float catchUpSpeedMultiplier = 2f; 

    private float baseSpeed; 

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.autoBraking = false;
        agent.angularSpeed = 720f;
        agent.acceleration = 20f;
        agent.baseOffset = 0f;
    }

    void Update()
    {
        if (rowSlot == null || !agent.isOnNavMesh) return;

        Vector3 targetPos = rowSlot.position;
        float distance = Vector3.Distance(transform.position, targetPos);

        
        if (distance > maxDistanceFromRow)
            agent.speed = baseSpeed * catchUpSpeedMultiplier;
        else
            agent.speed = baseSpeed;

        
        agent.SetDestination(targetPos);
    }

    public void SetRowSlot(Transform slot, float moveSpeed)
    {
        rowSlot = slot;
        baseSpeed = moveSpeed; 
        if (agent != null)
            agent.speed = baseSpeed;
    }
}
