using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Slate.ActionClips{

	[Attachable(typeof(MecanimTrack))]
	abstract public class MecanimBaseClip : ActorActionClip<Animator>{
		
		public override bool isValid{
			get {return actor != null && actor.runtimeAnimatorController != null;}
		}

		protected bool HasParameter(string name){
			if (actor == null){
				return false;
			}
			
			if (!actor.isInitialized){
				return true;
			}
			var parameters = actor.parameters;
			return parameters != null && parameters.FirstOrDefault(p => p.name == name) != null;
		}
	}
}