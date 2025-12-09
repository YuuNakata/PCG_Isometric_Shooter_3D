using UnityEngine;
using System.Collections;

public class LevelRenderer3D : MonoBehaviour
{
    public GameObject floorPrefab, wallPrefab, spawnPrefab, exitPrefab;
    [Header("Entities")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    [Header("Parameters")]
    public int width = 30;
    public int height = 30;
    public int seed = 12345;
    public float roomDensity = 0.15f;

    private LevelData currentLevelData;

    void Start()
    {
        StartDraw();
    }

    void Draw(LevelData data)
    {
        foreach (Transform child in transform) Destroy(child.gameObject);

        for (int x = 0; x < data.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < data.tiles.GetLength(1); y++)
            {
                Vector3 pos = new(x, 0, y);
                switch (data.tiles[x, y])
                {
                    case TileType.Floor: Instantiate(floorPrefab, pos, Quaternion.identity, transform); break;
                    case TileType.Wall: 
                        var w = Instantiate(wallPrefab, pos, Quaternion.identity, transform); 
                        w.transform.localScale += Vector3.up * 2f;
                        break;
                    case TileType.Spawn: Instantiate(spawnPrefab, pos + Vector3.up * 2f, Quaternion.identity, transform); break;
                    case TileType.Exit: Instantiate(exitPrefab, pos + Vector3.up * 2f, Quaternion.identity, transform); break;
                }
            }
        }
    }

    public void StartDraw()
    {
        var rng = new System.Random();
        var gen = new RoomsCorridorsGenerator();
        currentLevelData = gen.Generate(new LevelParams(width, height, seed, roomDensity), rng);
        Draw(currentLevelData);
        
        // Move Camera to Start
        if(Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(currentLevelData.start.x, 10, currentLevelData.start.y - 10);
        }

        // Use coroutine to ensure NavMesh is ready before spawning entities
        StartCoroutine(BakeNavMeshAndSpawn());
    }

    IEnumerator BakeNavMeshAndSpawn()
    {
        // Wait one frame for geometry to be fully created
        yield return null;
        
        BakeNavMesh();
        
        // Wait another frame for NavMesh to be ready
        yield return null;
        yield return new WaitForSeconds(0.1f);
        
        SpawnEntities(currentLevelData);
        
        Debug.Log("Entities spawned after NavMesh bake");
    }

    void BakeNavMesh()
    {
        Component[] components = GetComponents<Component>();
        foreach(var c in components)
        {
            if(c.GetType().Name == "NavMeshSurface")
            {
                c.GetType().GetMethod("BuildNavMesh")?.Invoke(c, null);
                Debug.Log("NavMesh Baked via Reflection");
                return;
            }
        }
        Debug.LogWarning("NavMeshSurface not found! Please add 'NavMeshSurface' component to LevelRenderer object for AI to work.");
    }

    void SpawnEntities(LevelData data)
    {
        if (playerPrefab != null)
        {
            // Spawn slightly above ground so NavMeshAgent can snap down
            Vector3 startPos = new Vector3(data.start.x, 0.5f, data.start.y);
            GameObject player = Instantiate(playerPrefab, startPos, Quaternion.identity, transform);
            Debug.Log($"Player spawned at {startPos}");
        }

        if (enemyPrefab != null && data.rooms.Count > 1)
        {
            for (int i = 1; i < data.rooms.Count; i++)
            {
                if (Random.value < 0.5f)
                {
                    var room = data.rooms[i];
                    Vector3 roomCenter = new Vector3(room.rect.center.x, 0.5f, room.rect.center.y);
                    Instantiate(enemyPrefab, roomCenter, Quaternion.identity, transform);
                }
            }
        }
    }
}
