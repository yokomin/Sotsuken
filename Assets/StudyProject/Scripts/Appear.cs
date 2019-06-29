using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appear : MonoBehaviour {
    [SerializeField] GameObject obj;
    [SerializeField] float appearTime;
    private float nowTime;
    private int cnt;

	// Use this for initialization
	void Start () {
        nowTime = 0f;
        cnt = 0;
	}

    // Update is called once per frame
    void Update()
    {
        if (cnt > 0)
            return;

        nowTime += Time.deltaTime;
        if (nowTime >= appearTime)
        {
            Instantiate(obj, transform.position, Quaternion.identity);
            cnt++;
        }
    }
}
