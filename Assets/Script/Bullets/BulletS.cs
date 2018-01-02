using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletS : Bullet {
	void Awake () {
		flightSpeed = 0.4f;
		damage = 10.0f;
	}
}