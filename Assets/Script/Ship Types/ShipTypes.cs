﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CarrierVars {
	public static readonly float maxSpeed = 5.0f; 		// Max speed of ship
    public static readonly int accelFrames = 60; 		// Number of frames needed to hit max speed
    public static readonly float angSpeed = 60.0f; 		// Degrees/second
    public static readonly int ticsPerTrailSwap = 10; 	// How many tics before swapping trails

    public static readonly int numGunsS = 1;
    public static readonly Gun[] gunSArray = new Gun[] {Objects.GunS};
    public static readonly float[,] gunPosS = new float[,] {{0.605F, 1.81F}};

    public static readonly int numGunsM = 1;
    public static readonly Gun[] gunMArray = new Gun[] {Objects.GunM};
	public static readonly float[,] gunPosM = new float[,] {{-0.265F, -2.0F}};

    public static readonly int numGunsL = 0;
    public static readonly Gun[] gunLArray = new Gun[] {};
    public static readonly float[,] gunPosL = new float[,] {{}};

    public static readonly int numTrails = 2;
    public static readonly GameObject[] trailArray = new GameObject[] {Objects.TrailOne, Objects.TrailTwo};
    public static readonly float[,] trailPos = new float[,] {{0.0F, -3.0F}, {0.0F, -3.0F}};
}