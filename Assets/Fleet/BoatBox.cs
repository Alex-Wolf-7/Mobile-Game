using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatBox : MonoBehaviour {
    public int boxNumber;
    Ship currentShip;

	// Use this for initialization
	void Start () {
        loadShip();
	}

    void loadShip () {
        Fleet.fleetCheck();
        if (Fleet.fleet.getShip(boxNumber) == null) {
            return;
        }

        currentShip = Instantiate(Fleet.fleet.getShip(boxNumber), transform.position, transform.rotation);
        currentShip.transform.localScale *= 1;
        Gun[] smallGuns = new Gun[currentShip.numGunsS];
        for (int i = 0; i < currentShip.numGunsS; i++) {
            smallGuns[i] = Fleet.fleet.getGuns(boxNumber, 's', i);
        }
        Gun[] mediumGuns = new Gun[currentShip.numGunsM];
        for (int i = 0; i < currentShip.numGunsM; i++) {
            mediumGuns[i] = Fleet.fleet.getGuns(boxNumber, 'm', i);
        }
        Gun[] largeGuns = new Gun[currentShip.numGunsL];
        for (int i = 0; i < currentShip.numGunsL; i++) {
            largeGuns[i] = Fleet.fleet.getGuns(boxNumber, 'l', i);
        }
        currentShip.createGuns(smallGuns, mediumGuns, largeGuns);

        currentShip.setFakeShip(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
