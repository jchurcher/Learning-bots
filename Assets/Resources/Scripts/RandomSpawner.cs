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

    public int numSpawns = 100;
    public int spawnAttempts = 100;
    public float obstacleSpacingRadius = 2.0f;
    public float targetSpawningRadius = 40;

    private Object[] prefabs;
    private List<Object> obstacles = new();
    private Object targetObject = null;

    struct Point
    {
        public int x, y;
        public Point(int px, int py)
        {
            x = px;
            y = py;
        }
    }

    private void Start()
    {
        prefabs = Resources.LoadAll("Assets/Blocks", typeof(Object));
        spawnCenter += GetComponentInParent<Transform>().parent.position;
    }

    public GameObject BeginSpawner()
    {
        // Create new list for all obstacles
        obstacles = new List<Object>();

        // Loop spawning objects
        for (int i = 0; i < numSpawns; i++)
        {
            int index = Random.Range(0, prefabs.Length);    // Pick random block shape
            Object block = prefabs[index];

            Object obj = SpawnObstacle(block, this.obstacleSpacingRadius);
            obstacles.Add(obj);
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
        GameObject target = (GameObject)SpawnTarget(targetSpawningRadius);
        targetObject = target;
        return target;
    }

    public void ResetSpawner()
    {
        // Destroy all obstacle objects
        foreach(Object obj in obstacles)
        {
            Destroy(obj);
        }

        // Destroy target
        Destroy(targetObject);
    }

    /// <summary>
    /// Generate a random x and y coordinate that is bound by the specified spawn area
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void RandomCoord(out float x, out float y)
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
    private void RandomCoordAboutCenter(out float x, out float y, float radius)
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
    public Object SpawnObstacle(Object obj, float radius)
    {
        RandomCoord(out float x, out float y);
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

            RandomCoord(out x, out y);  //Generate random coord
        }

        if (flag)   // If no free space was found, dont spawn
        {
            return null;
        }

        Vector3 location = new(x, y, 0);
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0,4) * 90));  //Random rotation

        Object newObject = Instantiate(obj, location, rotation);
        return newObject;
    }

    /// <summary>
    /// Spawn target prefab within radius around spawn center
    /// </summary>
    /// <param name="spawnRadius">Distance from center to spawn the target</param>
    public Object SpawnTarget(float spawnRadius)
    {
        RandomCoordAboutCenter(out float x, out float y, spawnRadius);

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

        Gizmos.DrawSphere(spawnCenter, this.obstacleSpacingRadius * 2);
    }
}
