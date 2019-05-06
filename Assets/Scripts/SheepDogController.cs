using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepDogController : MonoBehaviour
{
    public bool trainMode;
    public float speed;
    public float distance;
    public float angle = 0;

    void FixedUpdate()
    {
        float x = 0;
        float y = 0;

        if(trainMode)
        {
            angle += speed;

            x = Mathf.Sin(angle);
            y = Mathf.Cos(angle);            
        }
        
        else
        {
            x = -1 * Input.GetAxis("Horizontal");
            y = -1 * Input.GetAxis("Vertical");
        }


        if (x != 0 || y != 0)
        {
            Vector2 lookAt = new Vector2(x, y);
            
            lookAtPos(lookAt);

            transform.position += transform.up * speed * distance;    
        }
    }


    void lookAtPos(Vector2 pos)
    {
        float rot_z = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }

}
