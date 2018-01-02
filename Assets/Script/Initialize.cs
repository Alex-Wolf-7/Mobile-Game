using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : MonoBehaviour {
	public Carrier Carrier;
	public Cruiser Cruiser;
	public Gun GunS;
	public Gun GunM;
	public GameObject TrailOne;
	public GameObject TrailTwo;
	public BulletS bulletS;
	public BulletM bulletM;
	public SpawnPoint Spawn;
	public GameObject Border;
	public GameObject healthBar;

	// Use this for initialization
	void Start () {
		Objects.GunS = GunS;
		GunS.disable();
		Objects.GunM = GunM;
		GunM.disable();
		Objects.TrailOne = TrailOne;
		TrailOne.GetComponent<SpriteRenderer>().enabled = false;
		Objects.TrailTwo = TrailTwo;
		TrailTwo.GetComponent<SpriteRenderer>().enabled = false;
		Objects.bulletS = bulletS;
		bulletS.disable();
		Objects.bulletM = bulletM;
		bulletM.disable();
		Objects.Spawn = Spawn;
		// No disable line: disabled by nature
		Objects.Border = Border;
		Border.GetComponent<SpriteRenderer>().enabled = false;
		Objects.healthBar = healthBar;
		healthBar.GetComponent<SpriteRenderer>().enabled = false;

		// Gun Types
		Objects.GunSVars = new GunS();
		Objects.GunMVars = new GunM();

		// Ready boat types
		Objects.Carrier = Carrier;
		Carrier.ready();
		Objects.Cruiser = Cruiser;
		Cruiser.ready();

		// Array of allied ships and enemy ships
		Objects.allShips = new Ship[ShipVars.maxAllies];
		Objects.numShips = 0;
		Objects.allEnemies = new Ship[ShipVars.maxEnemies];
		Objects.numEnemies = 0;
	}

}
