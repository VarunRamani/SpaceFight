using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.UI;

public class BossScript : MonoBehaviour
{
    // Start is called before the first frame update
    public bool active = false;
    public bool laserMode;
    public GameObject player;
    public GameObject GameController;
    public GameObject simpleBullet;
    public GameObject laser;
    public GameObject firePoint;
    public GameObject warning;
    public GameObject redCircle;
    public GameObject yellowCircle;
    public GameObject defender;
    private int numSwitches;
    public Image healthBar;
    public UnityEvent BossDead;
    public int maxHealth;
    private float atkMultiplier;
    public bool attacking;
    public bool bossDead;
    private int randomNum;
    private bool rushMode;
    private Transform targetPoint; // The point to move towards
    public float moveSpeed = 120f; // The base movement speed
    public float velocityFactor = 5f; // How much the velocity affects turning
    public float turnRate = 180f; // Degrees per second the object can turn
    private Vector2 currentVelocity;
    private Vector2 targetDirection;
    private int atksSinceSwitch;


    public Quaternion lookingDirection;
    
    public int bossHealth;
    public string difficulty;
    public float shootCD;
    private float spreadAngle;
    private float angle;
    private IEnumerator coroutine;
    private float timeMult;
    public bool moving;
    public Vector3 newPos;
    public float bossSpeed = 110;
    public int mvmRangeMax = 250;
    public int mvmRangeMin = 100;
    public bool rotating;
    private int maxAttempts = 100;
    private int attempts;
    public Sprite laserSprite;
    public Sprite normalSpriteRed;
    public Sprite biteSprite;
    public Sprite normalSpriteYellow;
    public GameObject laserWarning;
    public UnityEvent laserSwitchWarning;
    public UnityEvent laserSwitch;
    public GameObject minion;

    [SerializeField] private AudioClip bossBasicShoot;
    [SerializeField] private AudioClip laserCircle;
    [SerializeField] private AudioClip bossSpreadShot;
    [SerializeField] private AudioClip bossHurt1;
    [SerializeField] private AudioClip bossHurt2;
    [SerializeField] private AudioClip bossHurt3;
    [SerializeField] private AudioClip laserChargeSmall;
    [SerializeField] private AudioClip laserChargeBig;
    [SerializeField] private AudioClip laserChargeMini;
    [SerializeField] private AudioClip laserShoot;
    [SerializeField] private AudioClip bossDestroy;


