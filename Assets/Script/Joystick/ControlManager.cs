using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class ControlManager : MonoBehaviour {

	void Update () {
        // 좌우
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        // 상하
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        // 버튼
        bool  b = CrossPlatformInputManager.GetButton("Action");
        // 확인
        Debug.Log(h.ToString() + "/" + v.ToString() + "/" + b.ToString());
	}
}
