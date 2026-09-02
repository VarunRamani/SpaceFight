using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heartScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject heart;
    public GameObject player;
    public int playerLives;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HeartRefresh()
    {   
        GameObject[] hearts = GameObject.FindGameObjectsWithTag("Heart");
        foreach (GameObject heart in hearts)
        {

            Destroy(heart);

        }
        Vector3 Pos = new Vector3(-420, 220, 10);
        playerLives = player.GetComponent<PlayerScript>().playerHealth;
        for (int i = 0; i < playerLives; i++) {

            Instantiate(heart, Pos, transform.rotation);
            Pos.x += 50;
                
         }

    }
}
