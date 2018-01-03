using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Public static class holding hidden objects. Used for instantiating new objects
public class Objects : MonoBehaviour {
	public static Objects objects;

	// list of objects
	public Carrier carrier;
	public Cruiser cruiser;
	public GunS gunS;
	public GunM gunM;
	public GameObject trailOne;
	public GameObject trailTwo;
	public BulletS bulletS;
	public BulletM bulletM;
	public SpawnPoint spawn;
	public GameObject border;
	public GameObject healthBar;

	public Ship[] allShips;
	public Ship[] allEnemies;
	public int numShips;
	public int numEnemies;

	// Lets only one of this class exist
	void Start () {
		if (Objects.objects == null) {
			Objects.objects = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	// Subscribes/Desubscribes us to "OnLevelFinishedLoading" calls
	void OnEnable() {
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    void OnDisable() {
    	SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    // When new level loads, remove our variables
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
    	allShips = new Ship[ShipVars.maxAllies];
		numShips = 0;
		allEnemies = new Ship[ShipVars.maxEnemies];
		numEnemies = 0;
		Ship.activeShip = null;
    }
}