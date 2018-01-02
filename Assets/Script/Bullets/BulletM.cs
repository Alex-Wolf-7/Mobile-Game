using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletM : Bullet {
	void Awake () {
		flightSpeed = 0.2f;
		damage = 100.0f;
	}
}