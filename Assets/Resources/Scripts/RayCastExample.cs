using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastExample : MonoBehaviour
{
    public BotAPI BotObject;
    public float offsetAngle = 45;

    void Start()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        /*lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.positionCount = 3;
        lineRenderer.SetPosition(1, transform.position);
        lineRenderer.SetPosition(2, transform.position);*/
    }

    // See Order of Execution for Event Functions for information on FixedUpdate() and Update() related to physics queries
    void FixedUpdate()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector2 origin = ray.origin;
        Vector2 direction = ray.direction;*/

        float rotation = BotObject.body.rotation;
        //rotation += offsetAngle;

        Vector2 origin = BotObject.transform.position;
        //Vector2 direction = BotObject.transform.TransformDirection(Vector2.up);
        Vector2 direction1 = calcDirectionVector(rotation + offsetAngle);
        Vector2 direction2 = calcDirectionVector(rotation - offsetAngle);

        RaycastHit2D hit1 = Physics2D.Raycast(origin, direction1);
        RaycastHit2D hit2 = Physics2D.Raycast(origin, direction2);

        print((hit1.point, hit2.point));

        lineRenderer.SetPosition(1, BotObject.body.position);

        if (hit1)
        {
            //print(hit1.distance);
            lineRenderer.SetPosition(0, hit1.point);
        }
        else
        {
            lineRenderer.SetPosition(0, origin + direction1 * 5);
        }

        if (hit2)
        {
            //print(hit2.distance);
            lineRenderer.SetPosition(2, hit2.point);
        }
        else
        {
            lineRenderer.SetPosition(2, origin + direction2 * 5);
        }

        /*lineRenderer.SetPosition(0, origin + direction1 * 5);
        lineRenderer.SetPosition(1, BotObject.body.position);
        lineRenderer.SetPosition(2, origin + direction2 * 5);*/

        Debug.DrawLine(origin, origin + direction1 * 5, Color.yellow);
        Debug.DrawLine(origin, origin + direction2 * 5, Color.yellow);
    }

    Vector3 calcDirectionVector(float angle)
    {
        angle *= Mathf.Deg2Rad;

        float x = Mathf.Sin(angle) * -1;
        float y = Mathf.Cos(angle);

        return new Vector3(x, y, 0);
    }
}