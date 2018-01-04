using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentShipEdit : MonoBehaviour {
    Ship currentShip;

	// Use this for initialization
	void Start () {
        if (Fleet.fleet == null) {
            new Fleet();
            Fleet.fleet.load();
        }

        Fleet.fleet.sortShips();
        if (Fleet.fleet.numShips() <= 0) {
            currentShip = null;
        } else {
            loadShip(0);
        }
	}
	
    public void loadShip(int index) {
        if (currentShip != null) {
            Destroy(currentShip.gameObject);
        }
        if (Fleet.fleet.getShip(index) == null) {
            return;
        }

        currentShip = Instantiate(Fleet.fleet.getShip(index), transform.position, transform.rotation);
        currentShip.transform.localScale *= 3;
        currentShip.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -10.0f);
        Gun[] smallGuns = new Gun[currentShip.numGunsS];
        for (int i = 0; i < currentShip.numGunsS; i++) {
            smallGuns[i] = Fleet.fleet.getGuns(index, 's', i);
        }
        Gun[] mediumGuns = new Gun[currentShip.numGunsM];
        for (int i = 0; i < currentShip.numGunsM; i++) {
            mediumGuns[i] = Fleet.fleet.getGuns(index, 'm', i);
        }
        Gun[] largeGuns = new Gun[currentShip.numGunsL];
        for (int i = 0; i < currentShip.numGunsL; i++) {
            largeGuns[i] = Fleet.fleet.getGuns(index, 'l', i);
        }
        currentShip.createGuns(smallGuns, mediumGuns, largeGuns);

        currentShip.setFakeShip(true);
    }


	// Update is called once per frame
	void Update () {
		
	}
}
