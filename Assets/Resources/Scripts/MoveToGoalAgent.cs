using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    public GameObject playerObject;
    public GameObject targetObject;
    public RandomSpawner spawner;
    public BotAPI botAPI;
    
    private float checkpointDistance = 100;

    public override void OnEpisodeBegin()
    {
        // Delete previous walls and target
        spawner.resetSpawner();

        // Reset player position and rotation
        playerObject.transform.position = Vector3.zero;
        playerObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        // Spawn walls and target
        GameObject target = spawner.beginSpawner();
        targetObject = target;

        // Reset checkpoint counter
        checkpointDistance = 100;
    }

    private void Update()
    {
        // Evaluate current distance
        float distance = Vector2.Distance(targetObject.transform.position, playerObject.transform.position);

        if (distance < checkpointDistance - 2)
        {
            checkpointDistance = distance;
            print(checkpointDistance);
            AddReward(+1f);
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 distance = targetObject.transform.position - playerObject.transform.position;

        float rotation = playerObject.transform.rotation.eulerAngles.z;
        rotation -= 180;

        //List<RaycastHit2D> raycasts = playerObject.GetComponent<BotAPI>().getRayCasts();
        List<RaycastHit2D> raycasts = botAPI.getRayCasts();
        Vector2 rays = new(raycasts[0].distance, raycasts[1].distance);

        sensor.AddObservation(distance);
        sensor.AddObservation(rotation);
        sensor.AddObservation(rays);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardVel = actions.ContinuousActions[0];
        float adjacentVel = actions.ContinuousActions[1];
        float angularVel = actions.ContinuousActions[2];

        botAPI.setForwardVel(forwardVel);
        botAPI.setAdjacentVel(adjacentVel);
        botAPI.setAngularVel(angularVel);
    }

    /*public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }*/

    // Triggers when bot collides with trigger (Wall or Goal)
    public void TriggerEnter(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            SetReward(+4f);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            SetReward(-0.3f);
            EndEpisode();
        }
    }

    // Reset on out of bounds
    public void OutofBounds()
    {
        SetReward(-1f);
        EndEpisode();
    }

    // Reward when player is closer to target
    public void rewardDistance()
    {

    }

    // Set the player object to move to
    public void setPlayer(GameObject target)
    {
        this.targetObject = target;
    }

    // Set the target object to move to
    public void setTarget(GameObject target)
    {
        this.targetObject = target;
    }

}
