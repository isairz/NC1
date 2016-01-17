using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SplineController))]
public class SplineEditer : Editor
{
	SplineController _target;
	GUIStyle style = new GUIStyle();
	public static int count = 0;

	void OnEnable(){
		//i like bold handle labels since I'm getting old:
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;
		_target = (SplineController)target;

		//lock in a default path name:
		if(!_target.initialized){
			_target.initialized = true;
			_target.SplineName = "New Path " + ++count;
			_target.initialName = _target.SplineName;
		}
	}

	public override void OnInspectorGUI(){
		base.OnInspectorGUI();

		//path name:
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Name");
		_target.SplineName = EditorGUILayout.TextField(_target.SplineName);
		EditorGUILayout.EndHorizontal();

		if(_target.SplineName == ""){
			_target.SplineName = _target.initialName;
		}

		_target.ReloadNodes();

		//node display:
		EditorGUI.indentLevel = 4;
		for (int i = 0; i < _target.rawNodes.Length; i++) {
			// _target.Nodes[i].transform.position = EditorGUILayout.Vector3Field("Node " + (i+1), _target.Nodes[i].position);
			_target.rawNodes[i].Time = EditorGUILayout.FloatField("Time " + (i+1), _target.rawNodes[i].Time);
		}

		//update and redraw:
		if(GUI.changed){
			EditorUtility.SetDirty(_target);
		}
	}

	void OnSceneGUI(){
		if (_target.pathVisible) {			
			if (_target.Nodes.Length > 0) {
				//allow path adjustment undo:
				Undo.RecordObject (_target, "Adjust Spline Path");

				//path begin and end labels:
				Handles.Label (_target.Nodes [0].position, "'" + _target.SplineName + "' Begin", style);
				Handles.Label (_target.Nodes [_target.Nodes.Length - 1].position, "'" + _target.SplineName + "' End", style);

				//node handle display:
				for (int i = 0; i < _target.Nodes.Length; i++) {
					_target.Nodes [i].transform.position = Handles.PositionHandle (_target.Nodes [i].position, Quaternion.identity);
				}	
			}	
		}
	}
}
