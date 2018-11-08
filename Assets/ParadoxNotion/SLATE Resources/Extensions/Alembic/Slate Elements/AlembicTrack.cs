#if !NO_UTJ

using UnityEngine;
using System.Linq;
using UTJ.Alembic;

namespace Slate{

	[Attachable(typeof(DirectorGroup))]
	[Description("The Alembic Track can sample imported Alembic (.abc) files. This track does not accept any clips. Instead a virtual clip will represent the active exported frame range of the alembic file, plus any extra offset set bellow.\n\n*Alembic files should be placed under 'Assets/StreamingAssets' folder*")]
	[Icon("AlembicIcon")]
	public class AlembicTrack : CutsceneTrack {

		[Tooltip("Assign the imported Alembic gameobject root here")]
		public AlembicStreamPlayer alembicPlayer;

		private AlembicPlaybackSettings playSettings{
			get {return alembicPlayer != null? alembicPlayer.m_PlaybackSettings : null;}
		}

		public float abcTimeOffset{
			get {return playSettings != null? playSettings.m_timeOffset : 0f;}
			set {if (playSettings != null) playSettings.m_timeOffset = value;}
		}

		public override string info{
			get {return alembicPlayer != null? alembicPlayer.name : "NONE";}
		}

		protected override void OnAfterValidate(){
			if (alembicPlayer != null){
				foreach(var cam in alembicPlayer.GetComponentsInChildren<Camera>()){
					cam.GetAddComponent<ShotCamera>();
				}
			}
		}

		protected override bool OnInitialize(){
			if ( alembicPlayer == null || playSettings == null ){
				return false;
			}

			if (!Application.isPlaying){ //AlembicPlayer OnEnable is not called right after it's imported if CreatePrefab is false.
				alembicPlayer.enabled = false;
				alembicPlayer.enabled = true;
			}

			return true;
		}

        float SnapTime(float time)
        {
            var cut = root as Cutscene;
            if (cut != null)
            {
                return (Mathf.Round(time / cut.SnapInterval) * cut.SnapInterval);
            }
            else {
                return time;
            }   
        }

        protected override void OnUpdate(float time, float previousTime){
			if (alembicPlayer == null || playSettings == null){
				return;
			}

			playSettings.m_OverrideTime = true;
			playSettings.m_preserveStartTime = true;
			playSettings.m_Time = SnapTime(time);
		}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		private float abcStartTime{
			get {return playSettings != null? playSettings.m_startTime + playSettings.m_timeOffset : float.NegativeInfinity;}
		}

		private float abcEndTime{
			get {return playSettings != null? playSettings.m_endTime + playSettings.m_timeOffset : float.NegativeInfinity;}
		}

		public override void OnTrackTimelineGUI(Rect posRect, Rect timeRect, float cursorTime, System.Func<float, float> TimeToPos, Rect? selectRect = null)
        {

			var clipRect = posRect;
			clipRect.xMin = TimeToPos(abcStartTime);
			clipRect.xMax = TimeToPos(abcEndTime);

			GUI.color = new Color(1,1,1, 0.3f);
			GUI.Box(clipRect, "", Slate.Styles.clipBoxStyle);

			GUI.color = Color.black;
			var inLabel = (abcStartTime * Prefs.frameRate).ToString("0");
			var outLabel = (abcEndTime * Prefs.frameRate).ToString("0");
			var inSize = new GUIStyle("label").CalcSize(new GUIContent(inLabel));
			var outSize = new GUIStyle("label").CalcSize(new GUIContent(outLabel));
			inSize.x = Mathf.Min(inSize.x, clipRect.width/2);
			outSize.x = Mathf.Min(outSize.x, clipRect.width/2);
			var inRect = new Rect(clipRect.x, clipRect.y, inSize.x, clipRect.height);
			GUI.Label(inRect, inLabel);
			var outRect = new Rect(clipRect.xMax - outSize.x, clipRect.y, outSize.x, clipRect.height);
			GUI.Label(outRect, outLabel);
			GUI.color = Color.white;

			if (clipRect.Contains(Event.current.mousePosition)){
				UnityEditor.EditorGUIUtility.AddCursorRect(clipRect, UnityEditor.MouseCursor.Link);
				if (Event.current.type == EventType.MouseDown){
					CutsceneUtility.selectedObject = this;
					Event.current.Use();
				}
			}
		}

		#endif
	}
}

#endif