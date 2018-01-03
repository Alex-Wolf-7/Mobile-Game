using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrier : Ship {
	override public void ready () {
		maxSpeed = 2.0f;
		accelFrames = 100;
		angSpeed = 30.0f;
		ticsPerTrailSwap = 10;
		maxHealth = health = 5000.0f;
		autoRange = 40.0f;

		numGunsS = 1;
		gunPosS = new float[,] {{0.605f, 1.81f}};
		numGunsM = 1;
		gunPosM = new float[,] {{-0.265f, -2.0f}};
		numGunsL = 0;
		gunPosL = new float[,] {{}};

		numTrails = 2;
		trails = new GameObject[] {Objects.objects.trailOne, Objects.objects.trailTwo};
        trailPos = new float[,] {{0.0f, -3.0f}, {0.0f, -3.0f}};
        trailScale = new Vector2 (2.0f, 1.0f);

        borderDims = new Vector2 (0.4f, 0.55f);
        healthDims = new Vector2 (5.0f, 0.1f);
        healthLoc = new Vector3(0, -4.0f, 0);

        prepareChildren();
	}
}
