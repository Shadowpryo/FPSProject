using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    private GameData GD;

	// Use this for initialization
	void Start () {
        GD = GameObject.Find("GameManager").GetComponent<GameData>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 110, 40, 100, 120), "Enemy Waves");

        if (GUI.Button(new Rect(Screen.width - 100, 70, 80, 20), "10"))
            GD.waves = 10;
        if (GUI.Button(new Rect(Screen.width - 100, 100, 80, 20), "30"))
            GD.waves = 30;
        if (GUI.Button(new Rect(Screen.width - 100, 130, 80, 20), "Never Ending"))
            GD.waves = 0;

        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 50, 200, 40),
            "Click Here or Press 'P' to Play") || Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
    }
}
