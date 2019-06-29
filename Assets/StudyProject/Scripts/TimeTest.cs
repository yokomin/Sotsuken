using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TimeTest : MonoBehaviour {

    private float nowTime;
    private int frame;

    // Use this for initialization
    void Start () {
        nowTime = 0f;
        frame = 0;
	}

    // Update is called once per frame
    void Update()
    {
        nowTime += Time.deltaTime;
        frame++;
        Debug.Log(Time.deltaTime);
        if (nowTime >= 1f)
        {
            Debug.Log("FramePerSecond is " + frame);
            frame = 0;
            nowTime -= 1f;
        }
    }
}


