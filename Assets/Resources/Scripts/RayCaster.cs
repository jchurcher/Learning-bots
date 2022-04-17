using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    RaycastHit2D hit1, hit2;
    Vector2 origin, direction1, direction2;
    public float distance = 5;

    /// <summary>
    /// Cast two rays at an angle from a given position and rotation
    /// </summary>
    /// <param name="offsetAngle">Angle between the two rays</param>
    /// <param name="rotation">Rotation of the origin in degrees</param>
    /// <param name="position">Position vector of the origin</param>
    /// <returns>ArrayList of two ray objects</returns>
    public List<RaycastHit2D> CastRays(float offsetAngle, float rotation, Vector2 position)
    {
        offsetAngle /= 2;

        origin = position;
        direction1 = CalcDirectionVector(rotation + offsetAngle);
        direction2 = CalcDirectionVector(rotation - offsetAngle);

        hit1 = Physics2D.Raycast(origin, direction1, distance);
        hit2 = Physics2D.Raycast(origin, direction2, distance);

        List<RaycastHit2D> list = new(2);
        list.Add(hit1);
        list.Add(hit2);

        return list;
    }

    /// <summary>
    /// Draw the two rays as lines from the origin
    /// </summary>
    public void DrawRays()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.SetPosition(1, origin);

        if (hit1)
        {
            //Draw line to hit point
            lineRenderer.SetPosition(0, hit1.point);
            //print(hit1.distance);
        }
        else
        {
            //Draw line into empty space
            lineRenderer.SetPosition(0, origin + direction1 * distance);
        }

        if (hit2)
        {
            //Draw line to hit point
            lineRenderer.SetPosition(2, hit2.point);
            //print(hit2.distance);
        }
        else
        {
            //Draw line into empty space
            lineRenderer.SetPosition(2, origin + direction2 * distance);
        }
    }

    private Vector3 CalcDirectionVector(float angle)
    {
        angle *= Mathf.Deg2Rad;

        float x = Mathf.Sin(angle) * -1;
        float y = Mathf.Cos(angle);

        return new Vector3(x, y, 0);
    }

    public void SetDistance(float distance)
    {
        this.distance = distance;
    }
}
