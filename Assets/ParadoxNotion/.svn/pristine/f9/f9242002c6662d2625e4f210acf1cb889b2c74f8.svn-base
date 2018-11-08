using UnityEngine;
using System.Collections;

#if SLATE_USE_POSTSTACK
using UnityEngine.PostProcessing;
#endif

namespace Slate{

	///The master director render camera for all cutscenes.
	public class DirectorCamera : MonoBehaviour, IDirectableCamera {

		[SerializeField] [HideInInspector]
		private bool _matchMainWhenActive   = true;
		[SerializeField] [HideInInspector]
		private bool _setMainWhenActive     = true;
		[SerializeField] [HideInInspector]
		private bool _autoHandleActiveState = true;
		[SerializeField] [HideInInspector]
		private bool _ignoreFOVChanges      = false;
		[SerializeField] [HideInInspector]
		private bool _dontDestroyOnLoad     = false;

		//max possible damp able to be used for post-smoothing
		public const float MAX_DAMP = 3f;

		///Raised when a camera cut takes place from one shot to another.
		public static event System.Action<IDirectableCamera> OnCut;
		///Raised when the Director Camera is activated/enabled.
		public static event System.Action OnActivate;
		///Raised when the Director Camera is deactivated/disabled.
		public static event System.Action OnDeactivate;

		private static DirectorCamera _current;
		private static Camera _cam;
		private static IDirectableCamera lastTargetShot;
#if SLATE_USE_POSTSTACK
		private static PostProcessingBehaviour postStack;
#endif

		public static DirectorCamera current{
			get
			{
				if (_current == null){
						_current = FindObjectOfType<DirectorCamera>();
						if (_current == null){
							_current = new GameObject("Director Camera Root").AddComponent<DirectorCamera>();
							_current.cam.nearClipPlane = 0.01f;
							_current.cam.farClipPlane = 1000;
						}
					}
				return _current;
			}
		}

		/////////

		public Camera cam{
			get
			{
				if (_cam == null){
					_cam = GetComponentInChildren<Camera>(true);
					if (_cam == null){
						_cam = CreateRenderCamera();
					}
				}
				return _cam;
			}
		}

		//////////
		//These properties are instance properties so that they can potentially be animated.
		public Vector3 position{
			get{return current.transform.position;}
			set {current.transform.position = value;}
		}

		public Quaternion rotation{
			get {return current.transform.rotation;}
			set {current.transform.rotation = value;}
		}

		public float fieldOfView{
			get {return cam.orthographic? cam.orthographicSize : cam.fieldOfView;}
			set { if (!ignoreFOVChanges) {cam.fieldOfView = value; cam.orthographicSize = value;} }
		}

        public float nearClipping
        {
            get { return cam.nearClipPlane; }
            set {
                cam.nearClipPlane = value;
            }
        }

        public float farClipping
        {
            get { return cam.farClipPlane; }
            set { cam.farClipPlane = value; }
        }

#if SLATE_USE_POSTSTACK
		public float focalPoint{
			get { return postStack != null && postStack.profile != null? postStack.profile.depthOfField.settings.focusDistance : 10f; }
			set
			{
				if (postStack != null && postStack.profile != null){
					var settings = postStack.profile.depthOfField.settings;
					settings.focusDistance = Mathf.Max(value, 0.1f);
					postStack.profile.depthOfField.settings = settings;
				}
			}
		}

		public float focalRange{
			get { return postStack != null && postStack.profile != null? postStack.profile.depthOfField.settings.focalLength : 50f; }
			set
			{
				if (postStack != null && postStack.profile != null){
					var settings = postStack.profile.depthOfField.settings;
					settings.focalLength = Mathf.Clamp(value, 1f, 300f);
					postStack.profile.depthOfField.settings = settings;
				}
			}
		}
#else
        public float focalPoint{get;set;}
		public float focalRange{get;set;}
#endif
		/////////

		///Should DirectorCamera be matched to Camera.main when active?
		public static bool matchMainWhenActive{
			get {return current._matchMainWhenActive;}
			set {current._matchMainWhenActive = value;}
		}

		///Should DirectorCamera be set as Camera.main when active?
		public static bool setMainWhenActive{
			get {return current._setMainWhenActive;}
			set {current._setMainWhenActive = value;}
		}

		///If true, the RenderCamera active state is automatically handled. This is highly recommended.
		public static bool autoHandleActiveState{
			get {return current._autoHandleActiveState;}
			set {current._autoHandleActiveState = value;}
		}

		///If true, any changes made by shots will be bypassed/ignored.
		public static bool ignoreFOVChanges{
			get {return current._ignoreFOVChanges;}
			set {current._ignoreFOVChanges = value;}
		}

