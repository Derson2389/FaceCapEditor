using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class FaceControllerComponent : MonoBehaviour {

    private SkinnedMeshRenderer SkinMesh = null;
    public List<BlendShape> blendShapeList = new List<BlendShape>();

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

    public void SetFaceController(int idx, float weight)
    {
        SkinMesh = this.GetComponent<SkinnedMeshRenderer>();
        if (SkinMesh != null)
        {
            SkinMesh.SetBlendShapeWeight(idx, weight);
        }
    }

	// Update is called once per frame
	void Update ()
    {
        


	}
}
