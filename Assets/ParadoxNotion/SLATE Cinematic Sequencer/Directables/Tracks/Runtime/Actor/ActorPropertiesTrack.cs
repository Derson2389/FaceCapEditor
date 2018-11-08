namespace Slate{

	[Attachable(typeof(ActorGroup))]
	public class ActorPropertiesTrack : PropertiesTrack {

		//just add some defaults for convenience
		protected override void OnCreate(){
			base.OnCreate();
			animationData.TryAddParameter( typeof(UnityEngine.Transform).RTGetProperty("localPosition"), this, null, null );
			animationData.TryAddParameter( typeof(UnityEngine.Transform).RTGetProperty("localEulerAngles"), this, null, null );
            /// @modify slate sequencer
            /// @TQ
            /// add default localScale to animationData 
            animationData.TryAddParameter( typeof(UnityEngine.Transform).RTGetProperty("localScale"), this, null, null);
            /// end
        }
	}
}