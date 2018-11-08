using UnityEngine;
using System.Collections;

namespace Slate{
	
	///An easy way to allow the user in the inspector to choose a Transform or a Vector3
	[System.Serializable]
	public struct PositionParameter : ITransformableHelperParameter {

		[SerializeField] [UnityEngine.Serialization.FormerlySerializedAs("transform")]
		private Transform _transform;
		[SerializeField] [UnityEngine.Serialization.FormerlySerializedAs("vector")]
		private Vector3 _vector;
		[SerializeField]
		private TransformSpace _space;

		public bool useAnimation{
			get {return _transform == null;}
		}

		public TransformSpace space{
			get {return transform != null? TransformSpace.WorldSpace : _space;}
			private set {_space = value;}
		}

		public Transform transform{
			get {return _transform;}
			private set {_transform = value;}
		}

		public Vector3 value{
			get {return transform != null? transform.position : _vector;}
			set {_vector = value;}
		}

		public override string ToString(){
			return transform != null? transform.name : _vector.ToString();
		}
	}
}