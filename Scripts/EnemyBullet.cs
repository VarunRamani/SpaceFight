using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBullet : MonoBehaviour
{
    // Start is called before the first frame update
    
    
    
    public GameObject player;
    private float xBound;
    private float yBound;
    
    public float bulletSpeed;
   
    void Start()
    {
        
        xBound = player.GetComponent<PlayerScript>().xRange;
        yBound = player.GetComponent<PlayerScript>().yRange;
       
    }

    // Update is called once per frame
    void Update()
    {

        
        transform.Translate(Vector2.right * Time.deltaTime * bulletSpeed);
        if (transform.position.x > xBound || transform.position.y > yBound || transform.position.x < -xBound || transform.position.y < -yBound)
        {

                Destroy(this.gameObject);

        }

      

        
        
    }
    

   
}
