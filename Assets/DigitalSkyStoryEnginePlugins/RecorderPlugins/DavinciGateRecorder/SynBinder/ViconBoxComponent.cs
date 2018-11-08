using UnityEngine;

[ExecuteInEditMode]
public class ViconBoxComponent : MonoBehaviour
{
    public Vector3 BoxSize = new Vector3(5, 5, 5);
    public float lineWidth = 0.02f;

    private LineRenderer line;
    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.matrix = transform.localToWorldMatrix;
    //    Gizmos.DrawWireCube(Vector3.zero, BoxSize);
    //}

    private Vector3[] points = new Vector3[] {
        new Vector3(0, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(1, 1, 0),
        new Vector3(1, 0, 0),

        new Vector3(0, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1),
        new Vector3(1, 0, 0),

        new Vector3(1, 1, 0),
        new Vector3(1, 1, 1),
        new Vector3(1, 0, 1),

        new Vector3(0, 0, 1),
        new Vector3(0, 1, 1),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 1),

        new Vector3(0, 1, 1),
        new Vector3(1, 1, 1),
    };


    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
            //
            line.hideFlags = HideFlags.DontSave;
            //设置材质  
            line.material = new Material(Shader.Find("Particles/Additive"));
            //设置颜色  
            line.startColor = Color.yellow;
            line.endColor = Color.yellow;
            //设置宽度  
            //line.startWidth = lineWidth;
            //line.endWidth = lineWidth;
            //
            line.positionCount = points.Length;
            line.useWorldSpace = false;
            //SetPoints();
        }
    }

    void Update()
    {
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        for (int i = 0; i < points.Length; ++i)
        {
            var pt = points[i];
            pt = pt - new Vector3(0.5f, 0.5f, 0.5f);

            pt = new Vector3(pt.x * BoxSize.x, pt.y * BoxSize.y, pt.z * BoxSize.z);
            //pt = transform.localToWorldMatrix.MultiplyPoint(pt);

            line.SetPosition(i, pt);
        }
    }

    void OnDestroy()
    {
        if(line)
        {
           // GameObject.DestroyImmediate(line);
        }
    }
}
