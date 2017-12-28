using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
	bool isEnabled;
	GameObject target;
	float targetDist;

	char size;
	float angSpeed;

	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// If disabled, don't bother animating
		if (isEnabled == false) return;

    	if (target != null) {
    		rotate();
    	} else {
    		rotation = -1; // mark rotation as linked to ship
    	}
	}

	float rotation;
	private void rotate() {
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

        } else if (angleDiff < 0) {
            // Turn negative (clockwise)
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - angSpeed);
        } else if (angleDiff > 0) {
            // Turn positive (counterclockwise)
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + angSpeed);
        }

        rotation = transform.eulerAngles.z; // Record rotation
    }

	public void setTarget (GameObject newTarget) {
		target = newTarget;
	}

	public void disable () {
		isEnabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
	}

	public void enable () {
		isEnabled = true;
		GetComponent<SpriteRenderer>().enabled = true;
	}

	// Initialize methods, sets up specifics for each new gun
	public void newGun(GunType gunType) {
		size = gunType.size;
		angSpeed = gunType.angSpeed;
	}
}
