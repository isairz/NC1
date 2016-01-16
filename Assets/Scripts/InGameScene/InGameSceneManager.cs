using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameSceneManager : MonoBehaviour{

    public GameObject[] LifeUIObjects;
    public int LifeValue;

    void Awake() {
        SetLife(3);
    }
    public void DecreaseLife() 
    {
        --LifeValue;
        UpdateLifeUI();
    }
    public void SetLife(int lifeNum)
    {
        LifeValue = lifeNum;
        UpdateLifeUI();
    }
    private void UpdateLifeUI() 
    {
        for (int index = 0; index < LifeUIObjects.Length; index++)
        {
            if (index < LifeValue)
                LifeUIObjects[index].SetActive(true);
            else
                LifeUIObjects[index].SetActive(false);
        }

    }
}
