using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class CSVLoader : MonoBehaviour {

    /* #csv 데이터 읽는 함수
     * csv에서 데이터를 읽어와 json형식으로 생성하고 dictionary로 반환함
     * Key값은 data의 고유 Alias이며 Value값은 Json임
     * Json 값을 검색하는 함수는 아래를 참조함
     * 
     * Dictionary<int, string> dic = CSVLoader.Load2Json("text_data\\data");
     * Debug.Log(CSVLoader.GetJsonData(dic, 1000, "NumberofParticle"));
     */
    public static Dictionary<int, string> Load2Json(string fileName)
    {
        string data = string.Empty;
        try
        {
             TextAsset _txtFile = (TextAsset)Resources.Load(fileName);
             data = _txtFile.text;
             if (string.IsNullOrEmpty(data) || data.CompareTo("") == 0)
                 return null;
        }
        catch (Exception e) { Debug.Log(e); }

        char[] patterns = { '\n', '\r' };
        string[] datalist = data.Split(patterns, System.StringSplitOptions.RemoveEmptyEntries);
        string[] nameData = datalist[0].Split(',');

        Dictionary<int, string> dic = new Dictionary<int, string>();
        for(int index = 1; index<datalist.Length;index++)
        { 
            string[] lineData = datalist[index].Split(',');
            string resultData = string.Empty;

            for(int jsonIndex=1;jsonIndex<lineData.Length;jsonIndex++)
                resultData += "{" + nameData[jsonIndex] + ":" + lineData[jsonIndex] + "},";

            dic.Add(int.Parse(lineData[0]), resultData);
        }
        return dic;
    }
    /* #JsonData를 읽어오는 함수
     * Input Data  : 사전 변수, 고유키, 데이터 이름
     * Output Data : 데이터가 존재하면(string) 없다면(empty)
     */
    public static string GetJsonData(Dictionary<int, string> dic, int Key, string dataName)
    {
        if (!dic.ContainsKey(Key))
            return string.Empty;

        string dic_data = dic[Key];

        int startIndex = dic_data.IndexOf(dataName + ":");
        startIndex += dataName.Length + 1;
        int endIndex = dic_data.IndexOf('}', startIndex);

        if (startIndex == -1 || endIndex == -1)
            return string.Empty;

        return dic_data.Substring(startIndex, endIndex - startIndex);
    }
}
