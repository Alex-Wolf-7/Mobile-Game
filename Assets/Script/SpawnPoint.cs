using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {
	public Transform spawnPoint;

	// Use this for initialization
	void Start () {
		spawnPoint = GetComponent<Transform>();
		spawnPoint.GetComponent<SpriteRenderer>().enabled = false;
	}

	public Transform getTransform () {
		return spawnPoint;
	}
}
