using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleController : MonoBehaviour
{
    /* Particle TagType { Living, Disapearing, Apearing } */
    public Transform particleBase;
    public int Maximum_particles = 40;
    public int _particlesNumber = 20;

    public float collectSpeed = 1f;
    public float spreadSpeed = 10f;

    protected new Transform transform;

    private SphereCollider _sphereCollider;
    private float particle_radius;
    private bool _isOn_curParticleState, _isOn_prevParticleState;
    private float _gauge = 0.3f;//
    private bool _piverTime = false;
    private int _combo = 0;
    public bool ISPEVER { get { return _piverTime; } }
    public float GAUAGE { get { return _gauge; } set { if (!_piverTime) _gauge = value; } }
    public float HP { get { return (float)_particlesNumber / (float)Maximum_particles; } }
    public bool Force { get { return _isOn_curParticleState; } }
    public int COMBO { get { return _combo; } set { _combo = value; } }
    // Use this for initialization
    void Awake()
    {
        transform = GetComponent<Transform>();
        for (int i = 0; i < Maximum_particles; i++)
        {
            MakeParticle(new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f)));
        }
        //off particle
        int index = 0;
        foreach (Transform child in transform)
        {
            if ((Maximum_particles - _particlesNumber) > index)
            {
                ++index;
                child.gameObject.SetActive(false);
            }
            else
                break;
        }
        // make SpereCheck Collider
        _sphereCollider = GetComponent<SphereCollider>();
    }
    void Start()
    {
        particle_radius = transform.GetChild(0).GetComponent<SphereCollider>().radius
            * transform.GetChild(0).transform.localScale.x;
    }
    public void AddParticle(Vector3 Position)
    {
        int add_number = (int)(_particlesNumber * 0.2);
        int index = 0;
        foreach (Transform child in transform)
        {
            if (add_number > index)
            {
                if (!child.gameObject.activeSelf)
                {
                    ++index;
                    child.gameObject.SetActive(true);
                    child.position = transform.position;//Position;
                    // Animation Speed 1f;
                    StartCoroutine(ApearParticle(child.gameObject, 0.5f));
                }
            }
            else
                break;
        }
    }
    public void DeleteParticle()
    {
        if (!_piverTime)
        {
            int index = 0;
            int delete_number = (int)(_particlesNumber * 0.3);
            if (_particlesNumber - delete_number <= 5)
            {
                // GAME OVER

            }
            else
            {
                foreach (Transform child in transform)
                {
                    if (delete_number > index)
                    {
                        if (child.gameObject.activeSelf && child.gameObject.CompareTag("Living"))
                        {
                            ++index;
                            // Animation Speed 2f;
                            StartCoroutine(DispearParticle(child.gameObject, 2f));
                        }
                    }
                    else
                        break;
                }
            }
        }
    }
    IEnumerator ApearParticle(GameObject particle, float time)
    {
        // TODO
        /*
        particle.tag = "Apearing";
        
        float startTime = 0f;
        
        Vector3 range = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        range.Normalize();

        while (time > startTime)
        {
            startTime += Time.deltaTime;
            particle.GetComponent<Rigidbody>().AddForce(range, ForceMode.Impulse);
            yield return null;
        }
         */
        yield return null;// ADD
        particle.tag = "Living";
    }
    IEnumerator DispearParticle(GameObject particle, float time)
    {
        particle.tag = "Disapearing";
        float startTime = 0f;
        Vector3 range = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        range.Normalize();

        while (time > startTime)
        {
            startTime += Time.deltaTime;
            Rigidbody rig = particle.GetComponent<Rigidbody>();
            rig.AddForce(range * rig.mass, ForceMode.Impulse);
            yield return null;
        }
        particle.SetActive(false);
    }
    IEnumerator PiverTime(float time)
    {
        SoundEffectControl.Instance.PlayEffectSound("Slide succes sound", 0.1f);
        float startTime = 0f;

        InGameSceneManager GCScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<InGameSceneManager>();
        GCScript.StartFeverUI(true);

        _piverTime = true;
        while (time > startTime)
        {
            startTime += Time.deltaTime;
            yield return null;
        }
        GCScript.StartFeverUI(false);
        _piverTime = false;
        // reset default energy
        _gauge = 0.3f;
    }
    private void MakeParticle(Vector3 Position)
    {
        Transform newParticle = Instantiate(particleBase);
        newParticle.position = Position;
        newParticle.parent = transform;
        newParticle.gameObject.tag = "Living";
        //Color c = new Color(0f, 1f, 0f);
        //newParticle.GetComponent<MeshRenderer>().material.color = c;
    }
    // Update is called once per frame
    private float rechargineTime = 0f, currentTime = 0f;
    void FixedUpdate()
    {
        bool boom = !Input.GetMouseButton(0);
        Vector3 massPoint = new Vector3();

        ///Enegy
        if (Mathf.Abs(rechargineTime) >= 0.001f)
        {
            currentTime += Time.deltaTime;
            if (currentTime > rechargineTime)
            {
                rechargineTime = 0f;
                currentTime = 0f;
            }
            else
                boom = true;
        }
        else if (GAUAGE <= 0f)
            rechargineTime = 3f;
        // invincibility Time
        else if (GAUAGE >= 1f)
        {
            // set piverTime 10f
            StartCoroutine(PiverTime(10f));
        }

        if (boom)
            ChangeParticleEnergy(true, (GAUAGE >= 1f) ? 1f : GAUAGE + 0.01f * Time.deltaTime);
        else
            ChangeParticleEnergy(true, (GAUAGE <= 0f) ? 0f : GAUAGE - 0.01f * Time.deltaTime);
        //Debug.Log(_gauge);
        ///End
        int massCounter = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf && child.gameObject.CompareTag("Living"))
            {
                // child.RotateAround (transform.position, new Vector3 (0, 1f, 0), Time.deltaTime * 10f);
                Rigidbody rig = child.GetComponent<Rigidbody>();
                Vector3 toCenter = (transform.position - child.position);
                Vector3 dir = toCenter.normalized;
                float dist = toCenter.magnitude;
                /*if (dist < 0.5f)
                    return;*/
                rig.AddForce(dir * dist * dist * collectSpeed, ForceMode.Acceleration);
                massPoint += child.position;
                ++massCounter;
            }
        }
        _particlesNumber = massCounter;
        massPoint /= _particlesNumber;
        // Translate collider
        _sphereCollider.center = massPoint - transform.position;
        // Size collider < o'radius * root(o'num) >

        _sphereCollider.radius = particle_radius * Mathf.Sqrt(_particlesNumber);
        int _particle_percentage = 0;

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf && child.gameObject.CompareTag("Living"))
            {
                // boxIn childs
                if (_sphereCollider.bounds.Contains(child.transform.position))
                    ++_particle_percentage;

                Rigidbody rig = child.GetComponent<Rigidbody>();
                Vector3 toMass = (massPoint - child.position);
                Vector3 dir = toMass.normalized;
                float dist = toMass.magnitude;
                if (boom)
                {
                    rig.AddForce(-dir * dist * spreadSpeed, ForceMode.Acceleration);
                }
            }
        }
        // num' particle > get percentage ( 0 ~ 1 )
        // trigger force
        _isOn_curParticleState = (((float)_particle_percentage / (float)_particlesNumber) >= 0.8f) ? true : false;

        if (_isOn_prevParticleState != _isOn_curParticleState)
        {
            if (_isOn_curParticleState)
                SoundEffectControl.Instance.PlayEffectSound("Power", 1f);
            // _isOn_curParticleState
            // ChangeParticleTriggerState(_isOn_curParticleState);
            //Debug.Log(_isOn_curParticleState);
        }

        _isOn_prevParticleState = _isOn_curParticleState;
    }
    public void Action(Vector3 ControllerForce)
    {
        Vector3 range = GameObject.FindGameObjectWithTag("MainCamera").
            GetComponent<Camera>().cameraToWorldMatrix * ControllerForce;
        range.Normalize();

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf && child.gameObject.CompareTag("Living"))
            {
                Rigidbody rig = child.GetComponent<Rigidbody>();
                rig.AddForce(range * 200f * rig.mass, ForceMode.Impulse);
            }
        }
    }
    public void ChangeParticleEnergy(bool equalType, float value)
    {
        if (equalType)
            GAUAGE = value;
        else
            GAUAGE += value;
    }

}
