using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{
    public BotAPI BotObject;

    public float moveVel = 5;
    public float angVel = 160;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float sideways = Input.GetAxisRaw("Horizontal");  //Is sideways pressed?
        float forward = Input.GetAxisRaw("Vertical");     //Is forward pressed?
        float angle = Input.GetAxisRaw("Rotate");         //Is rotation pressed?

        BotObject.setForwardVel(forward * moveVel);
        BotObject.setAdjacentVel(sideways * moveVel);
        BotObject.setAngularVel(angle * angVel);
    }
}
