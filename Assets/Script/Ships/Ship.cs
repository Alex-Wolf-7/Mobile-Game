using UnityEngine;
using System.Collections;

abstract public class Ship : MonoBehaviour {
    // Whatever ship is currently selected
    public static Ship activeShip;

    // Ship and related ship metrics and controls
    Rigidbody2D ship;
    public bool enemy; // true if enemy ship, false if allied
    public bool isEnabled = false;
    public float maxHealth;
    public float health;
    public float healthPercent;

    // Constant variables set by ship-deriving class
    protected float maxSpeed; 		// Max speed of ship
    protected int accelFrames; 		// Number of frames needed to hit max speed
    protected float angSpeed; 		// Degrees/second
    protected int ticsPerTrailSwap; // How many tics before swapping trails
    public int numGunsS;
    public int numGunsM;
    public int numGunsL;
    protected float autoRange;
    protected float[,] gunPosS;
    protected float[,] gunPosM;
    protected float[,] gunPosL;
    protected int numTrails;
    protected float[,] trailPos;
    protected Vector2 trailScale;
    protected Vector2 borderDims;
    protected Vector2 healthDims;
    protected Vector3 healthLoc;
    
    // Destination and related destination stuff
    Vector2 destination;
    bool tracking; // If true, we are moving towards target and not destination
    GameObject target;
    Vector2 rotationVector; // Unit vector in direction of rotation
    
    // Trail details
    protected GameObject[] trails;
    int activeTrail; // index of active trail, == numTrails for none
    bool isThrust = false;

    // Gun details
    Gun[] gunsS;
    Gun[] gunsM;
    Gun[] gunsL;

    GameObject border;
    GameObject healthBar;

    // Overrided method that sets all "protected" values above
    abstract public void ready ();

    void Awake () {
    	setup();
    }

    protected void setup() {
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
    
    // Method in charge of all movement, calls sub-methods for sub-functions
    private void move() {
        // If tracking a ship, set it as our destination unless already firing
    	if (tracking) {
    		bool firing = isFiring();
    		if (!firing && target != null) {
    			destination = target.transform.position;
    			isThrust = true;
    		} else {
    			isThrust = false;
    		}
    	}

        // Steer towards our destination
        if (isThrust) {
            rotate(); // Handles ship rotation
            thrust(); // Handlds ship movement
        } else {
            // If hit destination or close to tracking target, cut thrusters
        	slow();
        }

        trail();
    }
    
    // Handles rotating towards ship destination
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

    // Handles acceleration or not acceleration
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
        ship.angularVelocity = 0.0f; // If slowing, do not be turning
    	if (ship.velocity == Vector2.zero) return; // If stopped, does not need to slow more

        // Slow ship at same rate that it would otherwise speed up
    	ship.velocity -= rotationVector * (maxSpeed / accelFrames);

        // If slow too much and start moving backwards, stop
    	if (ship.velocity.x * rotationVector.x < 0) { // If facing different directions
    		ship.velocity = new Vector2(0.0f, ship.velocity.y);
    	}
    	if (ship.velocity.y * rotationVector.y < 0) { // If facing different directions
    		ship.velocity = new Vector2(ship.velocity.x, 0.0f);
    	}
    }
    
