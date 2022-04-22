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

    public override void CollectObservations(VectorSensor sensor)
    {
        // Get distance vector between player and target
        Vector2 distance = targetObject.transform.position - playerObject.transform.position;

        // Get rotation of player, convert to sin and cos, If angle needed back use arctan2(sin, cos)
        float r = playerObject.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 rotation = new(Mathf.Sin(r), Mathf.Cos(r));

        // Get raycast distances
        List<RaycastHit2D> raycasts = botAPI.getRayCasts();

        List<float> raysDists = new List<float>();
        List<float> raysHits = new List<float>();

        string sent = "";

        // Create list of ray distances and if they hit or not
        foreach(RaycastHit2D hit in raycasts)
        {
            raysDists.Add(hit ? hit.distance : botAPI.rayCaster.distance);
            raysHits.Add(hit ? 1 : 0);

            sent += "(" + (hit ? hit.distance : botAPI.rayCaster.distance).ToString() + "," + (bool)hit + "), ";
        }

        print("rays: " + sent);

        sensor.AddObservation(distance);
        sensor.AddObservation(rotation);
        sensor.AddObservation(raysDists);
        //sensor.AddObservation(raysHits);
    }

    // Runs when action recieved back from AI network
    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardVel = actions.DiscreteActions[0];
        float adjacentVel = actions.DiscreteActions[1];
        float angularVel = actions.DiscreteActions[2];

        string sent = "Actions: ";
        foreach (int action in actions.DiscreteActions)
        {
            sent += "(" + action + "), ";
        }

        print(sent);

        botAPI.setForwardVel(forwardVel - 1);
        botAPI.setAdjacentVel(adjacentVel - 1);
        botAPI.setAngularVel(angularVel - 1);

        AddReward(-0.005f);
    }

    // For debugging, gives player control with WASD QE
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> continuousActions = actionsOut.DiscreteActions;
        continuousActions[0] = (int)Input.GetAxisRaw("Vertical") + 1;    //Is forward pressed?
        continuousActions[1] = (int)Input.GetAxisRaw("Horizontal") + 1;  //Is sideways pressed?
        continuousActions[2] = (int)Input.GetAxisRaw("Rotate") + 1;      //Is rotation pressed?
    }

    // Triggers when bot collides with trigger (Wall or Goal)
    public void TriggerEnter(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            AddReward(+1f);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.05f);
            EndEpisode();
        }
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
