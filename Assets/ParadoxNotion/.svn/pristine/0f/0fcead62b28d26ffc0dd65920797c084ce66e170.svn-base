using UnityEngine;
using System.Collections;

namespace Slate.ActionClips{

	abstract public class SendMessage<T> : SendMessage {

		public T value;

		public override string info{
			get {return string.Format("Message\n{0}({1})", message, value != null? value.ToString() : "null");}
		}

		public override bool isValid{
			get {return !string.IsNullOrEmpty(message);}
		}

		protected override void OnEnter(){
			if (Application.isPlaying){
				Debug.Log(string.Format("<b>({0}) Actor Message Send:</b> '{1}' ({2})", actor.name, message, value));
				actor.SendMessage(message, value);
			}
		}
	}
}