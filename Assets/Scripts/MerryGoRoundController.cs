using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerryGoRoundController : MonoBehaviour
{
    public GameObject obj;
    public float speed;
    public float distance;
    public float angle = 0;
    
    void FixedUpdate()
    {
        angle += speed;

        float x = distance * Mathf.Sin(angle);
        float y = distance * Mathf.Cos(angle);
        Vector3 move = new Vector3(x, y, 0);

        obj.transform.position = this.transform.position + move;
    }
}
