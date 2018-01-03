using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour {
	Vector3 destination;
	Vector3 origin;
	Vector3 travelVector;
	bool shot = false;
	bool enemy;

	protected float damage;
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
			if (checkCollision()) {
				destroyBullet();
			}
		
		} else {
			destroyBullet();
		}
	}

	// Check if collided with enemy or ally
	bool checkCollision() {
		if (enemy) {
			Ship hitShip = Utility.onShip(transform.position);
			if (hitShip != null) {
				hitShip.damage(damage);
				return true;
			} else {
				return false;
			}

		} else {
			Ship hitEnemy = Utility.onEnemy(transform.position);
			if (hitEnemy != null) {
				hitEnemy.damage(damage);
				return true;
			
			} else {
				return false;
			}
		}
	}

	void destroyBullet () {
		Destroy(GetComponent<Transform>().gameObject);
	}

	// Set destination and prepare flight
	// Vector3 newDest: position of enemy at t=0
	// bool enemy: if bullet is allied or enemy 
	public void setDestination (Transform newDest, bool firedFromEnemy) {
		origin = transform.position;
		destination = newDest.position;
		enemy = firedFromEnemy;

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