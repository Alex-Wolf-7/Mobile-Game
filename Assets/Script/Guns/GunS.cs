using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunS : Gun {

	// Use this for initialization
	void Start () {
		bullet = Objects.objects.bulletS;
		size = 's';
		range = 20.0f;
		framesPerShot = 15;

		angSpeed = 5.0f;
	}
}
