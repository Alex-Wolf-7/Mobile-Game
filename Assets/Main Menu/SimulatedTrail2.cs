using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedTrail2 : MonoBehaviour {
	SpriteRenderer sRenderer;
	const int framesPerSwitch = 10;

	// Use this for initialization
	void Start () {
		sRenderer = GetComponent<SpriteRenderer>();
		sRenderer.enabled = false;
	}
	
	// Update is called once per frame
	int framesUntilSwitch = framesPerSwitch;
	void FixedUpdate () {
		if (framesUntilSwitch == 0) {
			sRenderer.enabled = !sRenderer.enabled;
			framesUntilSwitch = framesPerSwitch;
		} else {
			framesUntilSwitch--;
		}
	}
}
