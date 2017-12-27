using UnityEngine;
using System.Collections;

public abstract class Ship : MonoBehaviour {
    // Whatever ship is currently selected
    public static Ship activeShip;

    // Ship and related ship metrics and controls
    Rigidbody2D ship;
    public bool enemy; // true if enemy ship, false if allied
    public bool isEnabled = false;

    // Constant variables set by ship type class
    protected float maxSpeed; 		// Max speed of ship
    protected int accelFrames; 		// Number of frames needed to hit max speed
    protected float angSpeed; 		// Degrees/second
    protected int ticsPerTrailSwap; 	// How many tics before swapping trails
    protected int numGunsS;
    protected int numGunsM;
    protected int numGunsL;
    
    // Destination and related destination stuff
    Vector2 destination;
    
    // Trail details
    protected GameObject[] trails;
    protected int numTrails;
    protected int activeTrail; // index of active trail, == numTrails for none
    bool isThrust;

    // Gun details
    protected Gun[] gunsS;
    protected Gun[] gunsM;
    protected Gun[] gunsL;
    GameObject target;

    protected GameObject border;

    public abstract void newShip();

    void Awake () {
    	setup();
    }

    protected void setup() {
    	ship = GetComponent<Rigidbody2D>();
    	destination = ship.position;
    }
    
    void FixedUpdate() {
    	// Disabled ships do not need to move or do anything
    	if (isEnabled == false) return;

        if (ship.rotation > 180) {
            ship.rotation -= 360;
        } else if (ship.rotation < -180) {
            ship.rotation += 360;
        }
        
        if (destination != ship.position) {
            move();
        }

        border.GetComponent<SpriteRenderer>().enabled = (Ship.activeShip == this);
    }
    
    private void move() {
        if (ship.position != destination) {
            rotate(); // Handles ship rotation
            thrust(); // Handlds ship movement
            trail(); // Handles making cool bubble trails
        }
    }
    
    Vector2 rotationVector; // Unit vector in direction of rotation
    float destinationAngle; // The angle of our destination
    private void rotate() {
        Vector2 destinationDiff = destination - ship.position; // Destination in relation to ship
        destinationAngle = Vector2.SignedAngle(Vector2.up, destinationDiff);
        
        float heading = destinationAngle - ship.rotation;
        
        // The flip from -180 to +180 flips the correct turn direction, below block fixes
        if (Mathf.Abs(heading) > 180) { // It crosses over -180 -> +180 flip
            heading *= -1; // Turn against the expected heading
        }

        // If super close to correct angle, just snap to correct angle
        if (Mathf.Abs(heading) < 1.5) {
            ship.rotation = destinationAngle;
            ship.angularVelocity = 0;
        
        } else if (heading < 0) {
            // Turn negative (clockwise)
            ship.angularVelocity = -angSpeed * (ship.velocity.magnitude / maxSpeed);
        } else if (heading > 0) {
            // Turn positive (counterclockwise)
            ship.angularVelocity = angSpeed * (ship.velocity.magnitude / maxSpeed);;
        } else {
            ship.angularVelocity = 0;
        }
    }

    private void thrust() {
        // Calculate our rotationVector
        rotationVector.x = -Mathf.Sin(ship.rotation * Mathf.Deg2Rad);
        rotationVector.y = Mathf.Cos(ship.rotation * Mathf.Deg2Rad);
        rotationVector.Normalize();
        
        Vector2 destinationDiff = destination - ship.position; // Destination in relation to ship
        float distance = destinationDiff.magnitude;
        
        // If extremely close to destination, snap to destination and stop all motion
        if (distance < .01) {
            isThrust = false;
            ship.position = destination;
            ship.velocity = Vector2.zero;
            ship.angularVelocity = 0;
        
        // If close to destination, slow movement
        } else if (distance < 2) {
            isThrust = (distance > 1); // If distance is greater than one, thrust is on
            ship.velocity = ((.75f/2 * distance + .25f) * maxSpeed) * rotationVector;

        } else {
            // Move forward
            isThrust = true;
            if (ship.velocity.magnitude < maxSpeed) {
                Vector2 newVelocity = ship.velocity.magnitude * rotationVector + (maxSpeed
                	/ accelFrames) * rotationVector;
                if (newVelocity.magnitude > maxSpeed) {
                    newVelocity = maxSpeed * rotationVector;
                }
                ship.velocity = newVelocity;
            } else {
                Vector2 newVelocity = maxSpeed * rotationVector;
                ship.velocity = newVelocity;
            }
        }
    }
    
    // Handles bubble trail behind ship
    int ticsPerSwap = 0;
    private void trail() {
        if (isThrust) {
        	// If no trails active, set last trail to active so the first trail will be activated next
        	if (activeTrail == numTrails) activeTrail--;
        	// If its been ticsPerSwap tics, swap trail
            if (ticsPerSwap >= ticsPerTrailSwap) {
                ticsPerSwap = 0;
                // Disable active trail
                trails[activeTrail++].GetComponent<SpriteRenderer>().enabled = false;
                if (activeTrail >= numTrails) activeTrail = 0;
                // Enabled new trail
                trails[activeTrail].GetComponent<SpriteRenderer>().enabled = true;
            } else {
                ticsPerSwap++;
            }
        // If no thrust, don't show either trail
        } else {
        	if (activeTrail < numTrails) {
            	trails[activeTrail].GetComponent<SpriteRenderer>().enabled = false;
            }
            // activeTrail == numTrails means no trail active
            activeTrail = numTrails;
        }
    }
    
    // Set the destination for the boat to move to 
    public void setDestination (Vector2 newDestination) {
        destination = newDestination;
    }
    
    // Checks whether or not a given point is on our ship
    public bool pointOnShip (Vector3 point) {
        Collider2D[] results = new Collider2D[ship.attachedColliderCount];
        ship.GetAttachedColliders(results);
        return results[0].bounds.Contains(point);
    }

    // Instructs guns to target a given GameObject
    public void setTarget (GameObject newTarget) {
    	target = newTarget;
    	for (int i = 0; i < numGunsS; i++) {
    		gunsS[i].setTarget(newTarget);
    	}
    	for (int i = 0; i < numGunsM; i++) {
    		gunsM[i].setTarget(newTarget);
    	}
    	for (int i = 0; i < numGunsL; i++) {
    		gunsL[i].setTarget(newTarget);
    	}
    }

    // Returns ship GameObject
    public GameObject shipGameObject () {
    	return ship.gameObject;
    }

    // Disables boat: makes invisible and halts "FixedUpdate" method
    public void disable () {
    	isEnabled = false;
    	ship.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Makes visible again and restarts "FixedUpdate" method
    public void enable () {
    	isEnabled = true;
    	ship.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    	for (int i = 0; i < numGunsS; i++) {
    		gunsS[i].enable();
    	}
    	for (int i = 0; i < numGunsM; i++) {
    		gunsM[i].enable();
    	}
    	for (int i = 0; i < numGunsL; i++) {
    		gunsL[i].enable();
    	}
    	
    	destination = ship.position;
	}
}
