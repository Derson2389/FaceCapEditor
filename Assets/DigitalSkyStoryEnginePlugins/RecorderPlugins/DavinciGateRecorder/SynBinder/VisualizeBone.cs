using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class VisualizeBone : MonoBehaviour
{

    [HideInInspector]
    public bool showBone = true;
    [HideInInspector]
    public Color boneColor = Color.white;
    [HideInInspector]
    public float boneSize = 0.005f;
    [SerializeField]
    private List<Transform> children = new List<Transform>();

    public void AddChild(Transform child)
    {
        children.Add(child);
    }

    void OnDrawGizmos()
    {

        if (!showBone)
            return;

        Gizmos.color = boneColor;
        Gizmos.DrawSphere(transform.position, boneSize);

        foreach (Transform child in children)
        {
            Gizmos.DrawLine(transform.position, child.position);
        }
    }
}
