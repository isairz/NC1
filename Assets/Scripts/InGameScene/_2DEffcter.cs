using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class _2DEffcter : MonoBehaviour {

    /// Billboard Script
    ///.Warning: GameObject need '2DSprite' Componant
    ///
    public Sprite[] sprites;
    public float Animation_StartTime = 0f;
    public float Animation_PlayTime = 3f;
    public float Animation_num = 3f;
    public Vector2 Effecter_position;

    private GameObject _canvasEffecter;
    private int _imageNum = 0;
    private Image _UiImage;

    void Start(){
        _canvasEffecter = Instantiate((GameObject)Resources.Load("Prefabs/EffectPrefab", typeof(GameObject)));
        _canvasEffecter.transform.SetParent(GameObject.Find("Canvas").transform);

        _UiImage = _canvasEffecter.GetComponent<Image>();
        _UiImage.enabled = false;
        _canvasEffecter.GetComponent<RectTransform>().localPosition
            = new Vector3(Effecter_position.x, Effecter_position.y, 0f);
        StartCoroutine(Animation());  
    }

    IEnumerator Animation(){
        bool trigger = false;
        float current_time = 0f;
        int _imageNum2 = 0;
        while (sprites.Length * Animation_num > _imageNum2)
        {
            current_time += Time.deltaTime;

            if (!trigger && current_time > Animation_StartTime)
            {
                trigger = true;
                current_time = 0f;
                _UiImage.enabled = true;
                _UiImage.sprite = sprites[_imageNum];
            }
            if(trigger) { 
                if (sprites.Length <= _imageNum)
                    _imageNum = 0;

                if (current_time > (Animation_PlayTime / sprites.Length))
                {
                    current_time = 0f;
                    _UiImage.sprite = sprites[_imageNum++];
                    _imageNum2++;
                }
            }
            yield return null;
        }
        Destroy(_canvasEffecter);
        transform.gameObject.SetActive(false);
    }
}
