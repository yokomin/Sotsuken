using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delete : MonoBehaviour {

    private float nowTime;
    public float timeTriger;

    // Use this for initialization
    void Start()
    {
        nowTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        nowTime += Time.deltaTime;

        if (nowTime >= timeTriger)
        {
            this.gameObject.SetActive(false);
        }
    }
}
