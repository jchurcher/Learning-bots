using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    public List<GameObject> blockPrefabs;
    public MoveToGoalAgent agent;

    public Vector3 spawnCenter = new(0, 0, 0);
    public Vector3 spawnSize = new(100, 100, 0);

    public Vector2Int shapeDims = new(4,4);

    public int numSpawns = 100;
    public float radius = 2.0f;

    public int spawnAttempts = 100;

    struct Point
    {
        public int x, y;
        public Point(int px, int py)
        {
            x = px;
            y = py;
        }
    }

    public GameObject beginSpawner()
    {
        Object[] prefabs = Resources.LoadAll("Assets/Blocks", typeof(Object));

        // Loop spawning objects
        for (int i = 0; i < numSpawns; i++)
        {
            int index = Random.Range(0, prefabs.Length);    // Pick random block shape
            Object block = prefabs[index];

            spawnObstacle(block, this.radius);
        }

        // Remove any objects withing radius of spawn center
        Collider2D[] results = Physics2D.OverlapCircleAll(new Vector2(spawnCenter.x, spawnCenter.y), 5);
        foreach (Collider2D obj in results)
        {
            if (obj.CompareTag("Wall")) // Destroy any objects tagged as a wall
            {
                Destroy(obj.gameObject);
            }
        }

        // Spawn target object
        GameObject target = (GameObject)spawnTarget(Mathf.Min(spawnSize.x, spawnSize.y) * 0.4f);
        return target;
    }

    public void resetSpawner()
    {
        // Find Walls
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        for (var i = 0; i < walls.Length; i++)
            Destroy(walls[i]);


        // Find Target
        GameObject[] target = GameObject.FindGameObjectsWithTag("Target");

        for (var i = 0; i < target.Length; i++)
            Destroy(target[i]);
    }

    /// <summary>
    /// Generate a random x and y coordinate that is bound by the specified spawn area
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void randomCoord(out float x, out float y)
    {
        x = Mathf.Round(Random.Range(spawnCenter.x - spawnSize.x / 2, spawnCenter.x + spawnSize.x / 2) / 2) * 2;
        y = Mathf.Round(Random.Range(spawnCenter.y - spawnSize.y / 2, spawnCenter.y + spawnSize.y / 2) / 2) * 2;
    }

    /// <summary>
    /// Generate x and y coords randomly at a given radius from the spawn center
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="radius">Radius from center to find coords for</param>
    private void randomCoordAboutCenter(out float x, out float y, float radius)
    {
        float angle = Random.Range(0, 360);
        x = Mathf.Round((Mathf.Sin(angle) * radius + spawnCenter.x) / 2) * 2 + 1;
        y = Mathf.Round((Mathf.Cos(angle) * radius + spawnCenter.y) / 2) * 2 + 1;
    }

    /// <summary>
    /// Spawn the passed object in the scene, first checking if it overlaps any other objects 
    /// within a given radius
    /// </summary>
    /// <param name="obj">Object to be spawned</param>
    public void spawnObstacle(Object obj, float radius)
    {
        float x, y;
        randomCoord(out x, out y);
        Collider2D[] results;

        bool flag = true;

        // Loop finding a free space for object
        for(int i=0; i<spawnAttempts; i++)
        {
            //Check if overlapping occurs within radius
            results = Physics2D.OverlapCircleAll(new Vector2(x, y), radius);

            // If no overlapping break
            if (results.Length == 0)
            {
                flag = false;
                break;
            }

            randomCoord(out x, out y);  //Generate random coord
        }

        if (flag)   // If no free space was found, dont spawn
        {
            return;
        }

        Vector3 location = new(x, y, 0);
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0,4) * 90));  //Random rotation

        Object newObject = Instantiate(obj, location, rotation);

    }

    /// <summary>
    /// Spawn target prefab within radius around spawn center
    /// </summary>
    /// <param name="spawnRadius">Distance from center to spawn the target</param>
    public Object spawnTarget(float spawnRadius)
    {
        float x, y;
        randomCoordAboutCenter(out x, out y, spawnRadius);

        Object targetPrefab = Resources.Load("Assets/Target");

        // Remove any objects withing radius of spawn center
        Collider2D[] results = Physics2D.OverlapCircleAll(new Vector2(x, y), 4);
        foreach (Collider2D obj in results)
        {
            if (obj.CompareTag("Wall")) // Destroy any objects tagged as a wall
            {
                Destroy(obj.gameObject);
            }
        }

        //print("x: " + x.ToString() + " y: " + y.ToString());
        Object newTargetObject = Instantiate(targetPrefab, new Vector3(x, y, 0), Quaternion.identity);
        return newTargetObject;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(spawnCenter, spawnSize);

        Gizmos.DrawSphere(new Vector3(0,0,0), this.radius * 2);
    }
}
