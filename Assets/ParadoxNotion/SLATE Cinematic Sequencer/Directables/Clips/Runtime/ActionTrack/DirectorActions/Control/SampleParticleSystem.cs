using UnityEngine;
using System.Collections;
using System;

namespace Slate.ActionClips{

	[Category("Control")]
	public class SampleParticleSystem : DirectorActionClip {

		[SerializeField] [HideInInspector]
		private float _length = 1f;
        [HideInInspector]
        private float _preTime;
        [SerializeField]
        public string _particleGoName = "";


        private const float kUnsetTime = -1f;

        private float m_LastTime = -1f;

        private float m_LastPsTime = -1f;

        private uint m_RandomSeed = 1u;


        [Required]
		public ParticleSystem particles;

		private ParticleSystem.EmissionModule em;

        public ParticleSystem particleSystem 
        {
            get
            {
                return this.particles;
            }
        }

        public override string info{
			get {return string.Format("Particles ({0})\n{1}", particles && loop? "Looping" : "OneShot", particles? particles.gameObject.name : "NONE"); }
		}

		public override bool isValid{
			get
            {
#if UNITY_EDITOR
                if (particles != null)
                {
                    _particleGoName = particles.gameObject.name;
                    this.m_RandomSeed = Math.Max(1u, particles.randomSeed);
                }
                else
                {                    
                    if (!string.IsNullOrEmpty(_particleGoName))
                    {
                        GameObject go = GameObject.Find(_particleGoName);
                        if (go != null)
                        {
                            var comp = go.GetComponent<ParticleSystem>();
                            if (comp != null)
                            {
                                particles = comp;
                                this.m_RandomSeed = Math.Max(1u, particles.randomSeed);
                            }
                        }
                    }                   
                }
#endif
                return particles != null;
            }
		}

		public override float length{
			get	{return particles == null || loop? _length : duration + startLifetime; }
			set {_length = value;}
		}
		
		public override float blendOut{
			get {return isValid && !loop? startLifetime : 0.1f;}
		}

		private bool loop{
			#if UNITY_5_5_OR_NEWER
			get {return particles != null && particles.main.loop;}
			#else
			get {return particles != null && particles.loop;}
			#endif
		}

		private float duration{
			#if UNITY_5_5_OR_NEWER
			get {return particles != null? particles.main.duration : 0f;}
			#else
			get {return particles != null? particles.duration : 0f;}
			#endif
		}

		private float startLifetime{
			#if UNITY_5_5_OR_NEWER
			get
            {
                ///particles.main.startLifetime.constantMax
                return particles != null? Mathf.Max(particles.main.startLifetime.constantMax, particles.main.startLifetime.constantMin) : 0f;
            }
			#else
			get {return particles != null? particles.startLifetime : 0f;}
			#endif
		}

		protected override void OnEnter(){ Play(); }
		protected override void OnReverseEnter(){ Play(); }
		protected override void OnExit(){ Stop(); }
		protected override void OnReverse(){ Stop(); }

		void Play(){
			if (!particles.isPlaying){
				particles.useAutoRandomSeed = false;
			}
            this.m_LastTime = -1f;
            em = particles.emission;
			em.enabled = true;
		}


		protected override void OnUpdate(float time)
        {
          
            if (!(this.particleSystem == null) && this.particleSystem.gameObject.activeInHierarchy)
            {
                if (this.particleSystem.randomSeed != this.m_RandomSeed)
                {
                    this.particleSystem.Stop();
                    this.particleSystem.useAutoRandomSeed = false;
                    this.particleSystem.randomSeed = this.m_RandomSeed;
                }
               // float num = (float)PlayableExtensions.GetTime<Playable>(playable);
                bool flag = Mathf.Approximately(this.m_LastTime, -1f) || !Mathf.Approximately(this.m_LastTime, time);
                if (flag)
                {
                    float num2 = Time.fixedDeltaTime * 0.5f;
                    float num3 = time;
                    float num4 = num3 - this.m_LastTime;
                    bool flag2 = num3 < this.m_LastTime || num3 < num2 || Mathf.Approximately(this.m_LastTime, -1f) || num4 > this.particleSystem.main.duration || !Mathf.Approximately(this.m_LastPsTime, this.particleSystem.time);
                    if (flag2)
                    {
                        this.particleSystem.Simulate(0f, true, true);
                        this.particleSystem.Simulate(num3, true, false);
                    }
                    else
                    {
                        float num5 = num3 % this.particleSystem.main.duration;
                        float num6 = num5 - this.particleSystem.time;
                        if (num6 < -num2)
                        {
                            num6 = num5 + this.particleSystem.main.duration - this.particleSystem.time;

                            if (this.particleSystem.main.loop == false)
                            {
                                num6 = time - this.m_LastTime;
                            }
                        }
                        this.particleSystem.Simulate(num6, true, false);
                    }
                    this.m_LastPsTime = this.particleSystem.time;
                    this.m_LastTime = time;
                }
            }

        }

		void Stop(){
			em.enabled = false;
            particles.Stop();
            particles.time = 0;
            this.m_LastTime = -1;
            particles.Clear();
        }
	}
}