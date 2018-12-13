using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipControl : MonoBehaviour {
    public int speed = 10;

    private Rigidbody RB;
    private GameData GD;
    private Light spawnLight;

	// Use this for initialization
	void Start () {
        RB = GetComponent<Rigidbody>();
        GD = GameObject.Find("GameManager").GetComponent<GameData>();
        spawnLight = GetComponentInChildren<Light>();
        spawnLight.spotAngle = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (!GD.shipStopped)
            RB.AddForce(Vector3.forward * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "shipStop")
        {
            GD.shipStopped = true;
            spawnLight.spotAngle = 49;
            RB.velocity = Vector3.zero;
        }
    }
}
