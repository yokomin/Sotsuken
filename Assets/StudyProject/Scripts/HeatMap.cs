using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class HeatMap : MonoBehaviour {

    public string fileName;
    public GameObject objLeft;
    public GameObject objRight;
    private TextAsset csvFile; // CSVファイル
    private List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト
    private int height = 0; // CSVの行数

    void Start()
    {
        csvFile = Resources.Load(fileName) as TextAsset;
        StringReader reader = new StringReader(csvFile.text);

        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(',')); // リストに入れる
            height++; // 行数加算
        }

        int i;
        for(i = 0; i < csvDatas.Count; i++)
        {
            //Debug.Log(csvDatas[i][0]);
            Instantiate(objLeft, new Vector3(float.Parse(csvDatas[i][1]), float.Parse(csvDatas[i][2]), float.Parse(csvDatas[i][3])), Quaternion.identity);
            Instantiate(objRight, new Vector3(float.Parse(csvDatas[i][4]), float.Parse(csvDatas[i][5]), float.Parse(csvDatas[i][6])), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
