using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;
public class DataLoader : MonoBehaviour {

    /* #데이터 읽기
     * 데이터를 읽기 위해서는 TagName으로 pathManager를 가진 객체 하위에 빈 오브젝트를 생성한다.
     * 예시 : 
     * GameObject pathManager = new GameObject("pathManager");
     * pathManager.tag = "pathManager";
     * DataLoader.Load2Dat("text_data\\vectors");
     */
    public static void Load2Dat(string fileName) 
    {
        string data = string.Empty;
        try {
            TextAsset _txtFile = (TextAsset)Resources.Load(fileName);
            data = _txtFile.text;
        }
        catch (Exception e) { Debug.Log(e); }

        // 주의: Data로 정보가 읽혀지지 않은 경우 실패함
        if (string.IsNullOrEmpty(data) || data.CompareTo("") == 0)
            return;

        GameObject parentGameObject = GameObject.FindGameObjectWithTag("pathManager");
        // 주의: Tag로 pathManager가 있는 게임오브젝트가 없으면 실패함
        if (parentGameObject == null)
            return;

        char[] patterns = { '(', ')', ',', '\r', '\n' };
        string[] splitData = data.Split(patterns, System.StringSplitOptions.RemoveEmptyEntries);

        GameObject[] _path = new GameObject[(splitData.Length / 3)];

        for (int index = 0, arrayIndex = 0; index < splitData.Length; index += 3)
        {
            Vector3 temp_vector3Set = new Vector3(
                float.Parse(splitData[index]),
                float.Parse(splitData[index + 1]),
                float.Parse(splitData[index + 2]));
            _path[arrayIndex] = new GameObject("pos_object");
            _path[arrayIndex].transform.position = temp_vector3Set;
            _path[arrayIndex++].transform.parent = parentGameObject.transform;
        }
    }
    /* #데이터 저장
     * 데이터를 저장하기 위해서는 데이터를 가지는 부모객체를 생성하여 설정하거나
     * TagName으로 pathManager를 가진 객체를 설정한다.
     * 예시 : DataLoader.Save2Dat("Assets\\text_data\\vectors.txt", saveParentGameObject);
     */
    public static void Save2Dat(string fileName,GameObject pGameObject)
    {
        string line = string.Empty;

        Transform[] saveGameObjects = pGameObject.GetComponentsInChildren<Transform>();

        foreach (Transform obj in saveGameObjects)
        {
            line += obj.position.ToString();
            line += "\r\n";
        }
        try{
            File.WriteAllText(fileName, line);
        }
        catch (Exception e) { Debug.Log(e); }
    }
}
