using UnityEngine;
using System;
using System.Collections;

namespace Slate{

	[ExecuteInEditMode]
	///Handles subtitles, fades, crossfades etc.
	public class DirectorGUI : MonoBehaviour {

		//Exposed styling parameters
		public Font subtitlesFont;
		public Font overlayTextFont;

		//Constant styling parameters
		private const float CINEBOX_SIZE = 20f;
		private const float SUBS_SIZE    = 18f;

		//EVENT DELEGATES
		public delegate void SubtitlesGUIDelegate(string text, Color color);
		public delegate void TextOverlayGUIDelegate(string text, Color color, float size, TextAnchor alignment, Vector2 position);
		public delegate void TextureOverlayGUIDelegate(Texture texture, Color color, Vector2 position, Vector2 scale);
		public delegate void ScreenFadeGUIDelegate(Color color);
		public delegate void LetterboxGUIDelegate(float completion);

		///EVENTS
		///Subscribe to any of these events to handle the UI manualy for that element and override the default GUI.
		public static event SubtitlesGUIDelegate OnSubtitlesGUI;
		public static event TextOverlayGUIDelegate OnTextOverlayGUI;
		public static event TextureOverlayGUIDelegate OnTextureOverlayGUI;
		public static event ScreenFadeGUIDelegate OnScreenFadeGUI;
		public static event LetterboxGUIDelegate OnLetterboxGUI;

		public static event Action OnGUIEnable;
		public static event Action OnGUIDisable;
		public static event Action OnGUIUpdate;
		///


		[NonSerialized]
		private static DirectorGUI _current;
		public static DirectorGUI current{
			get
			{
				if (_current == null){
					_current = FindObjectOfType<DirectorGUI>();
					//add component on director camera gameobject purely for organization purposes.
					if (_current == null && DirectorCamera.current != null){
						_current = DirectorCamera.current.gameObject.GetAddComponent<DirectorGUI>();
					}
				}
				return _current;
			}
		}


		private static GUIStyle subsStyle{ get; set; }
		private static GUIStyle overlayTextStyle{ get; set; }

		///...
		void Awake(){
			if (_current != null && _current != this){
				DestroyImmediate(this);
				return;
			}

			_current = this;
		}

		///init styles
		void OnEnable(){

			//subs style
			subsStyle = new GUIStyle();
			subsStyle.normal.textColor = Color.white;
			subsStyle.richText = true;
			subsStyle.padding = new RectOffset(10,10,2,2);
			subsStyle.alignment = TextAnchor.LowerCenter;
			subsStyle.font = subtitlesFont;

			//overlay text style
			overlayTextStyle = new GUIStyle();
			overlayTextStyle.normal.textColor = Color.white;
			overlayTextStyle.richText = true;
			overlayTextStyle.font = overlayTextFont;


			if (OnGUIEnable != null){
				OnGUIEnable();
			}
		}

		///Reset values whenever disabled. Thus for example fading out from a cutscene, the next cutscene does not remain faded.
		void OnDisable(){
			UpdateDissolve(null, 0);
			UpdateLetterbox(0);
			UpdateFade(Color.clear);
			UpdateSubtitles(null, Color.clear);
			UpdateOverlayText(null, Color.clear, 0, default(TextAnchor), Vector2.zero);
			UpdateOverlayTexture(null, Color.clear, Vector2.zero, Vector2.zero);

			if (OnGUIDisable != null){
				OnGUIDisable();
			}
		}


		[NonSerialized] private static Texture dissolver;
		[NonSerialized] private static float dissolveCompletion;
		public static void UpdateDissolve(Texture texture, float completion){
			if (_current != null){
				dissolver = texture;
				dissolveCompletion = completion;
			}
		}


		[NonSerialized] private static float letterboxCompletion;
		public static void UpdateLetterbox(float completion){
			if (OnLetterboxGUI != null){
				OnLetterboxGUI(completion);
				return;
			}
			if (_current != null){
				letterboxCompletion = completion;
			}
		}


		[NonSerialized] public static Color fadeColor;
		public static void UpdateFade(Color color){
			if (OnScreenFadeGUI != null){
				OnScreenFadeGUI(color);
				return;
			}
			if (_current != null){
				fadeColor = color;
			}
		}


		[NonSerialized] private static string subsText;
		[NonSerialized] private static Color subsColor;
		public static void UpdateSubtitles(string text, Color color){
			if (OnSubtitlesGUI != null){
				OnSubtitlesGUI(text, color);
				return;
			}
			if (_current != null){
				subsText = text;
			}
			subsColor = color;
		}


		[NonSerialized] private static string overlayText;
		[NonSerialized] private static Color overlayTextColor;
		[NonSerialized] private static float overlayTextSize;
		[NonSerialized] private static TextAnchor overlayTextAnchor;
		[NonSerialized] private static Vector2 overlayTextPos;
		public static void UpdateOverlayText(string text, Color color, float size, TextAnchor anchor, Vector2 pos){
			if (OnTextOverlayGUI != null){
				OnTextOverlayGUI(text, color, size, anchor, pos);
				return;
			}
			if (_current != null){
				overlayText = text;
				overlayTextColor = color;
				overlayTextSize = size;
				overlayTextAnchor = anchor;
				overlayTextPos = pos;
			}
		}


