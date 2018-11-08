using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshColliderChecker : MonoBehaviour
{
    private SkinnedMeshRenderer m_MeshRenderer = null;
    private MeshCollider m_MeshCollider = null;

    public MeshCollider Collider
    {
        get
        {
            if(m_MeshCollider == null)
            {
                m_MeshCollider = gameObject.GetComponent<MeshCollider>();
                if(m_MeshCollider == null)
                {
                    m_MeshCollider = gameObject.AddComponent<MeshCollider>();
                }
                m_MeshCollider.convex = true;
            }

            return m_MeshCollider;
        }
    }

    public SkinnedMeshRenderer TargetMesh
    {
        get
        {
            if(m_MeshRenderer == null)
            {
                m_MeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            }
            return m_MeshRenderer;
        }
    }

    public void UpdateCollider()
    {
        if(TargetMesh == null || Collider == null)
        {
            return;
        }

        Mesh colliderMesh = new Mesh();
        TargetMesh.BakeMesh(colliderMesh);
        Collider.sharedMesh = colliderMesh;
    }

    private void Update()
    {
        UpdateCollider();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.collider.name);
    }
}
