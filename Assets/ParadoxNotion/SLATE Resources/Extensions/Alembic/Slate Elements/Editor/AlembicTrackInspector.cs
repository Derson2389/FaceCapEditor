#if !NO_UTJ

using UnityEditor;
using UnityEngine;
using UTJ.Alembic;

namespace Slate{

	[CustomEditor(typeof(AlembicTrack))]
	public class AlembicTrackInspector : Editor {

		private AlembicTrack track{
			get {return (AlembicTrack)target;}
		}
        float SnapTime(float time)
        {
            return (Mathf.Round(time / Prefs.snapInterval) * Prefs.snapInterval);
        }

        public override void OnInspectorGUI(){
			
			base.OnInspectorGUI();

			if (track.alembicPlayer != null){
				float offset = track.abcTimeOffset * Prefs.frameRate;
				offset = (int)EditorGUILayout.IntField("Offset", (int)offset);
				track.abcTimeOffset = SnapTime(offset * (1f/Prefs.frameRate));
				if (GUI.changed){
					EditorUtility.SetDirty(track.alembicPlayer);
				}
			}

			if (track.alembicPlayer == null){
				if (GUILayout.Button("Import Alembic File")){
					UTJ.Alembic.AlembicManualImporterEditor.ShowWindow();
				}

				EditorGUILayout.HelpBox("If you already have an Alembic file imported and in the scene, proceed to 4.\n1) Click 'Import Alembic File' to open the importer window.\n2) It's highly recomended to leave import settings to default.\n3) If you chose to 'Generate Prefab', add a prefab instance in the scene.\n4) Assign the imported Alembic gameobject in the 'Alembic Player' field above.", MessageType.None);
			}
		}
	}
}

#endif