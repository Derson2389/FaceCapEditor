using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class SynBinder
{
    [DllImport("SynGame")]
    private static extern IntPtr SynGameGetSuitName(int suitIndex);

    [DllImport("SynGame")]
    private static extern int SynGameInit(int suitIndex, string skelFilename, bool useCallback);

    [DllImport("SynGame")]
    private static extern int SynGameClose();

    [DllImport("SynGame")]
    private static extern void SynGameReadFrame(int suitIndex);

    [DllImport("SynGame")]
    private static extern IntPtr SynGameReadFrameWChar(int suitIndex);

    [DllImport("SynGame")]
    private static extern void SynGameSetRefPose(int suitIndex);

    [DllImport("SynGame")]
    private static extern void SynGameSetZeroPose(int suitIndex);

    [DllImport("SynGame")]
    private static extern void SynGameResetRef();

    [DllImport("SynGame")]
    private static extern uint SynGameGetNumNodes(int suitIndex);

    [DllImport("SynGame")]
    private static extern uint SynGameGetNumBones(int suitIndex);

    [DllImport("SynGame")]
    private static extern Vector3 SynGameReadNodePositionUnity(int suitIndex, uint node);

    [DllImport("SynGame")]
    private static extern Quaternion SynGameReadBoneRotationUnity(int suitIndex, uint bone);

    [DllImport("SynGame")]
    private static extern void SynGameSetFixRoot(int suitIndex, bool fixRoot);

    [DllImport("SynGame")]
    private static extern void SynGameApplicationQuit();

    [DllImport("SynGame")]
    private static extern IntPtr SynGameGetCurrentDirectory();

    private int m_SuitIndex = 0;
    private string m_SkelPath;
    private bool m_IsConnected = false;
    private SynSkelParser.Skel m_Skel = null;
    public Dictionary<BipBoneHelper.BipBone, string> SynSkelMapping = null;
    public static bool LockState = false;

    public bool IsConnected
    {
        get
        {
            return m_IsConnected;
        }
    }

    public int SuitIndex
    {
        get
        {
            return m_SuitIndex;
        }
    }

    public string SkelPath
    {
        get
        {
            return m_SkelPath;
        }
    }

    public SynSkelParser.Skel Skel
    {
        get
        {
            return m_Skel;
        }
    }

    public bool ConnectSuit(int suitIndex, string skelPath)
    {
        if (suitIndex == -1)
            return false;

        Debug.Log("Syn: Connecting to suit :" + suitIndex + " skel path:" + skelPath);

        m_Skel = SynSkelParser.Parse(skelPath);
        if(m_Skel == null)
        {
            Debug.Log("Sync: skel path parse error:" + skelPath);
            return false;
        }

        string mappingFilename = skelPath.Replace(".sk", "_mapping.txt");
        SynSkelMapping = SynMappingParser.ParseMapping(mappingFilename);
        
        m_SuitIndex = suitIndex;
        m_SkelPath = skelPath;
        int result = SynGameInit(m_SuitIndex, skelPath, false);
        if (result != 0)
        {
            m_IsConnected = false;
            Debug.Log("Syn: Could not connect!");
            return false;
        }

        m_IsConnected = true;
        Debug.Log("Syn: Successfully connected");

        return true;
    }

    private SynSkelNode[] m_SkelNodes = null;
    private int m_NumNodes;
    private SynSkelBone[] m_SkelBones = null;
    private int m_NumBones;
    private GameObject m_NodeHolder;
    private GameObject m_BoneHolder;

    private GameObject[] cubeBones;
    private GameObject cubeBoneHolder;

    private static void InitInstantiatedPreviewRecursive(GameObject go)
    {
        go.hideFlags = HideFlags.DontSave;
        go.layer = 0;

        foreach (Transform transform in go.transform)
        {
            InitInstantiatedPreviewRecursive(transform.gameObject);
        }
    }

    public GameObject BuildSkeleton()
    {
        GameObject skeObj = new GameObject(m_Skel.name);

        m_NumNodes = m_Skel.numNodes;
        m_SkelNodes = new SynSkelNode[m_NumNodes];

        if (m_NodeHolder != null)
        {
            GameObject.DestroyImmediate(m_NodeHolder);
        }

        m_NodeHolder = new GameObject("SkelNodes");
        m_NodeHolder.transform.SetParent(skeObj.transform, false);

        for (int i=0; i<m_NumNodes; i++)
        {
            SynSkelParser.Skel.Node node = m_Skel.nodes[i];
            GameObject skelNodeGO = new GameObject("Node:" + node.nodeName);
            skelNodeGO.transform.SetParent(skeObj.transform, false);

            SynSkelNode skelNode = skelNodeGO.AddComponent<SynSkelNode>();
            skelNode.id = node.id;
            skelNode.nodeName = node.nodeName;
            skelNode.pos = node.pos;

            skelNode.transform.localPosition = node.pos * 0.01f;

            m_SkelNodes[i] = skelNode;
        }

        m_NumBones = m_Skel.numBones;
        m_SkelBones = new SynSkelBone[m_NumBones];

        if (m_BoneHolder != null)
        {
            GameObject.DestroyImmediate(m_BoneHolder);
        }
        m_BoneHolder = new GameObject("SkelBones");
        m_BoneHolder.transform.SetParent(skeObj.transform, false);

        for (int i = 0; i < m_NumBones; i++)
        {
            SynSkelParser.Skel.Bone bone = m_Skel.bones[i];

            // Bones
            GameObject skelBoneGO = new GameObject(bone.boneName);
            skelBoneGO.transform.SetParent(m_BoneHolder.transform, false);

            // Bone
            SynSkelBone skelBone = skelBoneGO.AddComponent<SynSkelBone>();
            skelBone.id = bone.id;
            //skelNode.flags = node.;
            skelBone.boneName = bone.boneName;
            skelBone.parentId = bone.parent;
            skelBone.parentNode = m_SkelNodes[skelBone.parentId];
            skelBone.childId = bone.child;
            skelBone.childNode = m_SkelNodes[skelBone.childId];
            skelBone.imus = bone.IMUs;
            skelBone.imuScale = bone.imuScale;
            skelBone.thickness = bone.thickness;
            skelBone.childOffset = skelBone.parentNode.transform.position - skelBone.childNode.transform.position;
            skelBone.boneLength = skelBone.childOffset.magnitude;

            // Set bone pos
            skelBone.transform.position = skelBone.parentNode.transform.position;


            // Store
            m_SkelBones[i] = skelBone;
        }

        SynSkelDisplay visualiseController = skeObj.GetComponent<SynSkelDisplay>();
        if (visualiseController == null)
        {
            visualiseController = skeObj.AddComponent<SynSkelDisplay>();
        }
            
        visualiseController.SetupVisualise(m_SkelNodes, m_SkelBones);
        
        InitInstantiatedPreviewRecursive(skeObj);

        BuildCubeBone(skeObj);

        return skeObj;
    }

    void BuildCubeBone(GameObject skeObj)
    {
        Transform transform = skeObj.transform;
        SynSkelBone[] skelBones = m_SkelBones;
        //
        if (cubeBoneHolder != null) GameObject.DestroyImmediate(cubeBoneHolder);
        cubeBoneHolder = new GameObject("CubeBone");
        cubeBoneHolder.transform.SetParent(transform, false);
        //
        List<GameObject> cubeGos = new List<GameObject>();
        for (int i = 0; i < skelBones.Length; i++)
        {
            //SynSkelParser.Skel.Bone boneData = parsedSkel.bones[i];
            //id, name, parent
            GameObject cubeGo = new GameObject(skelBones[i].name);
            cubeGo.AddComponent<VisualizeCube>();
            cubeGo.transform.SetParent(cubeBoneHolder.transform, false);
            //
            cubeGo.transform.position = skelBones[i].transform.position;
            //
            cubeGos.Add(cubeGo);
        }
        cubeBones = cubeGos.ToArray();
        Debug.Log("cubeBones:" + cubeBones.Length);
        //
        for (int i = 0; i < cubeGos.Count; ++i)
        {
            SynSkelNode node = skelBones[i].childNode;
            if (node != null)
            {
                GameObject childGo = FindChildByNodeName(cubeGos, node.nodeName);
                if (childGo != null && childGo != cubeGos[i])
                {
                    childGo.transform.SetParent(cubeGos[i].transform, true);
                }
            }
        }
    }

    GameObject FindChildByNodeName(List<GameObject> gos, string name)
    {
        return gos.Find(g => g.name.Equals(name));
    }
    

    public SynSkelParser.Skel.Bone GetBoneFromName(string name)
    {
        foreach (SynSkelParser.Skel.Bone bone in m_Skel.bones)
        {
            if (bone.boneName == name)
            {
                return bone;
            }
        }
        return null;
    }

    public SynSkelBone GetSynBoneFromName(string name)
    {
        foreach (SynSkelBone bone in m_SkelBones)
        {
            if (bone.name == name)
            {
                return bone;
            }
        }
        return null;
    }

    public GameObject GetCubeBoneFromName(string name)
    {
        GameObject cube = null;
        cube = Array.Find<GameObject>(cubeBones, g => g.name.Equals(name));
        return cube;

    }
    

    public void DisconnectSuit()
    {
        Debug.Log("Syn: Disconnecting from suit...");
        SynGameClose();
    }

    public struct MocapFrame
    {
        public int frameNo;
        public Vector3[] nodePositions;
        public Quaternion[] boneRotations;
    }

    public SynSkelNode GetNodeFromID(int ID)
    {
        foreach (SynSkelNode node in m_SkelNodes)
            if (node.id == ID)
                return node;
        return null;
    }

    public SynSkelBone GetBoneFromID(int ID)
    {
        foreach (SynSkelBone bone in m_SkelBones)
            if (bone.id == ID)
                return bone;
        return null;
    }

    private MocapFrame m_MocapFrame;

    public void UpdateMocapFrame()
    {
        if(!m_IsConnected || m_Skel == null)
        {
            return;
        }

        SynGameReadFrame(m_SuitIndex);

        uint totalNodes = SynGameGetNumNodes(m_SuitIndex);
        m_MocapFrame.nodePositions = new Vector3[totalNodes];
        for (uint i = 0; i < totalNodes; i++)
        {
            m_MocapFrame.nodePositions[i] = SynGameReadNodePositionUnity(m_SuitIndex, i);
        }

        uint totalBones = SynGameGetNumBones(m_SuitIndex);
        m_MocapFrame.boneRotations = new Quaternion[totalBones];
        for (uint i = 0; i < totalBones; i++)
        {
            m_MocapFrame.boneRotations[i] = SynGameReadBoneRotationUnity(m_SuitIndex, i);
        }

        for (int i = 0; i < m_MocapFrame.nodePositions.Length; i++)
        {
            SynSkelNode node = GetNodeFromID(i);
            if (node != null)
            {
                node.pos = m_MocapFrame.nodePositions[i];
            }
        }
        
        for (int i = 0; i < m_MocapFrame.boneRotations.Length; i++)
        {
            SynSkelBone bone = GetBoneFromID(i);
            if (bone != null)
            {
                bone.rot = m_MocapFrame.boneRotations[i];
            }
        }

        for (int i = 0; i < m_SkelNodes.Length; i++)
        {
            if (!System.Single.IsNaN(m_SkelNodes[i].pos.x) && !System.Single.IsNaN(m_SkelNodes[i].pos.y) && !System.Single.IsNaN(m_SkelNodes[i].pos.z))
            {
                m_SkelNodes[i].transform.localPosition = m_SkelNodes[i].pos;
            }

        }
        for (int i = 0; i < m_SkelBones.Length; i++)
        {
            m_SkelBones[i].transform.localRotation = m_SkelBones[i].rot;
            m_SkelBones[i].transform.localPosition = m_SkelBones[i].parentNode.pos;
        }

        for (int i = 0; i < cubeBones.Length; ++i)
        {
            cubeBones[i].transform.rotation = m_SkelBones[i].rot;
        }
        
    }

//     public void UpdateBone(StandardBone bone)
//     {
//         if (!m_IsConnected || m_Skel == null)
//         {
//             return;
//         }
// 
//         SynSkelBone synBone = null;
//         BipBoneMapping.TryGetValue(bone.BoneType, out synBone);
//         if (synBone != null)
//         {
//             bone.BoneTransform.rotation = synBone.transform.rotation * synBone.skelToCharacterOffset;
//         }
//     }

    public static void SetRefPose(int index)
    {
        SynGameSetRefPose(index);
    }

    public static void SetZeroPose(int index)
    {
        SynGameSetZeroPose(index);
    }
}
