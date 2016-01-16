using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleController : MonoBehaviour {
	
	public Transform particleBase;
	public int particles = 10;

    protected new Transform transform;

    private SphereCollider _sphereCollider;
    private float _particle_percentage;
    public float Force { get { return _particle_percentage; } }
	// Use this for initialization
	void Awake () {
		transform = GetComponent<Transform> ();
		for (int i = 0; i < particles; i++){
			Transform newParticle = Instantiate (particleBase);
			newParticle.position = new Vector3 (Random.Range (-2f, 2f), Random.Range (-2f, 2f), Random.Range (-2f, 2f));
			newParticle.parent = transform;
            // make SpereCheck Collider
            _sphereCollider = GetComponent<SphereCollider>();
		} 
	}
	// Update is called once per frame
	void FixedUpdate () {
		bool boom = !Input.GetMouseButton (0);
		Vector3 massPoint = new Vector3();
		foreach (Transform child in transform)
		{
			// child.RotateAround (transform.position, new Vector3 (0, 1f, 0), Time.deltaTime * 10f);
			Rigidbody rig = child.GetComponent<Rigidbody>();
			Vector3 toCenter = (transform.position - child.position);
			Vector3 dir = toCenter.normalized;
			float dist = toCenter.magnitude;
			/*if (dist < 0.5f)
				return;*/
			rig.AddForce (dir * dist * dist, ForceMode.Acceleration);
			massPoint += child.position;
		}

		massPoint /= transform.childCount;

        // massPoint > Sphere
        // 충돌 이슈!
        //_sphereCollider.center = massPoint;
        //Debug.Log(_sphereCollider.transform.position);
        _particle_percentage = 0;

		foreach (Transform child in transform)
		{
            // boxIn childs
            if (_sphereCollider.bounds.Contains(child.transform.position))
                ++_particle_percentage;

			Rigidbody rig = child.GetComponent<Rigidbody>();
			Vector3 toMass = (massPoint - child.position);
			Vector3 dir = toMass.normalized;
			float dist = toMass.magnitude;
			if(boom){
				rig.AddForce (-dir * dist * 10f, ForceMode.Acceleration);
			}
		}
        // num' particle > powerW
        // get percentage ( 0 ~ 1 )
        _particle_percentage = _particle_percentage / transform.childCount;
        
	}
}
