using UnityEngine;
using System.Collections;

public class _2DEffecterTrigger : MonoBehaviour {

    private bool isCrash = false;
    void Awake() {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if (!isCrash)
        {
            isCrash = true;
            foreach (Transform child in transform)
                child.gameObject.SetActive(true);
        }
    }
}
