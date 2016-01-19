using UnityEngine;
using VolumetricLines;
using System.Collections;
using System.Collections.Generic;

public enum eOrientationMode { NODE = 0, TANGENT }
public enum eWrapMode { ONCE, LOOP }
public enum eEndPointsMode { EXPLICIT, AUTO, LOOP };

[AddComponentMenu("Splines/Spline Controller")]
public class SplineController : MonoBehaviour
{
	public string SplineName = "";
	public Material lineMat;
	public GameObject SplineRoot;

	// public List<Vector3> Nodes = new List<SplineNode> ();
	// public List<SplineNode> Nodes = new List<SplineNode> ();
	// public List<Vector3> Nodes = new List<Vector3>(){Vector3.zero, Vector3.zero};
	[HideInInspector]
	public SplineNode[] Nodes;
	public SplineNode[] rawNodes;
	public eOrientationMode OrientationMode = eOrientationMode.NODE;
	public eWrapMode WrapMode = eWrapMode.ONCE;
	public eEndPointsMode EndPointsMode = eEndPointsMode.AUTO;
	public bool AutoStart = true;
	public bool AutoClose = true;

	public Renderer SkyDome;

	[HideInInspector]
	public bool initialized = false;
	public string initialName = "";
	public bool pathVisible = true;

	float mCurrentTime;
	int mCurrentIdx = 1;
	string mState = "";

	void Awake()
	{
		mState = "Reset";
	}

	void Start()
	{
		if (AutoStart) {
			mState = WrapMode.ToString ();
		}
		DrawSpline ();
	}

	void Update()
	{
		if (mState == "Reset" || mState == "Stopped" || Nodes.Length < 4)
			return;

		mCurrentTime += Time.deltaTime;

		// We advance to next point in the path
		if (mCurrentTime >= Nodes[mCurrentIdx + 1].Time)
		{
			if (mCurrentIdx < Nodes.Length - 3)
			{
				NextStep ();
			}
			else
			{
				if (mState != "Loop")
				{
					mState = "Stopped";

					// We stop right in the end point
					transform.position = Nodes[Nodes.Length - 2].transform.position;
				}
				else
				{
					mCurrentIdx = 1;
					mCurrentTime = 0;
				}
			}
		}

		if (mState != "Stopped")
		{
			// Calculates the t param between 0 and 1
			float param = (mCurrentTime - Nodes[mCurrentIdx].Time) / (Nodes[mCurrentIdx + 1].Time - Nodes[mCurrentIdx].Time);

			// Smooth the param
			// param = MathUtils.Ease(param, Nodes[mCurrentIdx].EaseIO.x, Nodes[mCurrentIdx].EaseIO.y);
			transform.position = GetHermiteInternal(mCurrentIdx, param);
			transform.rotation = GetSquad(mCurrentIdx, param);
		}
	}

	void NextStep() {
		mCurrentIdx++;
		//Debug.Log (mCurrentIdx);

		if (Nodes [mCurrentIdx].skyMat) {
			SkyDome.sharedMaterial = Nodes [mCurrentIdx].skyMat;
		}
	}

	public void ReloadNodes()
	{
		if (!SplineRoot) {
			return;
		}

		rawNodes = SplineRoot.GetComponentsInChildren<SplineNode> ();
		switch (EndPointsMode) {
		case eEndPointsMode.EXPLICIT:
			Nodes = rawNodes;
			break;
		case eEndPointsMode.AUTO:
			Nodes = new SplineNode[rawNodes.Length + 2];
			rawNodes.CopyTo (Nodes, 1);
			Nodes [0] = rawNodes [0];
			Nodes [Nodes.Length - 1] = rawNodes [rawNodes.Length - 1];
			break;
		case eEndPointsMode.LOOP:
			Nodes = new SplineNode[rawNodes.Length + 3];
			rawNodes.CopyTo (Nodes, 1);
			Nodes [0] = rawNodes [rawNodes.Length - 1];
			Nodes [Nodes.Length - 2] = rawNodes [0];
			Nodes [Nodes.Length - 1] = rawNodes [1];
			break;
		}
        
		for (int c = 1; c < Nodes.Length; c++) {
			SplineNode node = Nodes [c];
			SplineNode prevNode = Nodes [c - 1];

			// Always interpolate using the shortest path -> Selective negation
			if (Quaternion.Dot (node.rotation, prevNode.rotation) < 0) {
                Nodes[c].transform.rotation = new Quaternion(-node.rotation.x, -node.rotation.y, -node.rotation.z, -node.rotation.w);
			}
		}
	}

