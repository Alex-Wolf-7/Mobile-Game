using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipType {
    public Ship hull;
    public float maxHealth;

	public float maxSpeed; 			// Max speed of ship
    public int accelFrames; 		// Number of frames needed to hit max speed
    public float angSpeed; 			// Degrees/second
    public int ticsPerTrailSwap; 	// How many tics before swapping trails

    public float autoRange;

    public int numGunsS;
    public Gun[] gunSArray;
    public float[,] gunPosS;

    public int numGunsM;
    public Gun[] gunMArray;
	public float[,] gunPosM;

    public int numGunsL;
    public Gun[] gunLArray;
    public float[,] gunPosL;

    public int numTrails;
    public GameObject[] trailArray;
    public float[,] trailPos;
    public float[] trailScale;

    public float[] borderDims;
}

// Heavy carrier with the ability to launch fighter strikes
public class Carrier : ShipType {
    public Carrier () {
        hull = Objects.CarrierHull;
        maxHealth = 5000.0f;

        maxSpeed = 4.0f; 		// Max speed of ship
        accelFrames = 200; 		// Number of frames needed to hit max speed
        angSpeed = 30.0f; 		// Degrees/second
        ticsPerTrailSwap = 10; 	// How many tics before swapping trails

        autoRange = 50.0f;

        numGunsS = 1;
        gunPosS = new float[,] {{0.605f, 1.81f}};

        numGunsM = 1;
        gunPosM = new float[,] {{-0.265f, -2.0f}};

        numGunsL = 0;
        gunPosL = new float[,] {{}};

        numTrails = 2;
        trailArray = new GameObject[] {Objects.TrailOne, Objects.TrailTwo};
        trailPos = new float[,] {{0.0f, -3.0f}, {0.0f, -3.0f}};
        trailScale = new float[] {2.0f, 1.0f};

        borderDims = new float[] {0.4f, 0.55f};
    }
}

// Small, fast ship with two small guns
public class Cruiser : ShipType {
    public Cruiser () {
        hull = Objects.CruiserHull;
        maxHealth = 2000.0f;

        maxSpeed = 4.0f;        // Max speed of ship
        accelFrames = 60;       // Number of frames needed to hit max speed
        angSpeed = 50.0f;       // Degrees/second
        ticsPerTrailSwap = 10;  // How many tics before swapping trails

        autoRange = 30.0f;

        numGunsS = 2;
        gunSArray = new Gun[] {Objects.GunS, Objects.GunS};
        gunPosS = new float[,] {{0.0f, 0.6f}, {0.0f, -1.0f}};

        numGunsM = 0;
        gunMArray = new Gun[] {};
        gunPosM = new float[,] {{}};

        numGunsL = 0;
        gunLArray = new Gun[] {};
        gunPosL = new float[,] {{}};

        numTrails = 2;
        trailArray = new GameObject[] {Objects.TrailOne, Objects.TrailTwo};
        trailPos = new float[,] {{0.0f, -1.5f}, {0.0f, -1.5f}};
        trailScale = new float[] {1.0f, 1.0f};

        borderDims = new float[] {0.25f, 0.35f};
    }
}