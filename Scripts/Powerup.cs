using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject player;
    public Sprite attackSpeed;
    public Sprite movementSpeed;
    public Sprite tempBoost;
    public Sprite attackDamage;
    public string boostType;
    private IEnumerator coroutine;
    private Vector3 scale;
    public int counter;
    private float transparency;
    

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        int randomNum = Random.Range(1, 5);
        boostType = "";
        
        counter = 1;
        scale = GetComponent<Transform>().localScale;
        if (randomNum == 1)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = attackSpeed;
            boostType = "atkSpeed";

        }
        if (randomNum == 2)
        {

            gameObject.GetComponent<SpriteRenderer>().sprite = movementSpeed;
            boostType = "mvmSpeed";
            scale = new Vector3(25, 25, 1);
            GetComponent<Transform>().localScale = scale;
        }
        if (randomNum == 3)
        {

            gameObject.GetComponent<SpriteRenderer>().sprite = tempBoost;
            boostType = "tempBoost";

        }
        if (randomNum == 4)
        {

            gameObject.GetComponent<SpriteRenderer>().sprite = attackDamage;
            boostType = "atkDmg";

        }
        coroutine = DestroyOverTime(10);
        StartCoroutine(coroutine);

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (counter <= 60)
        {

            scale.x += 0.4f;
            scale.y += 0.4f;

        }
        if (counter >= 60)
        {
            if (counter >= 120)
            {

                counter = 0;

            }
            else
            {

                scale.x -= 0.4f;
                scale.y -= 0.4f;
            }

        }
        counter++;
        GetComponent<Transform>().localScale = scale;


    }


    IEnumerator DestroyOverTime(float duration)
    {
        // Create an instance of the material to avoid affecting shared materials
        Material material = GetComponent<Renderer>().material;

        // Get the current color of the material
        Color currentColor = material.color;
        currentColor.a = 1;
        yield return new WaitForSeconds(7);

        // Calculate how much to decrease the alpha per frame
        float fadeRate = currentColor.a / duration; // Alpha decrement per second

        // Fade out the object over the specified duration
        while (currentColor.a > 0f)
        {
            // Reduce the alpha value based on the fade rate and elapsed time
            currentColor.a -= fadeRate * Time.deltaTime;

            // Clamp the alpha value to ensure it doesn’t go below 0
            currentColor.a = Mathf.Max(currentColor.a, 0f);

            // Apply the updated color
            material.color = currentColor;

            // Wait for the next frame
            yield return null;
        }

        // Fully fade and destroy the object
        Destroy(this.gameObject);
    }
}