		///Should DirectorCamera be persistant between level changes?
		public static bool dontDestroyOnLoad{
			get {return current._dontDestroyOnLoad;}
			set {current._dontDestroyOnLoad = value;}
		}

		///The actual camera from within cutscenes are rendered
		public static Camera renderCamera{get { return current.cam; }}
		///The gameplay camera
		public static GameCamera gameCamera{get; set;}
		///Is director enabled?
		public static bool isEnabled{get; private set;}

		void Awake(){

			if (_current != null && _current != this){
                // @modify slate sequencer
                // @hushuang
                // fix bug that the later instance should override the previous instance, caused in StoryAssets init.  
                //DestroyImmediate(this.gameObject);
                //return;
                DestroyImmediate(_current.gameObject);
            }

			_current = this;
			if (dontDestroyOnLoad){
				DontDestroyOnLoad(this.gameObject);
			}
			Disable();
		}


		Camera CreateRenderCamera(){
			_cam = new GameObject("Render Camera").AddComponent<Camera>();
			_cam.gameObject.AddComponent<AudioListener>();
			_cam.gameObject.AddComponent<FlareLayer>();
			_cam.transform.SetParent(this.transform);
			return _cam;
		}

		///Enable the Director Camera, while disabling the main camera if any
		public static void Enable(){

			//init gamecamera if any
			if (gameCamera == null){
				var main = Camera.main;
				if (main != null && main != renderCamera){
					gameCamera = main.GetAddComponent<GameCamera>();
				}
			}

			//use gamecamera and disable it
			if (gameCamera != null){
				gameCamera.gameObject.SetActive(false);
				if (matchMainWhenActive){
					var tempFOV = current.fieldOfView;
					renderCamera.CopyFrom(gameCamera.cam);

                    // @modify slate sequencer
                    // @rongxia
                    renderCamera.depth = Mathf.Max(0, gameCamera.cam.depth);
                    // @end

                    if (ignoreFOVChanges){
						renderCamera.fieldOfView = tempFOV;
					}
				}

				//set the root pos/rot
				current.transform.position = gameCamera.position;
				current.transform.rotation = gameCamera.rotation;
			}

			//reset render camera local pos/rot
			renderCamera.transform.localPosition = Vector3.zero;
			renderCamera.transform.localRotation = Quaternion.identity;

			//set render camera to MainCamera if option enabled
			if (setMainWhenActive){
				renderCamera.gameObject.tag = "MainCamera";
			}

			///enable
			if (autoHandleActiveState){
				renderCamera.gameObject.SetActive(true);
			}

#if SLATE_USE_POSTSTACK		
			postStack = renderCamera.GetComponent<PostProcessingBehaviour>();
#endif

			isEnabled = true;
			lastTargetShot = null;

			if (OnActivate != null){
				OnActivate();
			}
		}

		///Disable the Director Camera, while enabling back the main camera if any
		public static void Disable(){

			if (OnDeactivate != null){
				OnDeactivate();
			}

			//disable render camera
			if (autoHandleActiveState){
				renderCamera.gameObject.SetActive(false);
			}

			//reset tag
			if (setMainWhenActive){
				renderCamera.gameObject.tag = "Untagged";
			}

			//enable gamecamera
			if (gameCamera != null){
				gameCamera.gameObject.SetActive(true);
			}

			isEnabled = false;
		}


		///Ease from game camera to target. If target is null, eases to DirectorCamera current.
		public static void Update(IDirectableCamera source, IDirectableCamera target, EaseType interpolation, float weight, float damping = 0f){

            if (source == null){ source = gameCamera != null? (IDirectableCamera)gameCamera : (IDirectableCamera)current; }
			if (target == null){ target = current; }

			var targetPosition   = weight < 1? Easing.Ease(interpolation, source.position, target.position, weight)		 	: target.position;
			var targetRotation   = weight < 1? Easing.Ease(interpolation, source.rotation, target.rotation, weight)		 	: target.rotation;
			var targetFOV        = weight < 1? Easing.Ease(interpolation, source.fieldOfView, target.fieldOfView, weight)	: target.fieldOfView;
			var targetFocalPoint = weight < 1? Easing.Ease(interpolation, source.focalPoint, target.focalPoint, weight)	 	: target.focalPoint;
			var targetFocalRange = weight < 1? Easing.Ease(interpolation, source.focalRange, target.focalRange, weight)	 	: target.focalRange;

			var isCut = target != lastTargetShot;
			if (!isCut && damping > 0){
				current.position    = Vector3.Lerp(current.position, targetPosition, Time.deltaTime * (MAX_DAMP/damping));
				current.rotation    = Quaternion.Lerp(current.rotation, targetRotation, Time.deltaTime * (MAX_DAMP/damping));
				current.fieldOfView = Mathf.Lerp(current.fieldOfView, targetFOV, Time.deltaTime * (MAX_DAMP/damping));
				current.focalPoint  = Mathf.Lerp(current.focalPoint, targetFocalPoint, Time.deltaTime * (MAX_DAMP/damping));
				current.focalRange  = Mathf.Lerp(current.focalRange, targetFocalRange, Time.deltaTime * (MAX_DAMP/damping));
			
			} else {
				current.position    = targetPosition;
				current.rotation    = targetRotation;
				current.fieldOfView = targetFOV;
				current.focalPoint  = targetFocalPoint;
				current.focalRange  = targetFocalRange;
                // @modify slate sequencer
                // @TQ
                // sync the nearClipPlane and farClipPlane to rendercamer
                current.cam.nearClipPlane = target.cam.nearClipPlane;
                current.cam.farClipPlane = target.cam.farClipPlane;
                //end
			}

			if (isCut){
#if SLATE_USE_POSTSTACK
				if (postStack != null){
					postStack.ResetTemporalEffects();
				}
#endif
				if (OnCut != null){
					OnCut(target);
				}
			}
				
			lastTargetShot = target;
		}



