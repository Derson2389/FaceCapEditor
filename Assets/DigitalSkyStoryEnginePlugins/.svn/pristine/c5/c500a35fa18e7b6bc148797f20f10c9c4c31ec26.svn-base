using UnityEngine;
using System.Collections;

public class SynSkelDisplay : MonoBehaviour
{

    public bool showNodes = true;
    public Color nodeColor = Color.red;
    public float nodeSize = 0.01f;

    public bool showBones = true;
    public Color boneColor = Color.white;
    public float boneSize = 0.001f;

    [SerializeField]
    private VisualizeNode[] visualizeNodes;
    [SerializeField]
    private VisualizeBone[] visualizeBones;

    public void SetupVisualise(SynSkelNode[] nodes, SynSkelBone[] bones)
    {
        Debug.Log("Setting up visualise nodes and bones...");

        // Add visualise nodes
        visualizeNodes = new VisualizeNode[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            visualizeNodes[i] = nodes[i].gameObject.AddComponent<VisualizeNode>();
        }

        // Add visualise bones
        visualizeBones = new VisualizeBone[bones.Length];
        for (int i = 0; i < bones.Length; i++)
        {
            visualizeBones[i] = bones[i].gameObject.AddComponent<VisualizeBone>();
        }

        // Sort bone hierarchy
        foreach (SynSkelBone bone in bones)
        {
            // Get child
            SynSkelNode child = bone.childNode;
            VisualizeBone vb = bone.GetComponent<VisualizeBone>();
            vb.AddChild(child.transform);

            /*
            // Get my parent 
            SynSkelNode parentNode = bone.parentNode;

            // Is there a bone associated with parent
            SynSkelBone parentBone = skel.GetBoneFromNode(parentNode);

            // Get visualise bone
            if (parentBone != null)
            {
                VisualizeBone vb = parentBone.GetComponent<VisualizeBone>();
                vb.AddChild(bone.transform);
            }
            */
        }
    }

    void OnDrawGizmos()
    {
        return;
        foreach (VisualizeNode node in visualizeNodes)
        {
            node.showNode = showNodes;
            node.nodeColor = nodeColor;
            node.nodeSize = nodeSize;
        }

        foreach (VisualizeBone bone in visualizeBones)
        {
            bone.showBone = showBones;
            bone.boneColor = boneColor;
            bone.boneSize = boneSize;
        }
    }
}
