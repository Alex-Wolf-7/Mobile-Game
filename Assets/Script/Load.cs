using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Load : MonoBehaviour {

    public void click () {
        bool loaded = load();
        if (loaded) {
            GetComponentInChildren<Text>().text = "Loaded";
        }
    }

    bool load () {
        if (Fleet.fleet == null) {
            new Fleet();
        }
        return Fleet.fleet.load();
    }
}