    // Turns border on and off, and sets its color
    void handleBorder () {
		if (Ship.activeShip == this) {
        	border.GetComponent<SpriteRenderer>().enabled = true;
        	border.GetComponent<SpriteRenderer>().color = healthColor();
        	healthBar.GetComponent<SpriteRenderer>().enabled = false;
        } else {
        	border.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    // Turns health bar on and off, sets its color, and maintains its position under ship
    void handleHealthBar () {
        // Health bar on if health is not full
        if (health == maxHealth) {
        	healthBar.GetComponent<SpriteRenderer>().enabled = false;
        	return;
        
        } else {
       		healthBar.GetComponent<SpriteRenderer>().enabled = (health != maxHealth);
       		healthBar.GetComponent<SpriteRenderer>().color = healthColor();
       		healthBar.transform.position = transform.position + healthLoc;
       		healthBar.transform.eulerAngles = Vector3.zero;
       		// Health bar shrinks as damage is taken
       		healthBar.transform.localScale = new Vector3(healthPercent * healthDims[0],
       		healthBar.transform.localScale.y, healthBar.transform.localScale.z);
       	}
    }

    // Set the destination for the boat to move to 
    public void setDestination (Vector2 newDestination) {
    	tracking = false; // No longer has a Ship destination
    	isThrust = true; // Has to move to destination
        destination = newDestination;
    }
    
    // Checks whether or not a given point is on our ship
    public bool pointOnShip (Vector3 point) {
        Collider2D collider = GetComponent<Collider2D>();
        return collider.bounds.Contains(point);
    }

    // Instructs guns to target a given GameObject
    public void setTarget (GameObject newTarget) {
    	tracking = true; // Explicit attack command, move towards ship
    	target = newTarget;
    	for (int i = 0; i < numGunsS; i++) {
    		gunsS[i].setTarget(target);
    	}
    	for (int i = 0; i < numGunsM; i++) {
    		gunsM[i].setTarget(target);
    	}
    	for (int i = 0; i < numGunsL; i++) {
    		gunsL[i].setTarget(target);
    	}
    }

    // Targets the nearest boat without interrupting steering
    public void setTargetNearest () {
    	// If tracking a specific ship, don't autotarget
    	if (tracking && target != null && !enemy) return;

    	// Find the closest ship that is the opposite faction as this
    	float closestDist = autoRange;
    	GameObject closest = null;;
    	if (enemy) {
    		for (int i = 0; i < Objects.objects.numShips; i++) {
    			float dist = Mathf.Abs((transform.position -
                    Objects.objects.allShips[i].shipGameObject().transform.position).magnitude);
    			
    			if (closest == null || dist < closestDist) {
    				closestDist = dist;
    				closest = Objects.objects.allShips[i].shipGameObject();
    			}
    		}
    	
    	} else {
    		for (int i = 0; i < Objects.objects.numEnemies; i++) {
    			float dist = Mathf.Abs((transform.position -
                    Objects.objects.allEnemies[i].shipGameObject().transform.position).magnitude);
    			
    			if (closest == null || dist < closestDist) {
    				closestDist = dist;
    				closest = Objects.objects.allEnemies[i].shipGameObject();
    			}
    		}
    	}
    	// If no opposition exists, closest == null, which sets guns to be off

    	// If nothing in decent range, don't focus on anything
    	if (closestDist > autoRange) {
    		closest = null;
    	}

    	setTarget(closest);
    	// Enemies should move towards autotargeted ships, allies should not
    	tracking = enemy;
    }

    // Returns true if any gun is firing
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

    // Damages ship, destroys it if health is zero
    public void damage (float damage) {
    	health -= damage;
    	healthPercent = health / maxHealth;
    	if (health <= 0) destroyShip();
    }

    // Destorys ship and removes it from the list of all ships
    void destroyShip () {
    	// Remove ship from global arrays
    	if (enemy) {
    		Utility.removeEnemy(this);
    	} else if (!enemy) {
    		Utility.removeShip(this);
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

	/********************************************
	 * Initialization methods, sets up new ship *
	 ********************************************/
	public void prepareChildren () {
		createTrail();
		createBorder();
	}

	void createTrail () {
		for (int i = 0; i < numTrails; i++) {
			GameObject newTrail = Instantiate(trails[i], transform);
            trails[i] = newTrail;
			trails[i].transform.localPosition = new Vector3(trailPos[i, 0], trailPos[i, 1], transform.position.z);
			trails[i].transform.rotation = transform.rotation;
			trails[i].transform.localScale = new Vector3(trailScale[0], trailScale[1], 1.0f);
		}
	}

	void createBorder () {
		border = Instantiate(Objects.objects.border, transform);
		border.transform.localScale = new Vector3(borderDims[0], borderDims[1], 1);
		healthBar = Instantiate(Objects.objects.healthBar, transform);
		healthBar.transform.localScale = new Vector3(healthDims[0], healthDims[1], 1);
		healthLoc = new Vector3(healthLoc[0], healthLoc[1], healthLoc[2]);
	}


    public void createGuns (GunType[] smallGuns, GunType[] mediumGuns, GunType[] largeGuns) {
        gunsS = new Gun[numGunsS];
        for (int i = 0; i < numGunsS; i++) {
            gunsS[i] = Instantiate(smallGuns[i].gun, transform);
            gunsS[i].newGun(smallGuns[i]);
            gunsS[i].transform.localPosition = new Vector3(gunPosS[i, 0], gunPosS[i, 1], transform.position.z);
            gunsS[i].transform.rotation = transform.rotation;
        }

        gunsM = new Gun[numGunsM];
        for (int i = 0; i < numGunsM; i++) {
            gunsM[i] = Instantiate(mediumGuns[i].gun, transform);
            gunsM[i].newGun(mediumGuns[i]);
            gunsM[i].transform.localPosition = new Vector3(gunPosM[i, 0], gunPosM[i, 1], transform.position.z);
            gunsM[i].transform.rotation = transform.rotation;
        }

        gunsL = new Gun[numGunsL];
        for (int i = 0; i < numGunsL; i++) {
            gunsL[i] = Instantiate(largeGuns[i].gun, transform);
            gunsL[i].newGun(largeGuns[i]);
            gunsL[i].transform.localPosition = new Vector3(gunPosL[i, 0], gunPosL[i, 1], transform.position.z);
            gunsL[i].transform.transform.rotation = transform.rotation;
        }

        enable();
    }
}
