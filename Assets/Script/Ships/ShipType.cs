using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipType {
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
    public float[] healthDims;
    public float[] healthLoc;
}

// Small, fast ship with two small guns
public class CruiserVars : ShipType {
    public CruiserVars () {
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
        healthDims = new float[] {2.0f, 0.1f};
        healthLoc = new float[] {0, -2.5f, 0};
    }
}