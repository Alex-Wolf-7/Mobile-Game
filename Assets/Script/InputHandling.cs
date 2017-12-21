using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandling : MonoBehaviour {
    Camera currentCamera;
    Vector3 clickStart;
    // Camera scroll vars
    Vector3 cameraPos; // Initial camera position
    Vector3 downPos; // Where does click start?
    Vector3 dragOrigin; // Where are we moving?
    // Zoom vars
    Vector3 screenPosPrev;
    Vector3 worldPosPrev;
    // Touch handling vars
    int numTouchesLastFrame;
    const float scrollSpeed = 1.3f; // Adjusted at different zoom levels for consistancy
    const float zoomSpeedBase = 0.002f;
    const int framesForLongPress = 12;
    const float notClick = 25.0f; // If click moves more than a certain distance, it is not a click
    const float zoomFocusMultiplier = 1.3f; // The rate which the camera moves towards the target

    // Spawning variables
    public Ship baseShip;
    public Ship baseEnemy;
    Ship[] allShips;
    Ship[] allEnemies;

    private int numShips = 0;
    private int numEnemies = 0;

    public Transform spawnAllies;
    public Transform spawnEnemies;
    bool canClick = true;
    
    // Spawning consts
    const int maxShips = 10;
    
	// Use this for initialization
	void Start () {
        currentCamera = GetComponent<Camera>();
        allShips = new Ship[maxShips];
        allEnemies = new Ship[maxShips];
        baseShip.disable();
        baseEnemy.disable();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 mousePos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0) && canClick) {
            clickStart = mousePos;
        }

        // If mouse moves more than a certain distance since down press, it is no longer a click
        bool canMove = true;
        Vector2 clickMove = new Vector2(mousePos.x - clickStart.x, mousePos.y - clickStart.y);
        if (clickMove.magnitude >= notClick) {
            canMove = false;
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

        if (Input.touchCount != numTouchesLastFrame) {
            downPos = Vector3.zero;
        }

        // Zoom in or out
        if (Input.touchCount == 2 && canClick) {
            zoom(mousePos);

            numTouchesLastFrame = Input.touchCount;
            return;
        }

        // Drag logic
        if (Input.GetMouseButton(0)) {
            scroll(mousePos);
            numTouchesLastFrame = Input.touchCount;
            return;
        } else {
            downPos = Vector3.zero;
        }


        // When mouse button up... do single click activities
        if (Input.GetMouseButtonUp(0) && canClick && canMove) {
            Vector3 click = mousePos;
            float cameraHeight = currentCamera.ScreenToWorldPoint(click).z;

            click.z = cameraHeight * -1; // Opposite of cameraHeight to make normal in world coords
            click = currentCamera.ScreenToWorldPoint(click);

            // Check if click is on ally or enemy
            Vector2 point2D = new Vector2(click.x, click.y);
            bool found = selectAlly(point2D); // If on ally, select
            if (found) return;
            found = targetEnemy(point2D); // If on enemy, target
            if (found) return;
            
            // If nothing else special to do, move dat boat to click location
            Ship.activeShip.setDestination(click);
        }

        numTouchesLastFrame = Input.touchCount;
	}

    // Zooms in towards mousePos
    void zoom(Vector3 mousePos) {
        float zoomSpeed = zoomSpeedBase * currentCamera.orthographicSize;

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
        currentCamera.orthographicSize = Mathf.Max(currentCamera.orthographicSize, 0.1f);

        focusZoom(mousePos);
    }

    // Keep point between fingers in same spot as zoom
    void focusZoom(Vector3 mousePos) {
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
        scroll(mousePos);

        // Move camera in accordance with focus
        currentCamera.transform.position -= worldPosDiff * zoomFocusMultiplier;

        // Prep next run's vars
        screenPosPrev = mousePos;
        worldPosPrev = currentCamera.ScreenToWorldPoint(screenPosPrev);
    }

            // Move while zooming
    void scroll(Vector3 mousePos) {
        if (numTouchesLastFrame != Input.touchCount) {
            downPos = mousePos;
        }

        Vector3 mouseWorld = currentCamera.ScreenToWorldPoint(mousePos);
        Vector3 prevWorld = currentCamera.ScreenToWorldPoint(downPos);
        Vector3 posDiff = new Vector3(mouseWorld.x - prevWorld.x, mouseWorld.y - prevWorld.y, 0);

        currentCamera.transform.position -= posDiff * scrollSpeed;

        downPos = mousePos;
    }

    // Check if click is on ally, if so, select it
    bool selectAlly(Vector2 point2D) {
        for (int i = 0; i < numShips; i++) {
            if (allShips[i].pointOnShip(point2D)) {
                Ship.activeShip = allShips[i];
                numTouchesLastFrame = Input.touchCount;
                return true; // returns true if ally selected
            }
        }
        return false;
    }

    // Check if click is on enemy, if so, target it
    bool targetEnemy(Vector2 point2D) {
        for (int i = 0; i < numEnemies; i++) {
            if (allEnemies[i].pointOnShip(point2D)) {
                Ship.activeShip.setGunTarget(allEnemies[i].shipGameObject());
                numTouchesLastFrame = Input.touchCount;
                return true; // returns true if ally selected
            }
        }
        return false;
    }
    
    public void addShip () {
        if (numShips >= maxShips) return;
        allShips[numShips++] = Instantiate(baseShip, spawnAllies.position, spawnAllies.rotation) as Ship;
        Ship.activeShip = allShips[numShips - 1];
    }

    public void addEnemy () {
        if (numEnemies >= maxShips) return;
        allEnemies[numEnemies++] = Instantiate(baseEnemy, spawnEnemies.position, spawnEnemies.rotation) as Ship;
    }
    
    // Called when mouse moves on (or off of, below) button. Disables non-button mouse actions
    public void onButton () {
        canClick = false;
    }
    public void notOnButton () {
        canClick = true;
    }
}