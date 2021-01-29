using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineManager : MonoBehaviour
{
    public Transform controller;
    public Material material;
    public GameObject Marker;//
    public GameObject Output;

    private Vector3 previousPosition;
    private List<GameObject> markers;//
    List<Vector3> positions;
    private LineRenderer line;
    private LineManager lineMan;
    private bool drawing = false;
    private float minDistance = 0.04f;

    public void StartDraw()
    {
        markers = new List<GameObject>();//
        positions = new List<Vector3>();
        GameObject go = new GameObject();
        go.transform.position = controller.position;
        go.transform.rotation = controller.rotation;
        line = go.AddComponent<LineRenderer>();
        lineMan = go.AddComponent<LineManager>();
        line.SetWidth(0.02f, 0.02f);
        line.material = material;
        drawing = true;
        markers.Add(Instantiate(Marker, controller.position, controller.rotation));//
        positions.Add(controller.position);
        previousPosition = controller.position;
    }

    public void StopDrawing()
    {
        drawing = false;
        Output.GetComponent<MagixInterface>().Run(positions);
        foreach (GameObject mark in markers)
        {
            Destroy(mark);
        }
        positions.Clear();
    }

    private void AddPoints(Vector3 closestPosition)
    {
        for (float i = Vector3.Distance(closestPosition, controller.position); i > minDistance; i -= minDistance)
        {
            Vector3 vector = controller.position - closestPosition;
            vector.Normalize();
            Vector3 newPosition = closestPosition + (vector * minDistance);
            markers.Add(Instantiate(Marker, newPosition, controller.rotation));
            positions.Add(newPosition);
            previousPosition = newPosition;
        }
    }

    private void Update()
    {
        if (drawing && lineMan.ready)
        {
            lineMan.Draw(controller.position);
            Vector3 shortestPosition = previousPosition;
            float shortestDistance = 1f;
            if (Vector3.Distance(previousPosition, controller.position) > minDistance)
            {
                bool failed = false;
                if (positions.Count > 10)
                {
                    for (int i = positions.Count - 10; i < positions.Count; i++)
                    {
                        if (Vector3.Distance(positions[i], controller.position) < minDistance)
                        {
                            failed = true;
                            break;
                        }
                        if (Vector3.Distance(positions[i], controller.position) < shortestDistance)
                        {
                            shortestDistance = Vector3.Distance(positions[i], controller.position);
                            shortestPosition = positions[i];
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < positions.Count; i++)
                    {
                        if (Vector3.Distance(positions[i], controller.position) < minDistance)
                        {
                            failed = true;
                            break;
                        }
                        if (Vector3.Distance(positions[i], controller.position) < shortestDistance)
                        {
                            shortestDistance = Vector3.Distance(positions[i], controller.position);
                            shortestPosition = positions[i];
                        }
                    }
                }
                if (!failed)
                    AddPoints(shortestPosition);
            }
        }
    }
}
