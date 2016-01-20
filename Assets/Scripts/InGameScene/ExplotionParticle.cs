using UnityEngine;
using System.Collections;

public class ExplotionParticle : MonoBehaviour
{
    private enum CrashState { isDefault = 0, isCrash = 1, isNotCrash = 2, isEscape = 3, isEnd = 4 }
    public enum explotionName { cube = 0, sphere = 1, _default = 2}
    // particle_number
    public int particle_num = 8;
    public float exlpotion_speed = 100f;
    public explotionName particle_name;
    public GameObject originalParticle = null;
    public float trigger_time = 7f;
    private CrashState _crashState = CrashState.isDefault;
    public bool isCrash = false;

    private new Transform transform;
    private ParticleController _particleControllerScript;
    void Awake()
    {
        transform = GetComponent<Transform>();
        _particleControllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<ParticleController>();
        prepareObject();
    }
    void prepareObject()
    {
        // scale 0.5
        // position random
        // number(parameter)
        GameObject[] Child_GameObjects = new GameObject[particle_num];

        Vector3 newScale = (transform.localScale / particle_num) * 0.5f;
        Bounds bounds = transform.GetComponent<Renderer>().bounds;
        Vector3 min = bounds.min, max = bounds.max;

        for (int index = 0; index < particle_num; index++)
        {
            Vector3 newPosition = new Vector3(
           Random.Range(min.x, max.x),
           Random.Range(min.y, max.y),
           Random.Range(min.z, max.z));

            GameObject Child_GameObject = null;

            if (particle_name == explotionName.cube)
            {
                Child_GameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Child_GameObject.AddComponent<Rigidbody>();
                Child_GameObject.GetComponent<Rigidbody>().useGravity = false;
                Child_GameObject.GetComponent<Collider>().enabled = false;
            }
            else if (particle_name == explotionName.sphere)
            {
                Child_GameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Child_GameObject.AddComponent<Rigidbody>();
                Child_GameObject.GetComponent<Rigidbody>().useGravity = false;
                Child_GameObject.GetComponent<Collider>().enabled = false;
            }
            else if (particle_name == explotionName._default)
                Child_GameObject = Instantiate(originalParticle);

            Child_GameObject.transform.parent = transform;
            Child_GameObject.transform.position = newPosition;
            Child_GameObject.transform.localScale = newScale;

            Child_GameObject.SetActive(false);

            Child_GameObjects[index] = Child_GameObject;
        }
    }
    void OnTriggerStay(Collider other)
    {
        //enter the player
        if (other.CompareTag("Living"))
        {
            if (_crashState == CrashState.isDefault && Vector3.Distance(other.transform.position, transform.position)
                <= (transform.GetComponent<SphereCollider>().radius * transform.localScale.x 
                + other.GetComponent<SphereCollider>().radius * other.transform.localScale.x) * 1.5f)
            {
                transform.GetComponent<SphereCollider>().isTrigger = false;
                //get player_force(particle_num)
                if (isCrash && other.GetComponentInParent<ParticleController>().Force)
                {
                    isCrash = false;
                    _crashState = CrashState.isCrash;
                    //Debug.Log("Explotion!");
                    //onExplotion
                    OnExplotion(other.transform.position);
                    //makeParticle
                    other.GetComponentInParent<ParticleController>().AddParticle(transform.position);
                    //get COMBO
                    ++_particleControllerScript.COMBO;
                    //getGAUAGE
                    _particleControllerScript.ChangeParticleEnergy(false, 0.1f);
                    //SoundEffect
                    SoundEffectControl.Instance.PlayEffectSound("BOIDS sound", 0.5f);
                }
                else
                {
                    _crashState = CrashState.isNotCrash;
                    //Debug.Log("NotExplotion!");
                    //notExplotion
                    // set COMBO
                    _particleControllerScript.COMBO = 0;
                    // miss UI
                    GameObject.FindGameObjectWithTag("GameController").GetComponent<InGameSceneManager>().StartMissUI();
                    //deleteParticle
                    other.GetComponentInParent<ParticleController>().DeleteParticle();
                    //TODO
                    //SoundEffect
                    SoundEffectControl.Instance.PlayEffectSound("Slide Fail", 1.0f);
                }
            }
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Living") && _crashState == CrashState.isDefault)
        {
            // escape
            //Debug.Log("Escape!");
            _crashState = CrashState.isEnd;
            transform.GetComponent<BoxCollider>().isTrigger = false;
            //get COMBO
            ++_particleControllerScript.COMBO;
            //get GAUAGE
            _particleControllerScript.ChangeParticleEnergy(false, 0.1f);
            SoundEffectControl.Instance.PlayEffectSound("Slide succes", 1.0f);
        }
    }
    void OnExplotion(Vector3 src_position)
    {
        int child_num = transform.childCount;
        for (int index = 0; index < child_num; index++)
        {
            Transform child_transform = transform.GetChild(index);
            GameObject child_gameobject = child_transform.gameObject;
            child_gameobject.SetActive(true);
            child_gameobject.GetComponent<Rigidbody>().AddForce(
                (child_gameobject.transform.position - src_position) * exlpotion_speed,
                ForceMode.Impulse);
            StartCoroutine(disapear_child(child_gameobject, trigger_time));
        }
        transform.GetComponent<MeshRenderer>().enabled = false;
    }
     IEnumerator disapear_child(GameObject child, float limit_time)
    {
        float currentTime = 0f;
        Vector3 rotateAxis = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        rotateAxis.Normalize();
        while (limit_time > currentTime)
        {
            child.transform.Rotate(rotateAxis * 360f * Time.deltaTime);
            currentTime += Time.deltaTime;
            yield return null;
        }
        child.SetActive(false);
    }
}
