using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Laser1 : MonoBehaviour
{
    [SerializeField] private float defDistanceRay = 1200;
    public LineRenderer m_lineRenderer;
    public Transform laserFirePoint;
    private Transform m_transform;
     public float normalWidth;
    private bool laserReady;

    
    

    
    private void Start()
    {
        m_transform = GetComponent<Transform>();
        
        normalWidth = GetComponent<LineRenderer>().startWidth;
        
        laserReady = false;
        
    }

    private void Update()
    {
        if (laserReady == true)
        {
            shootLaser();
        }
    }

    void shootLaser()
    {

       

            Draw2DRay(laserFirePoint.position, laserFirePoint.position + laserFirePoint.transform.right * defDistanceRay);

        
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        m_lineRenderer.SetPosition(0, startPos);
        
        
        m_lineRenderer.SetPosition(1, endPos);
    }

    public void changeWidth(float newWidth) {

        GetComponent<LineRenderer>().startWidth = newWidth;
        
        
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
