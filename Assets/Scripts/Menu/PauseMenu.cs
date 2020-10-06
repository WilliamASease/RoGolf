﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class PauseMenu : MonoBehaviour
{
    public Canvas thisMenu;

    public Button button_1; //Options button
    public Button button_2; //Controls buton
    public Button button_3; //Restart button
    public Button button_4; //Quit button
    public Button button_5; //Close button
    public Button button_6;
    public Button button_7;
    public Button button_8;
    public Button button_9;
	
	public Canvas optionsMenu;
	public Canvas controlsMenu;

    // Start is called before the first frame update
    void Start()
    {
        if (thisMenu.enabled) 
            thisMenu.enabled = false;
        button_1.GetComponent<Button>().onClick.AddListener(task_1);
        button_2.GetComponent<Button>().onClick.AddListener(task_2);
        button_3.GetComponent<Button>().onClick.AddListener(task_3);
        button_4.GetComponent<Button>().onClick.AddListener(task_4);
        button_5.GetComponent<Button>().onClick.AddListener(task_5);
        /*button_6.GetComponent<Button>().onClick.AddListener(task_6);
        button_7.GetComponent<Button>().onClick.AddListener(task_7);
        button_8.GetComponent<Button>().onClick.AddListener(task_8);
        button_9.GetComponent<Button>().onClick.AddListener(task_9*/
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
			ToggleThisMenu();
    }
	
	void ToggleThisMenu()
	{
		Time.timeScale = (Time.timeScale == 0) ? 1 : 0;
            thisMenu.enabled = !thisMenu.enabled;
	}

    void task_1()
    {
		optionsMenu.enabled = true;
    }

    void task_2()
    {
		controlsMenu.enabled = true;
    }

    void task_3()
    {
		 button_3.GetComponentInChildren<Text>().text = "Maybe someday...";
		 button_3.interactable = false;
    }

    void task_4()
    {
		Application.Quit();
    }

    void task_5()
    {
		ToggleThisMenu();
    }

    void task_6()
    {
    }

    void task_7()
    {
    }

    void task_8()
    {
    }

    void task_9()
    {
    }
}
