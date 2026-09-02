using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthBarMove : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject boss;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = boss.transform.position + new Vector3(-10, 60, 0);
    }
}
