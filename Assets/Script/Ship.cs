using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
    // Whatever ship is currently selected
    public static Ship activeShip;

    // Ship and related ship metrics and controls
    Rigidbody2D ship;
    public bool enemy; // true if enemy ship, false if allied
    public bool isEnabled = false;
    public float maxHealth;
    public float health;
    public float healthPercent;

    // Constant variables set by ship type class
    float maxSpeed; 		// Max speed of ship
    int accelFrames; 		// Number of frames needed to hit max speed
    float angSpeed; 		// Degrees/second
    int ticsPerTrailSwap; 	// How many tics before swapping trails
    int numGunsS;
    int numGunsM;
    int numGunsL;
    
    // Destination and related destination stuff
    Vector2 destination;
    bool tracking; // If true, we are moving towards target and not destination
    GameObject target;
    Vector2 rotationVector; // Unit vector in direction of rotation
    
    // Trail details
    GameObject[] trails;
    int numTrails;
    int activeTrail; // index of active trail, == numTrails for none
    bool isThrust;

    // Gun details
    Gun[] gunsS;
    Gun[] gunsM;
    Gun[] gunsL;
    float autoRange;

    GameObject border;
    GameObject healthBar;
    Vector3 healthLoc;
    float maxHealthWidth;

    void Awake () {
    	setup();
    }

    void Start () {
		destination = ship.position;
    }

    void setup() {
    	ship = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate() {
    	// Disabled ships do not need to move or do anything
    	if (isEnabled == false) return;

        if (ship.rotation > 180) {
            ship.rotation -= 360;
        } else if (ship.rotation < -180) {
            ship.rotation += 360;
        }
        
        move();
        setTargetNearest();
        handleBorder();
        handleHealthBar();
    }
    
    private void move() {
    	if (tracking) {
    		bool firing = isFiring();
    		if (!firing) {
    			destination = target.transform.position;
    			isThrust = true;
    		} else {
    			destination = ship.position;
    		}
    	}

        if (isThrust) {
            rotate(); // Handles ship rotation
            thrust(); // Handlds ship movement
        } else {
        	slow();
        }

        trail();
    }
    
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
        if (distance < .1) {
        	isThrust = false;
            destination = ship.position;
            ship.angularVelocity = 0;
        
        } else {
            // Move forward
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
    private void trail () {
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

    // Slows ship after crossing destination
    void slow () {
    	if (ship.velocity == Vector2.zero) return;

    	ship.velocity -= rotationVector * (maxSpeed / accelFrames);

    	if (ship.velocity.x * rotationVector.x < 0) { // If facing different directions
    		ship.velocity = new Vector2(0.0f, ship.velocity.y);
    	}
    	if (ship.velocity.y * rotationVector.y < 0) { // If facing different directions
    		ship.velocity = new Vector2(ship.velocity.x, 0.0f);
    	}

    	destination = ship.position;
    }
    
    void handleBorder () {
		if (Ship.activeShip == this) {
        	border.GetComponent<SpriteRenderer>().enabled = true;
        	border.GetComponent<SpriteRenderer>().color = healthColor();
        	healthBar.GetComponent<SpriteRenderer>().enabled = false;
        } else {
        	border.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void handleHealthBar () {
        // Health bar on if health is not full
        if (health == maxHealth) {
        	healthBar.GetComponent<SpriteRenderer>().enabled = false;
        	return;
        
        } else {
       		healthBar.GetComponent<SpriteRenderer>().enabled = (health != maxHealth);
       		healthBar.GetComponent<SpriteRenderer>().color = healthColor();
       		healthBar.transform.position = this.transform.position + healthLoc;
       		healthBar.transform.eulerAngles = Vector3.zero;
       		// Health bar shrinks as damage is taken
       		healthBar.transform.localScale = new Vector3(healthPercent * maxHealthWidth,
       		healthBar.transform.localScale.y, healthBar.transform.localScale.z);
       	}
    }

    // Set the destination for the boat to move to 
    public void setDestination (Vector2 newDestination) {
    	tracking = false;
    	isThrust = true;
        destination = newDestination;
    }
    
    // Checks whether or not a given point is on our ship
    public bool pointOnShip (Vector3 point) {
        Collider2D collider = GetComponent<Collider2D>();
        return collider.bounds.Contains(point);
    }

    // Instructs guns to target a given GameObject
    public void setTarget (GameObject newTarget) {
    	tracking = true;
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

    // Targets the nearest boat without interrupting steering
    public void setTargetNearest () {
    	// If tracking a specific ship, don't autotarget
    	if (tracking && target != null) return;

    	// Find the closest ship that is the opposite faction as this
    	float closestDist = autoRange;
    	GameObject closest = null;;
    	if (enemy) {
    		for (int i = 0; i < Objects.numShips; i++) {
    			float dist = Mathf.Abs((transform.position - Objects.allShips[i].shipGameObject().transform.position).magnitude);
    			
    			if (closest == null || dist < closestDist) {
    				closestDist = dist;
    				closest = Objects.allShips[i].shipGameObject();
    			}
    		}
    	
    	} else {
    		for (int i = 0; i < Objects.numEnemies; i++) {
    			float dist = Mathf.Abs((transform.position - Objects.allEnemies[i].shipGameObject().transform.position).magnitude);
    			
    			if (closest == null || dist < closestDist) {
    				closestDist = dist;
    				closest = Objects.allEnemies[i].shipGameObject();
    			}
    		}
    	}
    	// If no opposition exists, closest == null, which sets guns to be off

    	// If nothing in decent range, don't focus on anything
    	if (closestDist > autoRange) {
    		closest = null;
    	}

    	setTarget(closest);
    	tracking = false; // Above method sets tracking to true, but autotarget should keep previous heading
    }

    bool isFiring () {
    	for (int i = 0; i < numGunsS; i++) {
    		if (gunsS[i].isFiring()) {
    			return true;
    		}
    	}
    	for (int i = 0; i < numGunsM; i++) {
    		if (gunsM[i].isFiring()) {
    			return true;
    		}
    	}
    	for (int i = 0; i < numGunsL; i++) {
    		if (gunsL[i].isFiring()) {
    			return true;
    		}
    	}
    	return false;
    }

    // Returns ship GameObject
    public GameObject shipGameObject () {
    	return ship.gameObject;
    }

    // Returns if ship is destroyed or not
    public void damage (float damage) {
    	health -= damage;
    	healthPercent = health / maxHealth;
    	if (health <= 0) destroyShip();
    }

    void destroyShip () {
    	// Remove ship from global arrays
    	if (enemy) {
    		Objects.removeEnemy(this);
    	} else if (!enemy) {
    		Objects.removeShip(this);
    	}

    	Destroy(ship.gameObject);
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

	/*
	 * white: 100% health exactly
	 * green: very high health
	 * yellow: half health
	 * red: very low health
	 * Colors change gradually from one to another as health drops (except white is only exactly 100%)
	 */
	public Color healthColor () {
		if (health == maxHealth) return Color.white;
		if (healthPercent > 0.5f) {
			return Color.Lerp(Color.yellow, Color.green, (healthPercent - 0.5f) * 2);
		} else {
			return Color.Lerp(Color.red, Color.yellow, healthPercent * 2);
		}

	}

	// *********************************************************************
	// * Initialization methods, sets up new ship as as a certain ShipType *
	// *********************************************************************
	public void newShip (ShipType shipType, GunType[] smallGuns, GunType[] mediumGuns, GunType[] largeGuns) {
		maxSpeed = shipType.maxSpeed;
		accelFrames = shipType.accelFrames;
		angSpeed = shipType.angSpeed;
		ticsPerTrailSwap = shipType.ticsPerTrailSwap;
		maxHealth = health = shipType.maxHealth;
		autoRange = shipType.autoRange;

		createGuns(shipType, smallGuns, mediumGuns, largeGuns);
		createTrail(shipType);
		createBorder(shipType);
		enable();
	}

	void createGuns (ShipType shipType, GunType[] smallGuns, GunType[] mediumGuns, GunType[] largeGuns) {
		numGunsS = shipType.numGunsS;
		gunsS = new Gun[numGunsS];
		for (int i = 0; i < numGunsS; i++) {
			gunsS[i] = Instantiate(smallGuns[i].gun, transform);
			gunsS[i].newGun(smallGuns[i]);
			gunsS[i].transform.localPosition = new Vector3(shipType.gunPosS[i, 0], shipType.gunPosS[i, 1], transform.position.z);
			gunsS[i].transform.rotation = transform.rotation;
		}

		numGunsM = shipType.numGunsM;
		gunsM = new Gun[numGunsM];
		for (int i = 0; i < numGunsM; i++) {
			gunsM[i] = Instantiate(mediumGuns[i].gun, transform);
			gunsM[i].newGun(mediumGuns[i]);
			gunsM[i].transform.localPosition = new Vector3(shipType.gunPosM[i, 0], shipType.gunPosM[i, 1], transform.position.z);
			gunsM[i].transform.rotation = transform.rotation;
		}

		numGunsL = shipType.numGunsL;
		gunsL = new Gun[numGunsL];
		for (int i = 0; i < numGunsL; i++) {
			gunsL[i] = Instantiate(largeGuns[i].gun, transform);
			gunsL[i].newGun(largeGuns[i]);
			gunsL[i].transform.localPosition = new Vector3(shipType.gunPosL[i, 0], shipType.gunPosL[i, 1], transform.position.z);
			gunsL[i].transform.transform.rotation = transform.rotation;
		}
	}

	void createTrail (ShipType shipType) {
		numTrails = shipType.numTrails;
		trails = new GameObject[numTrails];
		for (int i = 0; i < numTrails; i++) {
			trails[i] = Instantiate(shipType.trailArray[i], transform);
			trails[i].transform.localPosition = new Vector3(shipType.trailPos[i, 0], shipType.trailPos[i, 1], transform.position.z);
			trails[i].transform.rotation = transform.rotation;
			trails[i].transform.localScale = new Vector3(shipType.trailScale[0], shipType.trailScale[1], 1.0f);
		}
	}

	void createBorder (ShipType shipType) {
		border = Instantiate(Objects.Border, transform);
		border.transform.localScale = new Vector3(shipType.borderDims[0], shipType.borderDims[1], 1);
		healthBar = Instantiate(Objects.healthBar, transform);
		healthBar.transform.localScale = new Vector3(shipType.healthDims[0], shipType.healthDims[1], 1);
		maxHealthWidth = shipType.healthDims[0];
		healthLoc = new Vector3(shipType.healthLoc[0], shipType.healthLoc[1], shipType.healthLoc[2]);
	}
}
