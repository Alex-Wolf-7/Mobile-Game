using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GunType {
	public Gun gun;
	public Bullet bullet;
	public char size;
	public float range;
	public int framesPerShot;
	public float angSpeed;
}

public class GunS : GunType {
	public GunS () {
		gun = Objects.GunS;
		bullet = Objects.bulletS;
		size = 's';
		range = 20.0f;
		framesPerShot = 15;

		angSpeed = 5.0f;
	}
}

public class GunM : GunType {
	public GunM () {
		gun = Objects.GunM;
		bullet = Objects.bulletM;
		size = 'm';
		range = 30.0f;
		framesPerShot = 60;

		angSpeed = 3.0f;
	}
}
