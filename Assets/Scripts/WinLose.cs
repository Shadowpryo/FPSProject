using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLose : MonoBehaviour {
    public GameData GD;
    public Text winLoseText;

	// Use this for initialization
	void Start () {
        GD = GameObject.Find("GameManager").GetComponent<GameData>();
        winLoseText = GameObject.Find("WinLoseText").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (GD.won)
        {
            winLoseText.text = "Congrats you won!";
        }
        else
        {
            winLoseText.text = "Sorry you lost!";
        }
	}

    private void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width/2-150, Screen.height/2, 250, 60), "Press Here or R to restart the level") ||
                Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("MainMenu");
            Destroy(GD.gameObject);
        }
    }
}