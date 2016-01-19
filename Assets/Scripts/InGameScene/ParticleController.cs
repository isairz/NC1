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
    private bool _isOn_curParticleState, _isOn_prevParticleState;
    private float _gauge = 0.3f;
    private float _power = 1f;
    private bool _piverTime = false;
    private int _combo = 0;
    public float GAUAGE { get { return _gauge; } set { if (!_piverTime) _gauge = value; } }
    public int ParticlesNumber { get { return _particlesNumber; } set { if (!_piverTime) _particlesNumber = value; } }
    public float HP { get { return (float)ParticlesNumber / (float)Maximum_particles; } }
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
            if ((Maximum_particles - ParticlesNumber) > index)
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
    public void AddParticle(Vector3 Position)
    {
        int add_number = (int)(ParticlesNumber * 0.2);
        int index = 0;
        foreach (Transform child in transform)
        {
            if (add_number > index)
            {
                if (!child.gameObject.activeSelf)
                {
                    ++ParticlesNumber;
                    ++index;
                    child.gameObject.SetActive(true);
                    child.position = Position;
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
        int index = 0;
        int delete_number = (int)(ParticlesNumber * 0.3);
        foreach (Transform child in transform)
        {
            if (delete_number > index)
            {
                if (child.gameObject.activeSelf && child.gameObject.CompareTag("Living"))
                {
                    --ParticlesNumber;
                    ++index;
                    // Animation Speed 2f;
                    StartCoroutine(DispearParticle(child.gameObject, 2f));
                }
            }
            else
                break;
        }
    }
    IEnumerator ApearParticle(GameObject particle, float time)
    {
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
            particle.GetComponent<Rigidbody>().AddForce(range, ForceMode.Impulse);
            yield return null;
        }
        particle.SetActive(false);
    }
    IEnumerator PiverTime(float time) {
        float startTime = 0f;

        _piverTime = true;
        while (time > startTime)
        {
            startTime += Time.deltaTime;
            yield return null;
        }
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
        Color c = new Color(0f, 1f, 0f);
        newParticle.GetComponent<MeshRenderer>().material.color = c;
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
            }
        }

        massPoint /= ParticlesNumber;
        _sphereCollider.center = massPoint - transform.position;

        // massPoint > Sphere
        // 충돌 이슈!
        //_sphereCollider.center = massPoint;
        //Debug.Log(_sphereCollider.transform.position);
        float _particle_percentage = 0;

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
        _power = _particle_percentage / ParticlesNumber;
        // trigger force
        _isOn_curParticleState = (_power >= 0.8f) ? true : false;

        if (_isOn_prevParticleState != _isOn_curParticleState)
        {
            // _isOn_curParticleState
            ChangeParticleTriggerState(_isOn_curParticleState);
        }
        
        _isOn_prevParticleState = _isOn_curParticleState;
    }
    public void ChangeParticleTriggerState(bool state)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf && child.gameObject.CompareTag("Living"))
                child.GetComponent<Collider>().isTrigger = state;
        }
    }
    public void Action(Vector3 ControllerForce)
    {
        Vector3 range = GameObject.FindGameObjectWithTag("MainCamera").
            GetComponent<Camera>().cameraToWorldMatrix * ControllerForce;

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf && child.gameObject.CompareTag("Living"))
            {
                Rigidbody rig = child.GetComponent<Rigidbody>();
                rig.AddForce(range * 50f, ForceMode.Impulse);
            }
        }
    }
    public void ChangeParticleEnergy(bool equalType, float value) {
        if (equalType)
            GAUAGE = value;
        else
            GAUAGE += value;
    }

}
