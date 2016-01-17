using UnityEngine;
using System.Collections;

public class ExplotionParticle : MonoBehaviour
{
    public enum explotionName { cube = 0, sphere = 1, _default = 2}
    // particle_number
    public int particle_num = 8;
    public float exlpotion_speed = 100f;
    public explotionName particle_name;
    public GameObject originalParticle = null;
    // force(intType)
    private float trigger_force = 0.8f;
    // 
    public float trigger_time = 7f;
    private bool _isCrash = false;

    private new Transform transform;
    private InGameSceneManager _inGameSceneManagerScript;
    void Awake()
    {
        transform = GetComponent<Transform>();
        _inGameSceneManagerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<InGameSceneManager>();
        prepareObject();
    }
    void prepareObject()
    {
        // scale 0.5
        // position random
        // number(parameter)
        GameObject[] Child_GameObjects = new GameObject[particle_num];

        Vector3 newScale = transform.localScale / particle_num;
        Bounds bounds = transform.GetComponent<Renderer>().bounds;
        Vector3 min = bounds.min, max = bounds.max;

        for (int index = 0; index < particle_num; index++)
        {
            Vector3 newPosition = new Vector3(
           Random.Range(min.x, max.x),
           Random.Range(min.y, max.y),
           Random.Range(min.z, max.z));

            GameObject Child_GameObject = null;

            if(particle_name == explotionName.cube)
                Child_GameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            else if(particle_name == explotionName.sphere)
                Child_GameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            else if(particle_name == explotionName._default)
                Child_GameObject = Instantiate(originalParticle);

            Child_GameObject.transform.parent = transform;
            Child_GameObject.transform.position = newPosition;
            Child_GameObject.transform.localScale = newScale;

            Child_GameObject.AddComponent<Rigidbody>();
            Child_GameObject.GetComponent<Rigidbody>().useGravity = false;
            Child_GameObject.GetComponent<Collider>().enabled = false;
            Child_GameObject.SetActive(false);

            Child_GameObjects[index] = Child_GameObject;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        //enter the player
        if (other.CompareTag("Living") && !_isCrash)
        {
            _isCrash = true;
            transform.GetComponent<Collider>().isTrigger = false;

            //get player_force(particle_num)
            float force_power = other.GetComponentInParent<ParticleController>().Force;
            if (force_power >= trigger_force)
            {
                Debug.Log("Explotion :" + transform.gameObject.name);
                //onExplotion
                OnExplotion(other.transform.position);
                //makeParticle
                other.GetComponentInParent<ParticleController>().AddParticle(transform.position);
            }
            else
            {
                Debug.Log("NotExplotion :"+transform.gameObject.name);
                //notExplotion
                //life delete
                _inGameSceneManagerScript.DecreaseLife();
                //deleteParticle
                other.GetComponentInParent<ParticleController>().DeleteParticle();
            }
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
