using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class IncreaseBlur : MonoBehaviour
{

    private Material mat;
    private float blur;
    //private float repeat;
    //private int size;
    private float nowTime;

    [SerializeField] float maxBlur;
    [SerializeField] float rateOfChange;
    [SerializeField] float updateTime;
    [SerializeField] float timeTriger;

    // Use this for initialization
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        blur = mat.GetFloat("_Blur");
        //repeat = mat.GetFloat("_Repeat");
        //size = mat.GetInt("_Size");
        nowTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (blur >= maxBlur)
        {
            //Debug.Log("Reach Time of Max = " + Time.time);
            return;
        }
        if (Time.time > timeTriger)
        {
            nowTime += Time.deltaTime;

            if (nowTime >= updateTime)
            {
                //Debug.Log("nowTime 0f Inc = " + nowTime);
                blur += rateOfChange * 0.001f;
                if (blur > maxBlur)
                    blur = maxBlur;
                mat.SetFloat("_Blur", blur);
                nowTime -= updateTime;
            }
        }
    }
}
