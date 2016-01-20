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
    // Trnaslate Animation Parameter
    public Vector2 Translate_range;
    public float Translate_speed = 0f;

    private Vector2 _canvasRect;
    private GameObject _canvasEffecter;
    private int _imageNum;
    private Image _UiImage;

    Transform _canvasTransform;
    void Awake(){
        _canvasTransform = GameObject.Find("Canvas").transform;
    }
    void OnEnable() {
        _imageNum = 0;

        _canvasEffecter = Instantiate((GameObject)Resources.Load("Prefabs/EffectPrefab", typeof(GameObject)));
        _canvasEffecter.transform.SetParent(_canvasTransform);
        _canvasRect = new Vector2(_canvasTransform.GetComponent<RectTransform>().rect.width * 0.5f, _canvasTransform.GetComponent<RectTransform>().rect.height * 0.5f);

        _UiImage = _canvasEffecter.GetComponent<Image>();
        _UiImage.enabled = false;
        _canvasEffecter.GetComponent<RectTransform>().localPosition
            = new Vector3(_canvasRect.x * Effecter_position.x, _canvasRect.y * Effecter_position.y, 0f);
        StartCoroutine(Animation());
    }
    IEnumerator Animation(){
        bool trigger = false;
        float current_time = 0f;
        int _imageNum2 = 0;
        while (sprites.Length * Animation_num > _imageNum2)
        {
            current_time += Time.deltaTime;
            //Initialize
            if (!trigger && current_time > Animation_StartTime)
            {
                trigger = true;
                current_time = 0f;
                _UiImage.enabled = true;
                _UiImage.sprite = sprites[_imageNum];
                _UiImage.GetComponent<Image>().SetNativeSize();
            }
            // Update
            if(trigger) {
                if (sprites.Length <= _imageNum)
                {
                    // image
                    _imageNum = 0;
                    // translate
                    _canvasEffecter.GetComponent<RectTransform>().localPosition
                         = new Vector3(_canvasRect.x * Effecter_position.x, _canvasRect.y * Effecter_position.y, 0f);
                }
                if (current_time > (Animation_PlayTime / sprites.Length))
                {
                    // image
                    current_time = 0f;
                    _UiImage.sprite = sprites[_imageNum++];
                    _UiImage.GetComponent<Image>().SetNativeSize();
                    _imageNum2++;
                    // translate
                    if (Translate_speed != 0f)
                    {
                        Vector3 vector3_range = Translate_range;
                        _canvasEffecter.GetComponent<RectTransform>().localPosition
                            += vector3_range * Translate_speed;// *Time.deltaTime;
                    }
                }
            }
            yield return null;
        }
        Destroy(_canvasEffecter);

        transform.gameObject.SetActive(false);
    }
}
