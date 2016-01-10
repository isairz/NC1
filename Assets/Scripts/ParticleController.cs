using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleController : MonoBehaviour {

	public new Transform transform;
	public Transform particleBase;
	public int particles = 10;

	// Use this for initialization
	void Awake () {
		transform = GetComponent<Transform> ();
		for (int i = 0; i < particles; i++){
			Transform newParticle = Instantiate (particleBase);
			newParticle.position = new Vector3 (Random.Range (-2f, 2f), Random.Range (-2f, 2f), Random.Range (-2f, 2f));
			newParticle.parent = transform;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		bool boom = Input.GetMouseButton (0);
		Debug.Log (boom);
		foreach (Transform child in transform)
		{
			// child.RotateAround (transform.position, new Vector3 (0, 1f, 0), Time.deltaTime * 10f);
			Rigidbody rig = child.GetComponent<Rigidbody>();
			Vector3 toCenter = (transform.position - child.position);
			Vector3 dir = toCenter.normalized;
			float dist = toCenter.magnitude;
			/*if (dist < 0.5f)
				return;*/
			rig.AddForce (dir * dist * 1f, ForceMode.Acceleration);
			if(boom){
				rig.AddForce (-dir * 10f, ForceMode.Acceleration);
				// rig.AddExplosionForce(100f * Time.deltaTime, transform.position, 100f);
			}
		}
	}
}
