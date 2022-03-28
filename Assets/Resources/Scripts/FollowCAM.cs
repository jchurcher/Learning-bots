using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCAM : MonoBehaviour
{
    public Vector3 myPos;
    public Transform bot;

    // Start is called before the first frame update
    void Start()
    {
        bot = GameObject.Find("Bot").transform;
    }

    // Update is called once per frame
    void Update()
    {
        myPos = bot.position;
        myPos[2] = -10;
        transform.position = myPos;
    }
}
