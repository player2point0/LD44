using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterN : MonoBehaviour
{
    public float n;

    void Start()
    {
        Destroy(this.gameObject, n);        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) Destroy(this.gameObject, 0);
    }

}
