using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	Vector3 destination;
	Vector3 origin;
	Vector3 travelVector;
	bool shot = false;

	// Use this for initialization
	void Start () {
		origin = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Move towards destination if it has been set
		if (shot) {
			transform.position += travelVector * BulletVars.flightSpeed;
		}
	}

	// Set destination and prepare flight
	public void setDestination (Vector3 newDest) {
		destination = newDest;

		// Find path that must be travelled
		travelVector = new Vector3(destination.x - origin.x, destination.x - origin.x, 0);
		// Make vector normalized to act as an angle unit vector
		travelVector.Normalize();

		shot = true;
	}

	public void disable() {
		GetComponent<SpriteRenderer>().enabled = false;
	}

	public void enable() {
		GetComponent<SpriteRenderer>().enabled = true;
	}
}
