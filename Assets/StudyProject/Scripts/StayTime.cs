using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class StayTime : MonoBehaviour
{

    public string fileName;
    public GameObject GuideLeft;
    public GameObject GuideRight;
    private TextAsset csvFile; // CSVファイル
    private List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト
    private int height = 0; // CSVの行数
    private float stayTimeLeft = 0;
    private float stayTimeRight = 0;
    private Vector3 left;
    private Vector3 right;
    private float distanceLeft;
    private float distanceRight;
    private float stayDistance = 0.0625f;

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
        for (i = 0; i < csvDatas.Count; i++)
        {
            //Debug.Log(csvDatas[i][0]);
            left = new Vector3(float.Parse(csvDatas[i][1]), float.Parse(csvDatas[i][2]), float.Parse(csvDatas[i][3]));
            right = new Vector3(float.Parse(csvDatas[i][4]), float.Parse(csvDatas[i][5]), float.Parse(csvDatas[i][6]));
            distanceLeft = (left - GuideLeft.transform.position).sqrMagnitude;
            distanceRight = (right - GuideRight.transform.position).sqrMagnitude;
            if(distanceLeft < stayDistance)
            {
                if (i == 0)
                {
                    stayTimeLeft += float.Parse(csvDatas[i][0]);
                }
                else
                {
                    stayTimeLeft += float.Parse(csvDatas[i][0]) - float.Parse(csvDatas[i - 1][0]);                    
                }
            }
            //Debug.Log("distanceLeft" + distanceLeft);
            if (distanceRight < stayDistance)
            {
                if (i == 0)
                {
                    stayTimeRight += float.Parse(csvDatas[i][0]);
                }
                else
                {
                    stayTimeRight += float.Parse(csvDatas[i][0]) - float.Parse(csvDatas[i - 1][0]);
                }
            }
            //Debug.Log("distanceRight" + distanceRight);
        }

        StreamWriter sw;
        FileInfo fi;
        fi = new FileInfo(Application.dataPath + "/Resources/StayTimeLog.csv");
        sw = fi.AppendText();
        sw.WriteLine(stayTimeLeft + "," + stayTimeRight);
        sw.Flush();
        sw.Close();

        Debug.Log("StayTime = (" + stayTimeLeft + "," + stayTimeRight + ")");
    }

    // Update is called once per frame
    void Update()
    {

    }
}