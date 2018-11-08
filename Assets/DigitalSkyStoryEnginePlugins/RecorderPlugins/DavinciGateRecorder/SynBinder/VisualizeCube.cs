using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class VisualizeCube : MonoBehaviour
{

    [HideInInspector]
    public bool showBone = true;
    [HideInInspector]
    public Color boneColor = Color.yellow;
    [HideInInspector]
    public float cubeSize = 0.007f;


	void OnDrawGizmos() {

        if (!showBone)
            return;

        Gizmos.color = boneColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(cubeSize, cubeSize, cubeSize));
	}
}
