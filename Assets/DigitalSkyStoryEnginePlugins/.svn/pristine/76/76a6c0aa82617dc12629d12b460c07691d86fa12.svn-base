using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SphereColliderChecker : MonoBehaviour
{
    private bool m_IsOverlap = false;
    public float Radius = 0.01f;

    public Collider[] GetOverlap()
    {
        return Physics.OverlapSphere(transform.position, Radius);
    }

    public bool IsOverlap(SkinnedMeshCollider[] checkerList)
    {
        m_IsOverlap = false;
        Collider[] colliderList = GetOverlap();
        if(colliderList == null || colliderList.Length == 0)
        {
            return false;
        }

        foreach(var checker in checkerList)
        {
            for (int i=0; i<colliderList.Length; i++)
            {
                if(colliderList[i] == checker.Collider)
                {
                    m_IsOverlap = true;
                    return true;
                }
            }
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (!enabled)
            return;

        Gizmos.color = m_IsOverlap ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
