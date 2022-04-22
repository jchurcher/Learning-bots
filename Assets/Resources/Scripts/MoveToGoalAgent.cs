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
        playerObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 0));

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

    private static float Sigmoid(float value)
    {
        return 1.0f / (1.0f + (float)Mathf.Exp(-value));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Get distance vector between player and target
        Vector2 distance = targetObject.transform.position - playerObject.transform.position;
        distance /= 15;
        distance.x = (Sigmoid(distance.x) * 2) - 1;
        distance.y = (Sigmoid(distance.y) * 2) - 1;

        // Get rotation of player, convert to sin and cos
        // If angle needed back use arctan2(sin, cos)
        float r = playerObject.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 rotation = new(Mathf.Sin(r), Mathf.Cos(r));

        // Get raycast distances
        List<RaycastHit2D> raycasts = botAPI.getRayCasts();
        /*Vector2 rays = Vector2.zero;
        rays[0] = raycasts[0].distance / botAPI.rayCaster.distance;
        rays[1] = raycasts[1].distance / botAPI.rayCaster.distance;*/

        List<float> raysDists = new List<float>();
        List<float> raysHits = new List<float>();

        string sent = "";

        foreach(RaycastHit2D hit in raycasts)
        {
            raysDists.Add(hit ? hit.distance / botAPI.rayCaster.distance : botAPI.rayCaster.distance*4);
            raysHits.Add(hit ? 1 : 0);

            sent += "(" + (hit ? hit.distance / botAPI.rayCaster.distance : 1).ToString() + "," + (bool)hit + "), ";

            /*if (hit)
            {
                
            }
            else
            {
                rays.Add(Mathf.Infinity);
            }*/
        }

        //print("rays: " + sent);

        sensor.AddObservation(distance);
        sensor.AddObservation(rotation);
        sensor.AddObservation(raysDists);
        sensor.AddObservation(raysHits);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardVel = actions.ContinuousActions[0];
        float adjacentVel = actions.ContinuousActions[1];
        float angularVel = actions.ContinuousActions[2];

        string sent = "Actions: ";
        foreach(float action in actions.ContinuousActions)
        {
            sent += "(" + action + "), ";
        }

        print(sent);

        /*print(("Fwd, Adj, Ang: ", forwardVel, adjacentVel, angularVel));*/

        botAPI.setForwardVel(forwardVel);
        botAPI.setAdjacentVel(adjacentVel);
        botAPI.setAngularVel(angularVel);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");    //Is forward pressed?
        continuousActions[1] = Input.GetAxisRaw("Horizontal");  //Is sideways pressed?
        continuousActions[2] = Input.GetAxisRaw("Rotate");      //Is rotation pressed?
    }

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
