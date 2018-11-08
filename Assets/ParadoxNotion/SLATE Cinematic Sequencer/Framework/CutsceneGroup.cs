using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace Slate{

	///The topmost IDirectable of a Cutscene, containing CutsceneTracks and targeting a specific GameObject Actor
	abstract public class CutsceneGroup : MonoBehaviour, IDirectable {

		public enum ActorReferenceMode{
			UseOriginal,
			UseInstanceHideOriginal
		}

		public enum ActorInitialTransformation{
			UseOriginal,
			UseLocal
		}

		///Raised when a section has been reached
		public event System.Action<Section> OnSectionReached;

		[SerializeField] [HideInInspector]
		private List<CutsceneTrack> _tracks = new List<CutsceneTrack>();
		[SerializeField] [HideInInspector]
		private List<Section> _sections = new List<Section>();
		[SerializeField] [HideInInspector]
		private bool _isCollapsed = false;
		[SerializeField] [HideInInspector]
		private bool _active = true;		
		[SerializeField] [HideInInspector]
		private bool _isLocked = false;	

		private TransformSnapshot transformSnapshot;
		private ObjectSnapshot objectSnapshot;
		private GameObject originalActor;

		new abstract public string name{get;set;}

		///The actor gameobject that is attached to this group
		abstract public GameObject actor{get;set;}
		///The mode of reference for target actor
		abstract public ActorReferenceMode referenceMode{get;set;}
		///The mode of initial transformation for target actor
		abstract public ActorInitialTransformation initialTransformation{get;set;}
		///The local position of the actor in Cutscene Space if set to UseLocal
		abstract public Vector3 initialLocalPosition{get;set;}
		///The local rotation of the actor in Cutscene Space if set to UseLocal
		abstract public Vector3 initialLocalRotation{get;set;}
		///And editor option to display or not the mesh gizmo
		abstract public bool displayVirtualMeshGizmo{get;set;}

		//the child tracks
		public List<CutsceneTrack> tracks{
			get {return _tracks;}
			set {_tracks = value;}
		}

		//the sections defined for this group
		public List<Section> sections{
			get {return _sections;}
			set {_sections = value;}
		}

		IEnumerable<IDirectable> IDirectable.children{ get {return tracks.Cast<IDirectable>();} }
		float IDirectable.startTime{ get {return 0;} }
		float IDirectable.endTime{ get {return root.length;} }
		float IDirectable.blendIn{	get {return 0f;} }
		float IDirectable.blendOut{ get {return 0f;} }
		IDirectable IDirectable.parent{ get {return null;} }
		public IDirector root{get; private set;}

		public bool isActive{
			get	{return _active;}
			set {_active = value;}
		}
		
		public bool isCollapsed{
			get {return _isCollapsed;}
			set {_isCollapsed = value;}
		}

		public bool isLocked{
			get {return _isLocked;}
			set {_isLocked = value;}
		}
		/// @modify slate sequencer
        /// @TQ
        [SerializeField]
        private bool _hideCheceked = false;
        public bool HideCheceked
        {
            get { return _hideCheceked;  }
            set
            {
                _hideCheceked = value;
            }
        }
        [SerializeField]
        [HideInInspector]
        private bool _showInTimeLine= true;
        public bool ShowInTimeLine
        {
            get {
                return _showInTimeLine;
            }
            set { _showInTimeLine = value; }
        }
		///end

        //Validate the group and it's tracks
        public void Validate(IDirector root, IDirectable parent){
			this.root = root;
			var foundTracks = GetComponentsInChildren<CutsceneTrack>(true);
			for (var i = 0; i < foundTracks.Length; i++){
				if (!tracks.Contains(foundTracks[i])){
					tracks.Add(foundTracks[i]);
				}
			}
			if (tracks.Any(t => t == null)){ tracks = foundTracks.ToList(); }
			sections = sections.OrderBy(s => s.time).ToList();
		}

		//Get a Section it's name
		public Section GetSectionByName(string name){
			if (name.ToUpper() == "INTRO") return new Section("Intro", 0);
			return sections.Find(s => s.name.ToUpper() == name.ToUpper());
		}

		//Get a Section it's UID
		public Section GetSectionByUID(string UID){
			return sections.Find(s => s.UID == UID);
		}

		///Get a Section whos time is great specified time
		public Section GetSectionAfter(float time){
			return sections.FirstOrDefault(s => s.time > time);
		}

		///Get a Section whos time is less specified time
		public Section GetSectionBefore(float time){
			return sections.LastOrDefault(s => s.time < time);
		}

		///Transforms a point in specified space
		public Vector3 TransformPoint(Vector3 point, TransformSpace space){
			var t = GetSpaceTransform(space);
			return t != null? t.TransformPoint(point) : point;
		}

		///Inverse Transforms a point in specified space
		public Vector3 InverseTransformPoint(Vector3 point, TransformSpace space){
			var t = GetSpaceTransform(space);
			return t != null? t.InverseTransformPoint(point) : point;
		}

		///Returns the transform object used for specified Space transformations. Null if World Space.
		public Transform GetSpaceTransform(TransformSpace space){
			if (space == TransformSpace.CutsceneSpace){
				return root != null? root.context.transform : null;
			}
			if (space == TransformSpace.ActorSpace){
				return actor != null? actor.transform : null;
			}
			return null; //world space
		}

		///Returns the final actor position in specified Space (InverseTransform Space)
		public Vector3 ActorPositionInSpace(TransformSpace space){
			return actor != null? InverseTransformPoint(actor.transform.position, space) : root.context.transform.position;
		}

#if SLATE_USE_EXPRESSIONS
		///The ExpressionEnvironment used
		public StagPoint.Eval.Environment GetExpressionEnvironment(){
			return root.GetExpressionEnvironment();
		}
#endif

		bool IDirectable.Initialize(){

			if (actor == null){
				return false;
			}

			#if UNITY_EDITOR //do a fail safe checkup at least in editor
			var prefabType = UnityEditor.PrefabUtility.GetPrefabType(actor);
			if (prefabType == UnityEditor.PrefabType.Prefab || prefabType == UnityEditor.PrefabType.ModelPrefab){
				if (referenceMode == ActorReferenceMode.UseOriginal){
					Debug.LogWarning("A prefab is referenced in an Actor Group, but the Reference Mode is set to Use Original. This is not allowed to avoid prefab corruption. Please select the Actor Group and set Reference Mode to 'Use Instance'");
					return false;
				}
			}
#endif

			// @modify slate sequencer
			// @hushuang
			// support override OnInitialize for CutsceneGroup
            OnInitialize();

            return true;
		}

		///Store undo snapshot
		void IDirectable.Enter(){

			if (root.isReSampleFrame){
				return;
			}

			if (referenceMode == ActorReferenceMode.UseInstanceHideOriginal){
				InstantiateLocalActor();
				return; //if we get an instance, there is no need to store anything for undo.
			}

			Store();

			if (initialTransformation == ActorInitialTransformation.UseLocal){
				InitLocalCoords(actor);
			}
		}

		///Restore undo snapshot
		void IDirectable.Reverse(){

			if (root.isReSampleFrame){
				return;
			}

			if (referenceMode == ActorReferenceMode.UseInstanceHideOriginal){
				ReleaseLocalActorInstance();
				return; //if we had a now destroyed instance, no need to restore anything
			}

			Restore();
		}

		///...
		void IDirectable.Update(float time, float previousTime){

			if (root.isReSampleFrame){
				return;
			}

			if (OnSectionReached != null){
				for (var i = 0; i < sections.Count; i++){
					if (time >= sections[i].time && previousTime < sections[i].time){
						OnSectionReached(sections[i]);
					}
				}
			}

			// @modify slate sequencer
			// @hushuang
			// support override OnUpdate for CutsceneGroup
            OnUpdate(time, previousTime);
        }

		///...
		void IDirectable.Exit(){

			if (root.isReSampleFrame){
				return;
			}

			if (Application.isPlaying){
				if (referenceMode == ActorReferenceMode.UseInstanceHideOriginal){
					ReleaseLocalActorInstance();
				}
			}
		}

		///...
		void IDirectable.ReverseEnter(){

			if (root.isReSampleFrame){
				return;
			}

			if (Application.isPlaying){
				if (referenceMode == ActorReferenceMode.UseInstanceHideOriginal){
					InstantiateLocalActor();
				}
			}
		}

		void IDirectable.RootEnabled(){}
		void IDirectable.RootDisabled(){}
		void IDirectable.RootUpdated(float time, float previousTime){}

        // @modify slate sequencer
        // @hushuang
        // support override OnInitialize for CutsceneGroup
        virtual protected bool OnInitialize() { return true; }

        // @modify slate sequencer
        // @hushuang
        // support override OnUpdate for CutsceneGroup
        virtual protected void OnUpdate(float time, float previousTime) { }


#if UNITY_EDITOR
        ///Draw the gizmos of virtual actor references
        void IDirectable.DrawGizmos(bool selected){

			if (initialTransformation == ActorInitialTransformation.UseOriginal){
				return;
			}

			if ( !selected && !displayVirtualMeshGizmo ){
				return;
			}

			if (actor != null && isActive && root.currentTime == 0){
				var t = root.context.transform;
				foreach(var renderer in actor.GetComponentsInChildren<Renderer>()){
					Mesh mesh = null;
					var pos = t.TransformPoint(initialLocalPosition);
                    // @modify slate sequencer
                    // @hushuang
                    // fix mesh rotation degreen error when Renderer component has its own rotation.
                    var rot = Quaternion.Euler((t.eulerAngles + renderer.transform.eulerAngles + initialLocalRotation));
                    if (renderer is SkinnedMeshRenderer){ mesh = ((SkinnedMeshRenderer)renderer).sharedMesh; }
					else
					{
						var filter = renderer.GetComponent<MeshFilter>();
						if (filter != null){ mesh = filter.sharedMesh; }
					}
					Gizmos.DrawMesh(mesh, pos, rot, renderer.transform.localScale);
				}
				Gizmos.DrawLine(t.position, t.TransformPoint(initialLocalPosition));
			}
		}

		///Just the tools to handle the initial virtual actor reference pos and rot
		void IDirectable.SceneGUI(bool selected){
			
			if (!selected || !isActive){
				return;
			}

			if (initialTransformation == ActorInitialTransformation.UseOriginal){
				return;
			}

			if (actor != null && root.currentTime == 0){
				UnityEditor.EditorGUI.BeginChangeCheck();
				var pos = root.context.transform.TransformPoint(initialLocalPosition);
				pos = UnityEditor.Handles.PositionHandle(pos, Quaternion.identity);
				var rot = UnityEditor.Handles.RotationHandle( Quaternion.Euler(initialLocalRotation), pos ).eulerAngles;
				if (UnityEditor.EditorGUI.EndChangeCheck()){
					UnityEditor.Undo.RecordObject(this, "Local Actor Coordinates");
					initialLocalPosition = root.context.transform.InverseTransformPoint(pos);
					initialLocalRotation = rot;
					UnityEditor.EditorUtility.SetDirty(this);
				}
			}
		}
#endif


		//Store snapshots
		void Store(){
			objectSnapshot = new ObjectSnapshot(actor);
			transformSnapshot = new TransformSnapshot(actor, TransformSnapshot.StoreMode.All);
		}

		//Restore snapshots
		void Restore(){
			if (objectSnapshot != null){
				objectSnapshot.Restore();
			}
			if (transformSnapshot != null){
				transformSnapshot.Restore();
			}			
		}

		//Initialize actor reference mode
		void InstantiateLocalActor(){
			originalActor = actor;
			actor = (GameObject)Instantiate(actor);
			SceneManager.MoveGameObjectToScene(actor, root.context.scene);
			actor.transform.SetParent(originalActor.transform.parent, false);
			
			#if UNITY_EDITOR //not really needed, but avoids duplicate instaces when user undo immediately after an initialize.
			//UnityEditor.Undo.RegisterCreatedObjectUndo(actor, "Reference Instance");//this will cause undo bug
			#endif

			if (initialTransformation == ActorInitialTransformation.UseLocal){
				InitLocalCoords(actor);
			}

			actor.SetActive(true);
			originalActor.SetActive(false);
		}

		//Release actor reference mode
		void ReleaseLocalActorInstance(){
			if (actor != originalActor){ //just a failsafe
				DestroyImmediate(actor);
				actor = originalActor;
				actor.SetActive(true);
				originalActor.SetActive(true);
				originalActor = null;
			}
		}

		void InitLocalCoords(GameObject target){
			target.transform.position = root.context.transform.TransformPoint(initialLocalPosition);
			target.transform.eulerAngles = root.context.transform.eulerAngles + initialLocalRotation;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		///Add a new track to this group
		public T AddTrack<T>(string name = null) where T : CutsceneTrack { return (T)AddTrack(typeof(T), name); }
		public CutsceneTrack AddTrack(System.Type type, string name = null){

			if ( !type.IsSubclassOf(typeof(CutsceneTrack)) || type.IsAbstract ){
				return null;
			}

			var go = new GameObject(type.Name.SplitCamelCase());
			UnityEditor.Undo.RegisterCreatedObjectUndo(go, "New Track");
			var newTrack = UnityEditor.Undo.AddComponent(go, type) as CutsceneTrack;
			UnityEditor.Undo.SetTransformParent(newTrack.transform, this.transform, "New Track");
			UnityEditor.Undo.RegisterCompleteObjectUndo(this, "New Track");
			newTrack.transform.localPosition = Vector3.zero;
			if (name != null){ newTrack.name = name; }
			tracks.Add(newTrack);
			newTrack.PostCreate(this);
			root.Validate();
			CutsceneUtility.selectedObject = newTrack;
			return newTrack;
		}

		///Duplicate the track in this group
		public CutsceneTrack DuplicateTrack(CutsceneTrack track){
			
			if (track == null || track.GetType().RTGetAttribute<UniqueElementAttribute>(true) != null){
				return null;
			}

			var newTrack = (CutsceneTrack)Instantiate(track);
			UnityEditor.Undo.RegisterCreatedObjectUndo(newTrack.gameObject, "Duplicate Track");
			UnityEditor.Undo.SetTransformParent(newTrack.transform, this.transform, "Duplicate Track");
			UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Duplicate Track");
			newTrack.transform.localPosition = Vector3.zero;
			tracks.Add(newTrack);
			root.Validate();
			CutsceneUtility.selectedObject = newTrack;
			return newTrack;
		}

        ///Clone the track in this group
		public CutsceneTrack CloneTrack(CutsceneTrack track)
        {

            //if (track == null || track.GetType().RTGetAttribute<UniqueElementAttribute>(true) != null)
            //{
            //    return null;
            //}

            var newTrack = (CutsceneTrack)Instantiate(track);
            UnityEditor.Undo.RegisterCreatedObjectUndo(newTrack.gameObject, "Duplicate Track");
            UnityEditor.Undo.SetTransformParent(newTrack.transform, this.transform, "Duplicate Track");
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Duplicate Track");
            newTrack.transform.localPosition = Vector3.zero;
            tracks.Add(newTrack);
            root.Validate();
            CutsceneUtility.selectedObject = newTrack;
            return newTrack;
        }

        ///Delete a track of this group
        public void DeleteTrack(CutsceneTrack track){
			UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Delete Track");
			tracks.Remove(track);
			if ( ReferenceEquals(CutsceneUtility.selectedObject, track) ){
				CutsceneUtility.selectedObject = null;
			}
			UnityEditor.Undo.DestroyObjectImmediate(track.gameObject);
			root.Validate();
		}

		/// @modify slate sequencer
        /// @TQ
        ///Check a track has property track 
        public bool HasPropertyTrack()
        {
            for (int i = 0; i<_tracks.Count; i++)
            {
                if (_tracks[i] is PropertiesTrack)
                {
                    return true;
                }
            }
            return false;
        }

        public PropertiesTrack GetPropertyTrack()
        {
            for (int i = 0; i < _tracks.Count; i++)
            {
                if (_tracks[i] is PropertiesTrack)
                {
                    return _tracks[i] as PropertiesTrack;
                }
            }
            return null;
        }
        /// end
		#endif
	}
}