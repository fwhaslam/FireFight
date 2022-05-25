using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayController : MonoBehaviour{

    internal readonly float MIN_ZOOM = 1f;
    internal readonly float MAX_ZOOM = 7f;
    internal readonly float SCROLLWHEEL_FACTOR = 10f;
    internal readonly float PINCHZOOM_FACTOR = 0.01f;

    // accumulated pinch zoom
    float zoom_delta = 0f;

    // load before any Start()
    void Awake() {

		Input.multiTouchEnabled = true;

	}

	// Start is called before the first frame update
	void Start() {
     }

    // Update is called once per frame
    void Update() {
               
        // mouse wheel zoom :: pc
        float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll!=0f) {
//Debug.Log("SCROLL WHEEL = " + scroll);
			AddZoom( (int)Mathf.Round ( scroll * SCROLLWHEEL_FACTOR ) );
        }

		// touch pinch zoom :: mobile
		if (Input.touchCount == 2) {
//Debug.Log("TOUCH COUNT == 2");

			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

			float difference = currentMagnitude - prevMagnitude;

            // accumulate zoom
//Debug.Log("Difference="+difference);
            zoom_delta += difference * PINCHZOOM_FACTOR;
            int change = ( int ) Mathf.Round( zoom_delta );
            zoom_delta -= change;   // keep leftover

            AddZoom( change );
		}


    }

    /// <summary>
    /// Move zoom in or out ( - or + ) using integral steps
    /// </summary>
    /// <param name="change"></param>
    internal void AddZoom( int change ) {
//Debug.Log("ADD ZOOM = "+change);
        var next = Camera.main.orthographicSize - change;
        Camera.main.orthographicSize = Mathf.Clamp( next, MIN_ZOOM, MAX_ZOOM );
	}

}
