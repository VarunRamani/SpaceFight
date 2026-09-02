using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    public float bulletSpeedInit;
    public float bulletSpeedMult = 1.03f;
    private float bulletSpeedCurrent;
    private GameObject player;
    private float xBound;
    private float yBound;
    public UnityEvent bossHit;
    public Sprite normalBullet;
    public Sprite dmgUp1;
    public Sprite dmgUp2;

    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        xBound = player.GetComponent<PlayerScript>().xRange;
        yBound = player.GetComponent<PlayerScript>().yRange;
        bulletSpeedCurrent = bulletSpeedInit;
        
        
        if (player.GetComponent<PlayerScript>().bulletDMG > 7)
        {
            Debug.Log("blt2");
            gameObject.GetComponent<SpriteRenderer>().sprite = dmgUp2;
            gameObject.GetComponent<Transform>().localScale *= 1.75f;
            bulletSpeedMult = 1.02f;

        } else if (player.GetComponent<PlayerScript>().bulletDMG > 5)
        {

            gameObject.GetComponent<SpriteRenderer>().sprite = dmgUp1;
            gameObject.GetComponent<Transform>().localScale *= 1.5f;
            bulletSpeedMult = 1.025f;

        } else
        {

        
        
            gameObject.GetComponent<SpriteRenderer>().sprite = normalBullet;
            gameObject.GetComponent<Transform>().localScale = new Vector3(9, 9, 1);
            bulletSpeedMult = 1.03f;
            Debug.Log("blt3");
        }
    }

    // Update is called once per frame
    void Update()
    {

        
        transform.Translate(Vector2.right * Time.deltaTime * bulletSpeedCurrent);
        if (transform.position.x > xBound || transform.position.y > yBound || transform.position.x < -xBound || transform.position.y < -yBound)
        {

            Debug.Log("wall");
                Destroy(this.gameObject);

        }

      

        
        
    }
    private void FixedUpdate()
    {
        bulletSpeedCurrent = (bulletSpeedMult * bulletSpeedCurrent);
    }

   
}
