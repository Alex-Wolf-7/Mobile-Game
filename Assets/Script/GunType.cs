using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GunType {
	public Gun gun;
	public char size;

	public float angSpeed;
}

public class GunS : GunType {
	public GunS () {
		gun = Objects.GunS;
		size = 's';

		angSpeed = 5.0f;
	}
}

public class GunM : GunType {
	public GunM () {
		gun = Objects.GunM;
		size = 'm';

		angSpeed = 3.0f;
	}
}
