using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ArriveTime : MonoBehaviour
{
    public string fileName;
    public GameObject GuideLeft;
    public GameObject GuideRight;
    public float startTime = 1f;
    public float transitionTime = 0.5f;
    private TextAsset csvFile; // CSVファイル
    private List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト
    private int height = 0; // CSVの行数
    private Vector3 left;
    private Vector3 right;
    private float distanceLeft;
    private float distanceRight;
    private float stayTimeLeft = 0;
    private float stayTimeRight = 0;
    private bool transitionLeft;
    private bool transitionRight;
    private float stayDistance = 0.04f;

    void Start()
    {
        stayTimeLeft = 0f;
        stayTimeRight = 0f;
        transitionLeft = false;
        transitionRight = false;

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
            if (float.Parse(csvDatas[i][0]) < startTime)
            {
                continue;
            }

            //Debug.Log(csvDatas[i][0]);
            left = new Vector3(float.Parse(csvDatas[i][1]), float.Parse(csvDatas[i][2]), float.Parse(csvDatas[i][3]));
            right = new Vector3(float.Parse(csvDatas[i][4]), float.Parse(csvDatas[i][5]), float.Parse(csvDatas[i][6]));
            distanceLeft = (GuideLeft.transform.position - left).sqrMagnitude;
            if (stayTimeLeft > 0f && distanceLeft > stayDistance)
            {
                stayTimeLeft = 0f;
            }
            else if (distanceLeft < stayDistance)
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

            if (stayTimeLeft > transitionTime && !transitionLeft)
            {
                transitionLeft = true;
            }

            distanceRight = (GuideRight.transform.position - right).sqrMagnitude;
            if (stayTimeRight > 0f && distanceRight > stayDistance)
            {
                stayTimeRight = 0f;
            }
            else if (distanceRight < stayDistance)
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

            if (stayTimeRight > transitionTime && !transitionRight)
            {
                transitionRight = true;
            }

            if (transitionLeft && transitionRight)
            {
                StreamWriter sw;
                FileInfo fi;
                fi = new FileInfo(Application.dataPath + "/Resources/ArriveTimeLog.csv");
                sw = fi.AppendText();
                sw.WriteLine(csvDatas[i][0]);
                sw.Flush();
                sw.Close();
                break;
            }
        }
        Debug.Log("ArriveTime = " + transitionLeft + transitionRight);

        //Debug.Log("ArriveTime = " + csvDatas[i][0]);
    }

    // Update is called once per frame
    void Update()
    {

    }
}