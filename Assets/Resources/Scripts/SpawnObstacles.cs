using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacles : MonoBehaviour
{
    public GameObject ObjectPrefab;

    public Vector3 center;
    public Vector3 size;

    //private int frameCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        spawnObstacle();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (frameCount >= 10)
        {
            if (Input.GetKey(KeyCode.F))
            {
                frameCount = 0;
                spawnObstacle();
            }
        }   
        else
        {
            frameCount++;
        }*/

        if (Input.GetKey(KeyCode.F))
        {
            spawnObstacle();
        }
    }

    public void spawnObstacle()
    {
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));

        GameObject newObject = Instantiate(ObjectPrefab, pos, Quaternion.identity);

    }
    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }*/
}
