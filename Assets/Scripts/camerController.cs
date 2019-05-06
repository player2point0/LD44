using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerController : MonoBehaviour
{
    public GameObject target;
 
    void Start()
    {
        Vector3 targetPos = new Vector3(target.transform.position.x, target.transform.position.y, this.transform.position.z);
        this.transform.position = targetPos;    
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPos = new Vector3(target.transform.position.x, target.transform.position.y, this.transform.position.z);
        this.transform.position = targetPos;
    }
}
