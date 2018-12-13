using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class GunControl : MonoBehaviour
{

    public Vector3 aim;
    public Vector3 sideH;
    public int ammo;
    public int clip;
    public float aimSpeed = 40;

    public LayerMask enemyLM;
    public LayerMask headLM;
    public GameObject fpsCont;
    public GameObject barrel;
    public float gunRange;

 
    private bool canFire = true;


    // Use this for initialization
    void Start()
    {
        sideH = new Vector3(.56f, -.3f, .82f);
        aim = new Vector3(-.01f, -.05f, .82f);
        transform.localPosition = sideH;
        ammo = 180;
        clip = 9;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (ammo >= 1)
            {
                Debug.Log("Can add cool reload thing here!");
                int takeFromAmmo = 9 - clip;
                if(ammo <= -1)
                {
                    ammo = 0;
                }
                else if(ammo >= 0 && ammo <= 8)
                {
                    clip += ammo;
                    ammo = 0;
                }
                else
                {
                    ammo -= takeFromAmmo;
                    clip += takeFromAmmo;

                }
            }
            else
            {
                Debug.Log("need more ammo!");
            }

        }
        if (Input.GetButtonDown("Fire2"))
        {
            transform.localPosition = aim;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            transform.localPosition = sideH;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (clip > 0)
            {
                if (canFire)
                {
                    clip--;

                    RaycastHit hit;

                    Vector3 fwd = barrel.transform.TransformDirection(Vector3.forward);
                    Debug.DrawRay(barrel.transform.position, fwd * gunRange, Color.red);
                    if (Physics.Raycast(transform.position, fwd, out hit, gunRange, enemyLM))
                    {
                        int number1 = Random.Range(0, 250);
                        int number2 = Random.Range(0, 250);
                        int number3 = Random.Range(0, 250);

                        Debug.Log("Hit enemy");
                        if (hit.transform.gameObject.GetComponent<Renderer>() != null)
                        {
                            hit.transform.gameObject.GetComponent<Renderer>().material.color =
                                            new Color32((byte)number1, (byte)number2, (byte)number3, 255);
                        }
                        else
                        {
                            hit.collider.gameObject.GetComponent<Animator>().SetBool("isDead", true);
                            hit.collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                            hit.collider.gameObject.gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
                        }
                    }
                    if (Physics.Raycast(transform.position, fwd, out hit, gunRange, headLM))
                    {
                        Debug.Log("Head Shot!");
                        hit.collider.gameObject.GetComponentInParent<Animator>().SetBool("isDead", true);
                        hit.collider.gameObject.GetComponentInParent<Rigidbody>().isKinematic = true;
                        hit.collider.gameObject.GetComponent<ParticleSystem>().Play();
                    }
                }
            }
            else
            {
                Debug.Log("You need to reload, press R to do so");
            }
        }
    }

    IEnumerator recoil()
    {

        float recoilAroundX = -500;

        Quaternion startRot = Quaternion.Euler(transform.rotation.x, 0,0);

        Quaternion target = Quaternion.Euler(recoilAroundX, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 20);
        canFire = false;

        yield return new WaitForSeconds(2);

        transform.rotation = Quaternion.Slerp(transform.rotation, startRot, Time.deltaTime * 50);
        canFire = true;

    }
}
