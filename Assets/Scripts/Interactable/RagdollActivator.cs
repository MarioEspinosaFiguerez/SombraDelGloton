using System.Collections.Generic;
using UnityEngine;

public class RagdollActivator : MonoBehaviour
{
    private Rigidbody[] ragdollRigidbodies;
    public float stillThreshold = 0.1f;
    public float timeToDestroy = 2f;

    private float stillTimer = 0f;
    private bool watchRagdoll = false;

    private float maxTimeActivate = 6f;
    private float timeActivate = 0f;
    private List<(Collider, Collider)> ignoredColliders = new List<(Collider, Collider)>();


    void Awake()
    {
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        SetRagdollState(false);
    }
    void Update()
    {
        if(watchRagdoll) CheckRagdollQuiet();
    }

    public void ActivateRagdoll()
    {
        IgnoreSelfCollisions();
        SetRagdollState(true);
        watchRagdoll = true;
        stillTimer = 0f;
    }

    private void SetRagdollState(bool state)
    {
        foreach (var rb in ragdollRigidbodies)
        {
            if (state)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.detectCollisions = false;
            }
        }
    }
    void IgnoreSelfCollisions()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            for (int j = i + 1; j < colliders.Length; j++)
            {
                Physics.IgnoreCollision(colliders[i], colliders[j]);
                ignoredColliders.Add((colliders[i], colliders[j]));
            }
        }
    }

    public void CheckRagdollQuiet()
    {
        if (!watchRagdoll) return;

        timeActivate += Time.deltaTime;
        Debug.Log("timeActivate: "+timeActivate+"    maxTimeActive: "+maxTimeActivate +"                      "+ (timeActivate >= maxTimeActivate));
        if (timeActivate >= maxTimeActivate)
        {
            Destroy(gameObject);
        }

        foreach (var rb in ragdollRigidbodies)
        {
            if (rb.velocity.magnitude > stillThreshold)
            {
                stillTimer = 0f;
                return;
            }
        }

        stillTimer += Time.deltaTime;
        if (stillTimer >= timeToDestroy)
        {
            Destroy(gameObject);
        }
    }
}
