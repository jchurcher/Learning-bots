using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotClass : MonoBehaviour
{
    public float speed;
    public float rotation;

    const int speedLimit = 10;
    const int rotationLimit = 20;

    public BotClass(float speed, float rotation)
    {
        this.speed = speed;
        this.rotation = rotation;
    }

    public void SetSpeed(float speed)
    {
        if(speed > speedLimit)
        {
            this.speed = speed;
        }
    }

    public void SetRotation(float rotation)
    {
        this.rotation = rotation;
    }

    public Vector3 TranslateBot(float direction)
    {

        return new Vector3(0, 0, 0);
    }

    public Vector3 RotateBot()
    {
        return new Vector3(0, 0, 0);
    }
}
