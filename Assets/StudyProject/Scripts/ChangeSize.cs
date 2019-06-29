using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSize : MonoBehaviour {

    private float size;
    private float alpha = 0.27f;
    private Renderer renderer;
    private Color color;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void changeSize(int cnt, float uTime)
    {
        size = cnt * uTime;
        transform.localScale = new Vector3(size, size, size);
    }
    public void changeAlpha(float cnt)
    {
        renderer = GetComponent<Renderer>();
        color = renderer.material.color;
        color.a = alpha - cnt * 0.01f;
        renderer.material.color = color;
    }
}
