using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARFaceMeshData
{
    public Vector3 localPosition = Vector3.zero;
    public Vector3 localRotation = Vector3.zero;

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();

    public Texture2D texture = null;
    public int index = 0;
}
