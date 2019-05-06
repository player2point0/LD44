using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3[] pointsArr3D;

    public void setUp(List<Vector2> points, float width)
    {
        lineRenderer = GetComponent<LineRenderer>();
        pointsArr3D = new Vector3[points.Count];

        lineRenderer.positionCount = points.Count;
        lineRenderer.widthMultiplier = width * 2;

        for (int i = 0; i < points.Count; i++)
        {
            pointsArr3D[i] = points[i];
        }

        lineRenderer.SetPositions(pointsArr3D);
    }

    private void LateUpdate()
    {
        drawLine();
    }

    private void drawLine()
    {
        Vector3[] newPoints = new Vector3[pointsArr3D.Length];
        Quaternion angle = this.transform.rotation;

        for (int i = 0; i < pointsArr3D.Length; i++)
        {
            Vector2 tempPoint = pointsArr3D[i] + this.transform.position;

            newPoints[i] = tempPoint;
        }

        lineRenderer.SetPositions(newPoints);
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angles)
    {
        return angles * (point - pivot) + pivot;
    }
}
