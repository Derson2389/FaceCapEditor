using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Slate.ActionClips{

	[Attachable(typeof(MecanimTrack))]
	[Description("Make the actor look at target position. Please note that 'IK Pass' must be enabled in the Controller.")]
	public class AnimateLookAtIK : ActorActionClip<Animator>{

		[SerializeField] [HideInInspector]
		private float _length = 1f;
		[SerializeField] [HideInInspector]
		private float _blendIn = 0.2f;
		[SerializeField] [HideInInspector]
		private float _blendOut = 0.2f;
		
		[AnimatableParameter(0,1)]
		public float weight = 1;

		[AnimatableParameter(0,1)]
		public float bodyWeight = 0.25f;
		[AnimatableParameter(0,1)]
		public float headWeight = 0.95f;
		[AnimatableParameter(0,1)]
		public float eyesWeight = 1;
		
		public PositionParameter targetPosition;

		[AnimatableParameter(link="targetPosition")]
		[ShowTrajectory] [PositionHandle]
		public Vector3 targetPositionVector{
			get {return targetPosition.value;}
			set {targetPosition.value = value;}
		}

		private AnimatorDispatcher dispatcher{
			get {return (parent as MecanimTrack).dispatcher;}
		}

		public override string info{
			get {return string.Format("Look At IK");}
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

		protected override void OnCreate(){
			targetPosition.value = ActorPositionInSpace(targetPosition.space);
		}

		protected override void OnAfterValidate(){
			SetParameterEnabled("targetPositionVector", targetPosition.useAnimation);
		}

		protected override void OnEnter(){
			dispatcher.onAnimatorIK += OnAnimatorIK;
		}

		protected override void OnReverseEnter(){
			dispatcher.onAnimatorIK += OnAnimatorIK;
		}

		protected override void OnReverse(){
			dispatcher.onAnimatorIK -= OnAnimatorIK;
		}

		protected override void OnExit(){
			dispatcher.onAnimatorIK -= OnAnimatorIK;
		}

		void OnAnimatorIK(int index){
			var finalWeight = GetClipWeight() * weight;
			var pos = TransformPoint(targetPosition.value, targetPosition.space);
			actor.SetLookAtPosition(pos);
			actor.SetLookAtWeight(finalWeight, bodyWeight, headWeight, eyesWeight, 0.5f);
		}
	}
}