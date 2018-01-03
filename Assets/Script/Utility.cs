using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {
	// *******************
	// * Utility methods *
	// *******************
	// Checks which ship a point is on
	public static Ship onShip (Vector2 point) {
		for (int i = 0; i < Objects.objects.numShips; i++) {
            if (Objects.objects.allShips[i].pointOnShip(point)) {
                return Objects.objects.allShips[i];
            }
        }
        return null;
	}

	// Checks which enemy a point is on
	public static Ship onEnemy (Vector2 point) {
		for (int i = 0; i < Objects.objects.numEnemies; i++) {
            if (Objects.objects.allEnemies[i].pointOnShip(point)) {
                return Objects.objects.allEnemies[i];
            }
        }
        return null;
	}

	// Removes ship from array of all ships
	public static void removeShip (Ship ship) {
		int index = Objects.objects.numShips;  // Impossible value, check to see if it hasn't changed
		for (int i = 0; i < Objects.objects.numShips; i++) {
			if (ship == Objects.objects.allShips[i]) {
				index = i;
				break;
			}
		}
		// If index hasn't changed, no matching ships found, some error
		if (index == Objects.objects.numShips) return;

		Objects.objects.numShips--;
		for (int i = index; i < Objects.objects.numShips; i++) {
			Objects.objects.allShips[i] = Objects.objects.allShips[i + 1];
		}
		Objects.objects.allShips[Objects.objects.numShips] = null;
	}

	// Removes enemy from array of all enemies
	public static void removeEnemy (Ship enemy) {
		int index = Objects.objects.numEnemies;
		for (int i = 0; i < Objects.objects.numEnemies; i++) {
			if (enemy == Objects.objects.allEnemies[i]) {
				index = i;
				break;
			}
		}
		// If index hasn't changed, no matching enemies found, some error
		if (index == Objects.objects.numEnemies) return;

		Objects.objects.numEnemies--;
		for (int i = index; i < Objects.objects.numEnemies; i++) {
			Objects.objects.allEnemies[i] = Objects.objects.allEnemies[i + 1];
		}
		Objects.objects.allEnemies[Objects.objects.numEnemies] = null;
	}
}
