using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DStoryEngine
{
    [ExecuteInEditMode]
    public class StoryParticleSystem : MonoBehaviour
    {

        public bool bControlSubParticle = false;

        //粒子特效支持
        public ParticleSystemCurveMode StartDelayMode = ParticleSystemCurveMode.Constant;
        public float Start_Delay_ConstantMax = 0f;
        public float Start_Delay_ConstantMin = 0f;

        public ParticleSystemCurveMode StartSizeMode = ParticleSystemCurveMode.Constant;
        public float Start_Size_ConstantMax = 0f;
        public float Start_Size_ConstantMin = 0f;

        public ParticleSystemCurveMode StartLifeTimeMode = ParticleSystemCurveMode.Constant;
        public float Start_LifeTime_ConstantMax = 0f;
        public float Start_LifeTime_ConstantMin = 0f;

        public ParticleSystemCurveMode StartSpeedMode = ParticleSystemCurveMode.Constant;
        public float Start_Speed_ConstantMax = 0f;
        public float Start_Speed_ConstantMin = 0f;

        public ParticleSystemCurveMode StartRotationMode = ParticleSystemCurveMode.Constant;
        public float Start_Rotation_ConstantMax = 0f;
        public float Start_Rotation_ConstantMin = 0f;

        public ParticleSystemCurveMode StartGravityModifierMode = ParticleSystemCurveMode.Constant;
        public float Start_GravityModifier_ConstantMax = 0f;
        public float Start_GravityModifier_ConstantMin = 0f;

        private List<ParticleSystem> particles = null;

        private void Start()
        {
            var tempParticles = this.GetComponentsInChildren<ParticleSystem>();
            //var rootParticle = this.GetComponent<ParticleSystem>();

            if (particles == null || particles.Count == 0)
            {
                particles = new List<ParticleSystem>();
                foreach (var particle in tempParticles)
                {
                    particles.Add(particle);
                }
            }
        }

        private void Update()
        {
            if (!bControlSubParticle)
                return;
            for (int i = 0; i < particles.Count; ++i)
            {
                ParticleSystem.MainModule mainModule = particles[i].main;

                ParticleSystem.MinMaxCurve startSize = particles[i].main.startSize;
                startSize.constantMax = Start_Size_ConstantMax;
                startSize.constantMin = Start_Size_ConstantMin;
                startSize.mode = StartSizeMode;
                if (!startSize.Equals(particles[i].main.startSize))
                {
                    mainModule.startSize = startSize;
                }

                ParticleSystem.MinMaxCurve startDelay = particles[i].main.startDelay;
                startDelay.constantMax = Start_Delay_ConstantMax;
                startDelay.constantMin = Start_Delay_ConstantMin;
                startDelay.mode = StartDelayMode;
                if (!startDelay.Equals(particles[i].main.startDelay))
                {
                    mainModule.startDelay = startDelay;
                }

                ParticleSystem.MinMaxCurve startLifeTime = particles[i].main.startLifetime;
                startLifeTime.constantMax = Start_LifeTime_ConstantMax;
                startLifeTime.constantMin = Start_LifeTime_ConstantMin;
                startLifeTime.mode = StartDelayMode;
                if (!startLifeTime.Equals(particles[i].main.startLifetime))
                {
                    mainModule.startLifetime = startLifeTime;
                }

                ParticleSystem.MinMaxCurve startSpeed = particles[i].main.startSpeed;
                startSpeed.constantMax = Start_Speed_ConstantMax;
                startSpeed.constantMin = Start_Speed_ConstantMin;
                startSpeed.mode = StartSpeedMode;
                if (!startSpeed.Equals(particles[i].main.startSpeed))
                {
                    mainModule.startSpeed = startSpeed;
                }

                ParticleSystem.MinMaxCurve StartRotation = particles[i].main.startRotation;
                StartRotation.constantMax = Start_Rotation_ConstantMax;
                StartRotation.constantMin = Start_Rotation_ConstantMin;
                StartRotation.mode = StartRotationMode;
                if (!StartRotation.Equals(particles[i].main.startRotation))
                {
                    mainModule.startRotation = StartRotation;
                }

                ParticleSystem.MinMaxCurve StartGravityModifier = particles[i].main.gravityModifier;
                StartGravityModifier.constantMax = Start_GravityModifier_ConstantMax;
                StartGravityModifier.constantMin = Start_GravityModifier_ConstantMin;
                StartGravityModifier.mode = StartGravityModifierMode;
                if (!StartGravityModifier.Equals(particles[i].main.gravityModifier))
                {
                    mainModule.gravityModifier = StartGravityModifier;
                }
            }
        }
    }
}