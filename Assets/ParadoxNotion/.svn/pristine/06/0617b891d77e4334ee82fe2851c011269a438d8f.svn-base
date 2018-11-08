using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace Slate{

	[Name("Audio Track")]
	[Description("All audio clips played by this track will be send to the selected AudioMixer if any.")]
	[Icon("AudioClip Icon")]
	///AudioTracks are able to play AudioClips through the PlayAudio ActionClip
	abstract public class AudioTrack : CutsceneTrack {

		[SerializeField]
		private AudioMixerGroup _outputMixer;
		[SerializeField] [Range(0,1)]
		private float _masterVolume = 1f;
		[SerializeField] [Range(-1,1)]
		private float _stereoPan;
		[SerializeField] [Range(0,1)]
		private float _spatialBlend;
		[SerializeField]
		private bool _ignoreTimeScale;

/// @modify slate sequencer
/// @TQ
        [SerializeField][HideInInspector]
        private bool _isAutoRecord = false;

        [SerializeField]
        [HideInInspector]
        private bool _isNeedSound = true;
/// end

        public override string info{
			get {return string.Format("Mixer: {0} ({1})", mixer != null? mixer.name : "NONE", weight.ToString("0.0"));}
		}

		public AudioSource source{ get; private set; }

		public float weight{
			get {return _masterVolume;}
		}
/// @modify slate sequencer
/// @TQ
        public bool IsAutoRecord
        {
            get { return _isAutoRecord; }
            set { _isAutoRecord = value; }

        }

        public bool IsNeedSound
        {
            get { return _isNeedSound; }
            set { _isNeedSound = value; }

        }
        ///end


        public AudioMixerGroup mixer{
			get {return _outputMixer;}
			set	{_outputMixer = value;}
		}

		public float stereoPan{
			get {return _stereoPan;}
			set {_stereoPan = value;}
		}

		public float spatialBlend{
			get {return _spatialBlend;}
			set {_spatialBlend = value;}
		}

		public bool ignoreTimeScale{
			get {return _ignoreTimeScale;}
		}

		virtual public bool useAudioSourceOnActor{
			get {return false;}
		}

		protected override void OnEnter(){ Enable(); }
		protected override void OnReverseEnter(){ Enable(); }

		protected override void OnUpdate(float time, float previousTime){
			if (!useAudioSourceOnActor){
				if (source != null && !(parent is DirectorGroup)){
					source.transform.position = actor.transform.position;
				}
			}
		}

		protected override void OnExit(){ Disable(); }
		protected override void OnReverse(){ Disable(); }

		void Enable(){
			if (useAudioSourceOnActor){
				source = actor.GetComponent<AudioSource>();
			}
			if (source == null){
				source = AudioSampler.GetSourceForID(this);
			}
			ApplySettings();
		}

		void Disable(){
			if (!useAudioSourceOnActor){
				AudioSampler.ReleaseSourceForID(this);
			}
			source = null;
		}

		void ApplySettings(){
			if (source != null){
				source.outputAudioMixerGroup = mixer;
				source.volume                = weight;
				source.spatialBlend          = spatialBlend;
				source.panStereo             = stereoPan;
			}
		}

	}
}