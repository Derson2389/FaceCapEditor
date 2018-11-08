using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class VisualizeNode : MonoBehaviour
{

    [HideInInspector]
    public bool showNode = true;
    [HideInInspector]
    public Color nodeColor = Color.red;
    [HideInInspector]
    public float nodeSize = 0.01f;

    void OnDrawGizmos()
    {
        if (!showNode)
            return;

        Gizmos.color = nodeColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(nodeSize, nodeSize, nodeSize));
    }
}
