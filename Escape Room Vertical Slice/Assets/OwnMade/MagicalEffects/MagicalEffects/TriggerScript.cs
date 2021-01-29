using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class TriggerScript : MonoBehaviour
{
    [SerializeField] protected List<LayerMask> IgnoreCollisionWith;
    [SerializeField] protected List<LayerMask> CompleteCollisionWith;

    protected Collider collider;

    protected virtual void Start()
    {
        collider = GetComponent<Collider>();
    }

    public virtual void Initiate(Transform hand, MagixInterface mI)
    {
        
    }
    protected abstract void Update();

    public abstract void OnTriggerEnter(Collider collider);
}