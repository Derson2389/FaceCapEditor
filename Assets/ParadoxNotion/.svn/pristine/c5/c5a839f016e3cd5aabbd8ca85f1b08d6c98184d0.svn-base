namespace Slate.ActionClips{

	[Category("Utility")]
	[Description("Pauses the Cutscene (PlayMode Only). It's up to other scripts to resume it.")]
	public class PauseCutscene : DirectorActionClip {
		protected override void OnEnter(){
			if (UnityEngine.Application.isPlaying){
				root.Pause();
				root.Sample(this.startTime);
			}
		}
	}
}