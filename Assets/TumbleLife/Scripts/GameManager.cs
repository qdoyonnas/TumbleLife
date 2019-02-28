using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;

    public ConfigurableJoint joint;


    public TumbleBlock grabbedBlock = null;

    // Start is called before the first frame update
    void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = mainCamera.ScreenPointToRay(mousePos);
        Vector3 mouseWorldPos = new Vector3(mouseRay.origin.x, mouseRay.origin.y, 0);
        transform.position = mouseWorldPos;

        if( Input.GetMouseButtonDown(0) && grabbedBlock == null ) {
            RaycastHit hit;
            if( Physics.Raycast(mouseRay, out hit) ) {
                TumbleBlock blockScript = hit.collider.gameObject.GetComponent<TumbleBlock>();
                if( blockScript != null && blockScript.isGrabbable ) {
                    grabbedBlock = blockScript;
                    joint.connectedBody = grabbedBlock.rigidBody;
                }
            }
        }

        if( Input.GetMouseButtonUp(0) && grabbedBlock != null ) {
            joint.connectedBody = null;
            grabbedBlock = null;
        }
    }
}
