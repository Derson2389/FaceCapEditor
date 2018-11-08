using UnityEngine;
using System.Collections;

namespace Slate{

    ///An interface for TimePointers (since structs can't be abstract)
    public interface IDirectableTimePointer {
        float time { get; }
        void TriggerForward(float currentTime, float previousTime);
        void TriggerBackward(float currentTime, float previousTime);
        void Update(float currentTime, float previousTime);
        IDirectable Target{get; }
    } 

	///Wraps the startTime of a group, track or clip (IDirectable) along with it's relevant execution
	public struct StartTimePointer : IDirectableTimePointer{
		
		private bool triggered;
		private float lastTargetStartTime;
		private IDirectable target;
		float IDirectableTimePointer.time{ get {return target.startTime;} }
		float targetLength{get { return target.endTime - target.startTime; }}
        public IDirectable Target
        {
            get
            {
                return target;
            }
        }

		public StartTimePointer(IDirectable target){
			this.target = target;
			triggered = false;
			lastTargetStartTime = target.startTime;
		}
		
		void IDirectableTimePointer.TriggerForward(float currentTime, float previousTime){
			if (currentTime >= target.startTime){
				if (!triggered){
					triggered = true;
					target.Enter();
					target.Update( Mathf.Clamp(currentTime - target.startTime, 0, targetLength ), 0);
				}
			}
		}

		void IDirectableTimePointer.Update(float currentTime, float previousTime){

			//target directable callbacks
			if (currentTime >= target.startTime && currentTime < target.endTime && currentTime > 0 && currentTime < target.root.length){

				var localCurrentTime = currentTime - target.startTime;
				var localPreviousTime = (previousTime - target.startTime) + (target.startTime - lastTargetStartTime);

				localCurrentTime = Mathf.Clamp(localCurrentTime, 0, targetLength);
				localPreviousTime = Mathf.Clamp(localPreviousTime, 0, targetLength);

				#if UNITY_EDITOR
				if (!Application.isPlaying && target is IKeyable && !target.root.isReSampleFrame){
					if (localCurrentTime == localPreviousTime){
						if (Prefs.autoKey && GUIUtility.hotControl == 0){
							((IKeyable)target).TryAutoKey(localCurrentTime);
						}
					}
				}
				#endif

				target.Update(localCurrentTime, localPreviousTime);
				lastTargetStartTime = target.startTime;
			}


			//root updated callback
			target.RootUpdated(currentTime, previousTime);
		}

		void IDirectableTimePointer.TriggerBackward(float currentTime, float previousTime){
			if (currentTime < target.startTime || currentTime <= 0){
				if (triggered){
					triggered = false;
					target.Update(0, Mathf.Clamp(previousTime - target.startTime, 0, targetLength) );
					target.Reverse();
				}
			}
		}
	}

	///Wraps the endTime of a group, track or clip (IDirectable) along with it's relevant execution
	public struct EndTimePointer : IDirectableTimePointer{
		
		private bool triggered;
		private IDirectable target;
		float IDirectableTimePointer.time{ get {return target.endTime;} }
		float targetLength{get {return target.endTime - target.startTime; }}
        public IDirectable Target
        {
            get
            {
                return target;
            }
        }
        public EndTimePointer(IDirectable target){
			this.target = target;
			triggered = false;
		}

		void IDirectableTimePointer.TriggerForward(float currentTime, float previousTime){
			if (currentTime >= target.endTime || (currentTime == target.root.length && target.startTime < target.root.length) ){
				if (!triggered){
					triggered = true;
					target.Update(targetLength, Mathf.Clamp(previousTime - target.startTime, 0, targetLength) );
					target.Exit();
				}
			}
		}

		void IDirectableTimePointer.Update(float currentTime, float previousTime){
			//Update is/should never be called in TimeOutPointers
			throw new System.NotImplementedException();
		}

		void IDirectableTimePointer.TriggerBackward(float currentTime, float previousTime){
			if ( (currentTime < target.endTime || currentTime <= 0) && currentTime != target.root.length ){
				if (triggered){
					triggered = false;
					target.ReverseEnter();
					target.Update( Mathf.Clamp(currentTime - target.startTime, 0, targetLength), targetLength);
				}
			}
		}
	}
}