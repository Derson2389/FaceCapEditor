///#if USE_STORY_ENGINE

using Slate;
using Slate.ActionClips;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Slate.ActionClips.StoryEngineClip
{
    [Category("DStoryEngine")]
    [Slate.Name("Story SampleParticeSystem Clip")]
    public class StorySampleParticeSystemActionClip : ActorActionClip<DStoryEngine.StoryParticleSystem>, ICrossBlendable
    {
        [SerializeField]
        [HideInInspector]
        private float _length = 2f;

        // [Required]
        private List<ParticleSystem> particles = null;
        private List<ParticleSystem.EmissionModule> em = new List<ParticleSystem.EmissionModule>();


        public ParticleSystem get_particleSystem(int idx)
        {        
             return this.particles[idx];           
        }


        private const float kUnsetTime = -1f;

        private List<float> m_LastTime = new List<float>();

        private List<float> m_LastPsTime = new List<float>();

        private List<uint> m_RandomSeed = new List<uint>();


        public override string info
        {
            get {
                return string.Format("Particles ({0})\n{1}", particles!= null && particles.Count > 0 && loop ? "Looping" : "OneShot", particles != null && particles.Count > 0 ? actor.gameObject.name : "NONE");
            }
        }

        public override bool isValid
        {
            get
            {
                if (actor == null)
                    return false;

                var tempParticles = actor.GetComponentsInChildren<ParticleSystem>();
                var rootParticle = actor.GetComponent<ParticleSystem>();
                bool _isValid = actor != null && tempParticles != null;
                if (particles == null || particles.Count == 0)
                {
                    particles = new List<ParticleSystem>();
                    foreach (var particle in tempParticles)
                    {
                        particles.Add(particle);
                    }
                    if (rootParticle != null)
                    {
                        particles.Add(rootParticle);
                    }                   
                }
                foreach (var particle in particles)
                {
                    m_LastTime.Add(-1f);
                    m_LastPsTime.Add(-1f);
                    m_RandomSeed.Add(particle.randomSeed);
                }
                
                return _isValid;
            }
        }

        public override float length
        {
            get
            {
                return particles != null && particles.Count > 0 || loop ? _length : duration + startLifetime;
            }
            set
            {
                _length = value;
            }
        }

        public override float blendOut
        {
            get { return isValid && !loop ? startLifetime : 0.1f; }
        }

        private bool loop
        {
#if UNITY_5_5_OR_NEWER
            get
            {

                return particles != null && particles.Count > 0 && particles[0].main.loop;
            }
#else
			get {return particles != null && particles.Count >0 && particles.loop;}
#endif
        }

        private float duration
        {
#if UNITY_5_5_OR_NEWER
            get
            {
                return particles != null && particles.Count > 0 ? particles[0].main.duration : 0f;
            }
#else
			get {return particles != null && particles.Count > 0 ? particles[0].duration : 0f;}
#endif
        }

        private float startLifetime
        {
#if UNITY_5_5_OR_NEWER
            get
            {
                return particles != null && particles.Count > 0 ? particles[0].main.startLifetimeMultiplier : 0f;
            }
#else
			get {return particles != null && particles.Count > 0 ? particles[0].startLifetime : 0f;}
#endif
        }

        protected override void OnEnter()
        {
            Play();
        }
        protected override void OnReverseEnter()
        {
            Play();
        }
        protected override void OnExit()
        {
            Stop();
        }
        protected override void OnReverse()
        {
            Stop();
        }

        void Play()
        {
            for (int i = 0; i < particles.Count; ++i)
            {
                if (!particles[i].isPlaying)
                {
                    particles[i].useAutoRandomSeed = false;
                }
                ParticleSystem.EmissionModule _em = particles[i].emission;
                _em.enabled = true;
                if (!em.Contains(_em))
                {
                    em.Add(_em);
                }       
                this.m_LastTime[i] = -1f;
                particles[i].Play();
            }
        }


        protected override void OnUpdate(float time)
        {
            if (!Application.isPlaying)
            {
                for (int i = 0; i < particles.Count; ++i)
                {
                    if (!(this.get_particleSystem(i) == null) && this.get_particleSystem(i).gameObject.activeInHierarchy)
                    {
                        if (this.get_particleSystem(i).randomSeed != this.m_RandomSeed[i] && !this.get_particleSystem(i).isPlaying)
                        {
                            this.get_particleSystem(i).Stop();
                            this.get_particleSystem(i).useAutoRandomSeed = false;
                            this.get_particleSystem(i).randomSeed = this.m_RandomSeed[i];
                        }
                        // float num = (float)PlayableExtensions.GetTime<Playable>(playable);
                        bool flag = Mathf.Approximately(this.m_LastTime[i], -1f) || !Mathf.Approximately(this.m_LastTime[i], time);
                        if (flag)
                        {
                            float num2 = Time.fixedDeltaTime * 0.5f;
                            float num3 = time;
                            float num4 = num3 - this.m_LastTime[i];
                            bool flag2 = num3 < this.m_LastTime[i] || num3 < num2 || Mathf.Approximately(this.m_LastTime[i], -1f) || num4 > this.get_particleSystem(i).main.duration || !Mathf.Approximately(this.m_LastPsTime[i], this.get_particleSystem(i).time);
                            if (flag2)
                            {
                                this.get_particleSystem(i).Simulate(0f, true, true);
                                this.get_particleSystem(i).Simulate(num3, true, false);
                            }
                            else
                            {
                                float num5 = num3 % this.get_particleSystem(i).main.duration;
                                float num6 = num5 - this.get_particleSystem(i).time;
                                if (num6 < -num2)
                                {
                                    num6 = num5 + this.get_particleSystem(i).main.duration - this.get_particleSystem(i).time;
                                    if (this.get_particleSystem(i).main.loop == false)
                                    {
                                        /// num6 = 0.02f;
                                        num6 = time - this.m_LastTime[i];
                                    }
                                }
                                this.get_particleSystem(i).Simulate(num6, true, false);
                            }
                            this.m_LastPsTime[i] = this.get_particleSystem(i).time;
                            this.m_LastTime[i] = time;
                        }
                    }
                }

            }
            
        }

        void Stop()
        {
            for (int i = 0; i < particles.Count; ++i)
            {
                ParticleSystem.EmissionModule _em = em[i];
                _em.enabled = false;
                particles[i].Stop();
                particles[i].time = 0;
                this.m_LastTime[i] = -1;
                particles[i].Clear();
            }
            //particles.Clear();
            //m_RandomSeed.Clear();
            //m_LastPsTime.Clear();
            //m_LastTime.Clear();
        }
    }
}

///#endif