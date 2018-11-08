using UnityEngine;
using System.Collections;

public class SynSkelBone : MonoBehaviour
{

    public int id;
    public string boneName;
    public int parentId;
    public SynSkelNode parentNode;
    public int childId;
    public SynSkelNode childNode;
    public int[] imus;
    public float imuScale;
    public float thickness;
    public int flags;
    public Quaternion rot;
    public float boneLength;
    public Vector3 childOffset;

    public Quaternion skelToCharacterOffset;

}
