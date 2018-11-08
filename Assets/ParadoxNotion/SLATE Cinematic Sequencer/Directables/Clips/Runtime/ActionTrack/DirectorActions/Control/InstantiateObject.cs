using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Slate.ActionClips{

	[Category("Control")]
	[Description("Instantiates an object with optional popup animation if BlendIn is higher than zero. You can optionaly 'popdown' and destroy the object after a period of time, if you also set a BlendOut value higher than zero.")]
	public class InstantiateObject : DirectorActionClip {

		[SerializeField] [HideInInspector]
		private float _length = 2f;
		[SerializeField] [HideInInspector]
		private float _blendIn = 0f;
		[SerializeField] [HideInInspector]
		private float _blendOut = 0f;

		[Required]
		public GameObject targetObject;
		public Transform optionalParent;
		public Vector3 targetPosition;
		public Vector3 targetRotation;
		public MiniTransformSpace space;
		public EaseType popupInterpolation = EaseType.ElasticInOut;

		private GameObject instance;
		private Vector3 originalScale;

		public override bool isValid{
			get {return targetObject != null;}
		}

		public override float length{
			get {return _length;}
			set {_length = value;}
		}

		public override float blendIn{
			get {return _blendIn;}
			set {_blendIn = value;}
		}

		public override float blendOut{
			get {return _blendOut;}
			set {_blendOut = value;}
		}

		public override string info{
			get	{return string.Format("Instantiate\n{0}", targetObject != null? targetObject.name : "NULL");}
		}

		protected override void OnEnter(){
			originalScale = targetObject.transform.localScale;
			instance = (GameObject)Instantiate(targetObject);
			SceneManager.MoveGameObjectToScene(instance, root.context.scene);
			instance.transform.parent = optionalParent;
			instance.transform.localEulerAngles = targetRotation;
			instance.transform.localPosition = TransformPoint(targetPosition, (TransformSpace)space);
		}

		protected override void OnUpdate(float deltaTime){
			if (instance != null){
				instance.transform.localScale = Easing.Ease(popupInterpolation, Vector3.zero, originalScale, GetClipWeight(deltaTime));
			}
		}

		protected override void OnExit(){
			if (blendOut > 0){
				DestroyImmediate(instance, false);
			}
		}

		protected override void OnReverseEnter(){
			if (blendOut > 0){
				OnEnter();
			}
		}

		protected override void OnReverse(){
			DestroyImmediate(instance, false);
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnSceneGUI(){
			if (optionalParent == null){
				DoVectorPositionHandle( (TransformSpace)space, ref targetPosition);
			}
		}		

		#endif

	}
}