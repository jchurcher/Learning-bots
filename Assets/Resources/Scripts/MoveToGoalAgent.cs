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
    private List<Vector2> path;
    private Object pathObject;

    private bool wallCollisionFlag = false;
    private int wallCollisionCount = 0;

    [SerializeField] private float targetReward = +1f;
    [SerializeField] private float collisionReward = -0.06f;
    [SerializeField] private float timeReward = -0.005f;
    [SerializeField] private float checkpointReward = +0.02f;

    public void Start()
    {
        pathObject = Resources.Load("Assets/Path", typeof(Object));
    }

    public override void OnEpisodeBegin()
    {
        // Delete previous walls and target
        spawner.ResetSpawner();

        // Spawn walls and target
        GameObject target = spawner.BeginSpawner();
        targetObject = target;

        // Reset checkpoint counter
        checkpointDistance = Vector2.Distance(playerObject.transform.position, targetObject.transform.position);

        // The A* algorithm
        // Create a new grid
        Grid newGrid = new Grid((int)spawner.obstacleSpawnSize.x, (int)spawner.obstacleSpawnSize.y);
        MatrixPathfinding.PopulateGrid(ref newGrid);
        newGrid.SetNodeValueAtCoord(playerObject.transform.position, ObjectStates.Start);

        // Delete prev path
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Respawn");
        foreach(GameObject obj in objs)
        {
            Destroy(obj);
        }

        // Apply A*
        List<Node> pathNodes = MatrixPathfinding.ApplyAStar(newGrid);
        path = MatrixPathfinding.GetNodeCoordinates(pathNodes, newGrid);

        /*foreach (Node node in pathNodes)
        {
            Vector2 pos = node.GetCoord();
            pos = newGrid.Index2Coord(pos);

            Object pathObject = Resources.Load("Assets/Path", typeof(Object));
            Instantiate(pathObject, new Vector3(pos.x, pos.y, 2), Quaternion.identity);
        }*/

        foreach (Vector2 pos in path)
        {
            Instantiate(pathObject, new Vector3(pos.x, pos.y, 2), Quaternion.identity);
        }

        wallCollisionCount = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Get distance vector between player and target
        Vector2 distance = targetObject.transform.position - playerObject.transform.position;

        // Get rotation of player, convert to sin and cos, If angle needed back use arctan2(sin, cos)
        float r = playerObject.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 rotation = new(Mathf.Sin(r), Mathf.Cos(r));

        // Get raycast distances
        List<RaycastHit2D> raycasts = botAPI.GetRayCasts();

        List<float> raysDists = new List<float>();
        List<float> raysHits = new List<float>();

        string sent = "";

        // Create list of ray distances and if they hit or not
        foreach (RaycastHit2D hit in raycasts)
        {
            raysDists.Add(hit ? hit.distance : botAPI.rayCaster.distance);
            raysHits.Add(hit ? 1 : 0);

            sent += "(" + (hit ? hit.distance : botAPI.rayCaster.distance).ToString() + "," + (bool)hit + "), ";
        }

        //print("rays: " + sent);

        sensor.AddObservation(distance);
        sensor.AddObservation(rotation);
        sensor.AddObservation(raysDists);
        //sensor.AddObservation(raysHits);
    }

    // Runs when action recieved back from AI network
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Includes adjacent velocity
        float forwardVel = actions.DiscreteActions[0];
        float adjacentVel = actions.DiscreteActions[1];
        float angularVel = actions.DiscreteActions[2];

        // Excludes adjacent velocity
        /*float forwardVel = actions.DiscreteActions[0];
        float adjacentVel = 1;
        float angularVel = actions.DiscreteActions[1];*/

        // Update bot speeds
        botAPI.UpdateDirectionalVel(forwardVel - 1, adjacentVel - 1);
        botAPI.UpdateAngularVel(angularVel - 1);

        // ------------ Punishments ------------

        // Punish agent slowly for taking a long time
        AddReward(timeReward);

        // Punish if touching wall      --- Only punish once then move away 0.3 or more to be punished again
        if (wallCollisionFlag)
        {
            Collider2D coll = Physics2D.OverlapCircle(playerObject.transform.position, 0.5f, LayerMask.GetMask("Wall"));
            if (coll)
            {
                wallCollisionFlag = false;
                wallCollisionCount += 1;
                AddReward(collisionReward);
            }
        }
        else
        {
            Collider2D coll = Physics2D.OverlapCircle(playerObject.transform.position, 0.7f, LayerMask.GetMask("Wall"));
            if (!coll)
            {
                wallCollisionFlag = true;
            }
        }

        print("Wall collision count: " + wallCollisionCount);

        // Reward based on checkpoint distance
        float currentDistance = Vector2.Distance(playerObject.transform.position, targetObject.transform.position);
        if (checkpointDistance > currentDistance + 5)
        {
            checkpointDistance = currentDistance;
            AddReward(checkpointReward);
        }

        print(("Checkpoint dist: ", checkpointDistance, currentDistance));

        /*// Reward based on distance from path
        Vector2 playerPos = botAPI.transform.localPosition;
        Vector2 closestPos;
        float closestDistance = Mathf.Infinity;

        foreach (Vector2 pos in path)
        {
            float newDist = Vector2.Distance(playerPos, pos);
            if (newDist < closestDistance)
            {
                closestPos = pos;
                closestDistance = newDist;
            }
        }

        if (closestDistance <= 1)
        {
            AddReward(){

            }
        }*/
    }

    // For debugging, gives player control with WASD QE
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> actions = actionsOut.DiscreteActions;

        // Include adjacent
        /*actions[0] = (int)Input.GetAxisRaw("Vertical") + 1;    //Is forward pressed?
        actions[1] = (int)Input.GetAxisRaw("Horizontal") + 1;  //Is sideways pressed?
        actions[2] = (int)Input.GetAxisRaw("Rotate") + 1;      //Is rotation pressed?*/

        // Exclude adjacent;
        actions[0] = (int)Input.GetAxisRaw("Vertical") + 1;    //Is forward pressed?
        actions[1] = (int)Input.GetAxisRaw("Rotate") + 1;      //Is rotation pressed?
    }

    // Triggers when bot collides with trigger (Wall or Goal)
    public void TriggerEnter(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            AddReward(targetReward);
            EndEpisode();
        }
        /*if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Boundary"))
        {
            AddReward(-0.05f);
            EndEpisode();
        }*/
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
