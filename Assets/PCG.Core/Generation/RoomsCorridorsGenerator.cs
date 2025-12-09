using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomsCorridorsGenerator : ILevelGenerator
{
    public LevelData Generate(LevelParams p, System.Random rng)
    {
        var level = new LevelData
        {
            tiles = new TileType[p.width, p.height]
        };

        // Inicializar todo como muro
        for (int x = 0; x < p.width; x++)
            for (int y = 0; y < p.height; y++)
                level.tiles[x, y] = TileType.Wall;

        // Colocar habitaciones
        int minRoomSize = Mathf.Max(2, Mathf.Min(p.width, p.height) / 6);
        int maxRoomSize = Mathf.Max(minRoomSize + 1, Mathf.Min(p.width, p.height) / 4);
        int targetRooms = Mathf.RoundToInt(((p.width * p.height) / (maxRoomSize * maxRoomSize))*p.roomDensity);
        var placed = new List<RectInt>();
        targetRooms = targetRooms >= 2 ? targetRooms : targetRooms = 2;
        Debug.Log(targetRooms);
        for (int i = 0; i < targetRooms; i++)
        {
           
            int w = rng.Next(minRoomSize, maxRoomSize);
            int h = rng.Next(minRoomSize, maxRoomSize);
            int x = rng.Next(1, p.width - w - 1);
            int y = rng.Next(1, p.height - h - 1);
            var rect = new RectInt(x, y, w, h);

            bool overlaps = placed.Exists(r => r.Overlaps(rect));
            int attemps = 0;
            while (overlaps && attemps <10)
            {
                w = rng.Next(minRoomSize, maxRoomSize);
                h = rng.Next(minRoomSize, maxRoomSize);
                x = rng.Next(1, p.width - w - 1);
                y = rng.Next(1, p.height - h - 1);
                rect = new RectInt(x, y, w, h);
                overlaps = placed.Exists(r => r.Overlaps(rect));
                attemps++;
            }
            

            placed.Add(rect);
            level.rooms.Add(new Room { rect = rect });

            for (int ix = rect.xMin; ix < rect.xMax; ix++)
                for (int iy = rect.yMin; iy < rect.yMax; iy++)
                    level.tiles[ix, iy] = TileType.Floor;
        }

        // Conectar habitaciones con corredores
        for (int i = 1; i < placed.Count; i++)
        {
            Vector2Int a = Center(placed[i - 1]);
            Vector2Int b = Center(placed[i]);
            CarveCorridor(level, a, b);
        }

        // Spawn y salida
        if (placed.Count > 0)
        {
            level.start = Center(placed[0]);
            level.exit = Center(placed[^1]);
            level.tiles[level.start.x, level.start.y] = TileType.Spawn;
            level.tiles[level.exit.x, level.exit.y] = TileType.Exit;
        }

        return level;
    }

    private Vector2Int Center(RectInt r) => new((r.xMin + r.xMax) / 2, (r.yMin + r.yMax) / 2);

    private void CarveCorridor(LevelData level, Vector2Int a, Vector2Int b)
    {
        int x = a.x, y = a.y;
        while (x != b.x)
        {
            level.tiles[x, y] = TileType.Floor;
            x += (b.x > x) ? 1 : -1;
        }
        while (y != b.y)
        {
            level.tiles[x, y] = TileType.Floor;
            y += (b.y > y) ? 1 : -1;
        }
        level.tiles[b.x, b.y] = TileType.Floor;
    }
}
