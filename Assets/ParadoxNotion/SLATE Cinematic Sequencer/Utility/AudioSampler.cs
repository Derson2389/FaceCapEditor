using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Slate{

	///Samples/Plays an AudioClip and manages AudioSource instances
	public static class AudioSampler {

		private const string ROOT_NAME = "_AudioSources";
		private static GameObject root;
		private static Dictionary<object, AudioSource> sources = new Dictionary<object, AudioSource>();

		///Get an AudioSource for the specified key ID object
		public static AudioSource GetSourceForID(object keyID){
			AudioSource source = null;
			if (sources.TryGetValue(keyID, out source)){
				if (source != null){
					return source;
				}
			}

			if (root == null){
				root = GameObject.Find(ROOT_NAME);
				if (root == null){
					root = new GameObject(ROOT_NAME);
				}
			}

			var newSource = new GameObject("_AudioSource").AddComponent<AudioSource>();
			newSource.transform.SetParent(root.transform);
			newSource.playOnAwake = false;
			return sources[keyID] = newSource;
		}

		///Release/Destroy an AudioSource for the specified key ID object
		public static void ReleaseSourceForID(object keyID){
			AudioSource source = null;
			if (sources.TryGetValue(keyID, out source)){
				if (source != null){
					Object.DestroyImmediate(source.gameObject);
				} 
				sources.Remove(keyID);
			}

			if (sources.Count == 0){
				Object.DestroyImmediate(root);
			}
		}


		///Sample an AudioClip on the AudioSource of the specified key ID object
		public static void SampleForID(object keyID, AudioClip clip, float time, float previousTime, float volume, bool ignoreTimescale = false){
			var source = GetSourceForID(keyID);
			Sample(source, clip, time, previousTime, volume, ignoreTimescale);
		}

		///Sample an AudioClip in the specified AudioSource directly
		public static void Sample(AudioSource source, AudioClip clip, float time, float previousTime, float volume, bool ignoreTimescale = false){

			if (source == null){
				return;
			}

			if (previousTime == time){
				source.Stop();
				return;
			}

			source.clip   = clip;
			source.volume = volume;
			source.pitch  = ignoreTimescale? 1 : Time.timeScale;

			time = Mathf.Repeat(time, clip.length - 0.001f);

			if (!source.isPlaying){
				source.Play();
				source.time = time;
			}

			if (!ignoreTimescale){
				if (Mathf.Abs(source.time - time) > 0.1f * Time.timeScale ){
					source.time = time;
				}
			}
		}
	}
}