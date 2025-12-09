using UnityEngine;

public class ExitZone : MonoBehaviour
{
    [Header("Settings")]
    public float triggerRadius = 1f;
    
    private LevelRenderer3D levelRenderer;

    void Start()
    {
        levelRenderer = FindObjectOfType<LevelRenderer3D>();
    }

    void Update()
    {
        // Check if player is near
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= triggerRadius)
            {
                OnPlayerReachedExit();
            }
        }
    }

    void OnPlayerReachedExit()
    {
        Debug.Log("Player reached exit! Generating new level...");
        
        if (levelRenderer != null)
        {
            // Generate new seed for variety
            levelRenderer.seed = Random.Range(0, 99999);
            levelRenderer.StartDraw();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
