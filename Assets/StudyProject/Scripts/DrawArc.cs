using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawArc : MonoBehaviour {

    private Vector3 point1;
    private Vector3 point2;
    private float t;
    private int rate;
    private float nowTime;
    public GameObject[] obj;
    public GameObject left;
    public GameObject right;
    public GameObject guide;
    public float appearTime;

    // Use this for initialization
    void Start () {
        nowTime = 0f;
        rate = obj.Length;
    }
	
	// Update is called once per frame
	void Update () {

        nowTime += Time.deltaTime;
        if (nowTime >= appearTime)
        {
            t = 0;
            point1 = Vector3.Slerp(left.transform.position, right.transform.position, 0.5f);

            point2 = guide.transform.position;

            for (int i = 0; i < rate; i++)
            {
                t = (float)1.0 / (rate + 1) * (i + 1);
                obj[i].transform.position = Vector3.Slerp(point1, point2, t);

            }
        }  
    }
}
