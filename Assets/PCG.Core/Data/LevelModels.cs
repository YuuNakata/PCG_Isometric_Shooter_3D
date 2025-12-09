using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Empty, Floor, Wall, Spawn, Exit }

[Serializable]
public class LevelParams
{
    public int width = 30, height = 30;
    public int seed = 12345;
    public float roomDensity = 0.15f;

    public LevelParams(int width = 30,int height = 30,int seed = 12345,float roomDensity = 0.15f)
    {
        this.width = width;
        this.height = height;
        this.seed = seed;
        this.roomDensity = roomDensity;

    }
}

[Serializable]
public class Room
{
    public RectInt rect;
}

[Serializable]
public class LevelData
{
    public TileType[,] tiles;
    public List<Room> rooms = new();
    public Vector2Int start, exit;
}
