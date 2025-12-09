using UnityEngine;
using System.Collections.Generic;

public class ObstructingWallFader : MonoBehaviour
{
    [Header("Settings")]
    public Transform player; // Optional: Will auto-find "Player" tag if null
    public LayerMask obstacleLayer = ~0; // Default to everything
    public float fadedAlpha = 0.3f;
    public float fadeSpeed = 10f;
    
    [Header("Tag Filtering")]
    public bool filterByTag = false;
    public string obstacleTag = "Wall"; // Only fade if tag matches (optional)

    private class FadedObject
    {
        public Renderer renderer;
        public Material[] originalMaterials; // Use to restore or just lerp alpha
        public float currentAlpha;
    }

    private Dictionary<Renderer, FadedObject> currentlyFaded = new Dictionary<Renderer, FadedObject>();

    void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else return;
        }

        Vector3 dir = player.position - transform.position;
        float dist = dir.magnitude;

        // Cast ray from Camera to Player
        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, dist, obstacleLayer);
        HashSet<Renderer> hitsThisFrame = new HashSet<Renderer>();

        foreach (var hit in hits)
        {
            // Don't fade the player itself
            if (hit.transform == player) continue;

            // Optional Tag Filter
            if (filterByTag && !hit.transform.CompareTag(obstacleTag)) continue;

            Renderer r = hit.transform.GetComponent<Renderer>();
            if (r != null)
            {
                hitsThisFrame.Add(r);
                if (!currentlyFaded.ContainsKey(r))
                {
                    // Start fading
                    currentlyFaded.Add(r, new FadedObject { renderer = r, currentAlpha = 1f });
                }
            }
        }

        // Update alphas
        List<Renderer> toRemove = new List<Renderer>();

        foreach (var kvp in currentlyFaded)
        {
            Renderer r = kvp.Key;
            FadedObject obj = kvp.Value;

            float targetAlpha = hitsThisFrame.Contains(r) ? fadedAlpha : 1f;
            
            // Lerp Alpha
            obj.currentAlpha = Mathf.Lerp(obj.currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);

            // Apply Alpha
            if (r != null)
            {
                foreach (Material m in r.materials)
                {
                    if (m.HasProperty("_Color"))
                    {
                        Color c = m.color;
                        c.a = obj.currentAlpha;
                        m.color = c;
                    }
                    else if (m.HasProperty("_BaseColor")) // URP/HDRP often use BaseColor
                    {
                        Color c = m.GetColor("_BaseColor");
                        c.a = obj.currentAlpha;
                        m.SetColor("_BaseColor", c);
                    }
                }
            }

            // Cleanup if fully opaque and not hit
            if (!hitsThisFrame.Contains(r) && Mathf.Abs(obj.currentAlpha - 1f) < 0.01f)
            {
                // Snap to 1
                if (r != null)
                {
                    foreach (Material m in r.materials)
                    {
                        if (m.HasProperty("_Color"))
                        {
                            Color c = m.color;
                            c.a = 1f;
                            m.color = c;
                        }
                        else if (m.HasProperty("_BaseColor"))
                        {
                            Color c = m.GetColor("_BaseColor");
                            c.a = 1f;
                            m.SetColor("_BaseColor", c);
                        }
                    }
                }
                toRemove.Add(r);
            }
        }

        foreach (var r in toRemove)
        {
            currentlyFaded.Remove(r);
        }
    }
}
