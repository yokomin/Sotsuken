using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class GazePlot : MonoBehaviour
{

    public string fileName;
    public float updateTime = 0.1f;
    public float microSac = 0.017f;
    public GameObject numberL;
    public GameObject numberR;
    public GameObject objL;
    public GameObject objR;
    private TextAsset csvFile; // CSVファイル
    private List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト
    private int height = 0; // CSVの行数
    /* 
    private Vector3 PosL;
    private Vector3 PosR;
    private Vector3 prePosL;
    private Vector3 prePosR;
    private float DisL;
    private float DisR;
    private int holdCntL = 0;
    private int holdCntR = 0;
    private int ballCntL = 0;
    private int ballCntR = 0;
    */
    // 左と右で分かれていたprivate変数をそれぞれ配列にまとめる
    private Vector3[] pos = new Vector3[2];
    private Vector3[] prePos = new Vector3[2];
    private float[] dist = new float[2];
    private int[] holdCnt = new int[]{0, 0};
    private int[] ballCnt = new int[]{0, 0};

    // 左目右目を区別する列挙型の追加
    public enum Eye{
        left = 0, 
        right = 1
    }

    // マジックナンバーを定数化
    private int maxSize = 40;
    private int shift = 3;

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

        int i = 0;
        /* 
        prePosL = new Vector3(float.Parse(csvDatas[i][1]), float.Parse(csvDatas[i][2]), float.Parse(csvDatas[i][3]));
        prePosR = new Vector3(float.Parse(csvDatas[i][4]), float.Parse(csvDatas[i][5]), float.Parse(csvDatas[i][6]));
        */
        prePos[(int)Eye.left] = new Vector3(float.Parse(csvDatas[i][1]), float.Parse(csvDatas[i][2]), float.Parse(csvDatas[i][3]));
        prePos[(int)Eye.right] = new Vector3(float.Parse(csvDatas[i][1+shift]), float.Parse(csvDatas[i][2+shift]), float.Parse(csvDatas[i][3+shift]));
        while (++i < csvDatas.Count) 
        {
            /*
            PosL = new Vector3(float.Parse(csvDatas[i][1]), float.Parse(csvDatas[i][2]), float.Parse(csvDatas[i][3]));
            PosR = new Vector3(float.Parse(csvDatas[i][4]), float.Parse(csvDatas[i][5]), float.Parse(csvDatas[i][6]));
            DisL = (PosL - prePosL).sqrMagnitude;
            DisR = (PosR - prePosR).sqrMagnitude;
            if(DisL < microSac * microSac)
            {
                holdCntL++;
            }
            else
            {
                if (holdCntL > 0)
                {
                    putBallL(holdCntL, prePosL);
                    ballCntL++;
                    numberL.GetComponent<TextMesh>().text = ballCntL.ToString();
                    Instantiate(numberL, prePosL, Quaternion.identity);
                    holdCntL = 0; 
                    
                }  
            }
            if (DisR < microSac * microSac)
            {
                holdCntR++;
            }
            else
            {
                if (holdCntR > 0)
                {
                    putBallR(holdCntR, prePosR);
                    ballCntR++;
                    numberR.GetComponent<TextMesh>().text = ballCntR.ToString();
                    Instantiate(numberR, prePosR, Quaternion.identity);
                    holdCntR = 0; 
                }
            }
            prePosL = PosL;
            prePosR = PosR;
            */

            // 重複した処理をforで一つに
            int j;
            for(j = (int)Eye.left; j < (int)Eye.right+1; j++){
                pos[j] = new Vector3(float.Parse(csvDatas[i][1+shift*j]), float.Parse(csvDatas[i][2+shift*j]), float.Parse(csvDatas[i][3+shift*j]));
                dist[j] = (pos[j] - prePos[j]).sqrMagnitude;
                if(dist[j] < microSac * microSac){
                    holdCnt[j]++;
                }else{
                    if (holdCnt[j] > 0){
                        ballCnt[j]++;
                        putBall((Eye)j, holdCnt[j], ballCnt[j],prePos[j]);
                        holdCnt[j] = 0; 
                    }  
                }
                prePos[j] = pos[j];
            }
        } 
    }

    //preposにholdcntに応じた大きさの球を配置
    /* 
    public void putBallL(int cnt, Vector3 pos)
    {
        if (cnt == 0) return;
        if (cnt > 40) cnt = 40;
        GameObject obj = Instantiate(objL, pos, Quaternion.identity);
        obj.GetComponent<ChangeSize>().changeSize(cnt, updateTime);
    }
    public void putBallR(int cnt, Vector3 pos)
    {
        if (cnt == 0) return;
        if (cnt > 40) cnt = 40;
        GameObject obj = Instantiate(objR, pos, Quaternion.identity);
        objR.GetComponent<ChangeSize>().changeSize(cnt, updateTime);
    }
    */

    // putBallメソッドを一つに統一
    // 引数で右目左目を判断
    // numberを置く処理も追加
    public void putBall(Eye eye, int holdCnt, int ballCnt,Vector3 prePos)
    {
        if (holdCnt == 0) return;
        if (holdCnt > maxSize) holdCnt = maxSize;
        GameObject obj = new GameObject();
        switch(eye){
            case Eye.left:
                obj = Instantiate(objL, prePos, Quaternion.identity);
                numberL.GetComponent<TextMesh>().text = ballCnt.ToString();
                Instantiate(numberL, prePos, Quaternion.identity);
                break;
            case Eye.right:
                obj = Instantiate(objR, prePos, Quaternion.identity);
                numberR.GetComponent<TextMesh>().text = ballCnt.ToString();
                Instantiate(numberR, prePos, Quaternion.identity);
                break;
        }
        obj.GetComponent<ChangeSize>().changeSize(holdCnt, updateTime);
    }
}
