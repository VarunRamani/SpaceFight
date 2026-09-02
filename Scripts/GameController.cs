    using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Events;
using UnityEditor;
using TMPro;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool easy;
    public bool medium;
    public bool hard;
    public GameObject canvas;

    public GameObject player;

    public GameObject inGameCanvas;

    public GameObject boss;

    public bool gameActive;
    
    public UnityEvent gameStart;

    public UnityEvent menu;
    public GameObject trophy;
    public GameObject winText;
    public GameObject victory;
    public GameObject defeat;

    public int wins;

    

    public UnityEngine.UI.Button startButton;
    

    

    
    void Start()
    {
        wins = 0;
        StartMenu();

        
    }

    public void StartMenu() {

        canvas.SetActive(true);
        victory.SetActive(false);
        defeat.SetActive(false);
        if (boss.GetComponent<BossScript>().bossDead == true)
        {

            wins += 1;
            victory.SetActive(true);


        }
        if (player.GetComponent<PlayerScript>().playerHealth <= 0)
        {


            defeat.SetActive(true);
        }

        player.SetActive(false);
        boss.SetActive(false);
        inGameCanvas.SetActive(false);
        

        gameActive = false;

        menu.Invoke();
        if (wins >= 1)
        {

            trophy.SetActive(true);
            winText.SetActive(true);
            winText.GetComponent<TextMeshProUGUI>().text = wins.ToString();

        } else
        {

            trophy.SetActive(false);
            winText.SetActive(false);

        }
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {

            Destroy(bullet);

        }
        
        

        GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");
        foreach (GameObject minion in minions)
        {

            Destroy(minion);

        }

       





    }

    public void StartButton() {
        gameActive = true;
        ColorBlock colorVar = startButton.colors;
        if (hard == true || medium == true || easy == true) {

            
            GameStart();

                
            

        } 
        


    }

    public void GameStart() {

        canvas.SetActive(false);
        player.SetActive(true);
        boss.SetActive(true);
        inGameCanvas.SetActive(true);
        gameActive = true;
        gameStart.Invoke();
        

    }
    public void DifficultyEasy() {

        easy = true;
        medium = false;
        hard = false;

    }

    public void DifficultyMedium() {

        easy = false;
        medium = true;
        hard = false;

    }

    public void DifficultyHard() {

        easy = false;
        medium = false;
        hard = true;

    }
    
    

    // Update is called once per frame
    
    void Update()
    {


       

        
    }

    
}
