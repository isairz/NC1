using UnityEngine;
using System.Collections;

public class ExplotionParticle : MonoBehaviour
{
    public enum explotionName { cube = 0, sphere = 1 }
    // particle_number
    public int particle_num = 8;
    public float exlpotion_speed = 100f;
    public explotionName particle_name;

    // force(intType)
    public int trigger_force = 9;
    // 
    public float trigger_time = 7f;

    private new Transform transform;
    void Awake()
    {
        transform = GetComponent<Transform>();
        prepareObject();
    }
    void prepareObject()
    {
        // scale 0.5
        // position random
        // number(parameter)
        GameObject[] Child_GameObjects = new GameObject[particle_num];

        Vector3 newScale = transform.localScale * 0.5f;
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
        Debug.Log(other.tag);
        //enter the player
        if (other.tag.CompareTo("Player") == 0)
        {
            //get player_force(particle_num)
            int force_power = 10;//= other.GetComponent<scriptName>().particle_num;
            if (force_power >= trigger_force)
            {
                OnExplotion(other.transform.position);//onExplotion
            }
            else
            {
                ;//notExplotion
            }
            transform.GetComponent<BoxCollider>().isTrigger = false;
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
