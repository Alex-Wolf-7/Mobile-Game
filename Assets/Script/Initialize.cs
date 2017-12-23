using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : MonoBehaviour {
	public Ship Carrier;
	public Ship PurpleBox;
	public Gun GunS;
	public Gun GunM;
	public GameObject TrailOne;
	public GameObject TrailTwo;
	public Bullet BulletS;
	public Bullet BulletM;
	public SpawnPoint Spawn;

	// Use this for initialization
	void Awake () {
		Objects.Carrier = Carrier;
		Carrier.disable();
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
	}

}
