using UnityEngine;

[AddComponentMenu("Splines/Spline Node")]
public class SplineNode : MonoBehaviour {
	public enum eSplineMode { Spline, Line }
	public eSplineMode SplineMode = eSplineMode.Spline;
	public float Time;
	public Vector2 EaseIO;

	public Vector3 position {
		get {
			return transform.position;
		}
	}

	public Quaternion rotation {
		get {
			return transform.rotation;
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0, 0.2f, 1);
		Gizmos.DrawCube (position, new Vector3(1,1,1));
	}

	// SplineNode(Vector3 p, Quaternion q, float t, Vector2 io) { Point = p; Rot = q; Time = t; EaseIO = io; }
	// 
	// SplineNode(SplineNode o) { Rot = o.Rot; Time = o.Time; EaseIO = o.EaseIO; }
	// Use this for initialization
	void Awake () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
