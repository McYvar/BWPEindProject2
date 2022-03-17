using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    top = 0,
    left = 1,
    down = 2,
    right = 3
};

public class DungeonCrawlerController : MonoBehaviour
{
    public static List<Vector2Int> positionVisited = new List<Vector2Int>();
    private static readonly Dictionary<Direction, Vector2Int> directionMovementMap = new Dictionary<Direction, Vector2Int>()
    {
        {Direction.top, Vector2Int.up},
        {Direction.left, Vector2Int.left},
        {Direction.down, Vector2Int.down},
        {Direction.right, Vector2Int.right}
    };

    public static List<Vector2Int> GenerateDungeon(DungeonGenerationData dungeonData)
    {
        int iterations = Random.Range(dungeonData.iterationMin, dungeonData.iterationMax);

        DungeonCrawler dungeonCrawler = new DungeonCrawler(Vector2Int.zero);
        for (int i = 0; i <= iterations; i++)
        {
            Vector2Int newPos = dungeonCrawler.Move(directionMovementMap);
            if (positionVisited.Contains(newPos) || newPos == Vector2Int.zero) iterations++; // Added a line to make sure every room has an actual new position
            else positionVisited.Add(newPos);
        }

        return positionVisited;
    }
}
