using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunM : Gun {

	// Use this for initialization
	void Start () {
		bullet = Objects.objects.bulletM;
		size = 'm';
		range = 30.0f;
		framesPerShot = 60;

		angSpeed = 3.0f;
	}
}
