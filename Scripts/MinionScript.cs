using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionScript : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 newPos;
    private int attempts;
    private int maxAttempts = 200;
    private GameObject player;
    private GameObject boss;
    public int mvmRangeMax = 400;
    private bool rotating;
    private bool moving;
    private float angle;
    public int speed;
    private int bulletSpeed;
    private float shootCD;
    public GameObject bullet;
    private float shootTime;
    private int health;
    [SerializeField] private AudioClip bossHurt3;
    [SerializeField] private AudioClip destroy;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        boss = GameObject.FindGameObjectWithTag("Boss");
        health = Random.Range(20, 35);
        Move();
        shootCD = 0;
        shootTime = Random.Range(1.3f, 2.4f);
        bulletSpeed = Random.Range(350, 450);

    }

    // Update is called once per frame
    void Update()
    {
        if (moving == false && rotating == false)
        {



            Vector3 directionToPlayer = player.transform.position - transform.position;

            // Remove any Z-axis difference to ensure it's strictly 2D
            directionToPlayer.z = 0;
            shootCD += Time.deltaTime;
            if (shootCD > shootTime)
            {
                Shoot(bulletSpeed);
                shootCD = 0;
            }

            // Calculate the desired rotation
            angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

            // Apply the rotation to make the enemy face the player
            transform.rotation = Quaternion.Euler(0, 0, angle);

        }
        else if (rotating == true)
        {

        } else
        {
            if (Vector3.Distance(transform.position, newPos) > 0.001f)
            {

                transform.position = Vector3.MoveTowards(transform.position, newPos, speed * Time.deltaTime);




            }
            else if (!rotating)
            {

                moving = false;

                Vector3 directionToPlayer = player.transform.position - transform.position;
                // Remove any Z-axis difference to ensure it's strictly 2D
                directionToPlayer.z = 0;
                // Calculate the desired rotation
                angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
                StartCoroutine(Rotate(angle, 18));


            }
        }
    }
    public void Move()
    {
        attempts = 0;
        
        newPos = new Vector3(transform.position.x + Random.Range(-mvmRangeMax, mvmRangeMax), transform.position.y + Random.Range(-mvmRangeMax * .5625f, mvmRangeMax * .5625f), transform.position.z);
        while ((Mathf.Abs(newPos.x) > Mathf.Abs(player.GetComponent<PlayerScript>().xRange) || (Mathf.Abs(newPos.y) > Mathf.Abs(player.GetComponent<PlayerScript>().yRange))) && (attempts < maxAttempts) || (Vector3.Distance(newPos, boss.transform.position) < 100))
        {

            newPos = new Vector3(transform.position.x + Random.Range(-mvmRangeMax, mvmRangeMax), transform.position.y + Random.Range(-mvmRangeMax * .5625f, mvmRangeMax * .5625f), transform.position.z);
            attempts++;


        }

        if (attempts == maxAttempts)
        {
            Debug.Log("No space");
            return;


        }

        Vector3 directionToTarget = newPos - transform.position;

        // Remove any Z-axis difference to ensure it's strictly 2D
        directionToTarget.z = 0;

        // Calculate the desired rotation
        angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        // Apply the rotation to face the target
        if (!rotating)
        {
            StartCoroutine(Rotate(angle, 7f));
            moving = true;
        }

    }
    IEnumerator Rotate(float targetAngle, float rotateSpeed)
    {
        
        rotating = true;


        // Get the current Z-axis rotation.
        float currentAngle = transform.eulerAngles.z;
        int attempts = 0;
        // Ensure the shortest rotation direction is chosen.
        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 0.15f || attempts < 100)
        {
            // Smoothly interpolate toward the target angle.
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            attempts++;

            yield return null;
        }
        if (attempts > 99)
        {
            Debug.Log("error");
        }

        // Snap to the exact target angle to prevent small inaccuracies.
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

        // Mark rotation as complete.

        rotating = false;
        

    }
    public void Shoot(int speed)
    {
        moving = false;
        rotating = false;
        if (Random.Range(1, 6) == 1)
        {
            if (moving == false && rotating == false)
            {
                Debug.Log("move");
                Move();
            }

        }
        else
        {
            GameObject newBullet = Instantiate(bullet, new Vector3 (transform.position.x, transform.position.y, transform.position.z + 10), Quaternion.Euler(0f, 0f, transform.eulerAngles.z));
            newBullet.GetComponent<EnemyBullet>().bulletSpeed = bulletSpeed;
        }

    }
    public void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.gameObject.name.Contains("PlayerBullet"))
        {

            health -= player.GetComponent<PlayerScript>().bulletDMG;
            Destroy(collider.gameObject);
            
           
                SFXManager.instance.PlaySFXClip(bossHurt3, transform, 0.9f);

            


        }

        if (health <= 0)
        {

            
            SFXManager.instance.PlaySFXClip(destroy, transform, 0.8f);
            Destroy(this.gameObject);


        }


    }
}
