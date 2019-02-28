using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TumbleBlock : MonoBehaviour
{
    public bool isGrabbable = true;
    public Rigidbody rigidBody;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        Material mat = GetComponent<Renderer>().material;
        Color color = Random.ColorHSV();
        mat.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
