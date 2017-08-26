﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueFromPause : MonoBehaviour {

    public GameObject container;

	public void resumeGame()
    {
        for (int i = 0; i < container.transform.childCount; i++)
        {
            container.transform.GetChild(i).gameObject.SetActive(false);
        }
        Subject.Notify("EnableHUD");
        Time.timeScale = 1;
    }

}