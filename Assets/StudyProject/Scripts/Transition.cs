using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Transition : MonoBehaviour {

    private float nowTime;
    private float distance;
    private float cnt;
    private bool transition = false;
    private Vector3 DepEye;
    public GameObject Eye;
    public GameObject delObj;
    public GameObject otherObj;
    public float startTime = 1f;
    public float transitionTime = 0.5f;
    private float stayDistance = 0.04f;
    private bool i = true;
    private bool j = true;

    // Use this for initialization
    void Start()
    {
        nowTime = 0f;
        cnt = 0f;
        DepEye = transform.position;
    }

    // Latepdate ensures that the object doesn't lag behind the user's head motion
    void Update()
    {

        if (transition)
        {
            transform.position = Eye.transform.position;
            if (i)
            {
                StreamWriter sw;
                FileInfo fi;
                fi = new FileInfo(Application.dataPath + "/Resources/ArriveTimeLog.csv");
                sw = fi.AppendText();
                sw.WriteLine("ArriveTime=" + nowTime);
                sw.Flush();
                sw.Close();
                i = false;
            }
            if (j)
            {
                if (distance < stayDistance)
                {
                    cnt += Time.deltaTime;
                    distance = (DepEye - Eye.transform.position).sqrMagnitude;
                }
                else
                {
                    StreamWriter sw;
                    FileInfo fi;
                    fi = new FileInfo(Application.dataPath + "/Resources/StayTimeLogPre.csv");
                    sw = fi.AppendText();
                    sw.WriteLine("StayTime=" + cnt);
                    sw.Flush();
                    sw.Close();
                    j = false;
                }
            }
            return;
        }

        //startTimeをこえるまでは処理をしない。
        nowTime += Time.deltaTime;
        if (nowTime < startTime)
        {
            return;
        }

        //自分とEyeとの距離がある一定以下になった時間をカウント
        distance = (transform.position - Eye.transform.position).sqrMagnitude;
        
        if (distance >= 0.09f)
        {

            delObj.gameObject.SetActive(true);
            Debug.Log("true distance = " + distance);
            if (cnt > 0f && distance > stayDistance)
            {
                cnt = 0f;
                return;
            }
        }
        else
        {

            delObj.gameObject.SetActive(false);
            Debug.Log("delete distance = " + distance);
            if (distance <= stayDistance)
            {
                cnt += Time.deltaTime;
            }
            else if (cnt > 0f && distance > stayDistance)
            {
                cnt = 0f;
                return;
            }
        }
        
        if (cnt > transitionTime)
        {
            transition = true;
        }
        else if (otherObj.GetComponent<Transition>().GetTransition())
        {
            delObj.gameObject.SetActive(false);
            transition = true;
        }
    }

    public bool GetTransition()
    {
        return transition;
    }
}
