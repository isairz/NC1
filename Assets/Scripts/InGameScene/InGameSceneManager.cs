using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameSceneManager : MonoBehaviour{

    public GameObject[] LifeUIObjects;
    private int _lifeValue;

    void Awake() {
        SetLife(LifeUIObjects.Length);
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
}
