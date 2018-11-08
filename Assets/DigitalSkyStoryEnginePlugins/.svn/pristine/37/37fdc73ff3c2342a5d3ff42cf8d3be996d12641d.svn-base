using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider)), DisallowMultipleComponent]
public class SkinnedMeshCollider : MonoBehaviour
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
}
