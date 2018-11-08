using UnityEngine;
using System.Collections;

namespace Slate{

	[System.Serializable]
	///Defines a section...
	public class Section{

		public static readonly Color DEFAULT_COLOR = new Color(0,0,0,0.4f);

		[SerializeField]
		private string _UID;
		[SerializeField]
		private string _name;
		[SerializeField]
		private float _time;
		[SerializeField]
		private Color _color = DEFAULT_COLOR;
		[SerializeField]
		private bool _colorizeBackground = false;

		//Unique ID.
		public string UID{
			get {return _UID;}
			private set {_UID = value;}
		}

		///The name of the section.
		public string name{
			get {return _name;}
			set {_name = value;}
		}

		///It's time.
		public float time{
			get {return _time;}
			set {_time = value;}
		}

		///Preferrence color.
		public Color color{
			get {return _color.a > 0.1f? _color : DEFAULT_COLOR;}
			set {_color = value;}
		}

		///Will the timlines bg be colorized as well?
		public bool colorizeBackground{
			get {return _colorizeBackground;}
			set {_colorizeBackground = value;}
		}

		public Section(string name, float time){
			this.name = name;
			this.time = time;
			UID = System.Guid.NewGuid().ToString();
		}

		public override string ToString(){
			return string.Format("'{0}' Section Time: {1}", name, time);
		}
	}
}