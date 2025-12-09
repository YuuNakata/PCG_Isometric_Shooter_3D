using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("AI Settings")]
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float updatePathRate = 0.5f;
    
    [Header("Combat")]
    public float attackDamage = 10f;
    public float attackCooldown = 1f;

    private NavMeshAgent agent;
    private Transform target;
    private float nextPathUpdate;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(agent != null)
        {
            agent.stoppingDistance = attackRange * 0.8f;
        }
    }

    void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= detectionRange)
        {
            if (distance <= attackRange)
            {
                // In attack range - stop and attack
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                }
                
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackTarget();
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                // Chase the player
                ChaseTarget();
            }
        }
    }

    void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void ChaseTarget()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        agent.isStopped = false;
        
        if (Time.time >= nextPathUpdate)
        {
            agent.SetDestination(target.position);
            nextPathUpdate = Time.time + updatePathRate;
        }
    }
    
    void AttackTarget()
    {
        if (target == null) return;
        
        Health playerHealth = target.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"Enemy attacked Player for {attackDamage} damage!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
