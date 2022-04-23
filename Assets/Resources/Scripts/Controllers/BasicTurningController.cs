using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurningController : MonoBehaviour
{
    public BotAPI BotObject;

    public float moveVel = 5;
    public float angVel = 160;

    float sideways = 0;
    float forward = 1;
    float angle = 0;

    // Update is called once per frame
    void Update()
    {
        //print(BotObject);

        List<RaycastHit2D> rays = BotObject.GetRayCasts();

        //print(rays);
        //print(rays[0].distance);

        float leftDist = rays[0].distance;
        float rightDist = rays[1].distance;

        if (!rays[0])
            leftDist = Mathf.Infinity;

        if (!rays[1])
            rightDist = Mathf.Infinity;

        forward = 1;
        angle = 0;

        //print((leftDist, rightDist));

        if (!(leftDist == Mathf.Infinity && rightDist == Mathf.Infinity))
        {
            if(leftDist <= rightDist)
            {
                angle = -1;
            }
            else if (leftDist > rightDist)
            {
                angle = 1;
            }
        }

        BotObject.SetForwardVel(forward * moveVel);
        BotObject.SetAdjacentVel(sideways * moveVel);
        BotObject.SetAngularVel(angle * angVel);
    }
}
