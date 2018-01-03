using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TempAddShip : MonoBehaviour {
	// Use this for initialization
    string[] smallGuns;
    string[] mediumGuns;
    string[] largeGuns;
    System.Random rnd;

	void Start () {
        rnd = new System.Random();
	}

    public void addShip () {
        if (Fleet.fleet == null) {
            new Fleet();
        }

        int next = rnd.Next(0, 2);
        if (next == 1) {
            newCarrier();
        } else {
            newCruiser();
        }
    }
	
    void newCarrier () {
        int numShips = Fleet.fleet.numShips();
        if (numShips >= 10) return;
        smallGuns = new string[] { Objects.objects.gunS.gunName };
        mediumGuns = new string[] { Objects.objects.gunM.gunName };
        largeGuns = new string[] {};

        Fleet.fleet.setShip(numShips, Objects.objects.carrier.shipName, smallGuns, mediumGuns, largeGuns);
    }

    void newCruiser () {
        int numShips = Fleet.fleet.numShips();
        if (numShips >= 10) return;
        smallGuns = new string[] { Objects.objects.gunS.gunName, Objects.objects.gunS.gunName };
        mediumGuns = new string[] {};
        largeGuns = new string[] {};

        Fleet.fleet.setShip(numShips, Objects.objects.cruiser.shipName, smallGuns, mediumGuns, largeGuns);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
