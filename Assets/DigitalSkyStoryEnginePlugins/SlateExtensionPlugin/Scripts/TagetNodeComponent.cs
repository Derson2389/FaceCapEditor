using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagetNodeComponent : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Target_40x");
    }
}
