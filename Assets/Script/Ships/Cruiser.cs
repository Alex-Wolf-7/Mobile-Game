using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cruiser : Ship {
	void Awake () {
		shipName = "cruiser";

		maxSpeed = 4.0f;
		accelFrames = 60;
		angSpeed = 50.0f;
		ticsPerTrailSwap = 10;
		maxHealth = health = 2000.0f;
		autoRange = 30.0f;

		numGunsS = 2;
		gunPosS = new float[,] {{0.0f, 0.6f}, {0.0f, -1.0f}};
		numGunsM = 0;
		gunPosM = new float[,] {{}};
		numGunsL = 0;
		gunPosL = new float[,] {{}};

		numTrails = 2;
		trails = new GameObject[] {Objects.objects.trailOne, Objects.objects.trailTwo};
        trailPos = new float[,] {{0.0f, -1.5f}, {0.0f, -1.5f}};
        trailScale = new Vector2 (1.0f, 1.0f);

        borderDims = new Vector2 (0.25f, 0.35f);
        healthDims = new Vector2 (2.0f, 0.1f);
        healthLoc = new Vector3(0, -2.5f, 0);

        prepareChildren();
        setup();
	}
}
