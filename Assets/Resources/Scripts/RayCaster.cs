using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    RaycastHit2D hit1, hit2;
    RaycastHit2D[] hits;
    LineRenderer lineRenderer;
    Vector2 origin, direction1, direction2;
    Vector2[] directions;
    public float distance = 5;
    public int numberOfRays = 2;
    public float offsetAngle = 45f;

    private LayerMask layerMask;

    public void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numberOfRays * 2;

        layerMask = LayerMask.GetMask("Wall");
    }

    /// <summary>
    /// Cast two rays at an angle from a given position and rotation
    /// </summary>
    /// <param name="offsetAngle">Angle between the two rays</param>
    /// <param name="rotation">Rotation of the origin in degrees</param>
    /// <param name="position">Position vector of the origin</param>
    /// <returns>ArrayList of two ray objects</returns>
    public List<RaycastHit2D> CastTwoRays(float rotation, Vector2 position)
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

    public List<RaycastHit2D> CastRays(float rotation, Vector2 position)
    {
        origin = position;

        float[] newAngles = Arange(numberOfRays, offsetAngle);
        hits = new RaycastHit2D[numberOfRays];
        directions = new Vector2[numberOfRays];

        for (int i=0; i<newAngles.Length; i++)
        {
            directions[i] = CalcDirectionVector(rotation + newAngles[i]);
            hits[i] = Physics2D.Raycast(position, directions[i], distance, layerMask);
        }

        List<RaycastHit2D> list = new(hits);

        return list;
    }

    /// <summary>
    /// Draw the two rays as lines from the origin
    /// </summary>
    public void DrawRays()
    {
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            int lineIndex = (i * 2) + 1;

            lineRenderer.SetPosition(lineIndex - 1, origin);

            if (hit)
                //Draw line to hit point
                lineRenderer.SetPosition(lineIndex, hit.point);
            else
                //Draw line into empty space
                lineRenderer.SetPosition(lineIndex, origin + directions[i] * distance);
        }
    }

    // Divides a given angle across zero into a set number of equally spaced values
    // 3, 90 = (-45, 0, -45), 4, 90 = (-45, -15, 15, 45), etc
    private float[] Arange(int num, float angle)
    {
        float increment = angle / (num - 1);
        float start = 0 - (angle / 2);

        float[] points = new float[num];
        for (int i = 0; i < num; i++)
        {
            points[i] = start + (increment * i);
        }

        return points;
    }

    private Vector2 CalcDirectionVector(float angle)
    {
        angle *= Mathf.Deg2Rad;

        float x = Mathf.Sin(angle) * -1;
        float y = Mathf.Cos(angle);

        return new Vector2(x, y);
    }

    public void SetDistance(float distance)
    {
        this.distance = distance;
    }
}
