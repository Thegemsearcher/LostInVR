using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WindScript : TriggerScript
{
    public bool DestroyOnUnspecifiedCollision;
    public bool DestroyOnCompletedCollision;
    public float LifeTime;
    public float Speed;
    public float Force;
    public Vector3 Direction;
    LineRenderer Line;
    float SpinSpeed;

    public void Initialize(float lifeTime, float speed, float force, Vector3 direction, bool destroyOnCompletedCollision = false, bool destroyOnUnspecifiedCollision = true)
    {
        LifeTime = lifeTime;
        Speed = speed;
        Force = force;
        Direction = direction;
        SpinSpeed = (1 - 1 / (1 * Speed + 0.2f)) * 6;
        DestroyOnCompletedCollision = destroyOnCompletedCollision;
        DestroyOnCompletedCollision = destroyOnUnspecifiedCollision;
    }

    protected override void Start()
    {
        base.Start();
        Direction.Normalize();
        Invoke("StartTTL", LifeTime);
        Line = GetComponent<LineRenderer>();
        SpinSpeed = (1 - 1 / (1 * Speed + 0.2f)) * 6;
    }

    protected override void Update()
    {
        SpinSpeed = ((1 - 1 / (1 * (Speed + 30.6227766f) * ((Speed + 30.6227766f) * 0.001f) + 0.2f)) * 6);
        transform.Translate(Direction * Speed * Time.deltaTime);
        for (float i = 0; i < Line.positionCount; i++)
        {
            float Yposition = (Mathf.PI * 2f) * (Time.time * SpinSpeed - ((Line.positionCount - i) / Line.positionCount));
            float Xposition = (Mathf.PI * 2f) * (Time.time * SpinSpeed - ((Line.positionCount - i) / Line.positionCount));
            Yposition = Mathf.Sin(Yposition);
            Xposition = Mathf.Cos(Xposition);
            Vector3 newPosition = new Vector3(Xposition, Yposition, i);
            Line.SetPosition((int)i, newPosition);
        }
    }

    public override void OnTriggerEnter(Collider collider)
    {
        //Unnessecary for this script, but it's required because of the parent, lol.
    }

    public void OnTriggerStay(Collider collider)
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
                    collider.GetComponent<Rigidbody>().AddForce(transform.rotation * Direction * Force, ForceMode.Force);
                    if (DestroyOnCompletedCollision)
                        StartTTL();
                    return;
                }
            }
        }
        if (DestroyOnUnspecifiedCollision)
        {
            StartTTL();
        }
    }

    void StartTTL()
    {
        Invoke("TTL", 1f);
    }

    void TTL()
    {
        Destroy(gameObject);
    }
}