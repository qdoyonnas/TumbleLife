using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class TumbleBlock : MonoBehaviour
{
    public bool isGrabbable = true;
    public Rigidbody rigidBody;

    Collider collider;
    FixedJoint joint;

    public float physicsSleepTime = -1;
    float physicsSleepStamp = -1;

    public float maxStickSpeed = 0.05f;

    bool isInit = false;
    void Start()
    {
        Init(Vector3.one);
    }

    public void Init(Vector3 scale)
    {
        if( isInit ) { return; }

        rigidBody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        Material mat = GetComponent<Renderer>().material;
        Color color = Random.ColorHSV();
        mat.color = color;

        transform.localScale = scale;
        rigidBody.mass = scale.magnitude;

        GameManager.instance.blocks.Add(this);
        isInit = true;
    }

    private void OnCollisionStay( Collision collision )
    {
        if( physicsSleepTime < 0 ) { return; }

        if( rigidBody.velocity.magnitude + rigidBody.angularVelocity.magnitude < maxStickSpeed ) {
            if( physicsSleepStamp <= 0 ) {
                TumbleBlock otherBlock = collision.collider.GetComponent<TumbleBlock>();
                if( otherBlock != null ) {
                    physicsSleepStamp = Time.time + physicsSleepTime;
                }
            }
        }
    }

    private void Update()
    {
        if( isGrabbable && transform.position.y < GameManager.instance.boundsHeight ) {
            GameManager.instance.blocks.Remove(this);
            GameManager.instance.state = GameManager.GameState.replay;
            Destroy(this);
        }

        if( physicsSleepStamp >= 0 ) {
            if( rigidBody.velocity.magnitude + rigidBody.angularVelocity.magnitude > maxStickSpeed ) {
                physicsSleepStamp = -1;
            } else {
                if( Time.time >= physicsSleepStamp ) {
                    rigidBody.isKinematic = true;
                    isGrabbable = false;
                    transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if( !GameManager.instance.doDebug ) { return; }

        if( physicsSleepStamp >= 0 ) {
            if( rigidBody.isKinematic ) {
                Gizmos.color = Color.red;
            } else {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawWireCube(transform.position, collider.bounds.size);
        }
    }
}