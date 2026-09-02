using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Laser : MonoBehaviour
{
    
    public LineRenderer m_lineRenderer;
    public Transform laserFirePoint;
    private Transform m_transform;
    public GameObject player;
    public GameObject boss;
    public Vector2 playerPos;
    public UnityEvent laserHit;
    private BossScript bossScript;
    private bool laserReady;




    public float normalWidth;

    public float normalHeight;
    private void Start()
    {
        bossScript = boss.GetComponent<BossScript>();
        m_transform = GetComponent<Transform>();
        

        normalWidth = this.transform.localScale.x;
        normalHeight = this.transform.localScale.y;
        laserReady = false;
    }

    private void Update()
    {
        


    }

    

   

    public void changeWidth(float newWidth, float newHeight)
    {

        this.transform.localScale = new Vector3(newWidth, newHeight, 10);
        

    }

    public void changeStatus()
    {

        if (laserReady == true)
        {

            laserReady = false;


        }
        if (laserReady == false)
        {

            
            laserReady = true;

        }
    }

    

 
}