		private static float noiseTimer;
		private static Vector3 noisePosOffset;
		private static Vector3 noiseRotOffset;
		private static Vector3 noiseTargetPosOffset;
		private static Vector3 noiseTargetRotOffset;
		private static Vector3 noiseCamPosVel;
		private static Vector3 noiseCamRotVel;
		//Apply noise effect (steadycam). This is better looking than using a multi Perlin noise.
		public static void ApplyNoise(float magnitude, float weight){
			var posMlt = Mathf.Lerp(0.2f, 0.4f, magnitude);
			var rotMlt = Mathf.Lerp(5, 10f, magnitude);
			var damp = Mathf.Lerp(3, 1, magnitude);
			if (noiseTimer <= 0){
				noiseTimer = Random.Range(0.2f, 0.3f);
				noiseTargetPosOffset = Random.insideUnitSphere * posMlt;
				noiseTargetRotOffset = Random.insideUnitSphere * rotMlt;
			}
			noiseTimer -= Time.deltaTime;

			noisePosOffset = Vector3.SmoothDamp(noisePosOffset, noiseTargetPosOffset, ref noiseCamPosVel, damp);
			noiseRotOffset = Vector3.SmoothDamp(noiseRotOffset, noiseTargetRotOffset, ref noiseCamRotVel, damp);

			//Noise is applied as a local offset to the RenderCamera directly
			renderCamera.transform.localPosition = Vector3.Lerp(Vector3.zero, noisePosOffset, weight);
			renderCamera.transform.SetLocalEulerAngles( Vector3.Lerp(Vector3.zero, noiseRotOffset, weight) );
		}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		[SerializeField] [HideInInspector]
		private bool hasUpdate_152;
		
		void Reset(){
			hasUpdate_152 = true;
			CreateRenderCamera();
			Disable();
		}

		void OnValidate(){
			if (!hasUpdate_152){
				hasUpdate_152 = true;
				foreach(var behaviour in renderCamera.gameObject.GetComponents<Behaviour>()){
					behaviour.enabled = true;
				}
			}
			if (this == _current){
				Disable();
			}
		}

		void OnDrawGizmos(){

			var color = Prefs.gizmosColor;
			if (!isEnabled){ color.a = 0.2f;}
			Gizmos.color = color;

			var hit = new RaycastHit();
			if (Physics.Linecast(cam.transform.position, cam.transform.position - new Vector3(0, 100, 0), out hit)){
				var d = Vector3.Distance(hit.point, cam.transform.position);
				Gizmos.DrawLine(cam.transform.position, hit.point);
				Gizmos.DrawCube(hit.point, new Vector3(0.2f, 0.05f, 0.2f));
				Gizmos.DrawCube(hit.point + new Vector3(0, d/2, 0), new Vector3(0.02f, d, 0.02f));
			}

			Gizmos.DrawLine(transform.position, cam.transform.position);

			if (isEnabled){color = Color.green;}
			Gizmos.color = color;
			Gizmos.matrix = Matrix4x4.TRS(cam.transform.position, cam.transform.rotation, Vector3.one);
			var dist = isEnabled? 0.8f : 0.5f;
			Gizmos.DrawFrustum(new Vector3(0,0,dist), fieldOfView, 0, dist, 1);

			color.a = 0.2f;
			Gizmos.color = color;
			Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
			Gizmos.DrawFrustum( new Vector3(0,0,0.5f), fieldOfView, 0f, 0.5f, 1);
			Gizmos.color = Color.white;
		}			

		#endif
	}
}