using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TumbleBlock : MonoBehaviour
{
    public bool isGrabbable = true;
    public Rigidbody rigidBody;

    bool isInit = false;
    void Start()
    {
        Init(Vector3.one);
    }

    public void Init(Vector3 scale)
    {
        if( isInit ) { return; }

        rigidBody = GetComponent<Rigidbody>();

        Material mat = GetComponent<Renderer>().material;
        Color color = Random.ColorHSV();
        mat.color = color;

        transform.localScale = scale;
        rigidBody.mass = scale.magnitude;

        GameManager.instance.blocks.Add(this);
        isInit = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if( other.gameObject.name == "OutofBounds" ) {
            GameManager.instance.blocks.Remove(this);
            GameManager.instance.state = GameManager.GameState.replay;
            Destroy(this);
        }
    }
}