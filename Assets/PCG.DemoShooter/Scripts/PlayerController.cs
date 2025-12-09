using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float movementSpeed = 5f;
    public LayerMask groundLayer;
    
    [Header("Combat")]
    public float attackRange = 2f;
    public float attackDamage = 25f;
    public float attackCooldown = 0.5f;
    
    [Header("Visuals")]
    public GameObject selectionCircle;

    private NavMeshAgent agent;
    private bool isSelected = false;
    private Camera mainCamera;
    
    // Combat state
    private Transform currentTarget;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(agent != null)
        {
            agent.speed = movementSpeed;
            agent.updateRotation = false;
            agent.updateUpAxis = true;
        }

        mainCamera = Camera.main;
        UpdateSelectionVisual();
    }

    void Update()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (Mouse.current == null) return;

        HandleSelectionInput();
        HandleActionInput();
        
        // If we have a target, chase and attack
        if (currentTarget != null)
        {
            ChaseAndAttackTarget();
        }
    }

    void HandleSelectionInput()
    {
        // Left Click Select
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    isSelected = true;
                }
                else
                {
                    if (!hit.transform.CompareTag("Player"))
                    {
                        isSelected = false;
                    }
                }
                UpdateSelectionVisual();
            }
        }
    }

    void HandleActionInput()
    {
        if (!isSelected) 
        {
            return;
        }

        // Right Click = Move or Attack
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Debug.Log("Right click detected - Player is selected");
            
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            
            // First check if we clicked an enemy (priority)
            if (Physics.Raycast(ray, out RaycastHit directHit))
            {
                Debug.Log($"Direct hit on: {directHit.transform.name} (Tag: {directHit.transform.tag})");
                
                if (directHit.transform.CompareTag("Enemy"))
                {
                    currentTarget = directHit.transform;
                    Debug.Log("Target set to enemy");
                    return;
                }
            }
            
            // Otherwise, move to ground
            currentTarget = null;
            
            RaycastHit[] hits = Physics.RaycastAll(ray, 200f);
            Debug.Log($"RaycastAll found {hits.Length} hits");
            
            RaycastHit? floorHit = null;
            float lowestY = float.MaxValue;
            
            foreach (var hit in hits)
            {
                Debug.Log($"  Hit: {hit.transform.name} at Y={hit.point.y} (Tag: {hit.transform.tag})");
                
                if (hit.transform.CompareTag("Player") || hit.transform.CompareTag("Enemy")) continue;
                
                if (hit.point.y < lowestY)
                {
                    lowestY = hit.point.y;
                    floorHit = hit;
                }
            }
            
            if (floorHit.HasValue)
            {
                Debug.Log($"Floor hit found at {floorHit.Value.point}");
                
                if (agent != null)
                {
                    Debug.Log($"Agent isOnNavMesh: {agent.isOnNavMesh}");
                    
                    if (agent.isOnNavMesh)
                    {
                        agent.SetDestination(floorHit.Value.point);
                        agent.isStopped = false;
                        Debug.Log("Destination set!");
                    }
                    else
                    {
                        Debug.LogWarning("Agent is NOT on NavMesh! Check NavMeshSurface baking and Agent Radius.");
                    }
                }
                else
                {
                    Debug.LogError("NavMeshAgent is null!");
                }
            }
            else
            {
                Debug.LogWarning("No floor hit found!");
            }
        }
    }
    
    void ChaseAndAttackTarget()
    {
        if (currentTarget == null)
        {
            return;
        }
        
        float distance = Vector3.Distance(transform.position, currentTarget.position);
        
        if (distance <= attackRange)
        {
            // Stop moving and attack
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
            
            // Attack on cooldown
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack(currentTarget);
                lastAttackTime = Time.time;
            }
        }
        else
        {
            // Chase the target
            if (agent != null && agent.isOnNavMesh)
            {
                agent.SetDestination(currentTarget.position);
                agent.isStopped = false;
            }
        }
    }
    
    void Attack(Transform target)
    {
        Health targetHealth = target.GetComponent<Health>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage);
            Debug.Log($"Player attacked {target.name} for {attackDamage} damage!");
        }
        
        // If target died, clear it
        if (target == null || targetHealth == null || targetHealth.currentHealth <= 0)
        {
            currentTarget = null;
        }
    }

    void UpdateSelectionVisual()
    {
        if (selectionCircle != null)
        {
            selectionCircle.SetActive(isSelected);
        }
    }
}
