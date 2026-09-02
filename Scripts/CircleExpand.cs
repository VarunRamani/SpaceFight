using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleExpand : MonoBehaviour
{
    // Start is called before the first frame update
    
    private int counter;
    private float timer;
    public float largerAmt = 1.07f;
    private Transform scale;
    void Start()
    {
        timer = 0;
        scale = transform;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 5);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (timer < 0.6)
        {

            this.transform.localScale = new Vector3(transform.localScale.x * largerAmt, transform.localScale.y * largerAmt, 1);
            timer += Time.deltaTime;

        } else
        {
            Destroy(this.gameObject);

        }
        
        
    }

   
}
