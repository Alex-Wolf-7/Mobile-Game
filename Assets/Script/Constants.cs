using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraVars {
	public const float scrollSpeed = 1.3f; 			// Rate at which camera pans
    public const float zoomSpeedBase = 0.002f; 		// Zoom rate, adjusted at different zoom levels for consistancy
    public const float minZoom = 3.0f; 				// Closest zoom possible
    public const float maxZoom = 100.0f;
    public const float notClick = 25.0f; 			// If click moves more than a certain distance, it is not a click
    public const float zoomFocusMultiplier = 1.3f; 	// The rate which the camera moves towards the target
}

public static class ShipVars {
	public const float maxSpeed = 1.0f; 		// Max speed of ship
    public const int accelFrames = 60; 		// Number of frames needed to hit max speed
    public const float angSpeed = 60.0f; 		// Degrees/second
    public const int ticsPerTrailSwap = 10; 	// How many tics before swapping trails
}

public static class BulletVars {
	public const float flightSpeed = 0.1f;
}