    void Start()
    {
        
        bossDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (bossHealth % 20 < 5)
            {

                healthBar.fillAmount = ((float)bossHealth / maxHealth);

                atkMultiplier = timeMult * ((Mathf.Sqrt(((float)bossHealth / maxHealth))));


            }
            if (rotating == true && moving == false)
            {
                return;
            }
            if (rushMode)
            {
                targetPoint = player.transform;

                targetDirection = targetPoint.position - transform.position;
                float distanceToTarget = targetDirection.magnitude;

                // Calculate desired velocity
                Vector2 desiredVelocity = targetDirection.normalized * moveSpeed;

                // Deceleration
               

                // Smooth velocity change (important for slow turns)
                currentVelocity = Vector2.Lerp(currentVelocity, desiredVelocity, Time.deltaTime);

                // Calculate the rotation needed to face the target (2D)
                float targetAngle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

                // Smoothly rotate towards the target (2D)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnRate * Time.deltaTime);

                // Move the object based on the current velocity (2D)
                transform.position += (Vector3)currentVelocity * Time.deltaTime;
                return;
            }
            if (moving == false && rotating == false)
            {



                Vector3 directionToPlayer = player.transform.position - transform.position;

                // Remove any Z-axis difference to ensure it's strictly 2D
                directionToPlayer.z = 0;

                // Calculate the desired rotation
                angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

                // Apply the rotation to make the enemy face the player
                transform.rotation = Quaternion.Euler(0, 0, angle);
                return;

            }

            
            else 
            {
                
                    if (Vector3.Distance(transform.position, newPos) > 0.01f)
                    {
                        attacking = true;
                        transform.position = Vector3.MoveTowards(transform.position, newPos, bossSpeed * Time.deltaTime);
                        return;



                    }
                    else if(!rotating)
                    {
                        
                        moving = false;
                        
                        Vector3 directionToPlayer = player.transform.position - transform.position;
                        // Remove any Z-axis difference to ensure it's strictly 2D
                        directionToPlayer.z = 0;
                        // Calculate the desired rotation
                        angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
                        StartCoroutine(Rotate(angle, 18));
                        return;
                        

                    }


                


            }

        }

        
    }

        


    
    

    public void Menu() {


    }

    public void GameStart() {

        StopAllCoroutines();
        active = true;
        currentVelocity = Vector3.zero;
        moving = false;
        atksSinceSwitch = 0;
        rotating = false;
        laserWarning.SetActive(false);
        warning.SetActive(false);
        numSwitches = 0;
        laser.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().sprite = normalSpriteRed;
        laserMode = true;
        bossDead = false;
        

        if (GameController.GetComponent<GameController>().easy)
        {

            difficulty = "easy";
            maxHealth = 1350;
            timeMult = 1.3f;
            

        }
        if (GameController.GetComponent<GameController>().medium)
        {

            difficulty = "medium";
            maxHealth = 1700;
            timeMult = 1f;

        }
        if (GameController.GetComponent<GameController>().hard)
        {

            difficulty = "hard";
            maxHealth = 2000;
            timeMult = 0.7f;

        }
        transform.position = new Vector3(300, 0, transform.position.z);

        bossHealth = maxHealth;
        coroutine = AttackWait(4);
        StartCoroutine(coroutine);
        attacking = false;




    }

    public void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.gameObject.name.Contains("PlayerBullet"))
        {

            bossHealth -= player.GetComponent<PlayerScript>().bulletDMG;
            Destroy(collider.gameObject);
            randomNum = Random.Range(1, 4);
            if (randomNum == 1)
            {
                SFXManager.instance.PlaySFXClip(bossHurt1, transform, 0.6f);
            } 
            else if (randomNum == 1)
             {
                SFXManager.instance.PlaySFXClip(bossHurt2, transform, 0.4f);
            } 
            else
            {
                SFXManager.instance.PlaySFXClip(bossHurt3, transform, 0.5f);

            }
            

        }

        if (bossHealth <= 0)
        {

            bossDead = true;
            SFXManager.instance.PlaySFXClip(bossDestroy, transform, 1f);
            BossDead.Invoke();
            

        }
        

    }

    public void SpreadShot(int bulletNum, int spread, int speed)
    {
        attacking = true;
        spreadAngle = spread / bulletNum;
        float startingAngle = angle - (((bulletNum) / 2) * spreadAngle);
        SFXManager.instance.PlaySFXClip(bossSpreadShot, transform, 0.7f);

        GameObject newBullet = Instantiate(simpleBullet, transform.position + (Vector3.forward * 10), Quaternion.Euler(transform.rotation.x, transform.rotation.y, startingAngle));
        newBullet.GetComponent<EnemyBullet>().bulletSpeed = speed;
        for (int i = 1; i < bulletNum; i++)
        {

            newBullet = Instantiate(simpleBullet, transform.position + (Vector3.forward * 10), Quaternion.Euler(transform.rotation.x, transform.rotation.y, startingAngle + (spreadAngle * i)));
            newBullet.GetComponent<EnemyBullet>().bulletSpeed = speed;
        }
        attacking = false;


    }

    public void Spray(int numBullets, int spread, float delayBetweenBullets, int speed)
    {
        StartCoroutine(SprayBullets(numBullets, spread, delayBetweenBullets, speed));
    }

    private IEnumerator SprayBullets(int numBullets, int spread, float delayBetweenBullets, int speed)
    {
        attacking = true;
        for (int i = 0; i < numBullets; i++)
        {
            
            // Instantiate a bullet with random spread.
            GameObject newBullet = Instantiate(
                simpleBullet,
                transform.position + (Vector3.forward * 10),
                Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.eulerAngles.z + Random.Range(-spread, spread))
            );
            newBullet.GetComponent<EnemyBullet>().bulletSpeed = speed;
            

            SFXManager.instance.PlaySFXClip(bossBasicShoot, transform, 0.25f);

            // Wait for the specified delay before firing the next bullet.
            yield return new WaitForSeconds(delayBetweenBullets);
        }
        attacking = false;
    }
    public void ChooseAttack()
    {
        int randomNum;
        
        if (attacking == true)
        {
            return;
        }
        if (laserMode)
        {
            if ((float)bossHealth / maxHealth > 0.8f)
            {
                randomNum = Random.Range(1, 14);

            }
            else
            {
                
                randomNum = Random.Range(1, 28);
                if (atksSinceSwitch > 9)
                {

                    randomNum = 21;

                }
            }
            Debug.Log(randomNum);
            
            if (randomNum == 1 || randomNum == 2 || randomNum == 3)
            {
                if (randomNum == 1)
                {
                    Spray(Random.Range(25, 40), Random.Range(20, 30), 0.1f, 200);
                    shootCD = 9f * atkMultiplier;

                }
                else
                {

                    Spray(Random.Range(6, 15), Random.Range(10, 35), 0.06f, 200);
                    SpawnMinion();
                    shootCD = 2.5f * atkMultiplier;

                }



            }
            else if (randomNum == 4 || randomNum == 5 || randomNum == 6)
            {
                if (randomNum == 4)
                {

                    SpreadShot(Random.Range(9, 17), Random.Range(60, 90), 150);
                    shootCD = 3f * atkMultiplier;

                }
                else
                {

                    SpreadShot(Random.Range(4, 7), Random.Range(30, 61), 180);
                    shootCD = 2f * atkMultiplier;
                    SpawnDefender(1);

                }


            }
            else if (randomNum == 7)
            {
                SpawnDefender(2);
                gameObject.GetComponent<SpriteRenderer>().sprite = laserSprite;
                coroutine = Laser(2f * atkMultiplier, 0.15f, 3);

                StartCoroutine(coroutine);

                shootCD = 9f * atkMultiplier;

            }
            else if (randomNum == 8)
            {

                gameObject.GetComponent<SpriteRenderer>().sprite = laserSprite;
                coroutine = Laser(1.5f * atkMultiplier, 0.15f, 1);

                StartCoroutine(coroutine);

                shootCD = 3.5f * atkMultiplier;

            }
            else if (randomNum == 9)
            {

                gameObject.GetComponent<SpriteRenderer>().sprite = laserSprite;
                coroutine = Laser(3.5f * atkMultiplier, 0.15f, 6);

                StartCoroutine(coroutine);

                shootCD = 14f * atkMultiplier;

            }
            else if (randomNum == 10)
            {

                gameObject.GetComponent<SpriteRenderer>().sprite = laserSprite;
                laser.GetComponent<Laser>().changeWidth(30, 1f);
                laserWarning.GetComponent<Laser1>().changeWidth(80);
                coroutine = Laser(2f * atkMultiplier, 0.15f, 1);

                StartCoroutine(coroutine);

                shootCD = 10f * atkMultiplier;

            }

            else if (randomNum == 11 || randomNum == 12 || randomNum == 13)
            {

                Move();
                shootCD = 5f * atkMultiplier;
                if (randomNum == 13)
                {
                    SpawnMinion();
                    shootCD += 3f;
                }

            }
            else if (randomNum == 14)
            {

                SpreadShot(4, 360, 125);
                SpreadShot(12, 360, 175);
                SpreadShot(8, 360, 225);
                shootCD = 1f * atkMultiplier;
            }
            else if (randomNum == 15)
            {

                SpreadShot(15, 360, 200);
                SpreadShot(15, 360, 120);
                gameObject.GetComponent<SpriteRenderer>().sprite = laserSprite;
                laser.GetComponent<Laser>().changeWidth(30, .7f);
                laserWarning.GetComponent<Laser1>().changeWidth(56);
                coroutine = Laser(1.5f * atkMultiplier, 0.15f, 2);

                StartCoroutine(coroutine);
                shootCD = 10f * atkMultiplier;

            }
            else if (randomNum == 16)
            {
                SpawnDefender(2);
                SpreadShot(40, 360, 150);
                Spray(Random.Range(60, 70), 30, 0.07f, 170);
                shootCD = 13f * atkMultiplier;


            }
            else if (randomNum == 17)
            {

                Spray(40, 360, 0.05f, 180);
                gameObject.GetComponent<SpriteRenderer>().sprite = laserSprite;
                laser.GetComponent<Laser>().changeWidth(30, .8f);
                laserWarning.GetComponent<Laser1>().changeWidth(74);
                coroutine = Laser(2f * atkMultiplier, 0.15f, 2);

                StartCoroutine(coroutine);
                shootCD = 13f * atkMultiplier;



            }
            else if (randomNum == 18)
            {

                Spray(40, 20, 0.3f, 200);
                gameObject.GetComponent<SpriteRenderer>().sprite = laserSprite;
                laser.GetComponent<Laser>().changeWidth(30, .4f);

                laserWarning.GetComponent<Laser1>().changeWidth(40);
                coroutine = Laser(3f * atkMultiplier, 0.15f, 8);
                StartCoroutine(coroutine);

                shootCD = 16f * atkMultiplier;



            }
            else if (randomNum == 19)
            {

                Spray(45, 20, 0.01f, 190);
                shootCD = 6 * atkMultiplier;
                SpawnMinion();



            }
            else if (randomNum == 20)
            {

                Move();
                SpawnMinion();
                SpawnMinion();
                SpreadShot(25, 180, 180);
                shootCD = 6 * atkMultiplier;



            }
            else if (randomNum == 21 || randomNum == 22 || randomNum == 23)
            {
                SpawnDefender(4);

                StartCoroutine(SwitchMode());
                shootCD = 6 * atkMultiplier;



            }
            else if (randomNum == 24)
            {
                SpawnDefender(2);
                SpawnMinion();
                SpawnMinion();
                SpawnMinion();
                shootCD = 15 * atkMultiplier;
                Spray(15, 10, 0.3f, 350);



            }
            else if (randomNum == 25)
            {


                shootCD = 17 * atkMultiplier;
                Spray(120, 180, 0.04f, 125);
                SpawnMinion();
                
                SpawnMinion();



            }
            else if (randomNum == 26 || randomNum == 27)
            {

                SpawnMinion();
                SpawnMinion();
                SpawnMinion();
                SpawnDefender(2);
                SpawnMinion();
                SpawnMinion();
                shootCD = 18f;

            }
        }
        else
        {

            randomNum = Random.Range(1, 12);
            if (bossHealth < (0.4f * maxHealth) && numSwitches < 3)
            {
                randomNum = 1;
            }
            if (atksSinceSwitch > 7)
            {

                randomNum = 1;

            }
            

            if (randomNum == 1)
            {

                StartCoroutine(SwitchMode());
                SpawnMinion();
                SpawnDefender(2);
                SpawnMinion();
                shootCD = 6 * atkMultiplier;

            }
            if (randomNum == 2)
            {
                SpawnMinion();
                StartCoroutine(Rush(8, 290));
                SpreadShot(12, 360, 100);
                SpreadShot(8, 360, 175);
                SpreadShot(4, 360, 250);
                shootCD = 12 * atkMultiplier;

            }
            if (randomNum == 3)
            {

                StartCoroutine(Rush(8, 290));
                SpreadShot(4, 50, 180);
                shootCD = 10 * atkMultiplier;

            }
            if (randomNum == 4)
            {
                SpawnDefender(2);
                StartCoroutine(Rush(9, 270));
                Spray(20, 25, 0.1f, 200);
                shootCD = 10 * atkMultiplier;

            }
            if (randomNum == 5)
            {

                StartCoroutine(Rush(8, 250));

                Spray(64, 45, 0.125f, 185);
                shootCD = 15 * atkMultiplier;

            }
            if (randomNum == 6)
            {

                StartCoroutine(Rush(9, 250));
                Spray(15, 45, 0.5f, 185);
                shootCD = 14 * atkMultiplier;

            }
            if (randomNum == 7)
            {

                SpawnMinion();
                SpawnMinion();
                Spray(40, 90, 0.03f, 120);
                shootCD = 6 * atkMultiplier;

            }
            if (randomNum == 8)
            {

                SpawnMinion();
                SpreadShot(12, 360, 250);
                SpreadShot(8, 360, 125);
                StartCoroutine(Rush(13, 250));
                shootCD = 16 * atkMultiplier;

            }
            if (randomNum == 9)
            {

                SpawnMinion();
                SpawnMinion();
                Spray(8, 5, 0.4f, 350);
                shootCD = 16 * atkMultiplier;
            }
            if (randomNum == 10)
            {

                SpawnDefender(4);
                shootCD = 6 * atkMultiplier;
            }
            if (randomNum == 11)
            {


                SpawnDefender(2);
                SpawnMinion();
                SpawnMinion();
                SpawnMinion();
                SpawnMinion();
                shootCD = 11 * atkMultiplier;

            }


        }

        atksSinceSwitch++;
        coroutine = AttackWait(shootCD);
        StartCoroutine(coroutine);
    }

    public void Move()
    {
        attempts = 0;
        attacking = true;
        newPos = new Vector3(transform.position.x + Random.Range(-mvmRangeMax, mvmRangeMax), transform.position.y + Random.Range(-mvmRangeMax * .5625f, mvmRangeMax * .5625f), transform.position.z);
        while ((Mathf.Abs(newPos.x) + 30 > Mathf.Abs(player.GetComponent<PlayerScript>().xRange) || (Mathf.Abs(newPos.y) + 30 > Mathf.Abs(player.GetComponent<PlayerScript>().yRange))) && attempts < maxAttempts)
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

    IEnumerator Laser(float chargeTime, float activeTime, int numTimes) {

        
        rotating = true;
        attacking = true;
        //serWarning.transform.position = firePoint.transform.position;
        //serWarning.transform.rotation = this.transform.rotation;
        //ser.transform.position = firePoint.transform.position;
        //ser.transform.rotation = this.transform.rotation;
        laserWarning.SetActive(true);
        laserWarning.transform.position = firePoint.transform.position;
        laserWarning.transform.rotation = firePoint.transform.rotation;
        laserSwitchWarning.Invoke();



        gameObject.GetComponent<SpriteRenderer>().sprite = laserSprite;
        

        
        SFXManager.instance.PlaySFXClip(laserChargeMini, transform, 0.8f, 1 / laserChargeMini.length);
        yield return new WaitForSeconds(chargeTime);

        laserSwitchWarning.Invoke();
        laserWarning.SetActive(false);



        
        laser.SetActive(true);
        laser.transform.position = firePoint.transform.position;
        laser.transform.rotation = firePoint.transform.rotation;


        SFXManager.instance.PlaySFXClip(laserShoot, transform, 0.9f);
        yield return new WaitForSeconds(activeTime);

        
        laser.SetActive(false);





        for (int i = 0; i < numTimes - 1; i++) {

         chargeTime = Mathf.Max(chargeTime * 0.75f, 0.5f); // Prevent chargeTime from being too small       
         rotating = false;
        yield return new WaitForSeconds(0.1f);
        
        
        rotating = true;

            //serWarning.transform.position = firePoint.transform.position;
            //serWarning.transform.rotation = this.transform.rotation;
            //ser.transform.position = firePoint.transform.position;
            //laser.transform.rotation = this.transform.rotation;

            laserWarning.SetActive(true);

            

            laserSwitchWarning.Invoke();
            SFXManager.instance.PlaySFXClip(laserChargeMini, transform, 0.8f);

            yield return new WaitForSeconds(chargeTime);

            
            laserSwitchWarning.Invoke();
            laserWarning.SetActive(false);

           


            laser.SetActive(true);
            SFXManager.instance.PlaySFXClip(laserShoot, transform, 0.7f);



            yield return new WaitForSeconds(activeTime);
            
            laser.SetActive(false);
            
        rotating = false;

        }
        rotating = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = normalSpriteRed;
        laser.GetComponent<Laser>().changeWidth(laser.GetComponent<Laser>().normalWidth, laser.GetComponent<Laser>().normalHeight);
        laserWarning.GetComponent<Laser1>().changeWidth(laserWarning.GetComponent<Laser1>().normalWidth);
        attacking = false;
    

    }
    IEnumerator AttackWait(float time)
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(time);
        yield return new WaitForSeconds(0.5f);
        int counter = 0;
        while (attacking == true || moving == true || rushMode == true || rotating) {
            yield return new WaitForSeconds(0.5f);
                counter += 1;

                }
        ChooseAttack();
        
    }
    IEnumerator Rotate(float targetAngle, float rotateSpeed)
    {
        attacking = true;
        rotating = true;
        

        // Get the current Z-axis rotation.
        float currentAngle = transform.eulerAngles.z;

        // Ensure the shortest rotation direction is chosen.
        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 0.15f)
        {
            // Smoothly interpolate toward the target angle.
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);

            yield return null;
        }

        // Snap to the exact target angle to prevent small inaccuracies.
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

        // Mark rotation as complete.
        
        rotating = false;
        attacking = false;
        
    }

    IEnumerator GeneralTimer(float time) {

        yield return new WaitForSeconds(time);

    }
    private IEnumerator SwitchMode()
    {
        rotating = true;
        attacking = true;
        warning.SetActive(true);
        SFXManager.instance.PlaySFXClip(laserChargeMini, transform, 0.8f);
        yield return new WaitForSeconds(2f);
        if (laserMode)
        {
            
            SFXManager.instance.PlaySFXClip(laserCircle, transform, 0.5f);
            Instantiate(yellowCircle, transform.position, transform.rotation);
            
            laserMode = false;
            gameObject.GetComponent<SpriteRenderer>().sprite = normalSpriteYellow;

        } else
        {

            Instantiate(redCircle, transform.position, transform.rotation);
            SFXManager.instance.PlaySFXClip(laserCircle, transform, 0.5f);
            laserMode = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = normalSpriteRed;
        }
        
        warning.SetActive(false);
        numSwitches++;
        atksSinceSwitch = 0;
        
        yield return new WaitForSeconds(1.5f);
        rotating = false;
        attacking = false;
        

    }

    private IEnumerator Rush(float time, float speed)
    {
        if (!attacking && !rushMode)
        {



        
        
        
            moveSpeed = speed;
            rotating = true;
            attacking = true;
            moving = true;
            currentVelocity = Vector3.zero;
            targetDirection = player.transform.position - transform.position;
            rushMode = true;

            gameObject.GetComponent<SpriteRenderer>().sprite = biteSprite;




            yield return new WaitForSeconds(time);
            int counter = 0;
            while ((Mathf.Abs(transform.position.x) + 30 > Mathf.Abs(player.GetComponent<PlayerScript>().xRange) || (Mathf.Abs(transform.position.y) + 30 > Mathf.Abs(player.GetComponent<PlayerScript>().yRange))) && counter < 99)
            {
                yield return new WaitForSeconds(0.5f);
                counter += 1;

            }
            gameObject.GetComponent<SpriteRenderer>().sprite = normalSpriteYellow;
            rushMode = false;
            attacking = false;
            moving = false;
            rotating = false;






        }
    }

    private void SpawnMinion()
    {

        Debug.Log("Minion");
        Instantiate(minion, new Vector3(transform.position.x, transform.position.y, transform.position.z + 10), transform.rotation);

    }
    private void SpawnDefender(int num)
    {

        if (num > 3)
        {
            GameObject d = Instantiate(defender, new Vector3(transform.position.x - 90, transform.position.y, transform.position.z + 10), transform.rotation);
            d.GetComponent<DefenderScript>().rotationSpeed = 50;

            d = Instantiate(defender, new Vector3(transform.position.x + 90, transform.position.y, transform.position.z + 10), transform.rotation);
            d.GetComponent<DefenderScript>().rotationSpeed = 50;

            d = Instantiate(defender, new Vector3(transform.position.x, transform.position.y - 90, transform.position.z + 10), transform.rotation);
            d.GetComponent<DefenderScript>().rotationSpeed = 50;

            d = Instantiate(defender, new Vector3(transform.position.x, transform.position.y + 90, transform.position.z + 10), transform.rotation);
            d.GetComponent<DefenderScript>().rotationSpeed = 50;
        } else if (num > 1)
        {

            GameObject d = Instantiate(defender, new Vector3(transform.position.x - 90, transform.position.y, transform.position.z + 10), transform.rotation);
            d.GetComponent<DefenderScript>().rotationSpeed = 50;

            d = Instantiate(defender, new Vector3(transform.position.x + 90, transform.position.y, transform.position.z + 10), transform.rotation);
            d.GetComponent<DefenderScript>().rotationSpeed = 50;

            

        } else
        {

            GameObject d = Instantiate(defender, new Vector3(transform.position.x - 90, transform.position.y, transform.position.z + 10), transform.rotation);
            d.GetComponent<DefenderScript>().rotationSpeed = 50;

        }
            
            
            

    }

}
