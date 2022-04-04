using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RandomShapeGen : MonoBehaviour
{
    public List<GameObject> blockPrefabs;
    public GameObject wall;

    public Vector3 spawnCenter = new(0, 0, 0);
    public Vector3 spawnSize = new(100, 100, 0);

    public Vector2Int shapeDims = new(4,4);

    public int numSpawns = 100;
    public float radius = 2.0f;

    struct Point
    {
        public int x, y;
        public Point(int px, int py)
        {
            x = px;
            y = py;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Object[] prefabs = Resources.LoadAll("Assets/Blocks", typeof(Object));

        for (int i=0; i<numSpawns; i++)
        {
            int index = Random.Range(0, prefabs.Length);
            Object block = prefabs[index];

            spawnObstacle(block);
        }

        /*GameObject[] obstacleParents = GameObject.FindGameObjectsWithTag("Block");

        foreach(GameObject block in obstacleParents)
        {
            block.GetComponentsInChildren(typeof(Transform));
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*int[,] GenerateShape(Vector2Int dims, int numTurns)
    {
        int width = dims[0]; int height = dims[1];

        int[,] newShape = new int[width, height];

        Point pos = new(Random.Range(0, width), 0);
        newShape[pos.x, pos.y] = 1;

        for(int i=0; i < numTurns; i++)
        {

        }

        return newShape;
    }*/

    public void spawnObstacle(Object obj)
    {
        float x = spawnCenter[0] - spawnSize[0] / 2 + (Mathf.Round(Random.Range(0, spawnSize.x) / 2) * 2);
        float y = spawnCenter[0] - spawnSize[0] / 2 + (Mathf.Round(Random.Range(0, spawnSize.y) / 2) * 2);

        Collider2D[] results = Physics2D.OverlapCircleAll(new Vector2(x, y), this.radius);
        print("res: " + results.Length.ToString());

        bool flag = true;

        // Loop finding a free space for object
        for(int i=0; i<100; i++)
        {
            if (results.Length == 0)
            {
                flag = false;
                break;
            }

            /*foreach(Collider2D j in results)
            {
                if (j )
            }*/
            
            /*foreach (Collider2D colider in results)
            {
                if (LayerMask.LayerToName(colider.gameObject.layer) == "Wall")
                {
                    x = spawnCenter[0] - spawnSize[0] / 2 + (Mathf.Round(Random.Range(0, spawnSize.x) / 2) * 2);
                    y = spawnCenter[0] - spawnSize[0] / 2 + (Mathf.Round(Random.Range(0, spawnSize.y) / 2) * 2);
                    results = Physics2D.OverlapCircleAll(new Vector2(x, y), this.radius);
                }
            }*/

            x = spawnCenter[0] - spawnSize[0] / 2 + (Mathf.Round(Random.Range(0, spawnSize.x) / 2) * 2);
            y = spawnCenter[0] - spawnSize[0] / 2 + (Mathf.Round(Random.Range(0, spawnSize.y) / 2) * 2);
            results = Physics2D.OverlapCircleAll(new Vector2(x, y), this.radius);
        }

        print("flag: " + flag);

        if (flag)   // If no free space was found, dont spawn
        {
            return;
        }

        //print((x, y));

        //Vector3 location = spawnCenter - spawnSize/2 + new Vector3(x, y, 0);
        Vector3 location = new Vector3(x, y, 0);
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0,4) * 90));

        Object newObject = Instantiate(obj, location, rotation);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(spawnCenter, spawnSize);

        Gizmos.DrawSphere(new Vector3(0,0,0), this.radius);
    }
}
