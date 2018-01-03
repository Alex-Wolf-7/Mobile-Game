using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class Fleet {
	public static Fleet fleet;
	static ShipCode code;
   
    EncodedShip[] ships;

	public Fleet () {
        if (code == null) {
            code = new ShipCode();
        }
        if (Fleet.fleet == null) {
            Fleet.fleet = this;
        } else {
            return;
        }

        ships = new EncodedShip[ShipVars.maxAllies];
	}

    public bool load () {
        if (File.Exists(Application.persistentDataPath + "/fleet.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/fleet.dat", FileMode.Open);
            fleet = (Fleet)bf.Deserialize(file);
            file.Close();
            return true;
        }
            return false;
	}

    public bool save () {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/fleet.dat");

        bf.Serialize(file, this);
        file.Close();
        return true;
	}
        
    public void setShip (int shipNum, string shipName, string[] smallGuns, string[] mediumGuns, string[] largeGuns) {
        ships[shipNum] = new EncodedShip();
        ships[shipNum].ship = code.getShipCode(shipName);

        ships[shipNum].gunsS = new int[smallGuns.Length];
        for (int i = 0; i < smallGuns.Length; i++) {
            ships[shipNum].gunsS[i] = code.getGunSCode(smallGuns[i]);
        }

        ships[shipNum].gunsM = new int[mediumGuns.Length];
        for (int i = 0; i < mediumGuns.Length; i++) {
            ships[shipNum].gunsM[i] = code.getGunMCode(mediumGuns[i]);
        }

        ships[shipNum].gunsL = new int[largeGuns.Length];
        for (int i = 0; i < largeGuns.Length; i++) {
            ships[shipNum].gunsL[i] = code.getGunLCode(largeGuns[i]);
        }
	}

    public Ship getShip (int shipNum) {
        return code.getShip(ships[shipNum].ship);
	}

    public Gun getGuns (int shipNum, char size, int gunNum) {
        if (size == 's') {
            return code.getGunS(ships[shipNum].gunsS[gunNum]);
        } else if (size == 'm') {
            return code.getGunM(ships[shipNum].gunsM[gunNum]);
        } else if (size == 'l') {
            return code.getGunL(ships[shipNum].gunsL[gunNum]);
        } else {
            return null;
        }
	}

    // Returns the number of ships
    public int numShips () {
        for (int i = 0; i < ships.Length; i++) {
            if (ships[i] == null) {
                return i;
            }
        }
        return ships.Length;
    }

    // Sorts ships by shipRefs index
    public void sortShips () { 
        for (int i = 0; i < ships.Length; i++) {
            if (ships[i] == null) {
                ships[i] = new EncodedShip();
                ships[i].ship = 1000;
            }
        }

        Array.Sort(ships);

        for (int i = 0; i < ships.Length; i++) {
            if (ships[i].ship == 1000) {
                ships[i] = null;
            }
        }
    }

    public Ship[] allShips () {
        Ship[] shipList = new Ship[numShips()];
        for (int i = 0; i < numShips(); i++) {
            shipList[i] = code.getShip(ships[i].ship);
        }
        return shipList;
    }

    public Gun[] allGunsS (int shipIndex) {
        Gun[] allGuns = new Gun[ships[shipIndex].gunsS.Length];
        for (int i = 0; i < allGuns.Length; i++) {
            allGuns[i] = code.getGunS(ships[shipIndex].gunsS[i]);
        }
        return allGuns;
    }

    public Gun[] allGunsM (int shipIndex) {
        Gun[] allGuns = new Gun[ships[shipIndex].gunsM.Length];
        for (int i = 0; i < allGuns.Length; i++) {
            allGuns[i] = code.getGunM(ships[shipIndex].gunsM[i]);
        }
        return allGuns;
    }

    public Gun[] allGunsL (int shipIndex) {
        Gun[] allGuns = new Gun[ships[shipIndex].gunsL.Length];
        for (int i = 0; i < allGuns.Length; i++) {
            allGuns[i] = code.getGunL(ships[shipIndex].gunsL[i]);
        }
        return allGuns;
    }
};

[Serializable]
public class EncodedShip : IComparable<EncodedShip> {
    public int ship;
    public int[] gunsS;
    public int[] gunsM;
    public int[] gunsL;

    public int CompareTo(EncodedShip ship2) {
        if (this.ship > ship2.ship) {
            return 1;
        } else if (this.ship < ship2.ship) {
            return -1;
        } else {
            return 0;
        }
    }
}

// Used for encoding and decoding ships
public class ShipCode {
	Ship[] shipRefs;
	Gun[] gunSRefs;
	Gun[] gunMRefs;
	Gun[] gunLRefs;

	string[] shipStrings;
	string[] gunSStrings;
	string[] gunMStrings;
	string[] gunLStrings;

    int numShips;
    int numGunS;
    int numGunM;
    int numGunL;

	public ShipCode () {
		shipRefs = new Ship[] {null, Objects.objects.carrier, Objects.objects.cruiser};
		gunSRefs = new Gun[] {null, Objects.objects.gunS};
		gunMRefs = new Gun[] {null, Objects.objects.gunM};
		gunLRefs = new Gun[] {null};

        shipStrings = new string[shipRefs.Length];
        for (int i = 0; i < shipRefs.Length; i++) {
            if (shipRefs[i] == null) {
                shipStrings[i] = "";
            } else {
                shipStrings[i] = shipRefs[i].shipName;
            }
        }

        gunSStrings = new string[gunSRefs.Length];
        for (int i = 0; i < gunSRefs.Length; i++) {
            if (gunSRefs[i] == null) {
                gunSStrings[i] = "";
            } else {
                gunSStrings[i] = gunSRefs[i].gunName;
            }
        }

        gunMStrings = new string[gunMRefs.Length];
        for (int i = 0; i < gunMRefs.Length; i++) {
            if (gunMRefs[i] == null) {
                gunMStrings[i] = "";
            } else {
                gunMStrings[i] = gunMRefs[i].gunName;
            }
        }

        gunLStrings = new string[gunLRefs.Length];
        for (int i = 0; i < gunLRefs.Length; i++) {
            if (gunLRefs[i] == null) {
                gunLStrings[i] = "";
            } else {
                gunLStrings[i] = gunLRefs[i].gunName;
            }
        }
	}

    public int getShipCode (string shipName) {
        for (int i = 0; i < shipRefs.Length; i++) {
            if (shipName == shipStrings[i]) {
                return i;
            }
        }
        return 0;
    }

    public int getGunSCode (string gunName) {
        for (int i = 0; i < gunSRefs.Length; i++) {
            if (gunName == gunSStrings[i]) {
                return i;
            }
        }
        return 0;
    }

    public int getGunMCode (string gunName) {
        for (int i = 0; i < gunMRefs.Length; i++) {
            if (gunName == gunMStrings[i]) {
                return i;
            }
        }
        return 0;
    }

    public int getGunLCode (string gunName) {
        for (int i = 0; i < gunLRefs.Length; i++) {
            if (gunName == gunLStrings[i]) {
                return i;
            }
        }
        return 0;
    }

    public Ship getShip (int shipIndex) {
        return shipRefs[shipIndex];
    }

    public Gun getGunS (int gunIndex) {
        return gunSRefs[gunIndex];
    }

    public Gun getGunM (int gunIndex) {
        return gunMRefs[gunIndex];
    }

    public Gun getGunL (int gunIndex) {
        return gunLRefs[gunIndex];
    }
};