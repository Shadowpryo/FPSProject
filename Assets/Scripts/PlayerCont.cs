using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCont : MonoBehaviour {
    public int HP;
    public int maxHP;
    public GameObject ragDoll;
    public bool paused;
    public bool respawn = false;
    public Vector3 spawnPOS;

    private GunScript GS;
    private GameData GD;

	// Use this for initialization
	void Start () {
        maxHP = 100;
        HP = maxHP;
        spawnPOS = transform.position;
        GD = GameObject.Find("GameManager").GetComponent<GameData>();
        paused = false;        
	}
	
	// Update is called once per frame
	void Update () {
        if (spawnPOS == new Vector3(0, 0, 0))
            spawnPOS = transform.position;
        if (HP <=0)
        {
            /*StopAllCoroutines();
            SceneManager.LoadScene("WinLose");*/
            respawn = true;
            Debug.Log("You died!");
        }
        if (GS == null)
        {
            GS = GameObject.FindGameObjectWithTag("Weapon").GetComponent<GunScript>();
        }
        if (Input.GetKeyDown(KeyCode.P))
            Paused();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gun")
        {
            if (!GD.isFULL)
            {
                GD.ammo += 15;
                Destroy(other.gameObject);
            }
            else if (GD.isFULL)
            {
                GD.ammoFULL += 30;
                Destroy(other.gameObject);
            }
        }
        if(other.name == "deathTrap")
        {
            Debug.Log("Yo you died tsk tsk!");
            respawn = true;
            //StartCoroutine(GD.Respawn());
            //transform.position = spawnPOS;
        }
        if(other.tag == "PickUp")
        {
            if(other.name == "Damage")
            {
                StartCoroutine(DamageUp());
                Destroy(other.gameObject);
                GD.damagePU = false;
                GD.numPUS--;
            }
            else if(other.name == "Health")
            {
                HP += 10;
                if (HP > maxHP)
                {
                    HP = maxHP;
                }
                Destroy(other.gameObject);
                GD.healthPU = false;
                GD.numPUS--;
            }
            else if(other.name == "Ammo")
            {
                GD.ammo += 100;
                GD.ammoFULL += 200;
                Destroy(other.gameObject);
                GD.ammoPU = false;
                GD.numPUS--;
            }
        }

    }
    public IEnumerator DamageUp()
    {
        GS.damage = GS.damage * 2;
        yield return new WaitForSeconds(5);
        GS.damage /= 2;
    }

    void Paused()
    {
        if (paused)
        {
            Time.timeScale = 1;
            paused = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Time.timeScale = 0;
            paused = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
