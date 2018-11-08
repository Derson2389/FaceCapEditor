using UnityEngine;
using System.Collections;

namespace Slate.ActionClips{

	[Category("Utility")]
	public class DebugLog : DirectorActionClip {

		public string text;

		public override string info{
			get {return string.Format("Debug Log\n'{0}'", text);}
		}

		protected override void OnEnter(){
			Debug.Log(string.Format("<b>Cutscene:</b> {0}", text));
		}
	}
}