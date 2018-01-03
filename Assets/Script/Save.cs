using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Save : MonoBehaviour {

    public void click () {
        bool saved = save();
        if (saved) {
            GetComponentInChildren<Text>().text = "Saved";
        }
    }

    bool save () {
        if (Fleet.fleet == null) {
            new Fleet();
        }
        return Fleet.fleet.save();
    }
}
