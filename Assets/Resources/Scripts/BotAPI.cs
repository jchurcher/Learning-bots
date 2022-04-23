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

        rays = rayCaster.CastRays(body.transform.localRotation.eulerAngles.z, body.transform.position);
    }

    void FixedUpdate()
    {
        // Cast and draw rays from bot position and rotation
        rays = rayCaster.CastRays(body.transform.localRotation.eulerAngles.z, body.transform.position);
        rayCaster.DrawRays();
    }

    /// <summary>
    /// Calculate the absolute velocities given the velocites relative to the bot
    /// </summary>
    /// <param name="fVel">Forward velocity, forward and back velocities</param>
    /// <param name="aVel">Adjacent velocity, side to side velocities (Strafing)</param>
    /// <returns>Absolute x and y velocities</returns>
    private Vector2 CalculateVelComponents(float fVel, float aVel)
    {
        float rotation = body.rotation;
        rotation = Mathf.Deg2Rad * (90 - rotation);     //Convert to radians

        float xVel = (Mathf.Sin(rotation) * aVel) - (Mathf.Cos(rotation) * fVel);
        float yVel = (Mathf.Sin(rotation) * fVel) + (Mathf.Cos(rotation) * aVel);

        return new Vector2(xVel, yVel);
    }

    // Set forward velocity (expects -1 to 1)
    public void SetForwardVel(float vel)
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
    public void SetAdjacentVel(float vel)
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
    public void SetAngularVel(float vel)
    {
        vel *= maxAngularVel;

        float sign = Mathf.Sign(vel);
        if (sign * vel > maxAngularVel)
        {
            vel = sign * maxAngularVel;
        }

        this.angularVel = vel;
    }

    // Updates forward and adjacent velocities
    public void UpdateDirectionalVel(float forwardVel, float adjacentVel)
    {
        forwardVel *= maxVel;
        adjacentVel *= maxVel;

        // Set vels within bounds
        forwardVel = Mathf.Clamp(forwardVel, -maxVel, maxVel);
        adjacentVel = Mathf.Clamp(adjacentVel, -maxVel, maxVel);

        this.forwardVel = forwardVel;
        this.adjacentVel = adjacentVel;

        // Update velocities
        body.velocity = this.CalculateVelComponents(this.forwardVel, this.adjacentVel); // Calculate x and y vels
    }

    // Updates angular velocity
    public void UpdateAngularVel(float angularVel)
    {
        angularVel *= maxAngularVel;
        angularVel = Mathf.Clamp(angularVel, -maxAngularVel, maxAngularVel);

        this.angularVel = angularVel;

        // Update angular velocity
        body.angularVelocity = this.angularVel;
    }

    // Get raycast objects
    public List<RaycastHit2D> GetRayCasts()
    {
        return this.rays;
    }

    // Triggers when bot collides with trigger (Wall or Goal)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        agent.TriggerEnter(collision);
    }

}
