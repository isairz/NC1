using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {

    public GameObject Canavas;
    public Image Character_img;
    public Image BG_img;
    
    private Image _Button_img;
    
    public Sprite[] BG_imgs;
    public float animation_time = 3f;

    private float _current_time = 0f;
    private int _imgAnimationIndex = 0;
   
    private float _animation_fps;

    private Rect _canvas_bound;
    private Transform _character_transform;
    private Vector3 _range;
    private float _button_color_speed = -1.5f;
    void Awake() 
    {
        _Button_img = GameObject.FindGameObjectWithTag("Button").GetComponent<Image>();
        _animation_fps = animation_time / BG_imgs.Length;
        _canvas_bound = Canavas.GetComponent<Canvas>().pixelRect;
        _character_transform = Character_img.GetComponent<Transform>();
        _range = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 0f);
        _range.Normalize();
    }
    void Update()
    {
        /// BG Animation
        _current_time += Time.deltaTime;
        // Change Image(=animation)
        if (_current_time > _animation_fps)
        {
            ++_imgAnimationIndex;

            if (_imgAnimationIndex >= BG_imgs.Length)
                _imgAnimationIndex = 0;

            BG_img.sprite = BG_imgs[_imgAnimationIndex];
            _current_time = 0f;
        }
        /// END
        /// Button Alpha Change
        if(_Button_img.color.a > 1f || _Button_img.color.a < 0f)
            _button_color_speed *= -1;
        Color c = _Button_img.color;
        c.a += _button_color_speed * Time.deltaTime;
        _Button_img.color = c;
        /// END
        /// Chracter Move
        if (_canvas_bound.xMin >= _character_transform.position.x
                || _canvas_bound.xMax <= _character_transform.position.x)
            _range.x *= -1f;
        if (_canvas_bound.yMin >= _character_transform.position.y
            || _canvas_bound.yMax <= _character_transform.position.y)
            _range.y *= -1f;
        // speed
        float character_speed = 20f;
        _character_transform.Translate(_range * character_speed * Time.deltaTime);
        /// END
    }
    public void Button_Touch_Event() 
    {
        // change Scene
        Debug.Log("Change Scene");
        //Application.loadedLevel("game");
    }
}