	Quaternion GetSquad(int idxFirstPoint, float t)
	{
		Quaternion Q0 = Nodes [idxFirstPoint - 1].rotation;
		Quaternion Q1 = Nodes [idxFirstPoint].rotation;
		Quaternion Q2 = Nodes [idxFirstPoint + 1].rotation;
		Quaternion Q3 = Nodes [idxFirstPoint + 2].rotation;

		Quaternion T1 = MathUtils.GetSquadIntermediate (Q0, Q1, Q2);
		Quaternion T2 = MathUtils.GetSquadIntermediate (Q1, Q2, Q3);

		return MathUtils.GetQuatSquad (t, Q1, Q2, T1, T2);
	}

	public Vector3 GetHermiteInternal(int idxFirstPoint, float t)
	{
		if (Nodes [idxFirstPoint].SplineMode == SplineNode.eSplineMode.Line) {
			Vector3 P1 = Nodes [idxFirstPoint].position;
			Vector3 P2 = Nodes [idxFirstPoint + 1].position;
			return P1 * (1 - t) + P2 * t;
		} else {
			float t2 = t * t;
			float t3 = t2 * t;

			Vector3 P0 = Nodes [idxFirstPoint == 0 ? 0 : idxFirstPoint - 1].position;
			Vector3 P1 = Nodes [idxFirstPoint].position;
			Vector3 P2 = Nodes [idxFirstPoint + 1].position;
			Vector3 P3 = Nodes [idxFirstPoint >= Nodes.Length - 2 ? idxFirstPoint + 1 : idxFirstPoint + 2].position;

			float tension = 0.5f;	// 0.5 equivale a catmull-rom

			Vector3 T1 = tension * (P2 - P0);
			Vector3 T2 = tension * (P3 - P1);

			float Blend1 = 2 * t3 - 3 * t2 + 1;
			float Blend2 = -2 * t3 + 3 * t2;
			float Blend3 = t3 - 2 * t2 + t;
			float Blend4 = t3 - t2;

			return Blend1 * P1 + Blend2 * P2 + Blend3 * T1 + Blend4 * T2;
		}
	}

	public Vector3 GetPositionAtTime(float timeParam)
	{
		if (timeParam >= Nodes[Nodes.Length - 2].Time)
			return Nodes[Nodes.Length - 2].position;

		int c;
		for (c = 1; c < Nodes.Length - 2; c++)
		{
			for (c = 1; c < Nodes.Length - 2; c++)
			if (Nodes[c].Time > timeParam)
				break;
		}

		int idx = c - 1;
		float param = (timeParam - Nodes[idx].Time) / (Nodes[idx + 1].Time - Nodes[idx].Time);
		param = MathUtils.Ease(param, Nodes[idx].EaseIO.x, Nodes[idx].EaseIO.y);

		return GetHermiteInternal(idx, param);
	}

	// To show the lines in the editor
	void OnDrawGizmos() {
		const int step = 10;
		for(int i = 1; i < Nodes.Length - 2; i++) {
			Vector3 prevPos = GetHermiteInternal (i, 0f);
			for (float t = 0; t <= step; t++) {
				Vector3 pos = GetHermiteInternal (i, (float)t/step);
				GL.Begin(GL.LINES);
				lineMat.SetPass(0);
				GL.Color(new Color(lineMat.color.r, lineMat.color.g, lineMat.color.b, lineMat.color.a));
				// GL.Color(SplineColor);
				GL.Vertex(prevPos);
				GL.Vertex(pos);
				GL.End();
				prevPos = pos;
			}
		}
	}

	void DrawSpline ()
	{
		// Create an empty game object
		GameObject go = new GameObject();

		// Add the MeshFilter component, VolumetricLineStripBehavior requires it
		go.AddComponent<MeshFilter>();

		// Add a MeshRenderer, VolumetricLineStripBehavior requires it, and set the material
		var meshRenderer = go.AddComponent<MeshRenderer>();
		meshRenderer.material = lineMat;

		// Add the VolumetricLineStripBehavior and set parameters, like color and all the vertices of the line
		var volLineStrip = go.AddComponent<VolumetricLineStripBehavior>();
		volLineStrip.SetLineColorAtStart = true;
		volLineStrip.LineColor = Color.cyan;
		volLineStrip.LineWidth = 1.5f;

		const int step = 100;
		int idx = 1;
		var lineVertices = new Vector3[(Nodes.Length - 3) * step + 1];

		lineVertices [0] = GetHermiteInternal (1, 0);

		for(int i = 1; i < Nodes.Length - 2; i++) {
			for (int t = 1; t <= step; t++) {
				Vector3 pos = GetHermiteInternal (i, (float)t/step);
				lineVertices[idx++] = gameObject.transform.TransformPoint(pos);
				/*GL.Begin(GL.LINES);
				lineMat.SetPass(0);
				GL.Color(new Color(lineMat.color.r, lineMat.color.g, lineMat.color.b, lineMat.color.a));
				// GL.Color(SplineColor);
				GL.Vertex(prevPos);
				GL.Vertex(pos);
				GL.End();*/
			}
		}

		volLineStrip.UpdateLineVertices(lineVertices);
	}
}