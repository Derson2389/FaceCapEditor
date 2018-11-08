using UnityEngine;
using System.Collections;

namespace Slate.ActionClips{

	[Category("Transform")]
	[Description("Rotate the actor around target position or object by specified degrees and optionaly per second.")]
	public class RotateAround : ActorActionClip {

		[SerializeField] [HideInInspector]
		private float _length = 1;

		public Vector3 rotation = new Vector3(0, 360, 0);
		public bool perSecond;
		public bool lookTarget = false;
		public EaseType interpolation = EaseType.QuadraticInOut;
		public PositionParameter targetPosition;

		private Vector3 originalPos;
		private Quaternion originalRot;
		private Vector3 targetOriginalPos;


		[AnimatableParameter(link="targetPosition")]
		[ShowTrajectory] [PositionHandle]
		public Vector3 targetPositionVector{
			get {return targetPosition.value;}
			set {targetPosition.value = value;}
		}


		public override string info{
			get {return string.Format("Rotate {0}{1} Around\n{2}", rotation, perSecond? " Per Second" : "", targetPosition.useAnimation? "" : targetPosition.ToString()); }
		}

		public override float length{
			get {return _length;}
			set {_length = value;}
		}

		public override float blendIn{
			get {return length;}
		}

		protected override void OnAfterValidate(){
			SetParameterEnabled("targetPositionVector", targetPosition.useAnimation);
		}
		
		protected override void OnEnter(){
			originalPos = actor.transform.position;
			originalRot = actor.transform.rotation;
			targetOriginalPos = TransformPoint(targetPosition.value, targetPosition.space);
		}

		protected override void OnUpdate(float deltaTime){
			var pos = TransformPoint(targetPosition.value, targetPosition.space);
			var targetPos = originalPos + (rotation * (perSecond? length : 1) );
			var rot = Easing.Ease(interpolation, Vector3.zero, targetPos, GetClipWeight(deltaTime));

			var angle = Quaternion.Euler(rot);
			var rotatedPos = angle * ( originalPos - targetOriginalPos) + targetOriginalPos;
			actor.transform.position = rotatedPos + ( pos - targetOriginalPos );

			if (lookTarget){
				actor.transform.rotation = Quaternion.LookRotation(pos - actor.transform.position);;
			}
		}


		protected override void OnReverse(){
			actor.transform.position = originalPos;
			actor.transform.rotation = originalRot;
		}



		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
			
		protected override void OnDrawGizmosSelected(){
			var pos = TransformPoint(targetPosition.value, targetPosition.space);
			Gizmos.DrawLine(actor.transform.position, pos);
		}		

		#endif
	}
}