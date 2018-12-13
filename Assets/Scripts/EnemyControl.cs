using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControl : MonoBehaviour {
    public Animator anim;
    public int HP;
    public int maxHP;
    public int damage;
    public EnemyGunControl enemyGun;
    public float speed;
    public GameObject gun;
    public Transform hand;

    private GameObject PLAYER;
    private GameData GD;
    public NavMeshAgent agent;
    private bool justSpawn = true;
    private PlayerMovementScript PMS;


    // Use this for initialization
    void Start () {
        if(anim == null)
            anim = GetComponent<Animator>();
        PLAYER = GameObject.FindGameObjectWithTag("Player");
        //enemyGun = gun.GetComponent<EnemyGunControl>();
        maxHP = 100;
        HP = maxHP;
        GD = GameObject.Find("GameManager").GetComponent<GameData>();
        agent = GetComponent<NavMeshAgent>();
        PMS = PLAYER.GetComponent<PlayerMovementScript>();
        agent.speed = 6f;

    }

    // Update is called once per frame
    void Update()
    {
        if (PLAYER == null)
        {
            PLAYER = GameObject.FindGameObjectWithTag("Player");
        }
        if (PMS == null)
        {
            PMS = PLAYER.GetComponent<PlayerMovementScript>();
        }
        if (name == "zEnemy")
        {
            if (HP <= 0)
            {
                anim.SetBool("isMoving", false);
                anim.SetBool("isDead", true);
                agent.isStopped = true;
                GetComponent<Rigidbody>().isKinematic = true;
                Destroy(GetComponent<CapsuleCollider>());
                Destroy(GetComponentInChildren<SphereCollider>());
                gameObject.layer = 0;
                if (enemyGun != null)
                {
                    enemyGun.GetComponent<BoxCollider>().isTrigger = true;
                    enemyGun.transform.parent = null;
                }
                StartCoroutine(EneymyDie());
                //transform.LookAt(new Vector3(0, 0, 0));
            }
            else
            {
                if (anim.GetBool("isAttacking") == false)
                {
                    agent.SetDestination(PLAYER.transform.position);
                    float dist = Vector3.Distance(transform.position, PLAYER.transform.position);
                    if (dist >= 10)
                    {
                        if (agent.path.status != NavMeshPathStatus.PathPartial)
                        {
                            anim.SetBool("isMoving", true);
                            anim.SetFloat("Blend", 2.5f);
                            agent.isStopped = false;
                            agent.speed = 13;
                        }
                        else
                        {
                            anim.SetBool("isMoving", false);
                        }
                    }
                    else if (dist >= 3)
                    {
                        if (agent.path.status != NavMeshPathStatus.PathPartial)
                        {
                            anim.SetBool("isMoving", true);
                            anim.SetFloat("Blend", 1.5f);
                            agent.isStopped = false;
                            agent.speed = 6f;
                        }
                        else
                        {
                            anim.SetBool("isMoving", false);
                        }
                    }
                    else
                    {
                        Vector3 lookPOS = (PLAYER.transform.position - transform.position).normalized;
                        Quaternion lookROT = Quaternion.LookRotation(lookPOS);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookROT, Time.deltaTime * 10);
                        agent.isStopped = true;
                        if (anim.GetBool("isMoving") == true)
                            anim.SetBool("isMoving", false);
                        if (anim.GetBool("isAttacking") == false)
                        {
                            agent.isStopped = true;
                            StartCoroutine(Meele());
                        }
                    }
                }
            }
        }
        else if (name == "rEnemy")
        {
            if (HP <= 0)
            {
                anim.SetBool("isDead", true);
                agent.isStopped = true;
                GetComponent<Rigidbody>().isKinematic = true;
                Destroy(GetComponent<CapsuleCollider>());
                Destroy(GetComponentInChildren<CapsuleCollider>());
                gameObject.layer = 0;
                StartCoroutine(EneymyDie());
                //transform.LookAt(new Vector3(0, 0, 0));
            }
            else
            {
                if (anim.GetBool("isAiming") == false)
                {
                    if (agent.isOnOffMeshLink)
                    {
                        StartCoroutine(Jump());
                        agent.isStopped = true;
                    }
                    else
                    {
                        agent.SetDestination(PLAYER.transform.position);
                        float dist = Vector3.Distance(transform.position, PLAYER.transform.position);
                        if (dist >= 10)
                        {
                            if (agent.path.status != NavMeshPathStatus.PathPartial)
                            {

                                anim.SetBool("isMoving", true);
                                agent.isStopped = false;
                                agent.speed = 10;
                                if (PLAYER.transform.position.y >= transform.position.y + 2)
                                {
                                    anim.SetBool("isMoving", false);
                                    anim.SetBool("isAiming", true);
                                    agent.isStopped = true;
                                    enemyGun.Fire();
                                }
                            }
                        }
                        else if (dist >= 6)
                        {
                            if (agent.path.status != NavMeshPathStatus.PathPartial)
                            {

                                anim.SetBool("isMoving", true);
                                agent.isStopped = false;
                                agent.speed = 3.5f;
                                if (PLAYER.transform.position.y >= transform.position.y + 2)
                                {
                                    anim.SetBool("isMoving", false);
                                    anim.SetBool("isAiming", true);
                                    agent.isStopped = true;
                                    enemyGun.Fire();
                                }
                            }
                        }
                        else if (dist <= 5 && dist >= 2)
                        {
                            anim.SetBool("isMoving", false);
                            anim.SetBool("isAiming", true);
                            agent.isStopped = true;
                            enemyGun.Fire();
                        }
                        else
                        {
                            GetComponent<Rigidbody>().AddForce(Vector3.back * 10);
                        }
                    }
                }
                else
                {
                    Vector3 lookPOS = (PLAYER.transform.position - transform.position).normalized;
                    Quaternion lookROT = Quaternion.LookRotation(lookPOS);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookROT, Time.deltaTime * 10);
                    Transform rightArm = GameObject.Find("rEnemy/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm").GetComponent<Transform>();
                    Transform leftArm = GameObject.Find("rEnemy/Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm").GetComponent<Transform>();
                    rightArm.LookAt(PLAYER.transform.position);
                    leftArm.LookAt(PLAYER.transform.position);
                    enemyGun.gameObject.transform.LookAt(PLAYER.transform.position);
                }
            }
        }
        else if (name == "BossEnemy")
        {
            if (anim.GetBool("isScreaming") == false)
            {
                if (HP <= 0)
                {
                    anim.SetBool("isMoving", false);
                    anim.SetBool("isDead", true);
                    agent.isStopped = true;
                    GetComponent<Rigidbody>().isKinematic = true;
                    Destroy(GetComponent<CapsuleCollider>());
                    Destroy(GetComponentInChildren<SphereCollider>());
                    gameObject.layer = 0;
                    if (enemyGun != null)
                    {
                        enemyGun.GetComponent<BoxCollider>().isTrigger = true;
                        enemyGun.transform.parent = null;
                    }
                    StartCoroutine(EneymyDie());
                    //transform.LookAt(new Vector3(0, 0, 0));
                }
                else
                {
                    if (anim.GetBool("isAttacking") == false)
                    {
                        agent.SetDestination(PLAYER.transform.position);
                        float dist = Vector3.Distance(transform.position, PLAYER.transform.position);
                        if (dist >= 10)
                        {
                            if (agent.path.status != NavMeshPathStatus.PathPartial)
                            {
                                anim.SetBool("isMoving", true);
                                anim.SetFloat("Blend", 2.5f);
                                agent.isStopped = false;
                                agent.speed = 10;
                            }
                            else
                            {
                                anim.SetBool("isMoving", false);
                            }
                        }
                        else if (dist >= 3)
                        {
                            if (agent.path.status != NavMeshPathStatus.PathPartial)
                            {
                                anim.SetBool("isMoving", true);
                                anim.SetFloat("Blend", 1.5f);
                                agent.isStopped = false;
                                agent.speed = 3.5f;
                            }
                            else
                            {
                                anim.SetBool("isMoving", false);
                            }
                        }
                        else
                        {
                            Vector3 lookPOS = (PLAYER.transform.position - transform.position).normalized;
                            Quaternion lookROT = Quaternion.LookRotation(lookPOS);
                            transform.rotation = Quaternion.Slerp(transform.rotation, lookROT, Time.deltaTime * 10);
                            agent.isStopped = true;
                            if (anim.GetBool("isMoving") == true)
                                anim.SetBool("isMoving", false);
                            if (anim.GetBool("isAttacking") == false)
                            {
                                agent.isStopped = true;
                                StartCoroutine(Meele());
                            }
                        }
                    }
                }
            }
        }
    }
    /*private void OnGUI()
    {
        GUI.Box(new Rect(transform.position.x, transform.position.y + 100, 150, 25),
            string.Format("HP/MaxHP " + HP + "/" + maxHP));
    }*/

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.name == "BossSpawn")
        {
            if (justSpawn)
            {
                justSpawn = false;
                StartCoroutine(bossSpawn());
            }
        }
    }

    IEnumerator Meele()
    {
        anim.SetBool("isAttacking", true);
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(hand.position, fwd * 2f, Color.red, .3f);
        if (Physics.Raycast(hand.position, fwd, out hit, 2f))
        {
            switch (hit.collider.gameObject.layer)
            {
                case 9:
                    if (name == "BossEnemy")
                    {
                        hit.collider.gameObject.GetComponent<PlayerCont>().HP -= damage;
                        PLAYER.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 30000);
                        PLAYER.gameObject.GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * 150000);
                    }
                    else
                    {
                        hit.collider.gameObject.GetComponent<PlayerCont>().HP -= damage;
                        PLAYER.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 15000);
                        PLAYER.gameObject.GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * 75000);
                    }
                    break;
                case 11:
                    break;
            }
        }
        yield return new WaitForSeconds(.9f);
        anim.SetBool("isAttacking", false);
        yield return new WaitForSeconds(3);
        agent.isStopped = false;
    }

    IEnumerator EneymyDie()
    {
        yield return new WaitForSeconds(2.5f);
        anim.SetBool("isDead", false);
        yield return new WaitForSeconds(10);
        GD.enemies--;
        if (enemyGun != null)
        {
            enemyGun.GetComponent<BoxCollider>().isTrigger = true;
            enemyGun.transform.parent = null;
        }
        if (GD.enemies >= 0)
        {
            GD.oneShip = false;
            GD.shipStopped = false;
        }
        if (name == "BossEnemy")
        {
            GameObject.Find("bossMusic").GetComponent<AudioSource>().Stop();
            GameObject.Find("mainMusic").GetComponent<AudioSource>().Play();
        }
        Destroy(gameObject);
    }

    IEnumerator getUp()
    {
        Debug.Log("Started corotuoine");
        justSpawn = false;
        yield return new WaitForSeconds(3);
        /*Rigidbody[] RBs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in RBs)
        {
            rb.isKinematic = true;
        }
        //gameObject.AddComponent<Rigidbody>();*/
        anim.enabled = true;
        anim.SetTrigger("getUp");
    }

    IEnumerator Jump()
    {
        anim.SetBool("isJumping", true);
        yield return new WaitForSeconds(1.2f);
        anim.SetBool("isJumping", false);
        agent.isStopped = false;
    }

    IEnumerator bossSpawn()
    {
        if (tag == "Boss")
        {
            PMS.canMove = false;
            anim.SetBool("isScreaming", true);
            GetComponent<AudioSource>().volume = 200;
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(2.5f);
            anim.SetBool("isScreaming", false);
            PMS.canMove = true;
        }
    }
}
