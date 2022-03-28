using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformControlTest : MonoBehaviour
{
    int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        print("begin");
    }

    // Update is called once per frame
    void Update()
    {
        //print("update");
        //print("update");

        if (Mathf.Abs(transform.position.x) >= 5)
        {
            direction = direction * -1;
        }

        //transform.Translate(direction * 0.1f, 0, 0);

        print(transform.eulerAngles[2] > 180);
        if (transform.eulerAngles[2] > 180)
        {
            direction = direction * -1;
        }

        transform.Rotate(0, 0, direction * 1);
    }
}
