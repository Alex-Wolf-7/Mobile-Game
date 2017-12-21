using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
	GameObject gun;
	GameObject target;
	float targetDist;
	// Use this for initialization
	void Start () {
		gun = this.GetComponent<Transform>().gameObject;
	}
	
	// Update is called once per frame
	void Update () {
    	if (target != null) {
    		Vector2 relativeLocation = target.transform.position - gun.transform.position;

    		float angle = Vector2.SignedAngle(Vector2.up, relativeLocation);
    		gun.transform.eulerAngles = new Vector3(0, 0, angle);
    	}		
	}

	public void setTarget (GameObject newTarget) {
		target = newTarget;
	}
}
