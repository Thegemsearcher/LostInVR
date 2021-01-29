using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : TriggerScript
{
    SphereCollider sp;
  
    Rigidbody rB;
    Rigidbody thisRB;
    Vector3 direction;
    Transform hand;
    MagixInterface mI;
    Vector3 correctDirection;
    bool release = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        sp = gameObject.GetComponent<SphereCollider>();
        thisRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        release = mI.ReleaseSpell();

        if (release)
        {
            Destroy(gameObject);

            thisRB.useGravity = true;            
            correctDirection = hand.transform.forward;
            thisRB.AddForce(correctDirection * 5f, ForceMode.Impulse);
        }
        else
        {
            transform.position = hand.position;
        }
    }


    public override void Initiate(Transform hand, MagixInterface mI)
    {
       
        this.hand = hand;
        this.mI = mI;
        release = false;

    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (IgnoreCollisionWith.Count > 0)
        {
            foreach (LayerMask ignoreThis in IgnoreCollisionWith)
            {
                if ((ignoreThis.value & 1 << collider.gameObject.layer) != 0)
                {
                    return;
                }
            }
        }
        if (CompleteCollisionWith.Count > 0)
        {
            foreach (LayerMask completeThis in CompleteCollisionWith)
            {
                if ((completeThis.value & 1 << collider.gameObject.layer) != 0)
                {
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }
}
