using UnityEngine;
using System.Collections;

public class _2DEffcter : MonoBehaviour {

    /// Billboard Script
    ///.Warning: GameObject need '2DSprite' Componant
    
    protected new Transform transform;
    private Camera _mainCamera;
	// Use this for initialization
	void Awake () {
        transform = GetComponent<Transform>();
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera == null)
            Debug.Log("Err: Can't find MainCamera");
        _mainCamera = mainCamera.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        // billbaord script
        transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                _mainCamera.transform.rotation * Vector3.up);
	}
}
