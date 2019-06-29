using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighten : MonoBehaviour {

    private float nowTime;
    public int MaxRange = 50;
    public int RateOfChange = 5;
    public float updateTime = 0.1f;

    // Use this for initialization
    void Start () {
        nowTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        if(Time.time < 0.3f)
        {
            return;
        }

        if(GetComponent<Light>().range >= MaxRange)
        {
            return;
        }
        nowTime += Time.deltaTime;
        if(nowTime > updateTime)
        {
            GetComponent<Light>().range += RateOfChange;
            nowTime -= updateTime;
        }
	}
}
