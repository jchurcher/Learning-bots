using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAPI : MonoBehaviour
{
    public Rigidbody2D body;
    public RayCaster rayCaster;

    float forwardVel;
    float adjacentVel;
    float angularVel;

    public float maxVel = 5.0f;
    public float maxAngularVel = 160.0f;
    public float visionAngleOffset = 45;
    public float visionDistance = 5;

    List<RaycastHit2D> rays;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        forwardVel = 0;
        adjacentVel = 0;
        angularVel = 0;

        rayCaster.SetDistance(visionDistance);
    }

    void Update()
    {
        body.angularVelocity = this.angularVel;
        body.velocity = this.calculateVelComponents(this.forwardVel, this.adjacentVel);

        rays = rayCaster.CastRays(visionAngleOffset, body.rotation, body.position);
        rayCaster.DrawRays();

        print((rays[0].distance, rays[1].distance));
    }

    /// <summary>
    /// Calculate the absolute velocities given the velocites relative to the bot
    /// </summary>
    /// <param name="fVel">Forward velocity, forward and back velocities</param>
    /// <param name="aVel">Adjacent velocity, side to side velocities (Strafing)</param>
    /// <returns>Absolute x and y velocities</returns>
    private Vector2 calculateVelComponents(float fVel, float aVel)
    {
        float rotation = body.rotation;
        rotation = Mathf.Deg2Rad * (90 - rotation);     //Convert to radians

        float xVel = (Mathf.Sin(rotation) * aVel) - (Mathf.Cos(rotation) * fVel);
        float yVel = (Mathf.Sin(rotation) * fVel) + (Mathf.Cos(rotation) * aVel);

        return new Vector2(xVel, yVel);
    }

    public void setForwardVel(float vel)
    {
        float sign = Mathf.Sign(vel);
        if (sign * vel > maxVel)
        {
            vel = sign * maxVel;
        }

        this.forwardVel = vel;
    }

    public void setAdjacentVel(float vel)
    {
        float sign = Mathf.Sign(vel);
        if (sign * vel > maxVel)
        {
            vel = sign * maxVel;
        }

        this.adjacentVel = vel;
    }

    public void setAngularVel(float vel)
    {
        float sign = Mathf.Sign(vel);
        if (sign * vel > maxAngularVel)
        {
            vel = sign * maxAngularVel;
        }

        this.angularVel = vel;
    }

    public List<RaycastHit2D> getRayCasts()
    {
        return this.rays;
    }

}
