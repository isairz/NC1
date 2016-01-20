using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameSceneManager : MonoBehaviour{

    public GameObject HP_UI_Object;
    private ImageNumberControl HP_UI_Script;
    public GameObject COMBO_UI_Object;
    private ImageNumberControl COMBO_UI_Script;

    public GameObject UI_PAUSE;
    public GameObject UI_SCORE;

    public GameObject[] UIGUAGE;
    public GameObject UI_Pever_Object;
    public GameObject UI_Miss_Object;

    public int Asteroid_num = 3;

    private ParticleController _ParticleControllerScript;

    private int Combo = 0;
    void Awake() {
        //get canvas
        myCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _ParticleControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<ParticleController>();
        //Attatch UI Script
        HP_UI_Script = HP_UI_Object.GetComponent<ImageNumberControl>();
        COMBO_UI_Script = COMBO_UI_Object.GetComponent<ImageNumberControl>();
        //Make Asteroid
        AsteroidMake();
        //BGM
        SoundEffectControl.Instance.PlayBackgroundMusic("bgm_gameStage1");
    }
    public GameObject touchButton_bg, touchButton_action;
    private float _deadZoneRadius = 40f;
    private Vector2 first_position;
    private Canvas myCanvas;
    private enum MouseStatus { deafault = 0, OnDown = 1,Down = 2,OnUp = 3}
    private MouseStatus _mouseState = MouseStatus.deafault;
    public void StartFeverUI(bool isOn) {
        if (isOn)
            UI_Pever_Object.SetActive(isOn);
        UIGUAGE[6].SetActive(!isOn);
        UIGUAGE[7].SetActive(isOn);
    }
    public void StartMissUI() {
        UI_Miss_Object.SetActive(true);
    }
    public void PauseEvent() {
        Debug.Log("in");
        SplineController sc = GameObject.FindGameObjectWithTag("Finish").GetComponent<SplineController>();
        if (sc.mState.CompareTo("Stopped") == 0)
        {
            sc.mState = "Once";
            UI_PAUSE.SetActive(false);
        }
        else { 
            sc.mState = "Stopped";
            UI_PAUSE.SetActive(true);
        }
    }
    void UIUpdate() {
        /// #HP
        HP_UI_Script.SetValue((int)(_ParticleControllerScript.HP * 100));
        /// #COMBO
        int combo = _ParticleControllerScript.COMBO;
        if (Combo < combo)
        {
            Combo = combo;
            COMBO_UI_Script.SetValueTime(Combo);
        }
        else if (combo == 0)
            Combo = combo;
        /// #GUAGE
        int guage = (int)(_ParticleControllerScript.GAUAGE * 5f);
        for (int index = 0; index < 6; index++)
        {
            if (index <= guage)
                UIGUAGE[index].SetActive(true);
            else
                UIGUAGE[index].SetActive(false);
        }
    }
    Vector3 ControllerForce = Vector3.zero;
    void Update() {
        /// UI
        UIUpdate();
        /// END
        /// TouchButton Controller
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
            SoundEffectControl.Instance.PlayEffectSound("Slide main", 1f);
            Vector2 cur_position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out cur_position);
            touchButton_bg.GetComponent<RectTransform>().position = myCanvas.transform.TransformPoint(cur_position);
            touchButton_action.GetComponent<RectTransform>().position = myCanvas.transform.TransformPoint(cur_position);

            if (Vector2.Distance(first_position, cur_position) > _deadZoneRadius)
            {
                //call particleController
                ControllerForce = Vector3.MoveTowards(
                Vector3.zero,
                (cur_position - first_position).normalized,
                1f);
                //Debug.Log("call1");
            }
            _mouseState = MouseStatus.OnUp;
            touchButton_bg.SetActive(false);
            touchButton_action.SetActive(false);
        }
        /// End
        }
        void AsteroidMake() {
        GameObject asteroidManager = new GameObject("asteroidManager");
        GameObject asteroid;
        for (int index = 0; index < Asteroid_num; index++)
        {
            switch (Random.Range(0, 3))
            { 
                case 0:
                    asteroid = Instantiate((GameObject)Resources.Load("Prefabs/Asteroid_01", typeof(GameObject)));
                    break;
                case 1:
                    asteroid = Instantiate((GameObject)Resources.Load("Prefabs/Asteroid_02", typeof(GameObject)));
                    break;
                default:
                    asteroid = Instantiate((GameObject)Resources.Load("Prefabs/Asteroid_03", typeof(GameObject)));
                    break;
            }
            asteroid.transform.parent = asteroidManager.transform;
            StartCoroutine(AsteroidMove(asteroid));
        }
    }
    void FixedUpdate() 
    {
        if (ControllerForce != Vector3.zero) { 
            //Debug.Log("call2");
            _ParticleControllerScript.Action(ControllerForce);
            ControllerForce = Vector3.zero;
        }
    }
    Vector3 AsteroidRandomPostion()
    {
        return GameObject.FindGameObjectWithTag("Player").transform.position
            + new Vector3(Random.Range(50f, 100f)*((Random.Range(0,2)==0) ? 1f : -1f),
                Random.Range(50f, 100f) * ((Random.Range(0, 2) == 0) ? 1f : -1f),
                Random.Range(50f, 100f) * ((Random.Range(0, 2) == 0) ? 1f : -1f));
    }
    IEnumerator AsteroidMove(GameObject asteroid) {
        asteroid.transform.position = AsteroidRandomPostion();
        Vector3 range = (GameObject.FindGameObjectWithTag("Player").transform.position - asteroid.transform.position).normalized;

        while (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, asteroid.transform.position) < 500f)
        {
            float asteroidSpeed = 50f;
            asteroid.transform.position += range * asteroidSpeed * Time.deltaTime;
            yield return null;
        }
        StartCoroutine(AsteroidMove(asteroid));
    }
}
