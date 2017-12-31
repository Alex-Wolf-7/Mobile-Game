using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Public static class holding hidden objects. Used for instantiating new objects
public static class Objects {
	// list of objects
	public static Ship CarrierHull;
	public static Ship CruiserHull;
	public static Gun GunS;
	public static Gun GunM;
	public static GameObject TrailOne;
	public static GameObject TrailTwo;
	public static BulletS bulletS;
	public static BulletM bulletM;
	public static SpawnPoint Spawn;
	public static GameObject Border;

	// list of ship types
	public static Carrier CarrierVars;
	public static Cruiser CruiserVars;

	// list of gun types
	public static GunS GunSVars;
	public static GunM GunMVars;

	public static Ship[] allShips;
	public static Ship[] allEnemies;
	public static int numShips;
	public static int numEnemies;

	// *******************
	// * Utility methods *
	// *******************
	// Checks which ship a point is on
	public static Ship onShip (Vector2 point) {
		for (int i = 0; i < numShips; i++) {
            if (allShips[i].pointOnShip(point)) {
                return allShips[i];
            }
        }
        return null;
	}

	// Checks which enemy a point is on
	public static Ship onEnemy (Vector2 point) {
		for (int i = 0; i < numEnemies; i++) {
            if (allEnemies[i].pointOnShip(point)) {
                return allEnemies[i];
            }
        }
        return null;
	}

	// Removes ship from array of all ships
	public static void removeShip (Ship ship) {
		int index = numShips;  // Impossible value, check to see if it hasn't changed
		for (int i = 0; i < numShips; i++) {
			if (ship == allShips[i]) {
				index = i;
				break;
			}
		}
		// If index hasn't changed, no matching ships found, some error
		if (index == numShips) return;

		numShips--;
		for (int i = index; i + 1 < numShips; i++) {
			allShips[i] = allShips[i + 1];
		}
		allShips[numShips] = null;
	}

	// Removes enemy from array of all enemies
	public static void removeEnemy (Ship enemy) {
		int index = numEnemies;
		for (int i = 0; i < numEnemies; i++) {
			if (enemy == allEnemies[i]) {
				index = i;
				break;
			}
		}
		// If index hasn't changed, no matching enemies found, some error
		if (index == numEnemies) return;

		numEnemies--;
		for (int i = index; i < numEnemies; i++) {
			allEnemies[i] = allEnemies[i + 1];
		}
		allEnemies[numEnemies] = null;
	}
}