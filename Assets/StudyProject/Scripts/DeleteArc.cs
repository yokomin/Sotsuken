using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteArc : MonoBehaviour
{
    //arcの座標に問題あり、Guideの座標を使う必要ありか？

    private float nowTime;
    private float distanceLeft;
    private float distanceRight;
    private float cntLeft;
    private float cntRight;
    private bool transitionLeft;
    private bool transitionRight;
    public GameObject EyeLeft;
    public GameObject EyeRight;
    public GameObject GuideLeft;
    public GameObject GuideRight;
    public float startTime = 1f;
    public float transitionTime = 0.5f;
    private float stayDistance = 0.04f;

    // Use this for initialization
    void Start()
    {
        nowTime = 0f;
        cntLeft = 0f;
        cntRight = 0f;
        transitionLeft = false;
        transitionRight = false;
    }

    // Latepdate ensures that the object doesn't lag behind the user's head motion
    void Update()
    {

        if (transitionLeft && transitionRight)
        {
            this.gameObject.SetActive(false);
            return;
        }

        //startTimeをこえるまでは処理をしない。
        nowTime += Time.deltaTime;
        if (nowTime < startTime)
        {
            return;
        }

        //自分とEyeとの距離がある一定以下になった時間をカウント
        distanceLeft = (GuideLeft.transform.position - EyeLeft.transform.position).sqrMagnitude;
        if (cntLeft > 0f && distanceLeft > stayDistance)
        {
            cntLeft = 0f;
        }
        else if (distanceLeft < stayDistance)
        {
            cntLeft += Time.deltaTime;
        }

        if (cntLeft > transitionTime && !transitionLeft)
        {
            transitionLeft = true;
        }

        distanceRight = (GuideRight.transform.position - EyeRight.transform.position).sqrMagnitude;
        if (cntRight > 0f && distanceRight > stayDistance)
        {
            cntRight = 0f;
        }
        else if (distanceRight < stayDistance)
        {
            cntRight += Time.deltaTime;
        }

        if (cntRight > transitionTime && !transitionRight)
        {
            transitionRight = true;
        }

        //Debug.Log(transitionLeft);
        //Debug.Log(transitionRight);
        //Debug.Log("Eye = " + EyeLeft.transform.position.z);
    }
}
