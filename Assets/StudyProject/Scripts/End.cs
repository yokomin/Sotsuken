using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour {

    private float nowTime;
    public float endTime;

    // Use this for initialization
    void Start () {
        nowTime = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        nowTime += Time.deltaTime;
        if(nowTime > endTime)
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_STANDALONE
                Application.Quit();
            #endif
        }
    }
}
