using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewFaceEditCtrl", menuName = "Face/Controller Set")]
public class FaceControllerSet : ScriptableObject
{

	[SerializeField]
	public List<FaceController> ctrls = new List<FaceController>();

	[System.Serializable]
	public class FaceController
    {		
		public string ctrName = string.Empty;
        public int type = 0;
		public List<FaceblenderShape> ShapList = new List<FaceblenderShape>();			
	}
	public struct FaceblenderShape
	{
		public string blendName;
		public int blendIdx;
		public FaceblenderShape(string name, int idx)
		{
			this.blendName = name;
			this.blendIdx = idx;
		}
	}
}