		[NonSerialized] private static Texture overlayTexture;
		[NonSerialized] private static Color overlayTextureColor;
		[NonSerialized] private static Vector2 overlayTextureScale;
		[NonSerialized] private static Vector2 overlayTexturePosition;
		public static void UpdateOverlayTexture(Texture texture, Color color, Vector2 scale, Vector2 positionOffset){
			if (OnTextureOverlayGUI != null){
				OnTextureOverlayGUI(texture, color, scale, positionOffset);
				return;
			}
			if (_current != null){
				overlayTexture = texture;
				overlayTextureColor = color;
				overlayTextureScale = scale;
				overlayTexturePosition = positionOffset;
			}
		}

		//The order is obviously important
		void OnGUI(){

			if (dissolver != null ){
				DoDissolve();
			}

			if (letterboxCompletion > 0){
				DoLetterbox();
			}

			if (fadeColor.a > 0){
				DoFade();
			}

			if (overlayTextureColor.a > 0 && overlayTexture != null){
				DoOverlayTexture();
			}

			if (subsColor.a > 0 && !string.IsNullOrEmpty(subsText)){
				DoSubs();
			}

			if (overlayTextColor.a > 0 && !string.IsNullOrEmpty(overlayText) ){
				DoOverlayText();
			}

			#if UNITY_EDITOR
			if (!Application.isPlaying && Prefs.showRuleOfThirds){
				DoRuleOfThirds();
			}
			#endif

			if (OnGUIUpdate != null){
				OnGUIUpdate();
			}

		}


		///Dissolving
		void DoDissolve(){
			var rect = new Rect(0, 0, Screen.width, Screen.height);
			GUI.color = new Color(1, 1, 1, 1- dissolveCompletion);
			GUI.DrawTexture(rect, dissolver);
			GUI.color = Color.white;
		}

		
		///Letterbox
		void DoLetterbox(){
			var a = new Rect(0, 0, Screen.width, CINEBOX_SIZE);
			var b = new Rect(0, 0, Screen.width, CINEBOX_SIZE);
			
			var lerp = Easing.Ease(EaseType.QuadraticInOut, 0, 1, letterboxCompletion);
			a.y = Mathf.Lerp(-CINEBOX_SIZE, 0, lerp);
			b.y = Mathf.Lerp(Screen.height, Screen.height - CINEBOX_SIZE, lerp);

			GUI.color = new Color(0.05f, 0.05f, 0.05f, letterboxCompletion);
			GUI.DrawTexture(a, Texture2D.whiteTexture);
			GUI.DrawTexture(b, Texture2D.whiteTexture);
			GUI.color = Color.white;
		}


		///Fading
		void DoFade(){
			var rect = new Rect(0, 0, Screen.width, Screen.height);
			GUI.color = fadeColor;
			GUI.DrawTexture(rect, Texture2D.whiteTexture);
			GUI.color = Color.white;
		}

		void DoSubs(){
			var finalSubs = string.Format("<size={0}><b><i>{1}</i></b></size>", SUBS_SIZE, subsText);
			var size = subsStyle.CalcSize(new GUIContent(finalSubs));
			var rect = new Rect(0, 0, size.x, size.y);
			rect.center = new Vector2(Screen.width/2, Screen.height - (size.y/2) - 12);
			GUI.color = new Color(0,0,0, Mathf.Lerp(0, 0.2f, subsColor.a));
			GUI.DrawTexture(rect, Texture2D.whiteTexture);
			
			rect.center -= new Vector2(2,-2);
			GUI.color = new Color(0,0,0,subsColor.a);
			GUI.Label(rect, finalSubs, subsStyle);
			rect.center += new Vector2(2,-2);

			GUI.color = subsColor;
			GUI.Label(rect, finalSubs, subsStyle);
			GUI.color = Color.white;
		}


		void DoOverlayText(){
			overlayTextStyle.alignment = overlayTextAnchor;
			var rect = Rect.MinMaxRect(20, 10, Screen.width - 20, Screen.height - 10);
			overlayTextPos.y *= -1;
			rect.center += overlayTextPos;
			var finalText = string.Format("<size={0}><b>{1}</b></size>", overlayTextSize, overlayText);
			//shadow
			GUI.color = new Color(0,0,0,overlayTextColor.a);
			GUI.Label(rect, finalText, overlayTextStyle);
			rect.center += new Vector2(2, -2);
			//text
			GUI.color = overlayTextColor;
			GUI.Label(rect, finalText, overlayTextStyle);
			GUI.color = Color.white;
		}


		void DoOverlayTexture(){
			var rect = new Rect(0, 0, overlayTexture.width * overlayTextureScale.x, overlayTexture.height * overlayTextureScale.y);
			rect.center = new Vector2( Screen.width/2, Screen.height/2 ) + overlayTexturePosition;
			GUI.color = overlayTextureColor;
			GUI.DrawTexture(rect, overlayTexture);
			GUI.color = Color.white;
		}

		void DoRuleOfThirds(){
			var lineWidth = 1;
			var y1 = new Rect(Screen.width/3, 0, lineWidth, Screen.height);
			var y2 = new Rect(y1.x * 2, 0, lineWidth, Screen.height);
			var x1 = new Rect(0, Screen.height/3, Screen.width, lineWidth);
			var x2 = new Rect(0, x1.y * 2, Screen.width, lineWidth);
			GUI.color = new Color(1,1,1,0.5f);
			GUI.DrawTexture(x1, Texture2D.whiteTexture);
			GUI.DrawTexture(x2, Texture2D.whiteTexture);
			GUI.DrawTexture(y1, Texture2D.whiteTexture);
			GUI.DrawTexture(y2, Texture2D.whiteTexture);
			GUI.color = Color.white;
		}
	}
}