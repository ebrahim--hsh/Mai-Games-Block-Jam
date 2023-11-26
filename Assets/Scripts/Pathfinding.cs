using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance;

    private void Awake()
    {
        Instance = this;
    }

    Queue<Tile> AStar(Tile start, Tile goal)
    {
//        Debug.Log("AStar");
        Dictionary<Tile, Tile> NextTileToGoal = new Dictionary<Tile, Tile>();
        Dictionary<Tile, int> costToReachTile = new Dictionary<Tile, int>();

        PriorityQueue<Tile> frontier = new PriorityQueue<Tile>();
        frontier.Enqueue(goal, 0);
        costToReachTile[goal] = 0;

        while (frontier.Count > 0)
        {
            Tile curTile = frontier.Dequeue();
            if (curTile == start)
                break;

            foreach (Tile neighbor in MapGenerator.Instance.Neighbors(curTile))
            {
                int newCost = costToReachTile[curTile] + neighbor._Cost;
                if (costToReachTile.ContainsKey(neighbor) == false || newCost < costToReachTile[neighbor])
                {
                    if (neighbor.isEmpty || neighbor == start)
                    {
                        costToReachTile[neighbor] = newCost;
                        int priority = newCost + Distance(neighbor, start);
                        frontier.Enqueue(neighbor, priority);
                        NextTileToGoal[neighbor] = curTile;
                    }
                }
            }
        }

        //Get the Path

        //check if tile is reachable
        if (NextTileToGoal.ContainsKey(start) == false)
        {
            return null;
        }

        Queue<Tile> path = new Queue<Tile>();
        Tile pathTile = start;
        while (goal != pathTile)
        {
            pathTile = NextTileToGoal[pathTile];
            path.Enqueue(pathTile);
        }
        return path;
    }


    public Queue<Tile> FindPath(Tile start, Tile end)
    {
        return AStar(start, end);

    }


    int Distance(Tile t1, Tile t2)
    {
        return Mathf.Abs(t1._X - t2._X) + Mathf.Abs(t1._Y - t2._Y);
    }

}
