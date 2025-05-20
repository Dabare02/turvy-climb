using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGen : MonoBehaviour
{
    public Rigidbody2D hook;
    public GameObject ropePiecePrefab;
    public int numSegments = 5;

    // Start is called before the first frame update
    void Start()
    {
        GenerateRope();
    }

    void GenerateRope() {
        Rigidbody2D prevBody = hook;
        for (int i = 0; i < numSegments; i++) {
            GameObject newSeg = Instantiate(ropePiecePrefab);
            newSeg.transform.parent = transform;
            //newSeg.transform.position = prevBody.transform.position;
            HingeJoint2D hj = newSeg.GetComponent<HingeJoint2D>();
            hj.connectedBody = prevBody;

            prevBody = newSeg.GetComponent<Rigidbody2D>();
        }
    }
}
