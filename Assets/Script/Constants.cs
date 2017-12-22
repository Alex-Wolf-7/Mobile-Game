using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraVars {
	public const float scrollSpeed = 1.3f; // Rate at which camera pans
    public const float zoomSpeedBase = 0.002f; // Zoom rate, adjusted at different zoom levels for consistancy
    public const float minZoom = 0.1f; // Closest zoom possible
    public const float notClick = 25.0f; // If click moves more than a certain distance, it is not a click
    public const float zoomFocusMultiplier = 1.3f; // The rate which the camera moves towards the target
}
