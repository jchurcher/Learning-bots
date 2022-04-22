using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotUserController : MonoBehaviour
{
    Rigidbody2D body;

    float sideways;
    float forward;
    float angle;
    float rotation;
    float xAxis;
    float yAxis;

    public float runSpeed = 5.0f;
    public float angleSpeed = 160.0f;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        sideways = Input.GetAxisRaw("Horizontal");  //Is sideways pressed?
        forward = Input.GetAxisRaw("Vertical");     //Is forward pressed?
        angle = Input.GetAxisRaw("Rotate");         //Is rotation pressed?
    }
    private void FixedUpdate()
    {
        body.angularVelocity = angle * angleSpeed;

        rotation = body.rotation;
        print(("Rotation:", rotation));
        rotation = Mathf.Deg2Rad * (90 - rotation);     //Convert to radians
        sideways *= runSpeed;
        forward *= runSpeed;

        // Get x and y components of vectors forwards and sideways with angle
        xAxis = (Mathf.Sin(rotation) * sideways) - (Mathf.Cos(rotation) * forward);
        yAxis = (Mathf.Sin(rotation) * forward) + (Mathf.Cos(rotation) * sideways);

        /*xAxis = 1;
        yAxis = 1;*/

        body.velocity = new Vector2(xAxis, yAxis);

        print(("velocity: ", body.velocity, body.angularVelocity));
    }
}