using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class FaceControllerComponent : MonoBehaviour {

    private SkinnedMeshRenderer SkinMesh = null;
    public List<BlendShape> blendShapeList = new List<BlendShape>();

    //[SerializeField]
    //private string configPath;
    [SerializeField]
    public List<string> controllerList = new List<string>();

	// Use this for initialization
	void Start () {
        SkinMesh = this.GetComponent<SkinnedMeshRenderer>();
        if (SkinMesh != null)
        {
            blendShapeList.Clear();
            int blendShapesCnt = SkinMesh.sharedMesh.blendShapeCount;
            for (int idx = 0; idx < blendShapesCnt; idx++)
            {
                BlendShape newBlend = new BlendShape();
                newBlend.blendableIndex = idx;
                newBlend.blendableName = SkinMesh.sharedMesh.GetBlendShapeName(idx);
                newBlend.weight = SkinMesh.GetBlendShapeWeight(idx);
                blendShapeList.Add(newBlend);
            }
        }
    }

    public int GetMeshRenderIdxByName(string name)
    {
        int blenderIdx = 0;
        SkinMesh = this.GetComponent<SkinnedMeshRenderer>();
        if (SkinMesh != null)
        {
            blenderIdx = SkinMesh.sharedMesh.GetBlendShapeIndex(string.Format("{0}.{1}", "facial_blendShape", name));
        }
        return blenderIdx;
    }

    public void SetFaceController(int idx, float weight)
    {       
        SkinMesh = this.GetComponent<SkinnedMeshRenderer>();
        if (SkinMesh != null)
        {
            SkinMesh.SetBlendShapeWeight(idx, weight);
        }
    }
    public float getBlendShapePreValue(int idx )
    {
        SkinMesh = this.GetComponent<SkinnedMeshRenderer>();
        if (SkinMesh != null)
        {
           return SkinMesh.GetBlendShapeWeight(idx);
        }
        return 0;
    }


    public BlendShape GetBlendShapeIdxByName(string name)
    {
        BlendShape shape = null;
        for (int i = 0; i < blendShapeList.Count; i++)
        {
            var bs = blendShapeList[i];
            if (bs.blendableName == string.Format("{0}.{1}","facial_blendShape", name))
            {
                shape = bs;
            }
        }
        return shape;
    }


}
