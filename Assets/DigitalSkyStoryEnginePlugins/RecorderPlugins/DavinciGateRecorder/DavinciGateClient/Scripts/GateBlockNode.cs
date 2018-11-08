using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateBlockNode
{
    public Transform Target;
    public LinkColliderChecker LinkChecker = null;
    public SphereColliderChecker[] CheckerList = null;

    public List<GateBlockNode> Children = new List<GateBlockNode>();

    public Vector3 LastPos;
    public Quaternion LastRot;

    public GateBlockNode(Transform target)
    {
        Target = target;
        LinkChecker = Target.GetComponent<LinkColliderChecker>();
        CheckerList = Target.GetComponents<SphereColliderChecker>();

        for(int i=0; i<target.childCount; i++)
        {
            Children.Add(new GateBlockNode(target.GetChild(i)));
        }
    }

    public bool CheckHasBlock()
    {
        if(CheckerList.Length > 0)
        {
            return true;
        }

        for (int i = 0; i < Children.Count; i++)
        {
            if(Children[i].CheckHasBlock())
            {
                return true;
            }
        }

        return false;
    }
}
