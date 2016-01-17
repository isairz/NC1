using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameSceneManager : MonoBehaviour{

    public GameObject[] LifeUIObjects;
    private int _lifeValue;

    private ParticleController _ParticleControllerScript;
    void Awake() {
        SetLife(LifeUIObjects.Length);
        //get canvas
        myCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _ParticleControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<ParticleController>();
    }
    public void DecreaseLife() 
    {
        --_lifeValue;
        UpdateLifeUI();
    }
    public void SetLife(int lifeNum)
    {
        _lifeValue = lifeNum;
        UpdateLifeUI();
    }
    private void UpdateLifeUI() 
    {
        for (int index = 0; index < LifeUIObjects.Length; index++)
        {
            if (index < _lifeValue)
                LifeUIObjects[index].SetActive(true);
            else
                LifeUIObjects[index].SetActive(false);
        }

    }
    public GameObject touchButton_bg, touchButton_action;
    private float _deadZoneRadius = 40f;
    private Vector2 first_position;
    private Canvas myCanvas;
    private enum MouseStatus { deafault = 0, OnDown = 1,Down = 2,OnUp = 3}
    private MouseStatus _mouseState = MouseStatus.deafault;
    void Update() {
        if (_mouseState == MouseStatus.OnDown)
        {
            Vector2 cur_position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out cur_position);
            touchButton_bg.GetComponent<RectTransform>().position = myCanvas.transform.TransformPoint(cur_position);
            touchButton_action.GetComponent<RectTransform>().position = myCanvas.transform.TransformPoint(cur_position);

            touchButton_action.GetComponent<RectTransform>().position = myCanvas.transform.TransformPoint(cur_position);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            _mouseState = MouseStatus.OnDown;
            touchButton_bg.SetActive(true);
            touchButton_action.SetActive(true);

            Vector2 cur_position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out cur_position);
            touchButton_bg.GetComponent<RectTransform>().position = myCanvas.transform.TransformPoint(cur_position);
            touchButton_action.GetComponent<RectTransform>().position = myCanvas.transform.TransformPoint(cur_position);

            first_position = cur_position;
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 cur_position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out cur_position);
            touchButton_bg.GetComponent<RectTransform>().position = myCanvas.transform.TransformPoint(cur_position);
            touchButton_action.GetComponent<RectTransform>().position = myCanvas.transform.TransformPoint(cur_position);

            if (Vector2.Distance(first_position, cur_position) > _deadZoneRadius)
            {
                //call particleController
                Vector3 ControllerForce = Vector3.MoveTowards(
                Vector3.zero,
                (cur_position - first_position).normalized,
                1f);
                _ParticleControllerScript.Action(ControllerForce);
            }
            _mouseState = MouseStatus.OnUp;
            touchButton_bg.SetActive(false);
            touchButton_action.SetActive(false);
        }
    }
}
