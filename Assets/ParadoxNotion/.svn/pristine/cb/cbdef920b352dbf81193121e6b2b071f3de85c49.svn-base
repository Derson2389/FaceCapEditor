#if SLATE_USE_EXPRESSIONS

using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StagPoint.Eval;

namespace Slate.Expressions{

	///
	///Collection of wrappers for expressions (WORK IN PROGRESS)
	///

	///Global Wrapper
	public static partial class GlobalEnvironment{
		private static StagPoint.Eval.Environment env;
		public static StagPoint.Eval.Environment Get(){ return env; }
		static GlobalEnvironment(){
			env = new StagPoint.Eval.Environment();
			env.AddConstant("Mathf", typeof(Mathf));
			env.AddConstant("Vector2", typeof(Vector2));
			env.AddConstant("Vector3", typeof(Vector3));
			env.AddConstant("Quaternion", typeof(Quaternion));
			env.AddConstant("Color", typeof(Color));
			env.AddConstant("Slate", typeof(GlobalEnvironment));
		}

		public static float gameTime{get {return Time.realtimeSinceStartup;}}

		public static float Lerp(float a, float b, float t){ return Mathf.Lerp(a, b, t); }
		public static Vector3 Lerp(Vector3 a, Vector3 b, float t){ return Vector3.Lerp(a, b, t); }
		public static Vector2 Lerp(Vector2 a, Vector2 b, float t){ return Vector2.Lerp(a, b, t); }

		public static float Sin(float f){ return Mathf.Sin(f); }

		public static float Noise(float freq, float mag, float t){ return Mathf.PerlinNoise(t * freq, 0) * mag; }
		
	}

	///Cutscene Wrapper
	public partial struct ExpressionCutsceneWrapper{
		private Cutscene cutscene;
		public ExpressionCutsceneWrapper(Cutscene cutscene){ this.cutscene = cutscene; }
		public static void Wrap(Cutscene cutscene, Environment env){
			var wrapper = new ExpressionCutsceneWrapper(cutscene);
			env.AddVariable("cutscene", wrapper, wrapper.GetType());
		}

		public float time{
			get {return cutscene.currentTime;}
		}

		public GameObject GetActor(string name){
			return cutscene.groups.FirstOrDefault(g => g.name == name).actor;
		}
	}

	///Clip Wrapper
	public partial struct ExpressionActionClipWrapper{
		private ActionClip clip;
		public ExpressionActionClipWrapper(ActionClip clip){ this.clip = clip; }
		public static void Wrap(ActionClip clip, Environment env){
			var wrapper = new ExpressionActionClipWrapper(clip);
			env.AddVariable("clip", wrapper, wrapper.GetType());

			foreach(var animParam in clip.animationData.animatedParameters){
				env.AddVariable( new BoundVariable( animParam.parameterName.SplitCamelCase().Replace(" ", ""), animParam.ResolvedObject(), animParam.GetMemberInfo() ) );
			}
		}

		public float weight{
			get {return clip.GetClipWeight();}
		}

	}

	///Parameter Wrapper
	public partial struct ExpressionParameterWrapper{
		private AnimatedParameter animParam;
		public ExpressionParameterWrapper(AnimatedParameter animParam){ this.animParam = animParam; }
		public static void Wrap(AnimatedParameter animParam, Environment env){
			var wrapper = new ExpressionParameterWrapper(animParam);
			env.AddVariable("parameter", wrapper, wrapper.GetType());
			env.AddVariable(animParam.parameterName, wrapper, wrapper.GetType());
			// env.AddVariable(  new BoundVariable("value", animParam.ResolvedObject(), animParam.GetMemberInfo())  );
		}

		public object value{
			get {return animParam.GetEvalValue(animParam.keyable.root.currentTime - animParam.keyable.startTime);}
		}
	}
}

#endif