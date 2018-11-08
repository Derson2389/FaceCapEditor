using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateNodeInfo
{
    public string Name;
    public Vector3 Location;
    public Quaternion QRot;
    public List<GateNodeInfo> Children = new List<GateNodeInfo>();

    public GateNodeInfo(string name, GateNodeInfo parent)
    {
        Name = name;
        if(parent != null)
        {
            parent.Children.Add(this);
        }
    }

    public GateNodeInfo GetChild(string name)
    {
        GateNodeInfo info = null;
        for (int i=0; i<Children.Count; i++)
        {
            info = Children[i];
            if(info.Name.Contains(name))
            {
                return info;
            }
        }

        return null;
    }

    public Vector3 GetReceivePosition()
    {
        if(float.IsNaN(Location.x) || float.IsNaN(Location.y) || float.IsNaN(Location.z))
        {
            return Vector3.zero;
        }

        return new Vector3(-Location.x, Location.y, Location.z) * 0.01f;
    }

    public Quaternion GetReceivedRotation()
    {
        if (float.IsNaN(QRot.x) || float.IsNaN(QRot.y) || float.IsNaN(QRot.z) || float.IsNaN(QRot.w))
        {
            return Quaternion.identity;
        }

        return new Quaternion(-QRot.x, QRot.y, QRot.z, -QRot.w);
    }
}
