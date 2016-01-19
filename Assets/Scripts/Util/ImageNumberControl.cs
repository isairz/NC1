using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageNumberControl : MonoBehaviour
{
    public string ResourcesPath;
    public int Max_NumSize;
    public Vector2 Position;
    public Image AdditionImage;

    private Sprite[] _nummberImage = new Sprite[10];
    private GameObject[] _canvasImageObject;
    private Vector2 _canvasRect;
    private float _imageWidth;
    void Start()
    {
        for (int index = 0; index < 10; index++)
        {
            _nummberImage[index] = Resources.Load(ResourcesPath + index, typeof(Sprite)) as Sprite;
        }
        _imageWidth = _nummberImage[0].rect.width;

        //make GameObject
        Transform _canvasTransform = GameObject.Find("Canvas").transform;
        _canvasRect = new Vector2(_canvasTransform.GetComponent<RectTransform>().rect.width * 0.5f, _canvasTransform.GetComponent<RectTransform>().rect.height * 0.5f);
        _canvasImageObject = new GameObject[Max_NumSize];
        for (int index = 0; index < Max_NumSize; index++)
        {
            GameObject go = Instantiate((GameObject)Resources.Load("Prefabs/EffectPrefab", typeof(GameObject)));
            go.transform.SetParent(_canvasTransform);
            go.GetComponent<Image>().enabled = false;
            _canvasImageObject[index] = go;
        }
    }
    public void SetValue(int num_value)
    {
        // get length
        string string_value = num_value.ToString();
        for (int index = 0; index < _canvasImageObject.Length; index++)
        {
            if (index < string_value.Length)
            {
                _canvasImageObject[index].GetComponent<Image>().enabled = true;
                _canvasImageObject[index].GetComponent<Image>().sprite = _nummberImage[int.Parse(string_value.Substring(index, 1))];
                _canvasImageObject[index].GetComponent<Image>().SetNativeSize();
                _canvasImageObject[index].GetComponent<RectTransform>().localPosition
                    = new Vector3((_canvasRect.x * Position.x) + index * _imageWidth, _canvasRect.y * Position.y, 0f);
            }
            else
                _canvasImageObject[index].GetComponent<Image>().enabled = false;
        }
    }
    private IEnumerator coroutine = null;
    private int value = 0;
    public void SetValueTime(int num_value) {
        if (num_value != 0 && value != num_value)
        {
            if (coroutine != null) { 
                StopCoroutine(coroutine);
                coroutine = null;
            }
            coroutine = Animation(num_value, 1f);
            StartCoroutine(coroutine);
            value = num_value;
        }
    }
    IEnumerator Animation(int num_value, float time) 
    {
        float startTime = 0f;
        SetValue(num_value);
        AdditionImage.enabled = true;
        while (time > startTime)
        {
            startTime += Time.deltaTime;
            yield return null;
        }
        for (int index = 0; index < _canvasImageObject.Length; index++)
            _canvasImageObject[index].GetComponent<Image>().enabled = false;
        AdditionImage.enabled = false;
    }
}
