using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSphereScript : TriggerScript
{
    public GameObject IceParticles;
    Rigidbody thisRB;
    Transform hand;
    MagixInterface mI;
    Vector3 correctDirection;
    bool release = false;

    protected override void Start()
    {
        base.Start();
      
        thisRB = GetComponent<Rigidbody>();
    }
    protected override void Update()
    {

        release = mI.ReleaseSpell();

        if (release)
        {
            thisRB.useGravity = true;
            correctDirection = hand.transform.forward;
            thisRB.AddForce(correctDirection * 5f, ForceMode.Impulse);
            
        }
        else
        {
            transform.position = hand.position;
        }
    }
    //void Cooldown()
    //{
    //    cooldown = false;

    //}

    public override void Initiate(Transform hand, MagixInterface mI)
    {

        this.hand = hand;
        this.mI = mI;
        release = false;

    }
    public override void OnTriggerEnter(Collider collider)
    {
        GameObject particles;
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
                    collider.gameObject.AddComponent<Frozen>();
                    particles = Instantiate(IceParticles, collider.transform);
                    particles.tag = "IceParticles";
                    particles.transform.localScale = collider.transform.localScale;
                    Destroy(gameObject);
                    return;
                }
            }
        }
        particles = Instantiate(IceParticles, transform.position, Quaternion.identity);
        particles.tag = "IceParticles";
        particles.AddComponent<Frozen>();
        Destroy(gameObject);
    }
}