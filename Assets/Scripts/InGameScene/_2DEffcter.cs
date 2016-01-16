using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class _2DEffcter : MonoBehaviour {

    /// Billboard Script
    ///.Warning: GameObject need '2DSprite' Componant
    
    protected new Transform transform;
    private Camera _mainCamera;

    public Transform effectImage_transform;
    public Sprite[] sprites;
    public float AnimationTime = 3f;
    public float Animation_num = 3f;

    private int _imageNum = 0;
    private SpriteRenderer _spriteRenderer;
	// Use this for initialization
	void Awake () {
        transform = GetComponent<Transform>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

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
    void OnTriggerEnter(Collider other){
        transform.position = effectImage_transform.position;
        StartCoroutine(Animation());
    }

    IEnumerator Animation(){
        float current_time = 0f;
        int _imageNum2 = 0;
        while (sprites.Length * Animation_num > _imageNum2)
        {
            current_time += Time.deltaTime;

            if (sprites.Length <= _imageNum)
                _imageNum = 0;
            
            if (current_time > (AnimationTime / sprites.Length))
            {
                current_time = 0f;
                _spriteRenderer.sprite = sprites[_imageNum++];
                _imageNum2++;
            }
             
            yield return null;
        }
        transform.gameObject.SetActive(false);
    }
}
