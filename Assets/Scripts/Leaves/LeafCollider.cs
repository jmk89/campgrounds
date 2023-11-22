using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Ground" || other.gameObject.tag == "Leaf") {
            this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            this.gameObject.GetComponent<FallingLeaf>().enabled = false;
        }
    }
}
