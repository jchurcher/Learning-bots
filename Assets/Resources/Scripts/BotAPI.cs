using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAPI : MonoBehaviour
{
    public Rigidbody2D body;
    public RayCaster rayCaster;
    public MoveToGoalAgent agent;

    float forwardVel;
    float adjacentVel;
    float angularVel;

    [SerializeField] private float maxVel = 5.0f;
    [SerializeField] private float maxAngularVel = 160.0f;
    //[SerializeField] private float visionAngleOffset = 45;

    List<RaycastHit2D> rays;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        forwardVel = 0;
        adjacentVel = 0;
        angularVel = 0;

        //rayCaster.SetDistance(visionDistance);

        rays = rayCaster.CastRays(body.rotation, body.position);
    }

    void FixedUpdate()
    {
        // Set velocities
        body.angularVelocity = this.angularVel;
        body.velocity = this.calculateVelComponents(this.forwardVel, this.adjacentVel); // Calculate x and y vels

        // Cast and draw rays from bot position and rotation
        rays = rayCaster.CastRays(body.rotation, body.position);
        rayCaster.DrawRays();

        //print((rays[0].distance, rays[1].distance));  // Print returned distance

        if(Mathf.Abs(transform.position.x) > 50 || Mathf.Abs(transform.position.y) > 50)
        {
            agent.OutofBounds();
        }
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

    // Set forward velocity (expects -1 to 1)
    public void setForwardVel(float vel)
    {
        vel *= maxVel;

        float sign = Mathf.Sign(vel);
        if (sign * vel > maxVel)
        {
            vel = sign * maxVel;
        }

        this.forwardVel = vel;
    }

    // Set Adjacent velocity (expects -1 to 1)
    public void setAdjacentVel(float vel)
    {
        vel *= maxVel;

        float sign = Mathf.Sign(vel);
        if (sign * vel > maxVel)
        {
            vel = sign * maxVel;
        }

        this.adjacentVel = vel;
    }

    // Set Angular velocity (expects -1 to 1)
    public void setAngularVel(float vel)
    {
        vel *= maxAngularVel;

        float sign = Mathf.Sign(vel);
        if (sign * vel > maxAngularVel)
        {
            vel = sign * maxAngularVel;
        }

        this.angularVel = vel;
    }

    // Get raycast objects
    public List<RaycastHit2D> getRayCasts()
    {
        return this.rays;
    }

    // Triggers when bot collides with trigger (Wall or Goal)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        agent.TriggerEnter(collision);
    }

}
