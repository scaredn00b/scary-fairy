﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class GameManager : MonoBehaviour, IObserver
{



    public static GameManager instance = null;
    //levelNum: in welchem Level befinden wir uns gerade - wird inkrementiert wenn das Levelende erreicht wird
    //und ein neues Level geladen werden soll
    private int levelNum = 1;

    void Awake()
    {
        // GameManager wird nicht zerstört wenn ein neues Level geladen wird
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        //delete player states so first level starts with full health ranger and warrior
        PlayerPrefs.DeleteAll();

        //lade erstes Level
        initGame();
    }

    public void OnNotify(string gameEvent)
    {
        switch (gameEvent)
        {
            case "Next Level":
                levelNum += 1;
                SceneManager.LoadScene(levelNum);
                break;
            case "Main Menu":
                SceneManager.LoadScene("MainMenu");
                break;
            case "Current Level":
                SceneManager.LoadScene(levelNum);
                break;
	        case "Player Died":
                restartLevel(); //If one player has died reload the current scene -> alternative respawn mechanic?
                break;
            default:
                break;
        }

    }

    public void initGame()
    {
        //SceneManager.LoadScene lädt Level anhand deren Index in den Build Settings (strg + shift + B in Unity)
        //auch anhand des Namens möglich
        SceneManager.LoadScene("MainMenu");
    }

    void Start()
    {
        Subject.AddObserver(this);
    }

    void Update()
    {

    }

    void restartLevel()
    {
        SceneManager.LoadSceneAsync(levelNum); //Async is better here, because there is already a scene displayed
    }
}
