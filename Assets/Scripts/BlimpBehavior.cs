﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlimpBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject thisBlimp;
    public float rotSpeed;
    public float traSpeed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //dunno why it's gotta be like this but it do :^)
        thisBlimp.transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed);
        thisBlimp.transform.Translate(Vector3.right * Time.deltaTime * traSpeed);
    }
}
