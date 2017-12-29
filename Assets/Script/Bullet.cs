using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour {
	Vector3 destination;
	Vector3 origin;
	Vector3 travelVector;
	bool shot = false;

	protected float flightSpeed;
	float framesLeft;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!shot) return;

		// Move towards destination if it has been set
		if (framesLeft >= 0) {
			transform.position += travelVector * flightSpeed;
			framesLeft -= 1;
		
		} else {
			disable();
			Destroy(GetComponent<SpriteRenderer>());
			Destroy(this);
		}
	}

	// Set destination and prepare flight
	public void setDestination (Vector3 newDest) {
		origin = transform.position;
		destination = newDest;

		// Find path that must be travelled
		travelVector = new Vector3(destination.x - origin.x, destination.y - origin.y, 0);
		float range = travelVector.magnitude;
		// Make vector normalized to act as an angle unit vector
		travelVector.Normalize();

		framesLeft = range / flightSpeed + GunVars.overShoot;
		shot = true;
		enable();
	}

	public void disable() {
		shot = false;
		GetComponent<SpriteRenderer>().enabled = false;
	}

	public void enable() {
		GetComponent<SpriteRenderer>().enabled = true;
	}
}