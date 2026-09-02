using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject control;
    public float horizontalVel;

    public float verticalVel;

    public GameObject powerUp;
    
    public float hpInvul = 1;

    public float velMultiplier = 600;

    public float velCap = 150;

    public float defaultAtkSpeed;

    public float velDissipate = 350;

    public Quaternion lookingDirection;

    public GameObject bullet;

    public UnityEvent heartRefresh;
    public bool powerUpWaiting;
    private IEnumerator coroutine;

    private bool invul;

    public float shootCD;
    private float currentShootCD;
    public int bulletDMG = 5;
    public int playerHealth = 5;
    public UnityEvent playerDead;

    [SerializeField] private AudioClip shoot;
    [SerializeField] private AudioClip dmgUpAudio;
    [SerializeField] private AudioClip speedUpAudio;
    [SerializeField] private AudioClip atkSpeedUpAudio;
    [SerializeField] private AudioClip tempBoostAudio;
    [SerializeField] private AudioClip dmgTaken;



    public float playerSpeed;

    public float xRange = 450;
    public float yRange = 250;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get input axes
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Apply acceleration based on input
        if (horizontalInput != 0)
        {
            horizontalVel += horizontalInput * velMultiplier * Time.deltaTime;
        }
        if (verticalInput != 0)
        {
            verticalVel += verticalInput * velMultiplier * Time.deltaTime;
        }

        // Combine velocities into a single vector
        Vector2 currentVelocity = new Vector2(horizontalVel, verticalVel);

        // Apply a consistent cap on velocity magnitude (diagonal included)
        if (currentVelocity.magnitude > velCap)
        {
            currentVelocity = currentVelocity.normalized * velCap;
        }

        // Deceleration logic: Reduce the velocity vector's magnitude when no input
        if (horizontalInput == 0 && verticalInput == 0)
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, velDissipate * Time.deltaTime);
        }

        // Update individual velocity components from the current velocity vector
        horizontalVel = currentVelocity.x;
        verticalVel = currentVelocity.y;

        // Update position based on velocity
        transform.position += new Vector3(horizontalVel, verticalVel, 0f) * Time.deltaTime;

        // Clamp the position within defined bounds
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -xRange, xRange),
            Mathf.Clamp(transform.position.y, -yRange, yRange),
            transform.position.z
        );

        // Rotate to face the mouse cursor
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        direction.z = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Shooting logic
        if (Input.GetMouseButton(0))
        {
            if (currentShootCD > shootCD)
            {
                Shoot();
                currentShootCD = 0;
            }
        }
        currentShootCD += Time.deltaTime;
    }

    public void PlayerStart() {

        transform.position = new Vector3(-300, 0, transform.position.z);
        currentShootCD = shootCD;
        shootCD = 0.3f;
        velMultiplier = 650;
        velDissipate = 350;
        velCap = 150;
        bulletDMG = 5;
        horizontalVel = 0;
        defaultAtkSpeed = shootCD;
        verticalVel = 0;
        playerHealth = 5;
        Vector3 mousePos = Input.mousePosition;
        invul = false;
        heartRefresh.Invoke();
        powerUpWaiting = false;
        coroutine = powerUpWait(Random.Range(10, 15));
        StartCoroutine(coroutine);

    }

    public void Menu() {

        
    }

    private void Shoot()
    {
        
            Instantiate(bullet, new Vector3(transform.position.x, transform.position.y, 50), transform.rotation);
            SFXManager.instance.PlaySFXClip(shoot, transform, Random.Range(0.15f, 0.25f));
            
        
        
    }
    public void OnTriggerEnter2D(Collider2D collider)
    {

        if (invul == false) {

            if (collider.gameObject.name.Contains("EnemyBullet") || collider.gameObject.name.Contains("Laser"))
            {


                playerHealth--;
                heartRefresh.Invoke();
                StartCoroutine(playerInvulTimer(1));
                SFXManager.instance.PlaySFXClip(dmgTaken, transform, .9f);
                if (collider.gameObject.name.Contains("EnemyBullet"))
                {
                    Destroy(collider.gameObject);
                }
                if (playerHealth <= 0)
                {

                    playerDead.Invoke();

                }

            } 


    }
        if (collider.gameObject.name.Contains("PowerUp"))
        {

            string boostType = collider.gameObject.GetComponent<Powerup>().boostType;
            if (boostType == "atkSpeed")
            {

                shootCD *= 0.8f;
                defaultAtkSpeed *= 0.8f;
                SFXManager.instance.PlaySFXClip(atkSpeedUpAudio, transform, 0.7f);

            }
            if (boostType == "atkDmg")
            {

                bulletDMG += 1;
                SFXManager.instance.PlaySFXClip(dmgUpAudio, transform, 0.7f);


            }
            if (boostType == "mvmSpeed")
            {

                velCap += 50;
                velMultiplier += 100;
                velDissipate += 110;
                SFXManager.instance.PlaySFXClip(speedUpAudio, transform, 0.7f);


            }
            if (boostType == "tempBoost")
            {

                coroutine = boostTimer();
                StartCoroutine(coroutine);
                SFXManager.instance.PlaySFXClip(tempBoostAudio, transform, 0.7f);

            }
            Destroy(collider.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (invul == false)
        {
            Debug.Log("Collided");
            if (collision.gameObject.tag.Equals("Boss"))
            {
                playerHealth--;
                heartRefresh.Invoke();
                StartCoroutine(playerInvulTimer(1));
                SFXManager.instance.PlaySFXClip(dmgTaken, transform, .9f);
                if (playerHealth <= 0)
                {

                    playerDead.Invoke();

                }
            }
            if (collision.gameObject.tag.Equals("Minion"))
            {
                playerHealth--;
                heartRefresh.Invoke();
                StartCoroutine(playerInvulTimer(1));
                SFXManager.instance.PlaySFXClip(dmgTaken, transform, .9f);
                if (playerHealth <= 0)
                {

                    playerDead.Invoke();

                }
                Destroy(collision.gameObject);
            }
        }
    }



    IEnumerator playerInvulTimer(float time) {
        invul = true;
        yield return new WaitForSeconds(time);
        invul = false;

    }

    IEnumerator boostTimer()
    {
        
        float originalVelCap = velCap;
        float originalVelMultiplier = velMultiplier;
        float originalVelDisspate = velDissipate;
        shootCD *= 0.35f;
        velCap *= 1.4f;
        
        velMultiplier *= 3f;
        velDissipate *= 3f;
        while (shootCD < defaultAtkSpeed)
        {
            float delta = 0.03f * Time.deltaTime;
            
            shootCD += delta;
            
            yield return null;
        }

        shootCD = defaultAtkSpeed;
        velCap = originalVelCap;
        velMultiplier = originalVelMultiplier;
        velDissipate = originalVelDisspate;

    }

    IEnumerator powerUpWait(float time)
    {
        if (powerUpWaiting == false)
        {
            powerUpWaiting = true;
            yield return new WaitForSeconds(time);
            powerUpWaiting = false;
            Instantiate(powerUp, new Vector3(Random.Range(-xRange, xRange), Random.Range(-yRange, yRange), transform.position.z), Quaternion.Euler(0, 0, 0));
            coroutine = powerUpWait(Random.Range(8, 13));
            StartCoroutine(coroutine);

        }

    }



}

