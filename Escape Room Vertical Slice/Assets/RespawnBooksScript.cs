using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnBooksScript : MonoBehaviour
{
    public List<GameObject> Books;
    private List<Vector3> position;
    private List<Quaternion> rotation;
    // Start is called before the first frame update
    void Start()
    {
        position = new List<Vector3>();
        rotation = new List<Quaternion>();
        foreach(GameObject book in Books)
        {
            position.Add(book.transform.position);
            rotation.Add(book.transform.rotation);
        }
    }

    public void Respawn()
    {
        int i = 0;
        foreach (GameObject book in Books)
        {
            book.transform.position = position[i];
            book.transform.rotation = rotation[i];
            i++;
        }
    }
}
