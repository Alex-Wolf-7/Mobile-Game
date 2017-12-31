﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandling : MonoBehaviour {
    Camera currentCamera;
    Vector3 mousePos; // Position of mouse or average of all taps
    bool scrolling;
    
    // Touch handling vars
    int numTouchesLastFrame;

    SpawnPoint spawnAllies;
    SpawnPoint spawnEnemies;
    bool canClick = true;
    
    // Spawning consts
    const int maxShips = 10;

	// Use this for initialization
	void Start () {
        currentCamera = GetComponent<Camera>();

        Vector3 allyPos = new Vector3(0.0f, 0.0f, 0.0f);
        Vector3 allyRot = new Vector3(0.0f, 0.0f, 0.0f);
        spawnAllies = Instantiate(Objects.Spawn, allyPos, 
            Quaternion.Euler(allyRot)) as SpawnPoint;

        Vector3 enemyPos = new Vector3(8.0f, 5.0f, 0.0f);
        Vector3 enemyRot = new Vector3(0.0f, 0.0f, 90.0f);
        spawnEnemies = Instantiate(Objects.Spawn, enemyPos, 
            Quaternion.Euler(enemyRot)) as SpawnPoint;
    }
	
	// Update is called once per frame
    Vector3 clickStart; // location of downpress
	void Update () {
        mousePos = Input.mousePosition;

        if (Input.GetMouseButton(0) == false) {
            scrolling = false;
            numTouchesLastFrame = Input.touchCount;
            if (Input.GetMouseButtonUp(0) == false) {
                return;
            }
        }

        // Record position where click starts
        if (Input.GetMouseButtonDown(0) && canClick) {
            clickStart = mousePos;
        }

        // If mouse moves more than a certain distance since down press, it is no longer a click
        Vector2 clickMove = new Vector2(mousePos.x - clickStart.x, mousePos.y - clickStart.y);
        bool canMove;
        if (clickMove.magnitude >= CameraVars.notClick) {
            canMove = false;
        } else {
            canMove = true;
        }

        // If multiple touches, singular touch location is their average
        if (Input.touchCount > 1) {
            mousePos = new Vector3(0, 0, mousePos.z);
            for (int i = 0; i < Input.touchCount; i++) {
                mousePos.x += Input.GetTouch(i).position.x;
                mousePos.y += Input.GetTouch(i).position.y;
            }
            mousePos.x /= Input.touchCount;
            mousePos.y /= Input.touchCount;
        }

        // Restart drag if number of touches changes to prevent jumping
        if (Input.touchCount != numTouchesLastFrame) {
            downPos = Vector3.zero;
            scrolling = false;
        }

        // TWO TOUCHES: Zoom in or out
        if (Input.touchCount == 2 && canClick) {
            zoom();

            numTouchesLastFrame = Input.touchCount;
            return;
        }

        // ONE TOUCH: drag
        if (Input.GetMouseButton(0) && canMove == false) {
            scroll();
            numTouchesLastFrame = Input.touchCount;
            return;
        }

        // TOUCH ENDS THIS FRAME: do single click activities
        if (Input.GetMouseButtonUp(0) && canClick && canMove) {
            Vector3 click = mousePos;
            float cameraHeight = currentCamera.ScreenToWorldPoint(click).z;

            click.z = cameraHeight * -1; // Opposite of cameraHeight to make normal in world coords
            click = currentCamera.ScreenToWorldPoint(click);

            // Check if click is on ally or enemy
            Vector2 point2D = new Vector2(click.x, click.y);
            bool found = selectAlly(point2D); // If on ally, select
            if (found) {
                numTouchesLastFrame = Input.touchCount;
                return;
            }
            found = targetEnemy(point2D); // If on enemy, target
            if (found) {
                numTouchesLastFrame = Input.touchCount;
                return;
            }

            // If nothing else special to do, move dat boat to click location
            if (Ship.activeShip != null) {
                Ship.activeShip.setDestination(click);
            }
        }

        numTouchesLastFrame = Input.touchCount;
	}

    // Zooms in towards mousePos
    void zoom() {
        float zoomSpeed = CameraVars.zoomSpeedBase * currentCamera.orthographicSize;

        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        // Previous position of each touch
        Vector2 touchZeroPrev = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrev = touchOne.position - touchOne.deltaPosition;

        // Distance between touches right now and in previous frame
        float touchDist = (touchZero.position - touchOne.position).magnitude;
        float touchDistPrev = (touchZeroPrev - touchOnePrev).magnitude;

        // How much distance between touches chagned
        float distChange = touchDistPrev - touchDist;

        // Change zoom by changing field of view
        currentCamera.orthographicSize += distChange * zoomSpeed;
        // Keeps above 0
        currentCamera.orthographicSize = Mathf.Max(currentCamera.orthographicSize, CameraVars.minZoom);
        currentCamera.orthographicSize = Mathf.Min(currentCamera.orthographicSize, CameraVars.maxZoom);

        focusZoom();
    }

    // Keep point between fingers in same spot as zoom
    Vector3 screenPosPrev;
    Vector3 worldPosPrev;
    void focusZoom() {
        // If zoom is starting:
        if (numTouchesLastFrame != Input.touchCount) {
            screenPosPrev = mousePos;
            worldPosPrev = currentCamera.ScreenToWorldPoint(screenPosPrev);
        }

        // Where the center screen pos is in world coords now
        Vector3 worldPosNew = currentCamera.ScreenToWorldPoint(screenPosPrev);

        // How many world coords it has moved 
        Vector3 worldPosDiff = worldPosNew - worldPosPrev;

        // Handle scrolling while zooming
        scroll();

        // Move camera in accordance with focus
        currentCamera.transform.position -= worldPosDiff * CameraVars.zoomFocusMultiplier;

        // Prep next run's vars
        screenPosPrev = mousePos;
        worldPosPrev = currentCamera.ScreenToWorldPoint(screenPosPrev);
    }

    // Scroll camera
    Vector3 downPos; // Where does click start?
    void scroll() {
        if (numTouchesLastFrame != Input.touchCount || scrolling == false) {
            downPos = mousePos;
        }

        Vector3 mouseWorld = currentCamera.ScreenToWorldPoint(mousePos);
        Vector3 prevWorld = currentCamera.ScreenToWorldPoint(downPos);
        Vector3 posDiff = new Vector3(mouseWorld.x - prevWorld.x, mouseWorld.y - prevWorld.y, 0);

        currentCamera.transform.position -= posDiff * CameraVars.scrollSpeed;

        downPos = mousePos;
        scrolling = true;
    }

    // Check if click is on ally, if so, select it
    bool selectAlly(Vector2 point2D) {
        Ship ally = Objects.onShip(point2D);
        if (ally != null) {
            Ship.activeShip = ally;
            numTouchesLastFrame = Input.touchCount;
            return true;
        
        } else {
            return false;
        }
    }

    // Check if click is on enemy, if so, target it
    bool targetEnemy(Vector2 point2D) {
        Ship enemy = Objects.onEnemy(point2D);
        if (enemy != null) {
            Ship.activeShip.setTarget(enemy.shipGameObject());
            numTouchesLastFrame = Input.touchCount;
            return true;
        
        } else {
            return false;
        }
    }
    
    public void addCarrier () {
        if (Objects.numShips >= maxShips) return;
        GunType[] smallGuns = new GunType[] {Objects.GunSVars};
        GunType[] mediumGuns = new GunType[] {Objects.GunMVars};
        GunType[] largeGuns = new GunType[] {};
        Objects.allShips[Objects.numShips++] = Instantiate(Objects.CarrierVars.hull, spawnAllies.getTransform().position,
            spawnAllies.getTransform().rotation) as Ship;
        Objects.allShips[Objects.numShips - 1].newShip(Objects.CarrierVars, smallGuns, mediumGuns, largeGuns);
        Ship.activeShip = Objects.allShips[Objects.numShips - 1];
    }

    public void addCruiser () {
        if (Objects.numShips >= maxShips) return;
        GunType[] smallGuns = new GunType[] {Objects.GunSVars, Objects.GunSVars};
        GunType[] mediumGuns = new GunType[] {};
        GunType[] largeGuns = new GunType[] {};
        Objects.allShips[Objects.numShips++] = Instantiate(Objects.CruiserVars.hull, spawnAllies.getTransform().position,
            spawnAllies.getTransform().rotation) as Ship;
        Objects.allShips[Objects.numShips - 1].newShip(Objects.CruiserVars, smallGuns, mediumGuns, largeGuns);
        Ship.activeShip = Objects.allShips[Objects.numShips - 1];
    }

    public void addEnemy () {
        if (Objects.numEnemies >= maxShips) return;
        GunType[] smallGuns = new GunType[] {Objects.GunSVars};
        GunType[] mediumGuns = new GunType[] {Objects.GunMVars};
        GunType[] largeGuns = new GunType[] {};
        Objects.allEnemies[Objects.numEnemies] = Instantiate(Objects.CarrierVars.hull, spawnEnemies.getTransform().position,
            spawnEnemies.getTransform().rotation) as Ship;
        Objects.allEnemies[Objects.numEnemies].newShip(Objects.CarrierVars, smallGuns, mediumGuns, largeGuns);
        Objects.allEnemies[Objects.numEnemies].enemy = true;
        Objects.numEnemies++;
    }
    
    // Called when mouse moves on (or off of, below) button. Disables non-button mouse actions
    public void onButton () {
        canClick = false;
    }
    public void notOnButton () {
        canClick = true;
    }
}