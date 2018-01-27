﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberDisplay : MonoBehaviour {

    public float liveTime = 1;

    private float spawnTime;
    public Text number;

	// Use this for initialization
	void Start () {
        spawnTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += Vector3.up * Time.deltaTime;
        if(Time.time >= spawnTime + liveTime)
        {
            Destroy(gameObject);
        }
	}

    public void SetNumber(int value)
    {        
        number.text = value.ToString();
    }
}
