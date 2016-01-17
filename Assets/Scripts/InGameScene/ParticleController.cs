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
    private float _particle_percentage;
    public float Force { get { return _particle_percentage; } }
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
                    ++_particlesNumber;
                    ++index;
                    child.gameObject.SetActive(true);
                    child.position = Position;
                    // Animation Speed 1f;
                    StartCoroutine(ApearParticle(child.gameObject, 1f));
                }
            }
            else
                break;
        }
        Debug.Log("add result: " + _particlesNumber);
    }
    public void DeleteParticle()
    {
        int index = 0;
        int delete_number = (int)(_particlesNumber * 0.3);
        foreach (Transform child in transform)
        {
            if (delete_number > index)
            {
                if (child.gameObject.activeSelf && child.gameObject.CompareTag("Living"))
                {
                    --_particlesNumber;
                    ++index;
                    // Animation Speed 2f;
                    StartCoroutine(DispearParticle(child.gameObject, 2f));
                }
            }
            else
                break;
        }
        Debug.Log("delete result: " + _particlesNumber);
    }
    IEnumerator ApearParticle(GameObject particle, float time)
    {
        Color c = new Color(1f, 0f, 0f);
        particle.GetComponent<MeshRenderer>().material.color = c;
        particle.tag = "Apearing";
        float startTime = 0f;
        Vector3 range = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        range.Normalize();

        while (time > startTime)
        {
            startTime += Time.deltaTime;
            particle.GetComponent<Rigidbody>().AddForce(range, ForceMode.Impulse);
            yield return null;
        }
        particle.tag = "Living";
        c = new Color(0f, 1f, 0f);
        particle.GetComponent<MeshRenderer>().material.color = c;
    }
    IEnumerator DispearParticle(GameObject particle, float time)
    {
        Color c = new Color(0f, 0f, 1f);
        particle.GetComponent<MeshRenderer>().material.color = c;
        particle.tag = "Disapearing";
        float startTime = 0f;
        Vector3 range = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        range.Normalize();

        while (time > startTime)
        {
            startTime += Time.deltaTime;
            particle.GetComponent<Rigidbody>().AddForce(range, ForceMode.Impulse);
            yield return null;
        }
        particle.SetActive(false);
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
    void FixedUpdate()
    {
        bool boom = Input.GetMouseButton(0);
        Vector3 massPoint = new Vector3();

        //int count = 0;
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
        //Debug.Log("count:" + count + "/total: " + _particlesNumber);
        massPoint /= _particlesNumber;

        // massPoint > Sphere
        // 충돌 이슈!
        //_sphereCollider.center = massPoint;
        //Debug.Log(_sphereCollider.transform.position);
        _particle_percentage = 0;

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
            // num' particle > powerW
            // get percentage ( 0 ~ 1 )
            _particle_percentage = _particle_percentage / _particlesNumber;
        }
    }
    public void Action(Vector3 ControllerForce)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf && child.gameObject.CompareTag("Living"))
            {
                Rigidbody rig = child.GetComponent<Rigidbody>();
                rig.AddForce(ControllerForce * 50f, ForceMode.Impulse);
            }
        }
    }
}
