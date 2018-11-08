using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Slate.ActionClips{

	[Attachable(typeof(MecanimTrack))]
	[Description("Animate an actor IK Goal. Please note that 'IK Pass' must be enabled in the Controller.")]
	public class AnimateLimbIK : ActorActionClip<Animator>{

		[SerializeField] [HideInInspector]
		private float _length = 1f;
		[SerializeField] [HideInInspector]
		private float _blendIn = 0.2f;
		[SerializeField] [HideInInspector]
		private float _blendOut = 0.2f;
		
		public AvatarIKGoal IKGoal = AvatarIKGoal.RightHand;
		[AnimatableParameter(0,1)]
		public float weight = 1;
		public TransformationParameter IKTarget;

		[AnimatableParameter(link="IKTarget")]
		[ShowTrajectory] [PositionHandle]
		public Vector3 targetPosition{
			get {return IKTarget.position;}
			set {IKTarget.position = value;}
		}

		[AnimatableParameter(link="IKTarget")]
		public Vector3 targetRotation{
			get {return IKTarget.rotation;}
			set {IKTarget.rotation = value;}
		}

		private Vector3 lastPos;
		private Quaternion lastRot;
		private float lastWeight;

		private bool isEnter = false;
		private bool isReverse = false;

		private AnimatorDispatcher dispatcher{
			get {return (parent as MecanimTrack).dispatcher;}
		}

		public override string info{
			get {return string.Format("'{0}' IK", IKGoal.ToString());}
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
			IKTarget.position = ActorPositionInSpace(IKTarget.space);
		}

		protected override void OnAfterValidate(){
			SetParameterEnabled("targetPosition", IKTarget.useAnimation);
			SetParameterEnabled("targetRotation", IKTarget.useAnimation);
		}

		protected override void OnEnter(){
			dispatcher.onAnimatorIK += OnAnimatorIK;
			isEnter = true;
		}

		protected override void OnReverseEnter(){
			dispatcher.onAnimatorIK += OnAnimatorIK;
		}

		protected override void OnReverse(){
			dispatcher.onAnimatorIK -= OnAnimatorIK;
			isReverse = true;
		}

		protected override void OnExit(){
			dispatcher.onAnimatorIK -= OnAnimatorIK;
		}


		void OnAnimatorIK(int index){

			if (isEnter){
				isEnter    = false;
				lastPos    = actor.GetIKPosition(IKGoal);
				lastRot    = actor.GetIKRotation(IKGoal);
				lastWeight = actor.GetIKPositionWeight(IKGoal);
				return;
			}

			if (isReverse){
				isReverse = false;
				actor.SetIKPosition(IKGoal, lastPos);
				actor.SetIKRotation(IKGoal, lastRot);
				actor.SetIKPositionWeight(IKGoal, lastWeight);
				actor.SetIKRotationWeight(IKGoal, lastWeight);
				return;
			}

			var finalWeight = GetClipWeight() * weight;
			var pos = TransformPoint(IKTarget.position, IKTarget.space);
			var rot = Quaternion.Euler(targetRotation);
			actor.SetIKPosition(IKGoal, pos);
			actor.SetIKRotation(IKGoal, rot);
			actor.SetIKPositionWeight(IKGoal, finalWeight);
			actor.SetIKRotationWeight(IKGoal, finalWeight);
		}
	}
}