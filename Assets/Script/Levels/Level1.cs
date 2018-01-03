using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : MonoBehaviour {
	Ship[] shipList; // List of allied ships
	GunType[,][] shipGunList; // Guns to put on each ship: shipGunList[ship, S/M/L][gun]
	int numShips; // number of ships in shiplist
	SpawnPoint shipSpawn; // starting spawnpoint for ships
	
	// Same as above but for enemy ships
	Ship[] enemyList;
	GunType[,][] enemyGunList;
	int numEnemies;
	SpawnPoint enemySpawn;

	// Use this for initialization
	void Start () {
		shipList = new Ship[] {Objects.objects.carrier, Objects.objects.cruiser, Objects.objects.cruiser};

		// List of ship gun loadouts
		shipGunList = new GunType[,][] {
			{
				// Carrier
				new GunType[] {Objects.objects.gunSVars},
				new GunType[] {Objects.objects.gunMVars},
				new GunType[] {}
			},
			{
				// Cruiser
				new GunType[] {Objects.objects.gunSVars, Objects.objects.gunSVars},
				new GunType[] {},
				new GunType[] {}
			},
			{
				// Cruiser
				new GunType[] {Objects.objects.gunSVars, Objects.objects.gunSVars},
				new GunType[] {},
				new GunType[] {}
			}
		};

		numShips = 3;
		shipSpawn = Instantiate(Objects.objects.spawn, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));

		// Enemy ships
		enemyList = new Ship[] {Objects.objects.carrier, Objects.objects.cruiser};
		
		// Enemy ship gun loudouts
		enemyGunList = new GunType[,][] {
			{
				// Carrier
				new GunType[] {Objects.objects.gunSVars},
				new GunType[] {Objects.objects.gunMVars},
				new GunType[] {}
			},
			{
				// Cruiser
				new GunType[] {Objects.objects.gunSVars, Objects.objects.gunSVars},
				new GunType[] {},
				new GunType[] {}
			},
		};

		numEnemies = 2;
		enemySpawn = Instantiate(Objects.objects.spawn, new Vector3(0.0f, 40.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 180.0f));

		// Spawn ships as set up above
		spawn(shipList, shipGunList, numShips, shipSpawn, ref Objects.objects.allShips, ref Objects.objects.numShips, false);
		spawn(enemyList, enemyGunList, numEnemies, enemySpawn, ref Objects.objects.allEnemies, ref Objects.objects.numEnemies, true);
		// Active ship is first allied ship
		Ship.activeShip = Objects.objects.allShips[0];
	}

	/*
	 * Spawns new ship
	 * Params:
	 *		Ship[] shipList: array of ship to spawn
	 * 		GunType[,][] gunList: guns to spawn per ship - shipGunList[ship, S/M/L][gun]
	 * 		int numShips: number of ships
	 * 		SpawnPoint spawn: center of where to spawn ships at
	 *		ref Ship[] allShips: reference of where centralized list of ships is, to add to. Allied or enemy
	 *		ref int totalShips: reference to current size of allShips, wherever each are stored
	 *		bool enemy: true to make new ships enemies, false to keep allied
	 */ 
	void spawn (Ship[] shipList, GunType[,][] gunList, int numShips, SpawnPoint spawn, ref Ship[] allShips,
		ref int totalShips, bool enemy) {

		// Record position of SpawnPoint, to put it back when we are finished spawning ships
		Vector3 origPosition = spawn.getTransform().position;

		// Creates ships in order
		for (int i = 0; i < numShips; i++) {
			// Lays ships outward, first ships in middle, last ships on outside
			if (i % 2 != 0) {
				spawn.getTransform().position = new Vector3(spawn.getTransform().position.x + (i * ShipVars.spawnDist),
					spawn.getTransform().position.y, spawn.getTransform().position.z);
			} else {
				spawn.getTransform().position = new Vector3(spawn.getTransform().position.x - (i * ShipVars.spawnDist),
					spawn.getTransform().position.y, spawn.getTransform().position.z);
			}

			allShips[totalShips] = Instantiate(shipList[i], spawn.getTransform().position, spawn.getTransform().rotation);
			allShips[totalShips].ready();

			// Apply specifics to ship, including gun loadouts
			allShips[totalShips].createGuns(gunList[i, 0], gunList[i, 1], gunList[i, 2]);
			allShips[totalShips].enemy = enemy; // Set enemy if enemy

			totalShips++; // Mark ship as completed
		}
		// After completed, return SpawnPoint to original location
		spawn.getTransform().position = origPosition;
	}
}
