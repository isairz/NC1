using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour{

    public Image Character_img;
    public Image BG_img;
    
    private Image _Button_img;
    
    public Sprite[] BG_imgs;
    public float animation_time = 3f;

    private float _current_time = 0f;
    private int _imgAnimationIndex = 0;
   
    private float _animation_fps;

    private Transform _character_transform;
    private Vector3 _src_position, _des_position;
    private float _button_color_speed = -1.5f;
    void Awake() 
    {
        _Button_img = GameObject.FindGameObjectWithTag("Button").GetComponent<Image>();
        _animation_fps = animation_time / BG_imgs.Length;
        
        _character_transform = Character_img.GetComponent<Transform>();
        _src_position = _character_transform.localPosition;
        _des_position = _src_position + new Vector3(60f, 24f, 0f);
    }
    float timer = 0f;
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
        if (Vector3.Distance(_des_position, _character_transform.localPosition) < 0.1f)
        { 
            //swap Vector3
            Vector3 temp = _src_position;
            _src_position = _des_position;
            _des_position = temp;
            timer = 0f;
        }
        float character_speed = 0.01f;
        _character_transform.localPosition = Vector3.Lerp(_character_transform.localPosition, _des_position, timer);
        if (_des_position == _character_transform.localPosition)
        {
            // Go to position1 t = 0.0f
            timer = Mathf.Clamp(timer - Time.deltaTime * character_speed, 0.0f, 1.0f);
        }
        else
        {
            // Go to position2 t = 1.0f
            timer = Mathf.Clamp(timer + Time.deltaTime * character_speed, 0.0f, 1.0f);
        }
        /// END
    }
    public void Button_Touch_Event() 
    {
        // change Scene
        Debug.Log("Change Scene");
        //Application.loadedLevel("game");
    }
}
