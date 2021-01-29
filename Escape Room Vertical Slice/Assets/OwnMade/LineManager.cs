using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    private LineRenderer line;
    private bool killMe = false;
    private int elements = 0;
    public bool ready = false;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        Invoke("Kill", 3);
        ready = true;
    }

    private void Update()
    {
        if (killMe && line.positionCount > 0)
        {
            Kill();
        }
        else if (killMe)
        {
            Destroy(gameObject);
        }
    }

    private void Kill()
    {
        if (!killMe)
        {
            killMe = true;
        }
        int newPositionCount = line.positionCount - 1;
        Vector3[] newPositions = new Vector3[newPositionCount];

        for (int i = 0; i < newPositionCount; i++)
        {
            newPositions[i] = line.GetPosition(i + 1);
        }
        line.positionCount--;
        elements--;
        line.SetPositions(newPositions);
    }

    public void Draw(Vector3 pos)
    {
        line.positionCount = elements + 1;
        line.SetPosition(elements, pos);
        elements++;
    }
}
