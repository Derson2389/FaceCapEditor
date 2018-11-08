using UnityEngine;
using System.Collections;

namespace Slate{

	[ExecuteInEditMode]
	///Forwards Animator based calls
	public class AnimatorDispatcher : MonoBehaviour {
		
		public event System.Action onAnimatorMove;
		public event System.Action<int> onAnimatorIK;

		private bool wasRootMotion;
		private Animator _animator;
		private Animator animator{
			get {return _animator != null? _animator : _animator = GetComponent<Animator>();}
		}

		void Awake(){
			wasRootMotion = animator.applyRootMotion;
		}

		void OnAnimatorMove () {
			if (onAnimatorMove != null){
				onAnimatorMove();
				return;
			}

			if (wasRootMotion){
				animator.ApplyBuiltinRootMotion();
			}
		}

		void OnAnimatorIK(int index){
			if (onAnimatorIK != null){
				onAnimatorIK(index);
			}
		}
	}
}