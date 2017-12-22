using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
	public Transform gun;
	GameObject target;
	float targetDist;

	const float angSpeed = 3.0f;

	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
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
			gun.transform.eulerAngles = new Vector3(0, 0, rotation);
		}

		float gunAngle = gun.transform.eulerAngles.z;
		if (gunAngle > 180) {
			gunAngle -= 360;
		}

        Vector2 targetDiff = new Vector2(target.transform.position.x - gun.transform.position.x,
        	target.transform.position.y - gun.transform.position.y); // Destination in relation to gun

        float targetAngle = Vector2.SignedAngle(Vector2.up, targetDiff);
        
        float angleDiff = targetAngle - gunAngle;
        
        // The flip from -180 to +180 flips the correct turn direction, below block fixes
        if (Mathf.Abs(angleDiff) > 180) { // It crosses over -180 -> +180 flip
            angleDiff *= -1; // Turn against the expected angleDiff
        }

        // If super close to correct angle, just snap to correct angle
        if (Mathf.Abs(angleDiff) < angSpeed) {
        	gun.transform.eulerAngles = new Vector3(gun.transform.eulerAngles.x,
        		gun.transform.eulerAngles.y, targetAngle);

        } else if (angleDiff < 0) {
        	Debug.Log("<");
            // Turn negative (clockwise)
            gun.transform.eulerAngles = new Vector3(0, 0, gun.transform.eulerAngles.z - angSpeed);
        } else if (angleDiff > 0) {
        	Debug.Log(">");
            // Turn positive (counterclockwise)
            gun.transform.eulerAngles = new Vector3(0, 0, gun.transform.eulerAngles.z + angSpeed);
        }

        rotation = gun.transform.eulerAngles.z; // Record rotation
    }

	public void setTarget (GameObject newTarget) {
		target = newTarget;
	}
}
