using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DecreaseBlur : MonoBehaviour
{

    private Material material;
    private float blur;
    private float nowTime;

    [SerializeField] float minBlur;
    [SerializeField] float rateOfChange;
    [SerializeField] float updateTime;
    [SerializeField] float timeTriger;

    // Use this for initialization
    void Start()
    {
        material = GetComponent<Renderer>().material;
        blur = material.GetFloat("_Blur");
        nowTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (blur <= minBlur)
        {
            //Debug.Log("Reach Time of Min = " + Time.time);
            return;
        }
        if (Time.time > timeTriger)
        {
            nowTime += Time.deltaTime;
            

            if (nowTime >= updateTime)
            {
                //Debug.Log("nowTime of Dec = " + nowTime);
                blur -= rateOfChange * 0.001f;
                if (blur < minBlur)
                    blur = minBlur;
                material.SetFloat("_Blur", blur);
                nowTime -= updateTime;
            }
        }
    }
}
