using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricityBomb : TriggerScript
{
    SphereCollider sp;
  
    Rigidbody rB;
    Rigidbody thisRB;
    Vector3 direction;
    Transform hand;
    MagixInterface mI;
    Vector3 correctDirection;
    float explosion = 10f;
    bool cooldown = false;
    bool check = false;
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
            thisRB.useGravity = true;            
            correctDirection = hand.transform.forward;
            thisRB.AddForce(correctDirection * 5f, ForceMode.Impulse);
            cooldown = true;
        }
        else
        {
            transform.position = hand.position;
        }
    }
    void Cooldown()
    {
        cooldown = false;

    }

    public override void Initiate(Transform hand, MagixInterface mI)
    {
       
        this.hand = hand;
        this.mI = mI;
        release = false;

    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (!check)
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
                        sp.radius *= 4f;
                        check = true;
                        Invoke("Explosion", 1f);
                        return;
                    }
                }
            }
        }
    }

    public void OnTriggerStay(Collider collider)
    {
        if (check)
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
                        Debug.Log("explosion!!");
                        rB = collider.gameObject.GetComponent<Rigidbody>();
                        direction = sp.center - collider.transform.position;
                        direction.Normalize();
                        rB.AddForce(direction * explosion, ForceMode.Impulse);
                        return;
                    }
                }
            }
        }
    }

    private void Explosion()
    {
        Destroy(gameObject);
    }
}
