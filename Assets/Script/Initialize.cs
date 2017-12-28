using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : MonoBehaviour {
	public Ship CarrierHull;
	public Ship CruiserHull;
	public Gun GunS;
	public Gun GunM;
	public GameObject TrailOne;
	public GameObject TrailTwo;
	public Bullet BulletS;
	public Bullet BulletM;
	public SpawnPoint Spawn;
	public GameObject Border;

	// Use this for initialization
	void Awake () {
		Objects.CarrierHull = CarrierHull;
		CarrierHull.disable();
		Objects.CruiserHull = CruiserHull;
		CruiserHull.disable();
		Objects.GunS = GunS;
		GunS.disable();
		Objects.GunM = GunM;
		GunM.disable();
		Objects.TrailOne = TrailOne;
		TrailOne.GetComponent<SpriteRenderer>().enabled = false;
		Objects.TrailTwo = TrailTwo;
		TrailTwo.GetComponent<SpriteRenderer>().enabled = false;
		Objects.BulletS = BulletS;
		BulletS.disable();
		Objects.BulletM = BulletM;
		BulletM.disable();
		Objects.Spawn = Spawn;
		// No disable line: disabled by nature
		Objects.Border = Border;
		Border.GetComponent<SpriteRenderer>().enabled = false;

		// Ship types
		Objects.CarrierVars = new Carrier();
		Objects.CruiserVars = new Cruiser();

		// Gun Types
		Objects.GunSVars = new GunS();
		Objects.GunMVars = new GunM();
	}

}
