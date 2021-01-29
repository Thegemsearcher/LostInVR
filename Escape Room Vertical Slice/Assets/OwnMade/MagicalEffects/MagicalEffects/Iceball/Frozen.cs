using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frozen : MonoBehaviour
{
    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
        Invoke("Unfreeze", 2f);
    }

    void Unfreeze()
    {
        if(gameObject.tag == "IceParticles")
        {
            Destroy(gameObject);
            Destroy(this);
            return;
        }
        rigidbody.constraints = RigidbodyConstraints.None;
        foreach (Transform child in transform)
        {
            if (child.tag == "IceParticles")
            {
                Destroy(child.gameObject);
                break;
            }
        }
        Destroy(this);
    }
}
