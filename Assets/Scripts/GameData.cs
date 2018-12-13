using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public PlayerCont PC;
    public GameObject movingPlat;
    public GameObject[] enemys;
    public GameObject[] pickUps;
    public GameObject[] PUspawnLOCs;
    public Vector3 spawnLOC;
    public bool isFULL = false;
    public int ammo;
    public int clip;
    public int ammoFULL;
    public int clipFULL;
    public int enemies;
    public float spawnTime;
    public float puSpawnTime;
    public int waves;
    public Camera deathCam;
    public Transform shipSpawn;
    public GameObject ship;

    public bool isUp = false;
    public bool shipStopped = false;
    public bool oneShip = false;
    public int currentWave;

    public int numPUS = 0;
    public bool healthPU = false;
    public bool ammoPU = false;
    public bool damagePU = false;
    public bool won = false;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        currentWave=1;
        ammo = 180;
        clip = 30;
        ammoFULL = 300;
        clipFULL = 30;
        spawnTime = 3;
        puSpawnTime = Random.Range(30, 45);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main"))
        {
            if (PUspawnLOCs == null)
            {
                Debug.LogError("Not finding Spawn for pickups!");
            }
            if (shipSpawn == null)
            {
                shipSpawn = GameObject.Find("shipSpawn").transform;
            }
            if(deathCam == null)
            {
                deathCam = GameObject.Find("deahtCamera").GetComponent<Camera>();
                deathCam.gameObject.SetActive(false);
            }
            movingPlat = GameObject.Find("movingPlat");
            spawnLOC = GameObject.Find("enemySpawn").transform.position;
            if (PC == null)
                PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCont>();
            if (!isUp)
            {
                StartCoroutine(MovePlat());
            }
            if (enemies <= 0)
            {
                enemies = 0;
                if (waves != 0)
                {
                    if (currentWave <= waves)
                    {
                        if (spawnTime <= 0)
                        {
                            if (!oneShip)
                                SpawnShip();
                            if (shipStopped)
                                StartCoroutine(SpawnEnemies());
                            //Debug.Log("Restart spawn");
                        }
                        else
                        {
                            spawnTime -= Time.deltaTime;
                        }
                    }
                    else
                    {
                        SceneManager.LoadScene("WinLose");
                        Debug.Log("You win!");
                        won = true;
                        StopAllCoroutines();
                        Cursor.lockState = CursorLockMode.None;
                        //Destroy(gameObject);
                    }
                }
                else
                {
                    if (spawnTime <= 0)
                    {
                        if (!oneShip)
                            SpawnShip();
                        if (shipStopped)
                        {
                            StartCoroutine(SpawnEnemies());
                        }
                        //Debug.Log("Restart spawn");
                    }
                    else
                    {
                        spawnTime -= Time.deltaTime;
                    }
                }
            }
            if (PC.respawn == true)
            {
                StartCoroutine(Respawn());
            }
            if (puSpawnTime <= 0)
            {
                if (numPUS <= 2)
                {
                    GameObject spawnLOC = PUspawnLOCs[Random.Range(0, PUspawnLOCs.Length)];
                    GameObject pickup = pickUps[Random.Range(0, pickUps.Length)];
                    GameObject PU = (GameObject)Instantiate(pickup, new Vector3(spawnLOC.transform.position.x, spawnLOC.transform.position.y + 1, spawnLOC.transform.position.z), Quaternion.identity);
                    PU.name = pickup.name;
                    if (PU.name == "Health")
                        if (healthPU == true)
                        {
                            Destroy(PU.gameObject);
                            puSpawnTime += 1;
                        }
                        else
                        {
                            healthPU = true;
                            numPUS++;
                            puSpawnTime += Random.Range(30, 45);
                        }
                    if (PU.name == "Ammo")
                        if (ammoPU == true)
                        {
                            Destroy(PU.gameObject);
                            puSpawnTime += 1;
                        }
                        else
                        {
                            ammoPU = true;
                            numPUS++;
                            puSpawnTime += Random.Range(30, 45);
                        }
                    if (PU.name == "Damage")
                        if (damagePU == true)
                        {
                            Destroy(PU.gameObject);
                            puSpawnTime += 1;
                        }
                        else
                        {
                            damagePU = true;
                            numPUS++;
                            puSpawnTime += Random.Range(30, 45);
                        }
                }
            }
            else
            {
                puSpawnTime -= Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main"))
        {
            if (deathCam.gameObject.activeSelf == true)
            {
                GameObject RD = GameObject.Find("ragDoll");
                deathCam.transform.LookAt(RD.transform.position);
                Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, 100, 0) * Time.deltaTime);
                RD.GetComponent<Rigidbody>().MoveRotation(RD.GetComponent<Rigidbody>().rotation * deltaRotation);
            }
        }
    }

    public void OnGUI()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main"))
        {
            if (!isFULL)
            {
                GUI.Box(new Rect(Screen.width - 110, 10, 100, 25),
                    string.Format("Clip: " + clip));
                GUI.Box(new Rect(Screen.width - 220, 10, 100, 25),
                    string.Format("Ammo: " + ammo));
            }
            else
            {
                GUI.Box(new Rect(Screen.width - 110, 10, 100, 25),
                    string.Format("Clip: " + clipFULL));
                GUI.Box(new Rect(Screen.width - 220, 10, 100, 25),
                    string.Format("Ammo: " + ammoFULL));
            }
            GUI.Box(new Rect(0, 10, 150, 25),
                string.Format("HP/MaxHP " + PC.HP + "/" + PC.maxHP));
            GUI.Box(new Rect(0, 40, 150, 25),
                string.Format("Wave/Enemies " + (currentWave-1) + "/" + enemies));
            if (PC.paused)
            {
                GUI.Box(new Rect(Screen.width /2, Screen.height/2, 100, 50),
                    string.Format("PAUSED"));
            }
        }
    }

    IEnumerator MovePlat()
    {
        movingPlat.transform.Translate(Vector3.up * Time.deltaTime, Space.World);
        yield return new WaitForSeconds(3);
        isUp = true;
        movingPlat.transform.Translate(Vector3.down * Time.deltaTime, Space.World);
        yield return new WaitForSeconds(3);
        movingPlat.transform.Translate(Vector3.down * Time.deltaTime);
        yield return new WaitForSeconds(3);
        movingPlat.transform.Translate(Vector3.up * Time.deltaTime, Space.World);
        yield return new WaitForSeconds(3);
        isUp = false;
    }

    void SpawnShip()
    {
        oneShip = true;
        GameObject eShip = (GameObject)Instantiate(ship, shipSpawn.position, Quaternion.identity);
        eShip.name = "motherShip";
    }

    IEnumerator SpawnEnemies()
    {
        if (currentWave % 10 != 0)
        {
            /*while(enemies <= 9)
            {
                int num = Random.Range(0, 1);
                Debug.Log("number is: " + num);
                GameObject enemy = (GameObject)Instantiate(enemys[num], spawnLOC, Quaternion.identity);
                if (num == 0)
                    enemy.name = "rEnemy";
                else
                    enemy.name = "zEnemy";
                enemies++;
                yield return new WaitForSeconds(1);
            }*/
            while (enemies <= 4)
            {
                GameObject rE = (GameObject)Instantiate(enemys[0], spawnLOC, Quaternion.identity);
                rE.name = "rEnemy";
                enemies++;
                yield return new WaitForSeconds(1);
            }
            while (enemies > 4 && enemies <= 9)
            {
                GameObject zE = (GameObject)Instantiate(enemys[1], spawnLOC, Quaternion.identity);
                zE.name = "zEnemy";
                enemies++;
                yield return new WaitForSeconds(1);
            }
        }
        else{
            GameObject bE = (GameObject)Instantiate(enemys[2], spawnLOC, Quaternion.identity);
            Vector3 lookPOS = (PC.gameObject.transform.position - bE.transform.position).normalized;
            Quaternion lookROT = Quaternion.LookRotation(lookPOS);
            bE.transform.rotation = Quaternion.Slerp(bE.transform.rotation, lookROT, Time.deltaTime * 10);
            bE.name = "BossEnemy";
            GameObject.Find("mainMusic").GetComponent<AudioSource>().Stop();
            GameObject.Find("bossMusic").GetComponent<AudioSource>().Play();
            enemies++;
        }
        currentWave = currentWave + 1;
        spawnTime += 10;
        GameObject.Find("motherShip").GetComponent<Rigidbody>().AddForce(Vector3.forward * 500);
        yield return new WaitForSeconds(20);
        Destroy(GameObject.Find("motherShip").gameObject);
    }

    public IEnumerator Respawn()
    {
        if (PC.HP >= 1)
        {
            PC.respawn = false;
            deathCam.transform.position = new Vector3(PC.gameObject.transform.position.x,
                                                      PC.gameObject.transform.position.y + 3,
                                                      PC.gameObject.transform.position.z + 3
                                                      );
            GameObject RD = (GameObject)Instantiate(PC.ragDoll, PC.transform.position,
                                                    Random.rotation);
            PC.gameObject.transform.position = new Vector3(PC.spawnPOS.x, PC.spawnPOS.y + 30, PC.spawnPOS.z);
            RD.name = "ragDoll";
            deathCam.gameObject.SetActive(true);
            Rigidbody[] RDs = RD.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rd in RDs)
            {
                rd.AddForce(Vector3.forward * 1000);
                rd.AddForce(Vector3.up * 500);
            }
            PC.gameObject.SetActive(false);
            yield return new WaitForSeconds(5);
            deathCam.gameObject.SetActive(false);
            PC.gameObject.SetActive(true);
            Destroy(RD);
        }
        else
        {
            PC.respawn = false;
            deathCam.transform.position = new Vector3(PC.gameObject.transform.position.x,
                                                      PC.gameObject.transform.position.y + 3,
                                                      PC.gameObject.transform.position.z + 3
                                                      );
            GameObject RD = (GameObject)Instantiate(PC.ragDoll, PC.transform.position,
                                                    Random.rotation);
            PC.gameObject.transform.position = new Vector3(PC.spawnPOS.x, PC.spawnPOS.y + 30, PC.spawnPOS.z);
            RD.name = "ragDoll";
            deathCam.gameObject.SetActive(true);
            Rigidbody[] RDs = RD.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rd in RDs)
            {
                rd.AddForce(Vector3.forward * 1000);
                rd.AddForce(Vector3.up * 500);
            }
            PC.gameObject.SetActive(false);
            yield return new WaitForSeconds(5);
            deathCam.gameObject.SetActive(false);
            SceneManager.LoadScene("WinLose");
            Cursor.lockState = CursorLockMode.None;
            StopAllCoroutines();
            //Destroy(gameObject);
        }
    }
}
