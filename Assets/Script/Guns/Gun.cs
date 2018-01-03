using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour {
	protected Bullet bullet;
    public string gunName;
	protected char size;
	protected float range;
	protected int framesPerShot;
	protected float angSpeed;

	bool isEnabled;
	GameObject target;
	float targetDist;

	bool locked = false;
	bool firing = false;
	
	// Update is called once per frame
	void FixedUpdate () {
		// If disabled, don't bother animating
		if (isEnabled == false) return;
		
    	if (target != null) {
    		// Distance between gun and target
			targetDist = (target.transform.position - transform.position).magnitude;

			// Rotate to point at target
    		rotate();
            shoot();

    	} else {
    		alignWithShip();
    		rotation = -1; // mark rotation as linked to ship
    	}
	}

	float rotation;
	void rotate() {
		// Sets rotation to what it was last frame: negates effect of parent rotating child
		if (rotation != -1) { // negatives not possible for eulerAngle, thus means null
			transform.eulerAngles = new Vector3(0, 0, rotation);
		}

		float gunAngle = transform.eulerAngles.z;
		if (gunAngle > 180) {
			gunAngle -= 360;
		}

        Vector2 targetDiff = new Vector2(target.transform.position.x - transform.position.x,
        	target.transform.position.y - transform.position.y); // Destination in relation to gun

        float targetAngle = Vector2.SignedAngle(Vector2.up, targetDiff);
        
        float angleDiff = targetAngle - gunAngle;
        
        // The flip from -180 to +180 flips the correct turn direction, below block fixes
        if (Mathf.Abs(angleDiff) > 180) { // It crosses over -180 -> +180 flip
            angleDiff *= -1; // Turn against the expected angleDiff
        }

        // If super close to correct angle, just snap to correct angle
        if (Mathf.Abs(angleDiff) < angSpeed) {
        	transform.eulerAngles = new Vector3(transform.eulerAngles.x,
        		transform.eulerAngles.y, targetAngle);
        	locked = true;

        } else if (angleDiff < 0) {
            // Turn negative (clockwise)
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - angSpeed);
            locked = false;
        } else if (angleDiff > 0) {
            // Turn positive (counterclockwise)
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + angSpeed);
            locked = false;
        } else {
        	locked = false;
        }

        rotation = transform.eulerAngles.z; // Record rotation
    }

    void alignWithShip () {
    	if (transform.localEulerAngles.z == 0) {
    		return;

    	} else if (transform.localEulerAngles.z < angSpeed || transform.localEulerAngles.z + angSpeed > 360) {
    		transform.localEulerAngles = Vector3.zero;
    		return;

    	} else if (transform.localEulerAngles.z < 180) {
    		transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z - angSpeed);
    	
    	} else if (transform.localEulerAngles.z > 180) {
    		transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + angSpeed);
    	}
    }

	public void setTarget (GameObject newTarget) {
		target = newTarget;
	}

	public bool isFiring () {
		return firing;
	}

	// Fires a bullet in the direction of target
	int framesUntilShoot = 0;
	void shoot () {
		if (framesUntilShoot <= 0) {
			if (targetDist <= range && locked) {
				Bullet newBullet = Instantiate(bullet, transform.position, transform.rotation);
				newBullet.setDestination(target.transform, transform.parent.GetComponent<Ship>().enemy);
				framesUntilShoot = framesPerShot;
			}
        } else {
        	framesUntilShoot--;
        }

        firing = (targetDist <= range && locked);
	}

	public void disable () {
		isEnabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
	}

	public void enable () {
		isEnabled = true;
		GetComponent<SpriteRenderer>().enabled = true;
	}
}
