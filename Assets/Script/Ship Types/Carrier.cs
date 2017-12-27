using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrier : Ship {

	// Use this for initialization
	override public void newShip () {
		maxSpeed = CarrierVars.maxSpeed;
		accelFrames = CarrierVars.accelFrames;
		angSpeed = CarrierVars.angSpeed;
		ticsPerTrailSwap = CarrierVars.ticsPerTrailSwap;

		createGuns();
		createTrail();
		createBorder();
		enable();
	}

	void createGuns () {
		numGunsS = CarrierVars.numGunsS;
		gunsS = new Gun[numGunsS];
		for (int i = 0; i < numGunsS; i++) {
			gunsS[i] = Instantiate(CarrierVars.gunSArray[i], transform);
			gunsS[i].transform.localPosition = new Vector3(CarrierVars.gunPosS[i, 0], CarrierVars.gunPosS[i, 1], transform.position.z);
			gunsS[i].transform.rotation = transform.rotation;
		}

		numGunsM = CarrierVars.numGunsM;
		gunsM = new Gun[numGunsM];
		for (int i = 0; i < numGunsM; i++) {
			gunsM[i] = Instantiate(CarrierVars.gunMArray[i], transform);
			gunsM[i].transform.localPosition = new Vector3(CarrierVars.gunPosM[i, 0], CarrierVars.gunPosM[i, 1], transform.position.z);
			gunsM[i].transform.rotation = transform.rotation;
		}

		numGunsL = CarrierVars.numGunsL;
		gunsL = new Gun[numGunsL];
		for (int i = 0; i < numGunsL; i++) {
			gunsL[i] = Instantiate(CarrierVars.gunLArray[i], transform);
			gunsL[i].transform.localPosition = new Vector3(CarrierVars.gunPosL[i, 0], CarrierVars.gunPosL[i, 1], transform.position.z);
			gunsL[i].transform.transform.rotation = transform.rotation;
		}
	}

	void createTrail () {
		numTrails = CarrierVars.numTrails;
		trails = new GameObject[numTrails];
		for (int i = 0; i < numTrails; i++) {
			trails[i] = Instantiate(CarrierVars.trailArray[i], transform);
			trails[i].transform.localPosition = new Vector3(CarrierVars.trailPos[i, 0], CarrierVars.trailPos[i, 1], transform.position.z);
			trails[i].transform.rotation = transform.rotation;
		}
	}

	void createBorder () {
		border = Instantiate(Objects.Border, transform);
		border.transform.localScale = new Vector3(CarrierVars.borderDims[0], CarrierVars.borderDims[1], 1);
	}
}
