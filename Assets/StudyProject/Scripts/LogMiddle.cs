using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LogMiddle : MonoBehaviour
{

    private float nowTime;
    private int layerMask;
    //private int cnt;
    //private int end;

    [SerializeField]
    public FoveInterfaceBase foveInterface;
    public float updateTime;
    public float endTime;

    // Use this for initialization
    void Start()
    {
        nowTime = 0f;
        layerMask = 1 << 11 | 1 << 12;
        //cnt = 0;
        //end = (int)(endTime / updateTime);

        /*StreamWriter sw;
        FileInfo fi;
        fi = new FileInfo(Application.dataPath + "Resources/gazeMiddle.csv");
        sw = fi.AppendText();
        sw.WriteLine("Time,LeftX,LeftY,LeftZ,RightX,RightY,RightZ");
        sw.Flush();
        sw.Close();*/
    }

    // Latepdate ensures that the object doesn't lag behind the user's head motion
    void Update()
    {
        if (Time.time > endTime)
        {
            return;
        }

        nowTime += Time.deltaTime;

        if (nowTime >= updateTime)
        {
            FoveInterfaceBase.EyeRays rays = foveInterface.GetGazeRays();

            Ray rLeft = rays.left;
            Ray rRight = rays.right;

            RaycastHit hit;
            
            Physics.Raycast(rLeft, out hit, Mathf.Infinity, layerMask);
            Vector3 leftEye;
            if (hit.point != Vector3.zero) // Vector3 is non-nullable; comparing to null is always false
            {
                leftEye = hit.point;
            }
            else
            {
                leftEye = rLeft.GetPoint(3.0f);
            }

            Physics.Raycast(rRight, out hit, Mathf.Infinity, layerMask);
            Vector3 rightEye;
            if (hit.point != Vector3.zero) // Vector3 is non-nullable; comparing to null is always false
            {
                rightEye = hit.point;
            }
            else
            {
                rightEye = rRight.GetPoint(3.0f);
            }

            logSave(Time.time, leftEye, rightEye);
            nowTime -= updateTime;
        }
    }

    public void logSave(float t, Vector3 left, Vector3 right)
    {
        StreamWriter sw;
        FileInfo fi;
        fi = new FileInfo(Application.dataPath + "/Resources/gazeMiddle.csv");
        sw = fi.AppendText();
        /*sw.Write(n); sw.Write(", ");
        sw.Write(left.x); sw.Write(", "); sw.Write(left.y); sw.Write(", "); sw.Write(left.z); sw.Write(", ");
        sw.Write(right.x); sw.Write(", "); sw.Write(right.y); sw.Write(", "); sw.WriteLine(right.z);*/
        sw.WriteLine(t + "," + left.x + "," + left.y + "," + left.z + "," + right.x + "," + right.y + "," + right.z);
        sw.Flush();
        sw.Close();
    }
}