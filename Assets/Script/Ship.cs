using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
    // Whatever ship is currently selected
    public static Ship activeShip;
    public bool enemy;
    
    // Ship and related ship metrics and controls
    public Rigidbody2D ship;
    Vector2 rotationVector; // Unit vector in direction of rotation
    bool isEnabled;
    
    // Destination and related destination stuff
    Vector2 destination;
    float destinationAngle;
    
    // Const statistics of individual ship
    const float maxSpeed = 1.0f;
    const int accelFrames = 60; // Number of frames needed to hit max speed
    const float maxAngSpeed = 60.0f; // Degrees/second
    const int ticsPerTrailSwap = 10;
    
    // Trail details
    GameObject bubbleTrail1;
    GameObject bubbleTrail2;
    SpriteRenderer bubbleSpriteR1;
    SpriteRenderer bubbleSpriteR2;
    bool isThrust;
    static int ticsPerSwap = 0;
    static int activeTrail = 0; // 1 for bubbleTrail1, 2 for 2, 0 for neither

    // Gun details
    public Gun smallGun;
    public Gun mediumGun;
    GameObject gunTarget;
    
    void Start () {
    	// Set up ship details
        ship = GetComponent<Rigidbody2D>();
        destination = ship.position;
        if (!enemy) activeShip = this;

        // Record bubble trails
        bubbleTrail1 = transform.GetChild(0).gameObject;
        bubbleSpriteR1 = bubbleTrail1.GetComponent<SpriteRenderer>();
        bubbleTrail2 = transform.GetChild(1).gameObject;
        bubbleSpriteR2 = bubbleTrail2.GetComponent<SpriteRenderer>();

        // Record ship gun details
        gunTarget = null;

        enable();
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
    }
    
    private void move() {
        if (ship.position != destination) {
            rotate(); // Handles ship rotation
            thrust(); // Handlds ship movement
            trail(); // Handles making cool bubble trails
        }
    }
    
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
            ship.angularVelocity = -maxAngSpeed * (ship.velocity.magnitude / maxSpeed);
        } else if (heading > 0) {
            // Turn positive (counterclockwise)
            ship.angularVelocity = maxAngSpeed * (ship.velocity.magnitude / maxSpeed);;
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
                Vector2 newVelocity = ship.velocity.magnitude * rotationVector + (maxSpeed / accelFrames) * rotationVector;
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
    private void trail() {
        if (isThrust) {
        	// If its been ticsPerSwap tics, swap trail
            if (ticsPerSwap >= ticsPerTrailSwap) {
                ticsPerSwap = 0;
                if (activeTrail == 1 || activeTrail == 0) {
                    bubbleSpriteR1.enabled = false;
                    bubbleSpriteR2.enabled = true;
                    activeTrail = 2;
                } else {
                    bubbleSpriteR1.enabled = true;
                    bubbleSpriteR2.enabled = false;
                    activeTrail = 1;
                }
            } else {
                ticsPerSwap++;
            }
        // If no thrust, don't show either trail
        } else {
            activeTrail = 0;
            bubbleSpriteR1.enabled = false;
            bubbleSpriteR2.enabled = false;
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
    public void setGunTarget (GameObject newGunTarget) {
    	gunTarget = newGunTarget;
    	smallGun.setTarget(gunTarget);
    	mediumGun.setTarget(gunTarget);
    }

    // Returns ship GameObject
    public GameObject shipGameObject () {
    	return ship.gameObject;
    }

    // Disables boat: makes invisible and halts "FixedUpdate" method
    public void disable () {
    	isEnabled = false;
    	ship.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    	bubbleSpriteR1.enabled = false;
    	bubbleSpriteR2.enabled = false;
    	smallGun.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    	mediumGun.gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Makes visible again and restarts "FixedUpdate" method
    public void enable () {
    	isEnabled = true;
    	ship.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    	smallGun.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    	mediumGun.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
}
