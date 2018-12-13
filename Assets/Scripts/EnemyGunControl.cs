using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class EnemyGunControl : MonoBehaviour
{
    public Vector3 aim;
    public float fireRate = .25f;
    public int damage;
    public Vector3 sideH;
    public float aimSpeed = 40;
    public float gunRange;
    public GameObject barrel;

    private PlayerCont PC;
    private float nextFireTime;
    private EnemyControl EC;


    // Use this for initialization
    void Start()
    {
        sideH = transform.position;
        aim = new Vector3(-.33f, -.38f, .8f);
        //transform.localPosition = sideH;
        PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCont>();
        EC = GetComponentInParent<EnemyControl>();
    }

    // Update is called once per frame
    public void Fire()
    {
        //int hitC = Random.Range(0, 4);
        if (Time.time > nextFireTime)
        {
            StartCoroutine(burstFire());
            nextFireTime = Time.time + fireRate;
        }
    }
    public void Update()
    {
        nextFireTime -= Time.deltaTime;
    }

    public void Aim(bool isAim)
    {
        if (isAim == true)
            transform.localPosition = aim;
        else
            transform.localEulerAngles = sideH;

    }

    IEnumerator burstFire()
    {
        RaycastHit hit;

        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(barrel.transform.position, fwd * gunRange, Color.red,2f);
        if (Physics.Raycast(barrel.transform.position, fwd, out hit))
        {
            switch (hit.collider.gameObject.layer)
            {
                case 9:
                    int hitChance = Random.Range(0, 3);
                    if (hitChance == 1 || hitChance == 3)
                    {
                        PC.HP -= damage;
                    }
                    break;
                case 11:
                    break;
            }            
        }
        yield return new WaitForSeconds(2);
        EC.agent.isStopped = false;
        EC.anim.SetBool("isAiming", false);
        
    }
}
