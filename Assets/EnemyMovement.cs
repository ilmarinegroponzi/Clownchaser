using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;

    [Header("Settings")]
    public float killDistance = 2f;

    [Header("Jumpscare Settings")]
    public Vector3 jumpscarePosition;
    public Vector3 jumpscareRotationEuler;
    public float jumpscareDuration = 3f;
    public GameObject returnToLobbyUI;

    private bool hasTriggered;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (returnToLobbyUI != null)
            returnToLobbyUI.SetActive(false);
    }

    public void SetTarget(Transform playerTransform)
    {
        target = playerTransform;
    }

    private void Update()
    {
        if (target == null || !agent.isOnNavMesh)
            return;

        agent.SetDestination(target.position);

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= killDistance && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(TriggerJumpscare());
        }
    }

    private IEnumerator TriggerJumpscare()
    {
        if (target != null)
        {
            
            CharacterController controller = target.GetComponent<CharacterController>();
            if (controller != null)
                controller.enabled = false;

            
            MonoBehaviour[] scripts = target.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script != this)
                    script.enabled = false;
            }

            
            target.position = jumpscarePosition;
            target.rotation = Quaternion.Euler(jumpscareRotationEuler);
        }

        
        yield return new WaitForSeconds(jumpscareDuration);

        
        if (returnToLobbyUI != null)
            returnToLobbyUI.SetActive(true);

        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
